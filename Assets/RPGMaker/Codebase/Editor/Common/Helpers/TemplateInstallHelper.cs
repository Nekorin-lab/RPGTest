using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace RPGMaker.Codebase.Editor.Common
{
    internal static class TemplateInstallHelper
    {
        private static readonly List<string> s_templateNames = new List<string> 
        {
            "project_base_v",
            "masterdata_common_v",
            "masterdata_jp_v",
            "masterdata_en_v",
            "masterdata_cn_v",
            "defaultgame_common_v",
            "defaultgame_jp_v",
            "defaultgame_en_v",
            "defaultgame_cn_v",
        };

        private const string DEFAULTGAME_COMMON = "defaultgame_common_v";
        private const string DEFAULTGAME_JP = "defaultgame_jp_v";
        private const string DEFAULTGAME_EN = "defaultgame_en_v";
        private const string DEFAULTGAME_CN = "defaultgame_cn_v";

        private const string VERSION = "1.0.0";

        private static readonly string localTemplatePath = Directory.GetCurrentDirectory() + "/Assets/RPGMaker/Storage/System/Archive/";
        private static readonly string localCheckPath = Directory.GetCurrentDirectory() + "/Assets/RPGMaker/Storage/Initializations";

#if UNITY_EDITOR_WIN
        private static int folderSub = 2;
#else
        private static int folderSub = 4;
#endif

        private static bool IsTemplateAvailableForInstall() {
            string folderPath = Application.persistentDataPath;

            string[] folderSplit = folderPath.Split("/");
            folderPath = "";
            for (int i = 0; i < folderSplit.Length - folderSub; i++)
                folderPath += folderSplit[i] + "/";

            folderPath += ".RPGMaker/";

            //UnityEngine.Debug.Log("IsTemplateAvailableForInstall :: " + folderPath);

            int cnt = 0;
            foreach (var t in s_templateNames)
            {
                if (File.Exists(folderPath + GetFileName(t)) == false)
                {
                    if (File.Exists(localTemplatePath + t))
                    {
                        //UnityEngine.Debug.Log("IsTemplateAvailableForInstall :: " + (localTemplatePath + t));
                        cnt++;
                    }
                }
            }

            if (cnt == s_templateNames.Count)
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        ///     テンプレート配置処理/Install template files to Unity
        /// </summary>
        public static void InstallRPGMakerUniteTemplates() {

            if (IsTemplateAvailableForInstall())
            {
                return;
            }
            
            string folderPath = Application.persistentDataPath;

            string[] folderSplit = folderPath.Split("/");
            folderPath = "";
            for (int i = 0; i < folderSplit.Length - folderSub; i++)
                folderPath += folderSplit[i] + "/";

            folderPath += ".RPGMaker";

            //UnityEngine.Debug.Log("InstallRPGMakerUniteTemplates Start");
            if (!Directory.Exists(folderPath))
            {
                //UnityEngine.Debug.Log("Create Directory");
                Directory.CreateDirectory(folderPath);
            }

            //異なるVersionのStorageを削除する
            string[] files = Directory.GetFiles(folderPath);
            string[] fileNames = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                fileNames[i] = Path.GetFileName(files[i]);
            }

            folderPath += "/";
            for (var i = 0; i < fileNames.Length; i++)
            {
                bool flg = false;
                for (var j = 0; j < s_templateNames.Count; j++)
                {
                    if (fileNames[i] == GetFileName(s_templateNames[j]))
                    {
                        flg = true;
                        break;
                    }
                }
                if (!flg)
                {
                    File.Delete(folderPath + fileNames[i]);
                }
            }

            //Storageをユーザーフォルダにコピーする
            for (var i = 0; i < s_templateNames.Count; i++)
            {
                if (File.Exists(folderPath + GetFileName(s_templateNames[i])) == false)
                {
                    if (File.Exists(localTemplatePath + GetFileName(s_templateNames[i])))
                    {
                        //UnityEngine.Debug.Log("Copy File :: " + s_templateNames[i]);
                        File.Copy(localTemplatePath + GetFileName(s_templateNames[i]), folderPath + GetFileName(s_templateNames[i]));
                    }
                }
            }
        }

        public static void InitializeDefaultStorage() {
            if (!Directory.Exists(localCheckPath))
            {
                // AssetDatabaseを一時停止
                AssetDatabase.StartAssetEditing();

                //Systemフォルダが存在しない時、言語設定に従ったStorageを初期展開する
                //各zipファイル
                string folderPath = Application.persistentDataPath;

                string[] folderSplit = folderPath.Split("/");
                folderPath = "";
                for (int i = 0; i < folderSplit.Length - folderSub; i++)
                    folderPath += folderSplit[i] + "/";

                folderPath += ".RPGMaker/";

                // 現在の言語設定
                var assembly2 = typeof(EditorWindow).Assembly;
                var localizationDatabaseType2 = assembly2.GetType("UnityEditor.LocalizationDatabase");
                var currentEditorLanguageProperty2 = localizationDatabaseType2.GetProperty("currentEditorLanguage");
                var lang2 = (SystemLanguage) currentEditorLanguageProperty2.GetValue(null);

                //指定されたフォルダ内の、Storage領域に、共通Storageを解凍する
                ZipFile.ExtractToDirectory(folderPath + GetFileName(DEFAULTGAME_COMMON), Directory.GetCurrentDirectory() + "/Assets/RPGMaker", true);

                //指定されたフォルダ内の、Storage領域に、言語Storageを解凍する
                if (lang2 == SystemLanguage.Japanese)
                {
                    ZipFile.ExtractToDirectory(folderPath + GetFileName(DEFAULTGAME_JP), Directory.GetCurrentDirectory() + "/Assets/RPGMaker", true);
                }
                else if (lang2 == SystemLanguage.Chinese || lang2 == SystemLanguage.ChineseSimplified ||
                         lang2 == SystemLanguage.ChineseTraditional)
                {
                    ZipFile.ExtractToDirectory(folderPath + GetFileName(DEFAULTGAME_CN), Directory.GetCurrentDirectory() + "/Assets/RPGMaker", true);
                }
                else
                {
                    ZipFile.ExtractToDirectory(folderPath + GetFileName(DEFAULTGAME_EN), Directory.GetCurrentDirectory() + "/Assets/RPGMaker", true);
                }

                // AssetDatabaseを再開
                AssetDatabase.StopAssetEditing();
            }
        }

        private static string GetFileName(string name) {
            return name + VERSION + ".zip";
        }

        private static void MoveDirectory(string source, string destination) {
            // 移動元のディレクトリとその中身を取得
            DirectoryInfo sourceDirectory = new DirectoryInfo(source);

            // 移動先のディレクトリが存在しない場合は作成
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            // 移動元のディレクトリ内のファイルをすべて移動
            foreach (FileInfo file in sourceDirectory.GetFiles())
            {
                string tempPath = Path.Combine(destination, file.Name);
                file.MoveTo(tempPath);
            }

            // サブディレクトリの移動（再帰呼び出し）
            foreach (DirectoryInfo subDirectory in sourceDirectory.GetDirectories())
            {
                string tempPath = Path.Combine(destination, subDirectory.Name);
                MoveDirectory(subDirectory.FullName, tempPath);
            }

            // 移動元のディレクトリを削除
            sourceDirectory.Delete(true);
        }
    }
}