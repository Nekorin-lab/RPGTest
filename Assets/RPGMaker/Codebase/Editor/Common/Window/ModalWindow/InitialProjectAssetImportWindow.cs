using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Map;
using RPGMaker.Codebase.CoreSystem.Service.MapManagement;
using RPGMaker.Codebase.Editor.Common.View;
using RPGMaker.Codebase.Editor.Hierarchy.Enum;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.Common.Window.ModalWindow
{
    public class InitialProjectAssetImportWindow : BaseModalWindow
    {
        // テキスト
        private const string TITLE_TEXT = "WORD_2100";

        private const string BASIC_ASSETS = "WORD_2105";
        private const string FULL_GAME    = "WORD_2106";

        private const string DESCRIPTION_TEXT = "WORD_5005"; //"作成するプロジェクトを選択してください";
        private const string DESCRIPTION_TEXT2 = "WORD_5010"; //"指定言語のプロジェクトテンプレートがインストール\nされていません";

        private const string PROJECT_NAME_TEXT = "WORD_5008"; //"プロジェクト名";
        private const string PATH_TEXT = "WORD_5009"; //"保存場所";

        private const string DEFAULT_PROJECT_NAME = "New Unite Project";

        private const string PROJECT_BASE = "project_base_v";

        private const string MASTERDATA_COMMON = "masterdata_common_v";
        private const string MASTERDATA_JP = "masterdata_jp_v";
        private const string MASTERDATA_EN = "masterdata_en_v";
        private const string MASTERDATA_CN = "masterdata_cn_v";

        private const string DEFAULTGAME_COMMON = "defaultgame_common_v";
        private const string DEFAULTGAME_JP = "defaultgame_jp_v";
        private const string DEFAULTGAME_EN = "defaultgame_en_v";
        private const string DEFAULTGAME_CN = "defaultgame_cn_v";

        private const string VERSION = "1.0.0";

#if UNITY_EDITOR_WIN
        private int folderSub = 2;
#else
        private int folderSub = 4;
#endif

        private static readonly string[] LANGUAGE_NAMES = new [] {
            "WORD_2102","WORD_2103","WORD_2104"
        };

        private string _folderPath;
        private SystemLanguage _lang2;
        private VisualElement _labelFromUxml;
        private Button _okButton;
        private int _selectNum;

        private ImTextField _path;
        private Button _pathButton;
        private Label _pathLabel;
        private ImTextField _projectName;
        private Label _projectNameLabel;

        protected override string ModalUxml =>
            "Assets/RPGMaker/Codebase/Editor/Common/Window/ModalWindow/Uxml/initialize_project_asset_import.uxml";

        public void ShowWindow() {
            var wnd = GetWindow<InitialProjectAssetImportWindow>();

            // 処理タイトル名適用
            wnd.titleContent = new GUIContent(EditorLocalize.LocalizeText(TITLE_TEXT));
            wnd.Init();
            //サイズ固定用
            var size = new Vector2(600, 450);
            wnd.minSize = size;
            wnd.maxSize = size;
            wnd.maximized = false;
        }

        public override void Init() {
            // 各zipファイルが存在しているかどうかを確認し、無ければ説明欄の文言を切り替え、OKボタンを押下不可とする
            // 現在の言語設定
            var assembly2 = typeof(EditorWindow).Assembly;
            var localizationDatabaseType2 = assembly2.GetType("UnityEditor.LocalizationDatabase");
            var currentEditorLanguageProperty2 = localizationDatabaseType2.GetProperty("currentEditorLanguage");
            _lang2 = (SystemLanguage) currentEditorLanguageProperty2.GetValue(null);

            // 各zipファイル
            _folderPath = Application.persistentDataPath;

            string[] folderSplit = _folderPath.Split("/");
            _folderPath = "";
            for (int i = 0; i < folderSplit.Length - folderSub; i++)
                _folderPath += folderSplit[i] + "/";

            _folderPath += ".RPGMaker/";

            var root = rootVisualElement;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ModalUxml);
            _labelFromUxml = visualTree.CloneTree();
            EditorLocalize.LocalizeElements(_labelFromUxml);
            root.Add(_labelFromUxml);

            RadioButton radioBasicButton = root.Q<RadioButton>("radio_basic_asset");
            RadioButton radioFullButton = root.Q<RadioButton>("radio_full_asset");
            radioFullButton.value = true;

            _projectNameLabel = _labelFromUxml.Query<Label>("project_name_label");
            _projectName = _labelFromUxml.Query<ImTextField>("project_name");
            _pathLabel = _labelFromUxml.Query<Label>("path_label");
            _path = _labelFromUxml.Query<ImTextField>("path");
            _pathButton = _labelFromUxml.Query<Button>("path_button");

            // プロジェクト名とパス名の設定
            var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var projectName = DEFAULT_PROJECT_NAME;
            // プロジェクト名の重複チェック
            if (Directory.Exists(userPath + "/" + projectName))
            {
                var n = 1;
                while (true)
                {
                    if (!Directory.Exists(userPath + "/" + projectName + " (" + n + ")"))
                    {
                        projectName = DEFAULT_PROJECT_NAME + " (" + n + ")";
                        break;
                    }

                    n++;
                }
            }

            // テキスト設定
            _projectNameLabel.text = EditorLocalize.LocalizeText(PROJECT_NAME_TEXT);
            _pathLabel.text = EditorLocalize.LocalizeText(PATH_TEXT);
            _projectName.value = projectName;
            _path.value = userPath;
            _path.isReadOnly = true;

            VisualElement langSelect = _labelFromUxml.Query<VisualElement>("langSelect");
            var languages = LANGUAGE_NAMES.Select(lang => EditorLocalize.LocalizeText(lang)).ToList();

            _selectNum = 0;
            if (_lang2 == SystemLanguage.Japanese)
                _selectNum = 0;
            else if (_lang2 == SystemLanguage.Chinese || _lang2 == SystemLanguage.ChineseSimplified || _lang2 == SystemLanguage.ChineseTraditional)
                _selectNum = 2;
            else
                _selectNum = 1;

            var langSelectPopupField = new PopupFieldBase<string>(languages, _selectNum);
            langSelect.Clear();
            langSelect.Add(langSelectPopupField);
            langSelectPopupField.RegisterValueChangedCallback(evt =>
            {
                _selectNum = langSelectPopupField.index;
                CheckLanguageData();
            });

            // ボタン設定
            string iconPath = EditorGUIUtility.isProSkin ? "Dark" : "Light";
            _pathButton.style.backgroundImage =
                UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<Texture2D>(
                    "Assets/RPGMaker/Storage/Images/System/Menu/" + iconPath + "/Active/uibl_icon_menu_002.png");
            _pathButton.style.width = 25;
            _pathButton.clicked += () => { _path.value = EditorUtility.OpenFolderPanel("Open Folder", "", ""); };

            //OKボタン
            _okButton = _labelFromUxml.Query<Button>("OK_button");
            _okButton.clicked += () =>
            {
                //_folderPath : 解凍するzipファイルの格納フォルダ
                //_path.value : 新規PJ作成フォルダ
                //_projectName.value : 新規PJ作成フォルダ
                if (!Directory.Exists(_path.value + "/" + _projectName.value))
                {
                    //UnityEngine.Debug.Log("Create Directory");
                    Directory.CreateDirectory(_path.value + "/" + _projectName.value);
                }

                //指定されたフォルダ位置に、プログラムファイル一式を解凍する
                ZipFile.ExtractToDirectory(_folderPath + GetFileName(PROJECT_BASE), _path.value + "/" + _projectName.value, true);
                MoveDirectory(_path.value + "/" + _projectName.value + "/rpgmaker", _path.value + "/" + _projectName.value);

                //デフォルトゲーム入りの新規PJ作成
                if (radioFullButton.value)
                {
                    //指定されたフォルダ内の、Storage領域に、共通Storageを解凍する
                    ZipFile.ExtractToDirectory(_folderPath + GetFileName(DEFAULTGAME_COMMON), _path.value + "/" + _projectName.value + "/Assets/RPGMaker", true);

                    //指定されたフォルダ内の、Storage領域に、言語Storageを解凍する
                    if (_selectNum == 0)
                    {
                        ZipFile.ExtractToDirectory(_folderPath + GetFileName(DEFAULTGAME_JP), _path.value + "/" + _projectName.value + "/Assets/RPGMaker", true);
                    }
                    else if (_selectNum == 2)
                    {
                        ZipFile.ExtractToDirectory(_folderPath + GetFileName(DEFAULTGAME_CN), _path.value + "/" + _projectName.value + "/Assets/RPGMaker", true);
                    }
                    else
                    {
                        ZipFile.ExtractToDirectory(_folderPath + GetFileName(DEFAULTGAME_EN), _path.value + "/" + _projectName.value + "/Assets/RPGMaker", true);
                    }
                }
                //マスタデータのみの新規PJ作成
                else
                {
                    //指定されたフォルダ内の、Storage領域に、共通Storageを解凍する
                    ZipFile.ExtractToDirectory(_folderPath + GetFileName(MASTERDATA_COMMON), _path.value + "/" + _projectName.value + "/Assets/RPGMaker", true);

                    //指定されたフォルダ内の、Storage領域に、言語Storageを解凍する
                    if (_selectNum == 0)
                    {
                        ZipFile.ExtractToDirectory(_folderPath + GetFileName(MASTERDATA_JP), _path.value + "/" + _projectName.value + "/Assets/RPGMaker", true);
                    }
                    else if (_selectNum == 2)
                    {
                        ZipFile.ExtractToDirectory(_folderPath + GetFileName(MASTERDATA_CN), _path.value + "/" + _projectName.value + "/Assets/RPGMaker", true);
                    }
                    else
                    {
                        ZipFile.ExtractToDirectory(_folderPath + GetFileName(MASTERDATA_EN), _path.value + "/" + _projectName.value + "/Assets/RPGMaker", true);
                    }
                }

                //保存
                EditorApplication.ExecuteMenuItem("File/Save");

                //このWindowを閉じる
                Close();

                //作成したPJを開く
                EditorApplication.OpenProject(_path.value + "/" + _projectName.value);
                //UnityEngine.Debug.Log("OPEN :: " + _path.value + "/" + _projectName.value);
            };

            //CANCELボタン
            Button cancelButton = _labelFromUxml.Query<Button>("CANCEL_button");
            cancelButton.clicked += () => { Close(); };

            //初期状態
            CheckLanguageData();
        }

        private void CheckLanguageData() {
            Label descriptionLabel = _labelFromUxml.Query<Label>("description_text");

            bool exist = true;
            if (File.Exists(_folderPath + GetFileName(MASTERDATA_COMMON)) == false)
                exist = false;
            if (File.Exists(_folderPath + GetFileName(DEFAULTGAME_COMMON)) == false)
                exist = false;

            if (_selectNum == 0)
            {
                if (File.Exists(_folderPath + GetFileName(MASTERDATA_JP)) == false)
                    exist = false;
                if (File.Exists(_folderPath + GetFileName(DEFAULTGAME_JP)) == false)
                    exist = false;
            }
            else if (_selectNum == 2)
            {
                if (File.Exists(_folderPath + GetFileName(MASTERDATA_CN)) == false)
                    exist = false;
                if (File.Exists(_folderPath + GetFileName(DEFAULTGAME_CN)) == false)
                    exist = false;
            }
            else
            {
                if (File.Exists(_folderPath + GetFileName(MASTERDATA_EN)) == false)
                    exist = false;
                if (File.Exists(_folderPath + GetFileName(DEFAULTGAME_EN)) == false)
                    exist = false;
            }

            if (!exist)
            {
                descriptionLabel.text = EditorLocalize.LocalizeText(DESCRIPTION_TEXT2);
                _okButton.SetEnabled(false);
            }
            else
            {
                descriptionLabel.text = EditorLocalize.LocalizeText(DESCRIPTION_TEXT);
                _okButton.SetEnabled(true);
            }
        }

        private string GetFileName(string name) {
            return name + VERSION + ".zip";
        }
    }
}