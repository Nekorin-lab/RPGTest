using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Map;
using RPGMaker.Codebase.CoreSystem.Knowledge.Misc;
using RPGMaker.Codebase.CoreSystem.Service.MapManagement.ValueObject;
using RPGMaker.Codebase.CoreSystem.Service.DatabaseManagement;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using System.Threading.Tasks;
using RPGMaker.Codebase.CoreSystem.Knowledge.JsonStructure;
using System.Text;

namespace RPGMaker.Codebase.CoreSystem.Service.MapManagement.Repository
{
    public class TileRepository
    {
        private const string PROCESS_TEXT        = "WORD_0101";
        private const string CLOSE_TEXT          = "WORD_0102";
        private const string SIZE_LACK_TEXT      = "WORD_0103";
        private const string ADDRESS_LACK_TEXT   = "WORD_0104";
        private const string ADDRESS_EXCESS_TEXT = "WORD_0105";
        private const string YES_TEXT            = "WORD_0106";
        private const string NO_TEXT             = "WORD_0107";

        public const int TileDefaultSize = 98;

        private const string TileAssetFolderPath       = "Assets/RPGMaker/Storage/Map/TileAssets";
        private const string SystemTileAssetFolderPath = "Assets/RPGMaker/Storage/System/Map/";

        private static bool                _cacheUsable; // falseの場合キャッシュを利用しない
        private static List<TileDataModel> _tileDataModels;
        private static List<TileDataModel> _systemTileDataModels;

        // タイル分割用
        private static int _separeteAddress;
        private static bool _isWarningMessage;

#if UNITY_EDITOR
        private const string JsonFileTranslation = "Assets/RPGMaker/Storage/Map/JSON/tilename.json";
#endif

        /**
         * 登録済みの全タイルを取得する
         */
        public List<TileDataModel> GetTileEntities() {
            var tiles = new List<TileDataModel>();
            if (_tileDataModels != null && _systemTileDataModels != null && _cacheUsable) 
            {
                // タイルデータを統合して返す
                tiles = new List<TileDataModel>(_tileDataModels);
                tiles.AddRange(new List<TileDataModel>(_systemTileDataModels));
                return tiles;
            }

            _separeteAddress = 1;

            GetTiles();
            tiles = new List<TileDataModel>(GetSystemTiles());

#if ENABLE_DEVELOPMENT_FIX
            if (!File.Exists("Assets/systemtileinitialize.txt") || _systemTileDataModels.Count == 0)
            {
                FixSystemTile();
                tiles = new List<TileDataModel>(GetSystemTiles());
            }
#endif
            // タイルデータを統合して返す
            tiles.AddRange(new List<TileDataModel>(_tileDataModels));
            _cacheUsable = true;
            return tiles;
        }

        /**
         * 通常タイルを取得する
         */
        private List<TileDataModel> GetTiles() {
            if (_tileDataModels != null && _cacheUsable) return _tileDataModels;

#if UNITY_EDITOR
            _tileDataModels = Directory.GetFiles(TileAssetFolderPath)
                .Select(assetPath => UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<TileDataModel>(assetPath))
                .Where(tileAsset => tileAsset != null)
                .ToList()
                .OrderBy(tile => tile.serialNumber).ToList();
#else
            _tileDataModels = ScriptableObjectOperator.GetClass<TileDataModel>("SO/TileAssets.asset") as List<TileDataModel>;
#endif

            SetSerialNumbers();

            return _tileDataModels;
        }

        /**
         * システムタイルを取得する
         */
        private List<TileDataModel> GetSystemTiles() {
            if (_systemTileDataModels != null && _cacheUsable) return _systemTileDataModels;

#if UNITY_EDITOR
            //リージョンタイル
            List<TileDataModel> work =  Directory.GetFiles(SystemTileAssetFolderPath + "RegionTile")
                .Select(assetPath => UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<TileDataModel>(assetPath))
                .Where(tileAsset => tileAsset != null)
                .ToList()
                .OrderBy(tile => tile.serialNumber).ToList();

            //metaファイルのコンバート
            ConvertMetaData(work, "RegionTile");

            //ここまでの読込内容を代入
            _systemTileDataModels = work;

            //背景のコリジョンタイル
            work = Directory.GetFiles(SystemTileAssetFolderPath + "BackgroundCollisionTile")
                .Select(assetPath => UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<TileDataModel>(assetPath))
                .Where(tileAsset => tileAsset != null)
                .ToList()
                .OrderBy(tile => tile.serialNumber).ToList();

            //metaファイルのコンバート
            ConvertMetaData(work, "BackgroundCollisionTile");

            //ここまでの読込内容を代入
            _systemTileDataModels.AddRange(work);

            //影
            _systemTileDataModels.Add(UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<TileDataModel>(SystemTileAssetFolderPath + "ShadowMap.asset"));

#else
            _systemTileDataModels = ScriptableObjectOperator.GetClass<TileDataModel>("SO/TileAssets.asset") as List<TileDataModel>;
#endif
            for (var i = 0; i < _systemTileDataModels.Count; i++) _systemTileDataModels[i].listNumber = i + 1;

            return _systemTileDataModels;
        }

        /**
         * 旧Storage環境からの移行時に、metaファイル内が壊れている場合に修復する
         */
        private void ConvertMetaData(List<TileDataModel> work, string pathname) {
            string path = SystemTileAssetFolderPath + pathname;
            string metadata;
            string guid = "";
            string s;
            int index;

            bool flg = false;
            for (int i = 0; i < work.Count; i++)
            {
                if (work[i].m_DefaultSprite == null)
                {
                    //このケースでは、画像（defaultSprite.png）の meta ファイル内の、guid がズレてる
                    //metaファイルを書き換えて再読込する
                    flg = true;

                    //タイル側に書き込まれているguidを取得
                    using (StreamReader sr = File.OpenText(path + "/" + work[i].id + ".asset"))
                    {
                        while ((s = sr.ReadLine()) != null)
                        {
                            if (s.IndexOf("m_DefaultSprite:") >= 0)
                            {
                                index = s.IndexOf("guid: ");
                                guid = s.Substring(index + 6, 32);
                                break;
                            }
                        }
                    }

                    //画像側に書き込まれているguidを変更
                    using (StreamReader sr = File.OpenText(path + "/" + work[i].id + "/defaultSprite.png.meta"))
                    {
                        metadata = "";
                        while ((s = sr.ReadLine()) != null)
                        {
                            if (s.StartsWith("guid: "))
                                s = "guid: " + guid;
                            metadata += s + "\n";
                        }
                    }
                    File.WriteAllText(path + "/" + work[i].id + "/defaultSprite.png.meta", metadata);

                    //再読込
                    //work[i] = UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<TileDataModel>(path + "/" + work[i].id + ".asset");
                }
            }
            if (flg)
            {
                //置き換えが発生した場合、「名前」順にソートする
                work.Sort((a, b) => a.name.CompareTo(b.name));
                //その後SerialNumberを振りなおし、保存する
                for (int i = 0; i < work.Count; i++)
                {
                    work[i].serialNumber = (i + 1);
                    SaveInspectorTile(work[i]);
                }
            }
        }

        private void FixSystemTile() {
#if UNITY_EDITOR
#if ENABLE_DEVELOPMENT_FIX
            File.WriteAllText("Assets/systemtileinitialize.txt", "");

            foreach (var til in _tileDataModels)
            {
                string path = "";
                if (til.type == TileDataModel.Type.BackgroundCollision)
                    path = "BackgroundCollisionTile/";
                else if (til.type == TileDataModel.Type.Region)
                    path = "RegionTile/";

                if (path == "") continue;

                //ファイルが既にSystem下にある場合、特に移動処理を行う必要が無いため、continueする
                if (File.Exists(SystemTileAssetFolderPath + path + til.id + ".asset") &&
                    File.Exists(SystemTileAssetFolderPath + path + til.id + ".asset.meta") &&
                    File.Exists(SystemTileAssetFolderPath + path + til.id + ".meta") &
                    Directory.Exists(SystemTileAssetFolderPath + path + til.id)) {
                    //全て存在するので continueする
                    //移動予定だったファイルやフォルダが存在する場合には、ただ消去する
                    if (File.Exists(TileAssetFolderPath + "/" + til.id + ".asset"))
                        File.Delete(TileAssetFolderPath + "/" + til.id + ".asset");
                    if (File.Exists(TileAssetFolderPath + "/" + til.id + ".asset.meta"))
                        File.Delete(TileAssetFolderPath + "/" + til.id + ".asset.meta");
                    if (File.Exists(TileAssetFolderPath + "/" + til.id + ".meta"))
                        File.Delete(TileAssetFolderPath + "/" + til.id + ".meta");
                    if (Directory.Exists(TileAssetFolderPath + "/" + til.id))
                        Directory.Delete(TileAssetFolderPath + "/" + til.id, true);
                    continue;
                }

                if (File.Exists(SystemTileAssetFolderPath + path + til.id + ".asset"))
                    File.Delete(SystemTileAssetFolderPath + path + til.id + ".asset");
                if (File.Exists(SystemTileAssetFolderPath + path + til.id + ".asset.meta"))
                    File.Delete(SystemTileAssetFolderPath + path + til.id + ".asset.meta");
                if (File.Exists(SystemTileAssetFolderPath + path + til.id + ".meta"))
                    File.Delete(SystemTileAssetFolderPath + path + til.id + ".meta");
                if (Directory.Exists(SystemTileAssetFolderPath + path + til.id))
                    Directory.Delete(SystemTileAssetFolderPath + path + til.id, true);

                File.Move(TileAssetFolderPath + "/" + til.id + ".asset", SystemTileAssetFolderPath + path + til.id + ".asset");
                File.Move(TileAssetFolderPath + "/" + til.id + ".asset.meta", SystemTileAssetFolderPath + path + til.id + ".asset.meta");
                File.Move(TileAssetFolderPath + "/" + til.id + ".meta", SystemTileAssetFolderPath + path + til.id + ".meta");
                Directory.Move(TileAssetFolderPath + "/" + til.id, SystemTileAssetFolderPath + path + til.id);
            }
            AssetDatabase.Refresh();

            GetTiles();
#endif
#endif
        }

        /**
         * タイルを保存する
         */
        public async Task<bool> StoreTileEntity(TileDataModel tileDataModel) {
            var assetPath = GetAssetPath(tileDataModel);

            if (UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<TileDataModel>(assetPath))
            {
                // 上書き
#if UNITY_EDITOR
                var data = UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<TileDataModel>(assetPath);
                data = tileDataModel;

                // アニメーション速度の設定適用
                for (var i = 0; i < data.m_TilingRules.Count; i++)
                {
                    data.m_TilingRules[i].m_MaxAnimationSpeed = data.animationSpeed;
                    data.m_TilingRules[i].m_MinAnimationSpeed = data.animationSpeed;
                }

                EditorUtility.SetDirty(data);
#endif
                UnityEditorWrapper.AssetDatabaseWrapper.SaveAssets();

                _cacheUsable = false;

                return false;
            }

            // シリアルナンバーの設定
            var tiles = GetTileEntities();
            int maxSerialNo = 0;
            for (int i = 0; i < tiles.Count; i++)
                if (tiles[i].serialNumber > maxSerialNo)
                    maxSerialNo = tiles[i].serialNumber;

            tileDataModel.serialNumber = maxSerialNo + 1;

            // 大型パーツタイルの親パーツなら専用のメソッドを呼ぶ。
            if (tileDataModel.type == TileDataModel.Type.LargeParts && tileDataModel.largePartsDataModel == null)
            {
                await GenerateLargePartsTileAssets(tileDataModel, assetPath);
                return false;
            }

            List<GenerateTileAssetData> generateTileAssetData = new List<GenerateTileAssetData>();
#if UNITY_EDITOR
            // AssetDatabaseを一時停止
            AssetDatabase.StartAssetEditing();
#endif

            _separeteAddress = 1;
            _isWarningMessage = false;
            GenerateTileAssetCreateImage(tileDataModel, assetPath, generateTileAssetData);

#if UNITY_EDITOR
            // AssetDatabaseを再開
            AssetDatabase.StopAssetEditing();
#endif

            // Refresh
            UnityEditorWrapper.AssetDatabaseWrapper.Refresh2();

            // 1ms待つ（読み込み用）
            await Task.Delay(1);

#if UNITY_EDITOR
            // AssetDatabaseを一時停止
            AssetDatabase.StartAssetEditing();
#endif

            for (int i = 0; i < generateTileAssetData.Count; i++)
            {
                for (int j = 0; j < generateTileAssetData[i].textureData.Count; j++)
                {
                    UpdateTextureImporterSettings(generateTileAssetData[i].textureData[j], generateTileAssetData[i].tileDataModel.type == TileDataModel.Type.AutoTileC);
                }
            }

#if UNITY_EDITOR
            // AssetDatabaseを再開
            AssetDatabase.StopAssetEditing();
#endif

            // Refresh
            UnityEditorWrapper.AssetDatabaseWrapper.Refresh2();

            // 1ms待つ（読み込み用）
            await Task.Delay(1);

#if UNITY_EDITOR
            // AssetDatabaseを一時停止
            AssetDatabase.StartAssetEditing();
#endif

            for (int i = 0; i < generateTileAssetData.Count; i++)
            {
                _separeteAddress = 1;
                GenerateTileAssetSprite(generateTileAssetData[i].tileDataModel, generateTileAssetData[i].assetPath, generateTileAssetData[i]);
            }

#if UNITY_EDITOR
            // AssetDatabaseを再開
            AssetDatabase.StopAssetEditing();
#endif

            // Refresh
            UnityEditorWrapper.AssetDatabaseWrapper.Refresh2();

            _cacheUsable = false;

            return true;
        }

        /**
         * タイルを保存する
         */
        public async Task<List<bool>> StoreTileEntity(List<TileDataModel> tileDataModel) {
            List<GenerateTileAssetData> generateTileAssetData = new List<GenerateTileAssetData>();

            var tiles = GetTileEntities();
            int maxSerialNo = 0;
            for (int i = 0; i < tiles.Count; i++)
                if (tiles[i].serialNumber > maxSerialNo)
                    maxSerialNo = tiles[i].serialNumber;

#if UNITY_EDITOR
            // AssetDatabaseを一時停止
            AssetDatabase.StartAssetEditing();
#endif
            _isWarningMessage = false;

            for (int cnt = 0; cnt < tileDataModel.Count; cnt++)
            {
                var assetPath = GetAssetPath(tileDataModel[cnt]);

                if (UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<TileDataModel>(assetPath))
                {
                    // 上書き
#if UNITY_EDITOR
                    var data = UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<TileDataModel>(assetPath);
                    data = tileDataModel[cnt];

                    // アニメーション速度の設定適用
                    for (var i = 0; i < data.m_TilingRules.Count; i++)
                    {
                        data.m_TilingRules[i].m_MaxAnimationSpeed = data.animationSpeed;
                        data.m_TilingRules[i].m_MinAnimationSpeed = data.animationSpeed;
                    }

                    EditorUtility.SetDirty(data);
#endif
                    UnityEditorWrapper.AssetDatabaseWrapper.SaveAssets();

                    _cacheUsable = false;

                    continue;
                }

                // シリアルナンバーの設定
                tileDataModel[cnt].serialNumber = maxSerialNo++;

                // 画像生成
                _separeteAddress = 1;
                GenerateTileAssetCreateImage(tileDataModel[cnt], assetPath, generateTileAssetData);
            }

#if UNITY_EDITOR
            // AssetDatabaseを再開
            AssetDatabase.StopAssetEditing();
#endif
            // Refresh
            UnityEditorWrapper.AssetDatabaseWrapper.Refresh2();

            // 1ms待つ（読み込み用）
            await Task.Delay(1);

#if UNITY_EDITOR
            // AssetDatabaseを一時停止
            AssetDatabase.StartAssetEditing();
#endif
            for (int i = 0; i < generateTileAssetData.Count; i++)
            {
                for (int j = 0; j < generateTileAssetData[i].textureData.Count; j++)
                {
                    UpdateTextureImporterSettings(generateTileAssetData[i].textureData[j], generateTileAssetData[i].tileDataModel.type == TileDataModel.Type.AutoTileC);
                }
            }

#if UNITY_EDITOR
            // AssetDatabaseを再開
            AssetDatabase.StopAssetEditing();
#endif
            // Refresh
            UnityEditorWrapper.AssetDatabaseWrapper.Refresh2();

            // 1ms待つ（読み込み用）
            await Task.Delay(1);

#if UNITY_EDITOR
            // AssetDatabaseを一時停止
            AssetDatabase.StartAssetEditing();
#endif

            for (int i = 0; i < generateTileAssetData.Count; i++)
            {
                _separeteAddress = 1;
                GenerateTileAssetSprite(generateTileAssetData[i].tileDataModel, generateTileAssetData[i].assetPath, generateTileAssetData[i]);
            }

#if UNITY_EDITOR
            // AssetDatabaseを再開
            AssetDatabase.StopAssetEditing();
#endif
            // Refresh
            UnityEditorWrapper.AssetDatabaseWrapper.Refresh2();

            var successList = new List<bool>();

            for (int i = 0; i < generateTileAssetData.Count; i++)
            {
                successList.Add(generateTileAssetData[i].IsSuccess);
            }
            _cacheUsable = false;

            return successList;
        }

        //タイルの保存のみ
        public void SaveInspectorTile(TileDataModel tileDataModel) {
#if UNITY_EDITOR
            EditorUtility.SetDirty(tileDataModel);
            UnityEditorWrapper.AssetDatabaseWrapper.SaveAssets();
#endif
        }

        /**
         * タイルを削除する
         */
        public void DeleteTile(TileDataModel tileDataModel) {
            _tileDataModels.Remove(tileDataModel);
            SetSerialNumbers();
        }

        public void ResetTileEntity() {
            _tileDataModels = null;
        }

        //-------------------------------------------------------------------------------------
        // private methods
        //-------------------------------------------------------------------------------------
        private void SetSerialNumbers() {
            // serial numberフィールドがあるモデルには連番を設定する
            for (var i = 0; i < _tileDataModels.Count; i++) _tileDataModels[i].listNumber = i + 1;
        }

        private class GenerateTileAssetData
        {
            public TileDataModel tileDataModel;
            public List<GenerateTextureData> textureData;
            public string assetPath;
            public bool IsSuccess;
        }

        private class GenerateTextureData
        {
            public string texturePath;
            public float spritePixelsPerUnit;
        }

        /**
         * タイル画像を生成する
         */
        private static void GenerateTileAssetCreateImage(TileDataModel tileDataModel, string assetPath, List<GenerateTileAssetData> generateTileAssetData) {
            var originalImageTex = tileDataModel.tileImageDataModel.texture;
            // タイルごとにimage格納ディレクトリを掘る
            UnityEditorWrapper.AssetDatabaseWrapper.CreateFolder(TileAssetFolderPath, tileDataModel.id);

            // :memo
            // オートタイルABCは画像の補正を考慮しない
            // 通常タイル、エフェクトは分割or拡縮前提
            // エフェクトはアニメーション前提、その他はデータにより
            GenerateTileAssetData data = new GenerateTileAssetData();
            data.tileDataModel = tileDataModel;
            data.assetPath = assetPath;
            data.textureData = new List<GenerateTextureData>();

            switch (tileDataModel.type)
            {
                case TileDataModel.Type.NormalTile:
                    var defaultSize = TileDefaultSize;

                    // サイズチェック
                    if (!CheckSize(originalImageTex, new Vector2(defaultSize, defaultSize),
                        tileDataModel.animationFrame, _separeteAddress))
                    {
                        // フォルダを削除する
                        UnityEditorWrapper.FileUtilWrapper.DeleteFileOrDirectory(
                            TileAssetFolderPath + tileDataModel.id);
                        return;
                    }

                    // 分割時の処理
                    if (tileDataModel.imageAdjustType == TileDataModel.ImageAdjustType.Split)
                    {
                        // タイルの分割数取得
                        var tileSeparate = GetTileSeparate(originalImageTex, new Vector2(defaultSize, defaultSize));

                        var slicedTexturesNormal = SliceTexture_Normal(originalImageTex, tileSeparate.x, tileSeparate.y);

                        // DefaultSpriteを設定
                        var tileSize = defaultSize - 2;
                        var texture = slicedTexturesNormal[_separeteAddress - 1];
                        if (tileDataModel.type == TileDataModel.Type.LargeParts)
                        {
                            DebugUtil.Assert(tileDataModel.hasAnimation);

                            tileSize = TileDataModel.LargePartsTileSize;
                            texture = ImageUtility.Instance.GetTextureRect(
                                texture,
                                new RectInt(
                                    (defaultSize - tileSize) / 2,
                                    (defaultSize - tileSize) / 2,
                                    tileSize,
                                    tileSize));
                        }

                        data.textureData.Add(ImportImage(
                            texture,
                            tileSize,
                            tileSize,
                            tileDataModel.id + "/defaultSprite"
                        ));

                        if (tileDataModel.hasAnimation)
                        {
                            var tileAnimSeparate = GetTileAnimationSeparate(originalImageTex,
                                new Vector2(defaultSize - 2, defaultSize - 2), tileDataModel.animationFrame);

                            // アニメーションSprite設定
                            List<GenerateTextureData> path = CreateTileAnimationImage(
                                slicedTexturesNormal,
                                new Vector2Int(defaultSize - 2, defaultSize - 2),
                                tileDataModel.animationFrame, _separeteAddress,
                                tileAnimSeparate.x,
                                tileDataModel.id,
                                tileDataModel.animationSpeed);

                            for (int i = 0; i < path.Count; i++)
                            {
                                data.textureData.Add(path[i]);
                            }
                        }

                        // 自動分割
                        if (_separeteAddress == 1 && tileDataModel.hasAnimation == false)
                        {
                            for (int y = 0; y < tileSeparate.y; y++)
                            {
                                for (int x = 0; x < tileSeparate.x; x++)
                                {
                                    if (y + x == 0)
                                        continue;

                                    _separeteAddress++;

                                    // タイルデータ作成
                                    var tileData = TileDataModel.CreateLargePartsChildTileDataModel(tileDataModel, _separeteAddress);
                                    GenerateTileAssetCreateImage(tileData, GetAssetPath(tileData), generateTileAssetData);
                                }
                            }
                        }
                    }
                    else if (tileDataModel.imageAdjustType == TileDataModel.ImageAdjustType.Scale)
                    {
                        // アニメーション設定時
                        if (tileDataModel.hasAnimation)
                        {
                            var tileSeparate = GetTileSeparate(originalImageTex,
                                new Vector2(originalImageTex.width / tileDataModel.animationFrame,
                                    originalImageTex.height));

                            var slicedTexturesNormal = SliceTexture(originalImageTex, tileSeparate.x, tileSeparate.y);

                            data.textureData.Add(ImportImage(
                                originalImageTex,
                                originalImageTex.width,
                                originalImageTex.height,
                                tileDataModel.id + "/defaultSprite"
                            ));

                            // アニメーションSprite設定
                            List<GenerateTextureData> path = CreateTileAnimationImage(
                                slicedTexturesNormal,
                                new Vector2Int(originalImageTex.width / tileDataModel.animationFrame,
                                    originalImageTex.height),
                                tileDataModel.animationFrame, 1,
                                1,
                                tileDataModel.id,
                                tileDataModel.animationSpeed);

                            for (int i = 0; i < path.Count; i++)
                            {
                                data.textureData.Add(path[i]);
                            }
                        }
                        else
                        {
                            data.textureData.Add(ImportImage(
                                originalImageTex,
                                originalImageTex.width,
                                originalImageTex.height,
                                tileDataModel.id + "/defaultSprite"
                            ));
                        }
                    }
                    else if (tileDataModel.imageAdjustType == TileDataModel.ImageAdjustType.None)
                    {
                        return;
                    }

                    break;

                case TileDataModel.Type.AutoTileA:
                    var defaultSizeAutoTileA = 48;

                    // アニメーション設定時
                    if (tileDataModel.hasAnimation)
                    {
                        // サイズチェック
                        if (!CheckSize(originalImageTex,
                            new Vector2(defaultSizeAutoTileA * 4, defaultSizeAutoTileA * 6),
                            tileDataModel.animationFrame, 1))
                        {
                            // フォルダを削除する
                            UnityEditorWrapper.FileUtilWrapper.DeleteFileOrDirectory(
                                TileAssetFolderPath + tileDataModel.id);
                            return;
                        }

                        var slicedAnimationTextures = SliceTexture(originalImageTex,
                            originalImageTex.width / (defaultSizeAutoTileA * 4), 1);
                        var slicedTextures = new List<Texture2D>();

                        for (var i = 0; i < slicedAnimationTextures.Count; i++)
                            slicedTextures.AddRange(SliceTexture(slicedAnimationTextures[i], 4, 6));

                        var oneSideSize = slicedTextures[0].width * 2;

                        // デフォルト
                        var defaultTextureIndexes = AutoTileRuleA.GetSlicedTextureIndexesOfThumbnail();
                        data.textureData.Add(ImportImage(
                            CombineFourTexturesToOne(
                                slicedTextures[defaultTextureIndexes[0]],
                                slicedTextures[defaultTextureIndexes[1]],
                                slicedTextures[defaultTextureIndexes[2]],
                                slicedTextures[defaultTextureIndexes[3]]
                            ),
                            oneSideSize,
                            oneSideSize,
                            tileDataModel.id + "/defaultSprite"
                        ));

                        foreach (var kv in AutoTileRuleA.TileShapeBySurroundings)
                        {
                            var surroundings = kv.Key; // 下・右・上・左・左下・右下・右上・左上の順でフラグが立っている配列
                            var shapeType = kv.Value;

                            var rule = new RuleTile.TilingRule
                            {
                                m_NeighborPositions = new List<Vector3Int>
                                {
                                    new Vector3Int(0, -1, 0), // 下
                                    new Vector3Int(1, 0, 0), // 右
                                    new Vector3Int(0, 1, 0), // 上
                                    new Vector3Int(-1, 0, 0), // 左
                                    new Vector3Int(-1, -1, 0), // 左下
                                    new Vector3Int(1, -1, 0), // 右下
                                    new Vector3Int(1, 1, 0), // 右上
                                    new Vector3Int(-1, 1, 0) // 左上
                                },
                                m_Neighbors = surroundings
                            };

                            var textureIndexes = AutoTileRuleA.GetSlicedTextureIndexesByShapeType(shapeType);
                            rule.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Animation;
                            rule.m_MaxAnimationSpeed = tileDataModel.animationSpeed;
                            rule.m_MinAnimationSpeed = tileDataModel.animationSpeed;

                            for (var i = 0; i < tileDataModel.animationFrame; i++)
                                data.textureData.Add(ImportImage(
                                    CombineFourTexturesToOne(
                                        slicedTextures[
                                            textureIndexes[0] + i * slicedTextures.Count /
                                            (originalImageTex.width / (defaultSizeAutoTileA * 4))],
                                        slicedTextures[
                                            textureIndexes[1] + i * slicedTextures.Count /
                                            (originalImageTex.width / (defaultSizeAutoTileA * 4))],
                                        slicedTextures[
                                            textureIndexes[2] + i * slicedTextures.Count /
                                            (originalImageTex.width / (defaultSizeAutoTileA * 4))],
                                        slicedTextures[
                                            textureIndexes[3] + i * slicedTextures.Count /
                                            (originalImageTex.width / (defaultSizeAutoTileA * 4))]
                                    ),
                                    oneSideSize,
                                    oneSideSize,
                                    tileDataModel.id + "/shape" + shapeType + "_" + i
                                ));
                        }
                    }
                    else
                    {
                        // サイズチェック
                        if (!CheckSize(originalImageTex,
                            new Vector2(defaultSizeAutoTileA * 4, defaultSizeAutoTileA * 6), 1, 1))
                        {
                            // フォルダを削除する
                            UnityEditorWrapper.FileUtilWrapper.DeleteFileOrDirectory(
                                TileAssetFolderPath + tileDataModel.id);
                            return;
                        }

                        var slicedTextures = SliceTexture(originalImageTex, 4, 6); // 192x288を横4分割・縦6分割
                        var oneSideSize = slicedTextures[0].width * 2;

                        // デフォルト
                        var defaultTextureIndexes = AutoTileRuleA.GetSlicedTextureIndexesOfThumbnail();
                        data.textureData.Add(ImportImage(
                            CombineFourTexturesToOne(
                                slicedTextures[defaultTextureIndexes[0]],
                                slicedTextures[defaultTextureIndexes[1]],
                                slicedTextures[defaultTextureIndexes[2]],
                                slicedTextures[defaultTextureIndexes[3]]
                            ),
                            oneSideSize,
                            oneSideSize,
                            tileDataModel.id + "/defaultSprite"
                        ));

                        foreach (var kv in AutoTileRuleA.TileShapeBySurroundings)
                        {
                            var surroundings = kv.Key; // 下・右・上・左・左下・右下・右上・左上の順でフラグが立っている配列
                            var shapeType = kv.Value;

                            var rule = new RuleTile.TilingRule
                            {
                                m_NeighborPositions = new List<Vector3Int>
                                {
                                    new Vector3Int(0, -1, 0), // 下
                                    new Vector3Int(1, 0, 0), // 右
                                    new Vector3Int(0, 1, 0), // 上
                                    new Vector3Int(-1, 0, 0), // 左
                                    new Vector3Int(-1, -1, 0), // 左下
                                    new Vector3Int(1, -1, 0), // 右下
                                    new Vector3Int(1, 1, 0), // 右上
                                    new Vector3Int(-1, 1, 0) // 左上
                                },
                                m_Neighbors = surroundings
                            };

                            var textureIndexes = AutoTileRuleA.GetSlicedTextureIndexesByShapeType(shapeType);
                            data.textureData.Add(ImportImage(
                                CombineFourTexturesToOne(
                                    slicedTextures[textureIndexes[0]],
                                    slicedTextures[textureIndexes[1]],
                                    slicedTextures[textureIndexes[2]],
                                    slicedTextures[textureIndexes[3]]
                                ),
                                oneSideSize,
                                oneSideSize,
                                tileDataModel.id + "/shape" + shapeType
                            ));
                        }
                    }

                    break;

                case TileDataModel.Type.AutoTileB:
                    var defaultSizeAutoTileB = 48;

                    // アニメーション設定時
                    if (tileDataModel.hasAnimation)
                    {
                        // サイズチェック
                        if (!CheckSize(originalImageTex,
                            new Vector2(defaultSizeAutoTileB * 4 + 2, defaultSizeAutoTileB * 4 + 2),
                            tileDataModel.animationFrame, 1))
                        {
                            // フォルダを削除する
                            UnityEditorWrapper.FileUtilWrapper.DeleteFileOrDirectory(
                                TileAssetFolderPath + tileDataModel.id);
                            return;
                        }

                        var slicedAnimationTexturesB = SliceTexture(originalImageTex,
                            originalImageTex.width / (defaultSizeAutoTileB * 4), 1);
                        var slicedTexturesB = new List<Texture2D>();

                        for (var i = 0; i < slicedAnimationTexturesB.Count; i++)
                            slicedTexturesB.AddRange(SliceTexture_AnimatinAutoTileB(slicedAnimationTexturesB[i]));

                        var oneSideSizeB = slicedTexturesB[0].width * 2;

                        // デフォルト
                        var defaultTextureIndexesB = AutoTileRuleB.GetSlicedTextureIndexesOfThumbnail();
                        data.textureData.Add(ImportImage(
                            CombineFourTexturesToOne(
                                slicedTexturesB[defaultTextureIndexesB[0]],
                                slicedTexturesB[defaultTextureIndexesB[1]],
                                slicedTexturesB[defaultTextureIndexesB[2]],
                                slicedTexturesB[defaultTextureIndexesB[3]]
                            ),
                            oneSideSizeB,
                            oneSideSizeB,
                            tileDataModel.id + "/defaultSprite"
                        ));

                        foreach (var kv in AutoTileRuleB.TileShapeBySurroundings)
                        {
                            var surroundings = kv.Key; // 下・右・上・左の順でフラグが立っている配列
                            var shapeType = kv.Value;

                            var rule = new RuleTile.TilingRule
                            {
                                m_NeighborPositions = new List<Vector3Int>
                                {
                                    new Vector3Int(0, -1, 0), // 下
                                    new Vector3Int(1, 0, 0), // 右
                                    new Vector3Int(0, 1, 0), // 上
                                    new Vector3Int(-1, 0, 0) // 左
                                },
                                m_Neighbors = surroundings
                            };

                            var textureIndexes = AutoTileRuleB.GetSlicedTextureIndexesByShapeType(shapeType);
                            rule.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Animation;
                            rule.m_MaxAnimationSpeed = tileDataModel.animationSpeed;
                            rule.m_MinAnimationSpeed = tileDataModel.animationSpeed;

                            for (var i = 0; i < tileDataModel.animationFrame; i++)
                            {
                                data.textureData.Add(ImportImage(
                                    CombineFourTexturesToOne(
                                        slicedTexturesB[
                                            textureIndexes[0] + i * slicedTexturesB.Count /
                                            (originalImageTex.width / (defaultSizeAutoTileB * 4))],
                                        slicedTexturesB[
                                            textureIndexes[1] + i * slicedTexturesB.Count /
                                            (originalImageTex.width / (defaultSizeAutoTileB * 4))],
                                        slicedTexturesB[
                                            textureIndexes[2] + i * slicedTexturesB.Count /
                                            (originalImageTex.width / (defaultSizeAutoTileB * 4))],
                                        slicedTexturesB[
                                            textureIndexes[3] + i * slicedTexturesB.Count /
                                            (originalImageTex.width / (defaultSizeAutoTileB * 4))]
                                    ),
                                    oneSideSizeB,
                                    oneSideSizeB,
                                    tileDataModel.id + "/shape" + shapeType + "_" + i
                                ));
                            }
                        }
                    }
                    else
                    {
                        // サイズチェック
                        if (!CheckSize(originalImageTex,
                            new Vector2(defaultSizeAutoTileB * 4, defaultSizeAutoTileB * 4), 1, 1))
                        {
                            // フォルダを削除する
                            UnityEditorWrapper.FileUtilWrapper.DeleteFileOrDirectory(
                                TileAssetFolderPath + tileDataModel.id);
                            return;
                        }

                        var slicedTexturesB = SliceTexture(originalImageTex, 4, 4); // 192x192を横4分割・縦4分割
                        var oneSideSizeB = slicedTexturesB[0].width * 2;

                        // デフォルト
                        var defaultTextureIndexesB = AutoTileRuleB.GetSlicedTextureIndexesOfThumbnail();
                        data.textureData.Add(ImportImage(
                            CombineFourTexturesToOne(
                                slicedTexturesB[defaultTextureIndexesB[0]],
                                slicedTexturesB[defaultTextureIndexesB[1]],
                                slicedTexturesB[defaultTextureIndexesB[2]],
                                slicedTexturesB[defaultTextureIndexesB[3]]
                            ),
                            oneSideSizeB,
                            oneSideSizeB,
                            tileDataModel.id + "/defaultSprite"
                        ));

                        foreach (var kv in AutoTileRuleB.TileShapeBySurroundings)
                        {
                            var surroundings = kv.Key; // 下・右・上・左の順でフラグが立っている配列
                            var shapeType = kv.Value;

                            var rule = new RuleTile.TilingRule
                            {
                                m_NeighborPositions = new List<Vector3Int>
                                {
                                    new Vector3Int(0, -1, 0), // 下
                                    new Vector3Int(1, 0, 0), // 右
                                    new Vector3Int(0, 1, 0), // 上
                                    new Vector3Int(-1, 0, 0) // 左
                                },
                                m_Neighbors = surroundings
                            };

                            var textureIndexes = AutoTileRuleB.GetSlicedTextureIndexesByShapeType(shapeType);
                            data.textureData.Add(ImportImage(
                                CombineFourTexturesToOne(
                                    slicedTexturesB[textureIndexes[0]],
                                    slicedTexturesB[textureIndexes[1]],
                                    slicedTexturesB[textureIndexes[2]],
                                    slicedTexturesB[textureIndexes[3]]
                                ),
                                oneSideSizeB,
                                oneSideSizeB,
                                tileDataModel.id + "/shape" + shapeType
                            ));
                        }
                    }

                    break;

                case TileDataModel.Type.AutoTileC:
                    // 基本サイズは192:208
                    var defaultSizeAutoTileC = 48;

                    // サイズチェック
                    if (!CheckSize(originalImageTex,
                        new Vector2(defaultSizeAutoTileC * 4, defaultSizeAutoTileC * 4 + 16), 1, 1))
                    {
                        // フォルダを削除する
                        UnityEditorWrapper.FileUtilWrapper.DeleteFileOrDirectory(
                            TileAssetFolderPath + tileDataModel.id);
                        return;
                    }

                    var slicedTexturesC = SliceTexture_FixedSize(originalImageTex, defaultSizeAutoTileC,
                        defaultSizeAutoTileC, true); // 192x192を横4分割・縦4分割

                    // 脚部分を別途作成する
                    for (var i = 0; i < 4; i++)
                    {
                        var slicedTexturesC_leg_color = originalImageTex.GetPixels(defaultSizeAutoTileC * i, 0,
                            defaultSizeAutoTileC, defaultSizeAutoTileC + 16);
                        var slicedTexturesC_leg = new Texture2D(defaultSizeAutoTileC, defaultSizeAutoTileC + 16);
                        slicedTexturesC_leg.SetPixels(slicedTexturesC_leg_color);

                        slicedTexturesC[12 + i] = slicedTexturesC_leg;
                    }

                    // デフォルト
                    var defaultTextureIndexesC = AutoTileRuleB.GetSlicedTextureIndexesOfThumbnail();
                    data.textureData.Add(ImportImage(
                        CombineFourTexturesToOne_DifferentSize(
                            slicedTexturesC[defaultTextureIndexesC[0]],
                            slicedTexturesC[defaultTextureIndexesC[1]],
                            slicedTexturesC[defaultTextureIndexesC[2]],
                            slicedTexturesC[defaultTextureIndexesC[3]]
                        ),
                        (slicedTexturesC[defaultTextureIndexesC[0]].width +
                         slicedTexturesC[defaultTextureIndexesC[1]].width +
                         slicedTexturesC[defaultTextureIndexesC[2]].width +
                         slicedTexturesC[defaultTextureIndexesC[3]].width) / 2,
                        (slicedTexturesC[defaultTextureIndexesC[0]].height +
                         slicedTexturesC[defaultTextureIndexesC[1]].height +
                         slicedTexturesC[defaultTextureIndexesC[2]].height +
                         slicedTexturesC[defaultTextureIndexesC[3]].height) / 2,
                        tileDataModel.id + "/defaultSprite"
                    ));

                    foreach (var kv in AutoTileRuleB.TileShapeBySurroundings)
                    {
                        var surroundings = kv.Key; // 下・右・上・左の順でフラグが立っている配列
                        var shapeType = kv.Value;

                        var rule = new RuleTile.TilingRule
                        {
                            m_NeighborPositions = new List<Vector3Int>
                            {
                                new Vector3Int(0, -1, 0), // 下
                                new Vector3Int(1, 0, 0), // 右
                                new Vector3Int(0, 1, 0), // 上
                                new Vector3Int(-1, 0, 0) // 左
                            },
                            m_Neighbors = surroundings
                        };

                        var textureIndexes = AutoTileRuleB.GetSlicedTextureIndexesByShapeType(shapeType);
                        // タイルの幅高さ
                        var tileWidth =
                            (slicedTexturesC[textureIndexes[0]].width +
                             slicedTexturesC[textureIndexes[1]].width +
                             slicedTexturesC[textureIndexes[2]].width +
                             slicedTexturesC[textureIndexes[3]].width) / 2;
                        var tileHeight =
                            (slicedTexturesC[textureIndexes[0]].height +
                             slicedTexturesC[textureIndexes[1]].height +
                             slicedTexturesC[textureIndexes[2]].height +
                             slicedTexturesC[textureIndexes[3]].height) / 2;

                        data.textureData.Add(ImportImage(
                            CombineFourTexturesToOne_DifferentSize(
                                slicedTexturesC[textureIndexes[0]],
                                slicedTexturesC[textureIndexes[1]],
                                slicedTexturesC[textureIndexes[2]],
                                slicedTexturesC[textureIndexes[3]]
                            ),
                            tileWidth,
                            tileHeight,
                            tileDataModel.id + "/shape" + shapeType
                        ));
                    }

                    break;

                case TileDataModel.Type.LargeParts:
                    {
                        var tileSize = TileDataModel.LargePartsTileSize;

                        // アニメーション設定時
                        if (!tileDataModel.hasAnimation)
                        {
                            tileDataModel.tileImageDataModel.texture =
                                ImageUtility.Instance.GetTextureRect(
                                    originalImageTex,
                                    new RectInt(
                                        tileDataModel.largePartsDataModel.x * tileSize,
                                        tileDataModel.largePartsDataModel.y * tileSize,
                                        tileSize,
                                        tileSize));

                            var texture = tileDataModel.tileImageDataModel.texture;
                            data.textureData.Add(ImportImage(
                                texture,
                                texture.width,
                                texture.height,
                                tileDataModel.id + "/defaultSprite"
                            ));
                        }

                        break;
                    }

                case TileDataModel.Type.Effect:
                    // エフェクトタイルタイプは、以下の設定状態のノーマルタイルタイプ扱い。
                    // ・「画像の補正」が「分割」のみ。
                    // ・「アニメーション」が「する」固定。
                    tileDataModel.imageAdjustType = TileDataModel.ImageAdjustType.Split;
                    tileDataModel.hasAnimation = true;
                    goto case TileDataModel.Type.NormalTile;

                case TileDataModel.Type.Region:
                    data.textureData.Add(ImportImage(
                        originalImageTex,
                        originalImageTex.width,
                        originalImageTex.height,
                        tileDataModel.id + "/defaultSprite"
                    ));
                    break;

                case TileDataModel.Type.BackgroundCollision:
                    data.textureData.Add(ImportImage(
                        originalImageTex,
                        originalImageTex.width,
                        originalImageTex.height,
                        tileDataModel.id + "/defaultSprite"
                    ));
                    break;

                default:
                    return;
            }
            generateTileAssetData.Add(data);
        }

        /**
         * タイルを構成するアセットファイルを生成・保存する
         */
        private static void GenerateTileAssetSprite(TileDataModel tileDataModel, string assetPath, GenerateTileAssetData generateTileAssetData) {
            var originalImageTex = tileDataModel.tileImageDataModel.texture;

            generateTileAssetData.IsSuccess = false;

            // :memo
            // オートタイルABCは画像の補正を考慮しない
            // 通常タイル、エフェクトは分割or拡縮前提
            // エフェクトはアニメーション前提、その他はデータにより
            int index = 0;
            switch (tileDataModel.type)
            {
                case TileDataModel.Type.NormalTile:
                    var defaultSize = TileDefaultSize;

                    // 分割時の処理
                    if (tileDataModel.imageAdjustType == TileDataModel.ImageAdjustType.Split)
                    {
                        // タイルの分割数取得
                        var tileSeparate = GetTileSeparate(originalImageTex, new Vector2(defaultSize, defaultSize));

                        tileDataModel.m_DefaultSprite = ImportSpriteFromPath(
                            generateTileAssetData.textureData[index++]
                        );

                        if (tileDataModel.hasAnimation)
                        {
                            var tileAnimSeparate = GetTileAnimationSeparate(originalImageTex,
                                new Vector2(defaultSize - 2, defaultSize - 2), tileDataModel.animationFrame);

                            // アニメーションSprite設定
                            var rule = CreateTileAnimationSpritesPath(
                                generateTileAssetData.textureData,
                                tileDataModel.animationFrame,
                                tileDataModel.animationSpeed);
                            tileDataModel.m_TilingRules.Add(rule);
                        }
                    }
                    else if (tileDataModel.imageAdjustType == TileDataModel.ImageAdjustType.Scale)
                    {
                        // アニメーション設定時
                        if (tileDataModel.hasAnimation)
                        {
                            tileDataModel.m_DefaultSprite = ImportSpriteFromPath(
                                generateTileAssetData.textureData[index++]
                            );

                            // アニメーションSprite設定
                            var rule = CreateTileAnimationSpritesPath(
                                generateTileAssetData.textureData,
                                tileDataModel.animationFrame,
                                tileDataModel.animationSpeed);

                            tileDataModel.m_TilingRules.Add(rule);
                        }
                        else
                        {
                            tileDataModel.m_DefaultSprite = ImportSpriteFromPath(
                                generateTileAssetData.textureData[index++]
                            );
                        }
                    }
                    else if (tileDataModel.imageAdjustType == TileDataModel.ImageAdjustType.None)
                    {
                        return;
                    }

                    break;

                case TileDataModel.Type.AutoTileA:
                    // アニメーション設定時
                    if (tileDataModel.hasAnimation)
                    {
                        // デフォルト
                        tileDataModel.m_DefaultSprite = ImportSpriteFromPath(
                            generateTileAssetData.textureData[index++]
                        );

                        foreach (var kv in AutoTileRuleA.TileShapeBySurroundings)
                        {
                            var surroundings = kv.Key; // 下・右・上・左・左下・右下・右上・左上の順でフラグが立っている配列

                            var rule = new RuleTile.TilingRule
                            {
                                m_NeighborPositions = new List<Vector3Int>
                                {
                                    new Vector3Int(0, -1, 0), // 下
                                    new Vector3Int(1, 0, 0), // 右
                                    new Vector3Int(0, 1, 0), // 上
                                    new Vector3Int(-1, 0, 0), // 左
                                    new Vector3Int(-1, -1, 0), // 左下
                                    new Vector3Int(1, -1, 0), // 右下
                                    new Vector3Int(1, 1, 0), // 右上
                                    new Vector3Int(-1, 1, 0) // 左上
                                },
                                m_Neighbors = surroundings
                            };
                            rule.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Animation;
                            rule.m_MaxAnimationSpeed = tileDataModel.animationSpeed;
                            rule.m_MinAnimationSpeed = tileDataModel.animationSpeed;

                            var sprites = new List<Sprite>();
                            for (var i = 0; i < tileDataModel.animationFrame; i++)
                            {
                                sprites.Add(ImportSpriteFromPath(
                                    generateTileAssetData.textureData[index++]
                                ));
                            }
                            rule.m_Sprites = sprites.ToArray();
                            tileDataModel.m_TilingRules.Add(rule);
                        }
                    }
                    else
                    {
                        // デフォルト
                        tileDataModel.m_DefaultSprite = ImportSpriteFromPath(
                            generateTileAssetData.textureData[index++]
                        );

                        foreach (var kv in AutoTileRuleA.TileShapeBySurroundings)
                        {
                            var surroundings = kv.Key; // 下・右・上・左・左下・右下・右上・左上の順でフラグが立っている配列

                            var rule = new RuleTile.TilingRule
                            {
                                m_NeighborPositions = new List<Vector3Int>
                                {
                                    new Vector3Int(0, -1, 0), // 下
                                    new Vector3Int(1, 0, 0), // 右
                                    new Vector3Int(0, 1, 0), // 上
                                    new Vector3Int(-1, 0, 0), // 左
                                    new Vector3Int(-1, -1, 0), // 左下
                                    new Vector3Int(1, -1, 0), // 右下
                                    new Vector3Int(1, 1, 0), // 右上
                                    new Vector3Int(-1, 1, 0) // 左上
                                },
                                m_Neighbors = surroundings
                            };
                            rule.m_Sprites = new[]
                            {
                                ImportSpriteFromPath(
                                    generateTileAssetData.textureData[index++]
                                )
                            };
                            tileDataModel.m_TilingRules.Add(rule);
                        }
                    }

                    break;

                case TileDataModel.Type.AutoTileB:
                    // アニメーション設定時
                    if (tileDataModel.hasAnimation)
                    {
                        // デフォルト
                        tileDataModel.m_DefaultSprite = ImportSpriteFromPath(
                            generateTileAssetData.textureData[index++]
                        );

                        foreach (var kv in AutoTileRuleB.TileShapeBySurroundings)
                        {
                            var surroundings = kv.Key; // 下・右・上・左の順でフラグが立っている配列

                            var rule = new RuleTile.TilingRule
                            {
                                m_NeighborPositions = new List<Vector3Int>
                                {
                                    new Vector3Int(0, -1, 0), // 下
                                    new Vector3Int(1, 0, 0), // 右
                                    new Vector3Int(0, 1, 0), // 上
                                    new Vector3Int(-1, 0, 0) // 左
                                },
                                m_Neighbors = surroundings
                            };
                            rule.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Animation;
                            rule.m_MaxAnimationSpeed = tileDataModel.animationSpeed;
                            rule.m_MinAnimationSpeed = tileDataModel.animationSpeed;

                            var sprites = new List<Sprite>();
                            for (var i = 0; i < tileDataModel.animationFrame; i++)
                            {
                                sprites.Add(ImportSpriteFromPath(
                                    generateTileAssetData.textureData[index++]
                                ));
                            }

                            rule.m_Sprites = sprites.ToArray();
                            tileDataModel.m_TilingRules.Add(rule);
                        }
                    }
                    else
                    {
                        // デフォルト
                        tileDataModel.m_DefaultSprite = ImportSpriteFromPath(
                            generateTileAssetData.textureData[index++]
                        );

                        foreach (var kv in AutoTileRuleB.TileShapeBySurroundings)
                        {
                            var surroundings = kv.Key; // 下・右・上・左の順でフラグが立っている配列

                            var rule = new RuleTile.TilingRule
                            {
                                m_NeighborPositions = new List<Vector3Int>
                                {
                                    new Vector3Int(0, -1, 0), // 下
                                    new Vector3Int(1, 0, 0), // 右
                                    new Vector3Int(0, 1, 0), // 上
                                    new Vector3Int(-1, 0, 0) // 左
                                },
                                m_Neighbors = surroundings
                            };

                            rule.m_Sprites = new[]
                            {
                                ImportSpriteFromPath(
                                    generateTileAssetData.textureData[index++]
                                )
                            };
                            tileDataModel.m_TilingRules.Add(rule);
                        }
                    }

                    break;

                case TileDataModel.Type.AutoTileC:
                    // デフォルト
                    tileDataModel.m_DefaultSprite = ImportSpriteFromPath(
                        generateTileAssetData.textureData[index++]
                    );

                    foreach (var kv in AutoTileRuleB.TileShapeBySurroundings)
                    {
                        var surroundings = kv.Key; // 下・右・上・左の順でフラグが立っている配列

                        var rule = new RuleTile.TilingRule
                        {
                            m_NeighborPositions = new List<Vector3Int>
                            {
                                new Vector3Int(0, -1, 0), // 下
                                new Vector3Int(1, 0, 0), // 右
                                new Vector3Int(0, 1, 0), // 上
                                new Vector3Int(-1, 0, 0) // 左
                            },
                            m_Neighbors = surroundings
                        };

                        rule.m_Sprites = new[]
                        {
                            ImportSpriteFromPath(
                                generateTileAssetData.textureData[index++]
                            )
                        };
                        tileDataModel.m_TilingRules.Add(rule);
                        tileDataModel.isCounter = true;
                    }

                    break;

                case TileDataModel.Type.LargeParts:
                    {
                        var tileSize = TileDataModel.LargePartsTileSize;

                        // アニメーション設定時
                        if (tileDataModel.hasAnimation)
                        {
                            var animeTileSize = 98;
                            var withoutAnimeWidth = tileDataModel.tileImageDataModel.texture.width /
                                                    tileDataModel.animationFrame;
                            var centerOffset = (animeTileSize - tileSize) / 2;

                            var texture = new Texture2D(animeTileSize * tileDataModel.animationFrame, animeTileSize,
                                originalImageTex.format, false);
                            ImageUtility.Instance.FillTexture(texture, Color.clear);

                            for (var animeIndex = 0; animeIndex < tileDataModel.animationFrame; animeIndex++)
                                ImageUtility.Instance.CopyTextureRect(
                                    originalImageTex,
                                    new RectInt(
                                        tileDataModel.largePartsDataModel.x * tileSize + withoutAnimeWidth * animeIndex,
                                        tileDataModel.largePartsDataModel.y * tileSize,
                                        tileSize,
                                        tileSize),
                                    texture,
                                    new Vector2Int(centerOffset + animeTileSize * animeIndex, centerOffset));

                            tileDataModel.tileImageDataModel.texture = texture;

#if DEBUG
                            DebugUtil.SaveTextureToPng(texture,
                                $"AnimeParts[{tileDataModel.largePartsDataModel.y}][{tileDataModel.largePartsDataModel.x}]");
#endif

                            // 以下の設定状態のノーマルタイルタイプ扱い。
                            // ・「画像の補正」が「分割」。
                            // ・「アニメーション」が「する」。
                            tileDataModel.imageAdjustType = TileDataModel.ImageAdjustType.Split;
                            originalImageTex = tileDataModel.tileImageDataModel.texture;
                            goto case TileDataModel.Type.NormalTile;
                        }
                        else
                        {
                            tileDataModel.m_DefaultSprite = ImportSpriteFromPath(
                                generateTileAssetData.textureData[index++]
                            );
                        }

                        break;
                    }

                case TileDataModel.Type.Effect:
                    // エフェクトタイルタイプは、以下の設定状態のノーマルタイルタイプ扱い。
                    // ・「画像の補正」が「分割」のみ。
                    // ・「アニメーション」が「する」固定。
                    tileDataModel.imageAdjustType = TileDataModel.ImageAdjustType.Split;
                    tileDataModel.hasAnimation = true;
                    goto case TileDataModel.Type.NormalTile;

                case TileDataModel.Type.Region:
                    tileDataModel.m_DefaultSprite = ImportSpriteFromPath(
                        generateTileAssetData.textureData[index++]
                    );
                    break;

                case TileDataModel.Type.BackgroundCollision:
                    tileDataModel.m_DefaultSprite = ImportSpriteFromPath(
                        generateTileAssetData.textureData[index++]
                    );
                    break;

                default:
                    return;
            }

            UnityEditorWrapper.AssetDatabaseWrapper.CreateAsset(tileDataModel, assetPath);
#if UNITY_EDITOR
            AddressableManager.Path.SetAddressToAsset(assetPath);
#endif
            generateTileAssetData.IsSuccess = true;
        }

        /**
         * 大型パーツタイルを構成するアセットファイル群を生成・保存する
         */
        private static async Task GenerateLargePartsTileAssets(TileDataModel tileDataModel, string assetPath) {
            var tileSize = 96;

            var texture = tileDataModel.tileImageDataModel.texture;

            // サイズチェック
            if (!CheckSize(texture, new Vector2(tileSize, tileSize), tileDataModel.animationFrame, 1))
            {
                // フォルダを削除する
                UnityEditorWrapper.FileUtilWrapper.DeleteFileOrDirectory(TileAssetFolderPath + tileDataModel.id);
                return;
            }

#if UNITY_EDITOR
            // AssetDatabaseを一時停止
            AssetDatabase.StartAssetEditing();
#endif
            List<GenerateTileAssetData> generateTileAssetData = new List<GenerateTileAssetData>();

            var animationFrame = tileDataModel.animationFrame == 0 ? 1 : tileDataModel.animationFrame;
            var parentTileDataModel = tileDataModel;
            for (var y = 0; y < texture.height / tileSize; y++)
            {
                for (var x = 0; x < texture.width / animationFrame / tileSize; x++)
                {
                    // タイルデータを生成。
                    tileDataModel = TileDataModel.CreateLargePartsChildTileDataModel(parentTileDataModel);
                    tileDataModel.largePartsDataModel = new LargePartsDataModel(parentTileDataModel.id, x, y);

                    // タイルアセットを生成。
                    _separeteAddress = 1;
                    GenerateTileAssetCreateImage(tileDataModel, GetAssetPath(tileDataModel), generateTileAssetData);
                }
            }

#if UNITY_EDITOR
            // AssetDatabaseを再開
            AssetDatabase.StopAssetEditing();
#endif
            // Refresh
            UnityEditorWrapper.AssetDatabaseWrapper.Refresh2();

            // 1ms待つ（読み込み用）
            await Task.Delay(1);

#if UNITY_EDITOR
            // AssetDatabaseを一時停止
            AssetDatabase.StartAssetEditing();
#endif
            for (int i = 0; i < generateTileAssetData.Count; i++)
            {
                for (int j = 0; j < generateTileAssetData[i].textureData.Count; j++)
                {
                    UpdateTextureImporterSettings(generateTileAssetData[i].textureData[j], generateTileAssetData[i].tileDataModel.type == TileDataModel.Type.AutoTileC);
                }
            }

#if UNITY_EDITOR
            // AssetDatabaseを再開
            AssetDatabase.StopAssetEditing();
#endif
            // Refresh
            UnityEditorWrapper.AssetDatabaseWrapper.Refresh2();

#if UNITY_EDITOR
            // AssetDatabaseを一時停止
            AssetDatabase.StartAssetEditing();
#endif
            for (int i = 0; i < generateTileAssetData.Count; i++)
            {
                _separeteAddress = 1;
                GenerateTileAssetSprite(generateTileAssetData[i].tileDataModel, generateTileAssetData[i].assetPath, generateTileAssetData[i]);
            }

#if UNITY_EDITOR
            // AssetDatabaseを再開
            AssetDatabase.StopAssetEditing();
#endif
            // Refresh
            UnityEditorWrapper.AssetDatabaseWrapper.Refresh2();

            _cacheUsable = false;
        }

        // 画像サイズの確認
        // 元テクスチャ、1タイルのサイズ、アニメーション数、分割数
        private static bool CheckSize(Texture2D texture, Vector2 tileSize, int animationNum, int separateNum) {
#if UNITY_EDITOR
            if (_isWarningMessage) return true;

            var tileSeparate = GetTileSeparate(texture, tileSize);
            var tileAnimSeparate = GetTileAnimationSeparate(texture, tileSize, animationNum);

            // 画像数がない
            if (tileSeparate.x * tileSeparate.y == 0)
            {
                EditorUtility.DisplayDialog(CoreSystemLocalize.LocalizeText(PROCESS_TEXT), CoreSystemLocalize.LocalizeText(SIZE_LACK_TEXT), CoreSystemLocalize.LocalizeText(CLOSE_TEXT));
                return false;
            }

            // 画像サイズが割り切れない
            if (texture.width % tileSize.x != 0 ||
                texture.height % tileSize.y != 0)
            {
                if (!EditorUtility.DisplayDialog(CoreSystemLocalize.LocalizeText(PROCESS_TEXT), CoreSystemLocalize.LocalizeText(ADDRESS_EXCESS_TEXT).Replace("%1", tileSize.x.ToString()).Replace("%2", tileSize.y.ToString()), CoreSystemLocalize.LocalizeText(YES_TEXT), CoreSystemLocalize.LocalizeText(NO_TEXT)))
                    return false;

                // 一度警告を表示した場合は、一連の処理が終了するまでの間は表示しない
                _isWarningMessage = true;
            }

            // 指定番地の画像がなく、作成できない
            if (tileAnimSeparate.x * tileAnimSeparate.y < separateNum)
            {
                EditorUtility.DisplayDialog(CoreSystemLocalize.LocalizeText(PROCESS_TEXT), CoreSystemLocalize.LocalizeText(ADDRESS_LACK_TEXT), CoreSystemLocalize.LocalizeText(CLOSE_TEXT));
                return false;
            }
#endif
            return true;
        }

        // 指定アニメーション数で分割時のタイル数を取得する
        private static Vector2Int GetTileAnimationSeparate(Texture2D texture, Vector2 tileSize, int animationNum) {
            // 0割り防止
            if (animationNum == 0)
                animationNum = 1;

            // 分割時の枚数取得
            var animation_x = texture.width / animationNum;
            var animation_y = texture.height;
            var x = (int) (animation_x / tileSize.x);
            var y = (int) (animation_y / tileSize.y);

            return new Vector2Int(x, y);
        }

        // タイル画像群作成処理
        private static List<GenerateTextureData> CreateTileAnimationImage(
            List<Texture2D> slicedTex,
            Vector2Int tileSize,
            int animationFrame,
            int separeteAddress,
            int separeteAnimation,
            string assetId,
            int animationSpeed
        ) {
            var data = new List<GenerateTextureData>();
            for (var i = 0; i < animationFrame; i++)
            {
                var textureData = ImportImage(
                    slicedTex[separeteAddress - 1 + separeteAnimation * i],
                    tileSize.x,
                    tileSize.y,
                    assetId + "/shape" + i
                );
                data.Add(textureData);
            }
            return data;
        }

        // タイル画像群作成処理
        private static RuleTile.TilingRule CreateTileAnimationSpritesPath(
            List<GenerateTextureData> target,
            int animationFrame,
            int animationSpeed
        ) {
            var sprites = new List<Sprite>();
            for (var i = 0; i < animationFrame; i++)
            {
                var tex = ImportSpriteFromPath(
                    target[i + 1]
                );
                sprites.Add(tex);
            }

            var rule = new RuleTile.TilingRule();
            rule.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Animation;
            rule.m_MaxAnimationSpeed = animationSpeed;
            rule.m_MinAnimationSpeed = animationSpeed;
            rule.m_Sprites = sprites.ToArray();
            return rule;
        }

        // 指定タイルサイズで分割時のタイル数を取得する
        private static Vector2Int GetTileSeparate(Texture2D texture, Vector2 tileSize) {
            // 分割時の枚数取得
            var x = (int) (texture.width / tileSize.x);
            var y = (int) (texture.height / tileSize.y);

            return new Vector2Int(x, y);
        }

        private static GenerateTextureData ImportImage(
            Texture2D texture,
            int width,
            int height,
            string path
        ) {
            var targetPath = TileAssetFolderPath + "/" + path + ".png";
            var existing = UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<Sprite>(targetPath);
            if (existing != null) return null;

            // sprite生成
            Sprite sprite;
            try
            {
                sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, width, height),
                    new Vector2(0.5f, 0.5f),
                    width
                );
            }
            catch (Exception)
            {
                sprite = Sprite.Create(
                    new Texture2D(width, height, DefaultFormat.LDR, TextureCreationFlags.None),
                    new Rect(0, 0, width, height),
                    new Vector2(0.5f, 0.5f),
                    width
                );
            }

            // テクスチャをpngとして保存
            File.WriteAllBytes(targetPath, sprite.texture.EncodeToPNG());

            GenerateTextureData data = new GenerateTextureData();
            data.texturePath = targetPath;
            data.spritePixelsPerUnit = sprite.pixelsPerUnit;

            return data;
        }

        private static void UpdateTextureImporterSettings(
            GenerateTextureData target,
            bool tileC = false
        ) {
#if UNITY_EDITOR
            // テクスチャインポーターで、改めてpngからspriteをインポート
            var textureImporter = AssetImporter.GetAtPath(target.texturePath) as TextureImporter;
            textureImporter.spritePixelsPerUnit = target.spritePixelsPerUnit;

            textureImporter.mipmapEnabled = false;
            textureImporter.textureType = TextureImporterType.Sprite;
            if (tileC)
            {
                var texSettings = new TextureImporterSettings();
                textureImporter.ReadTextureSettings(texSettings);
                texSettings.spriteMeshType = SpriteMeshType.FullRect;
                texSettings.spriteAlignment = (int) SpriteAlignment.Custom;
                textureImporter.SetTextureSettings(texSettings);
                textureImporter.spritePivot = new Vector2(0.5f, 0.57f);
            }
            else
            {
                var texSettings = new TextureImporterSettings();
                textureImporter.ReadTextureSettings(texSettings);
                texSettings.spriteMeshType = SpriteMeshType.FullRect;
                textureImporter.SetTextureSettings(texSettings);
            }

            EditorUtility.SetDirty(textureImporter);
            textureImporter.SaveAndReimport();
#endif
        }

        private static Sprite ImportSpriteFromPath(
            GenerateTextureData target,
            bool tileC = false
        ) {
            // インポートしたspriteを返却
            var sprite = UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<Sprite>(target.texturePath);
            return sprite;
        }

        private static Texture2D CombineFourTexturesToOne(
            Texture2D textureLeftTop,
            Texture2D textureRightTop,
            Texture2D textureLeftBottom,
            Texture2D textureRightBottom
        ) {
            var halfW = textureLeftTop.width; // 4テクスチャがそれぞれ同じサイズであることを前提
            var halfH = textureLeftTop.height;

            var combinedTexture = new Texture2D(halfW * 2, halfH * 2, TextureFormat.RGBA32, false);

            // 左下
            combinedTexture.SetPixels(
                0, 0, halfW, halfH,
                textureLeftBottom.GetPixels(0, 0, halfW, halfH)
            );
            // 右下
            combinedTexture.SetPixels(
                halfW, 0, halfW, halfH,
                textureRightBottom.GetPixels(0, 0, halfW, halfH)
            );
            // 左上
            combinedTexture.SetPixels(
                0, halfH, halfW, halfH,
                textureLeftTop.GetPixels(0, 0, halfW, halfH)
            );
            // 右上
            combinedTexture.SetPixels(
                halfW, halfH, halfW, halfH,
                textureRightTop.GetPixels(0, 0, halfW, halfH)
            );
            combinedTexture.Apply();

            return combinedTexture;
        }

        private static Texture2D CombineFourTexturesToOne_DifferentSize(
            Texture2D textureLeftTop,
            Texture2D textureRightTop,
            Texture2D textureLeftBottom,
            Texture2D textureRightBottom
        ) {
            var halfW = new List<int>
            {
                textureLeftTop.width,
                textureRightTop.width,
                textureLeftBottom.width,
                textureRightBottom.width
            };
            var halfH = new List<int>
            {
                textureLeftTop.height,
                textureRightTop.height,
                textureLeftBottom.height,
                textureRightBottom.height
            };

            var widthMax = halfW[0] + halfW[1];
            var heightMax = halfH[0] + halfH[2];
            var combinedTexture = new Texture2D(widthMax, heightMax, TextureFormat.RGBA32, false);

            // 左下
            combinedTexture.SetPixels(
                0, 0, halfW[2], halfH[2],
                textureLeftBottom.GetPixels(0, 0, halfW[2], halfH[2])
            );
            // 右下
            combinedTexture.SetPixels(
                halfW[2], 0, halfW[3], halfH[3],
                textureRightBottom.GetPixels(0, 0, halfW[3], halfH[3])
            );
            // 左上
            combinedTexture.SetPixels(
                0, halfH[2], halfW[0], halfH[0],
                textureLeftTop.GetPixels(0, 0, halfW[0], halfH[0])
            );
            // 右上
            combinedTexture.SetPixels(
                halfW[0], halfH[2], halfW[1], halfH[1],
                textureRightTop.GetPixels(0, 0, halfW[1], halfH[1])
            );
            combinedTexture.Apply();

            return combinedTexture;
        }

        private static List<Texture2D> SliceTexture(Texture2D originalTexture, int x, int y) {
            var slicedW = originalTexture.width / x;
            var slicedH = originalTexture.height / y;

            var sliceBitmaps = new List<Texture2D>();
            for (var yNum = y; yNum > 0; yNum--)
            for (var xNum = 0; xNum < x; xNum++)
            {
                var slicedTexture = new Texture2D(slicedW, slicedH, TextureFormat.RGBA32, false);
                slicedTexture.SetPixels(ReadPixelsFromTexture(originalTexture, xNum, yNum, slicedW, slicedH));
                slicedTexture.Apply();

                sliceBitmaps.Add(slicedTexture);
            }

            return sliceBitmaps;
        }

        private static List<Texture2D> SliceTexture_Normal(Texture2D originalTexture, int x, int y) {
            var slicedW = originalTexture.width / x;
            var slicedH = originalTexture.height / y;

            var sliceBitmaps = new List<Texture2D>();
            for (var yNum = y; yNum > 0; yNum--)
                for (var xNum = 0; xNum < x; xNum++)
                {
                    var slicedTexture = new Texture2D(slicedW, slicedH, TextureFormat.RGBA32, false);
                    slicedTexture.SetPixels(ReadPixelsFromTexture(originalTexture, xNum, yNum, slicedW, slicedH));
                    slicedTexture.Apply();

                    var clipTexture = new Texture2D(96, 96, TextureFormat.RGBA32, false);
                    clipTexture.SetPixels(slicedTexture.GetPixels(1,1,96,96));
                    clipTexture.Apply();

                    sliceBitmaps.Add(clipTexture);
                }

            return sliceBitmaps;
        }

        private static List<Texture2D> SliceTexture_FixedSize(
            Texture2D originalTexture,
            int width,
            int height,
            bool tileC = false
        ) {
            var x = originalTexture.width / width;
            var y = originalTexture.height / height;

            var sliceBitmaps = new List<Texture2D>();
            for (var yNum = y; yNum > 0; yNum--)
            for (var xNum = 0; xNum < x; xNum++)
            {
                var slicedTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                slicedTexture.SetPixels(ReadPixelsFromTexture(originalTexture, xNum, yNum, width, height, tileC));
                slicedTexture.Apply();

                sliceBitmaps.Add(slicedTexture);
            }

            return sliceBitmaps;
        }

        private static List<Texture2D> SliceTexture_AnimatinAutoTileB(Texture2D originalTexture) {
            var x = (int) (originalTexture.width / 48.5f);
            var y = (int) (originalTexture.height / 48.5f);

            var sliceBitmaps = new List<Texture2D>();
            for (var yNum = y; yNum > 0; yNum--)
            for (var xNum = 0; xNum < x; xNum++)
            {
                var slicedTexture = new Texture2D(48, 48, TextureFormat.RGBA32, false);
                slicedTexture.SetPixels(ReadPixelsFromTexture(originalTexture, xNum, yNum, 48, 48, false, true));
                slicedTexture.Apply();

                sliceBitmaps.Add(slicedTexture);
            }

            return sliceBitmaps;
        }

        private static Color[] ReadPixelsFromTexture(
            Texture2D texture,
            int xIndex,
            int yIndex,
            int targetW,
            int targetH,
            bool tileC = false,
            bool tileB = false
        ) {
            var margin = 0;
            if (tileC) margin = 16;
            var marginX = 0;
            var marginY = 0;
            if (tileB)
            {
                marginX = 1 + xIndex / 4 * 2;
                marginY = 1;
            }

            return texture.GetPixels(xIndex * targetW + marginX, (yIndex - 1) * targetH + margin + marginY, targetW,
                targetH);
        }

        private static Texture2D ReadImage(string path) {
            return ReadPng(path);
        }

        private static Texture2D ReadPng(string path) {
            var readBinary = ReadPngFile(path);

            var pos = 16; // 16バイトから開始

            var width = 0;
            for (var i = 0; i < 4; i++) width = width * 256 + readBinary[pos++];

            var height = 0;
            for (var i = 0; i < 4; i++) height = height * 256 + readBinary[pos++];

            var texture = new Texture2D(width, height);
            texture.LoadImage(readBinary);

            return texture;
        }

        private static byte[] ReadPngFile(string path) {
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var bin = new BinaryReader(fileStream);
            var values = bin.ReadBytes((int) bin.BaseStream.Length);

            bin.Close();

            return values;
        }

        /**
         * タイルアセットパスを取得。
         */
        private static string GetAssetPath(TileDataModel tileDataModel) {
            if (tileDataModel.type == TileDataModel.Type.BackgroundCollision)
                return SystemTileAssetFolderPath + "BackgroundCollisionTile/" + tileDataModel.id + ".asset";
            else
                return TileAssetFolderPath + "/" + tileDataModel.id + ".asset";
        }

#if ENABLE_DEVELOPMENT_FIX
        // タイル修正用処理
        public static void FixImageSize() {
            //return;

            if(File.Exists("Assets/initialize.txt"))
                return;

            if(_tileDataModels == null)
                _tileDataModels = Directory.GetFiles(TileAssetFolderPath)
                    .Select(assetPath => UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<TileDataModel>(assetPath))
                    .Where(tileAsset => tileAsset != null)
                    .ToList();

            File.WriteAllText("Assets/initialize.txt", "");
            UnityEditorWrapper.AssetDatabaseWrapper.Refresh2();

            for (int i = 0; i < _tileDataModels.Count; i++)
            {
                if(_tileDataModels[i].type != TileDataModel.Type.NormalTile)  continue;

                //if(_tileDataModels[i].id == "80524fa9-6d55-43f3-b898-57102fcd6641")
                {
                    var targetPath = TileAssetFolderPath + "/" + _tileDataModels[i].id + "/defaultSprite" + ".png";
                    var originalImageTex = UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<Sprite>(targetPath);
                    if (originalImageTex == null) continue;
                    if (originalImageTex.texture.width == 96 && originalImageTex.pixelsPerUnit == 96)
                    {
                        if (originalImageTex.texture.isReadable == true)
                            AssetManageImporter.ChangeTextureTypeFix(targetPath, false);
                        continue;
                    }
                    else if(originalImageTex.texture.width == 96)
                    {
                        AssetManageImporter.ChangeTextureTypeFix(targetPath, false);
                        continue;
                    }
                    if (originalImageTex.texture.width < 97)
                    {
                        if (originalImageTex.texture.isReadable == true)
                            AssetManageImporter.ChangeTextureTypeFix(targetPath, false);
                        continue;
                    }
                    AssetManageImporter.ChangeTextureTypeFix(targetPath, true, true);
                    originalImageTex = UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<Sprite>(targetPath);
                    if (originalImageTex == null) continue;
                    {
                        var croppedTexture = new Texture2D(96, 96);
                        var pixels = originalImageTex.texture.GetPixels(1,
                                                                1,
                                                                96,
                                                                96);
                        croppedTexture.SetPixels(pixels);
                        croppedTexture.Apply();
                        var sprite = Sprite.Create(
                            croppedTexture,
                            new Rect(0, 0, 96, 96),
                            new Vector2(0.5f, 0.5f),
                            96
                        );

                        File.WriteAllBytes(targetPath, sprite.texture.EncodeToPNG());
                        UnityEditorWrapper.AssetDatabaseWrapper.Refresh2();
                        AssetManageImporter.ChangeTextureTypeFix(targetPath, false, false);
                    }
                }
            }
        }

        // タイル修正用処理
        public static void FixImageMeshType() {
            //return;

            if (File.Exists("Assets/initializetilemesh.txt"))
                return;

            if (_tileDataModels == null)
                _tileDataModels = Directory.GetFiles(TileAssetFolderPath)
                    .Select(assetPath => UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<TileDataModel>(assetPath))
                    .Where(tileAsset => tileAsset != null)
                    .ToList();

            File.WriteAllText("Assets/initializetilemesh.txt", "");
            UnityEditorWrapper.AssetDatabaseWrapper.Refresh2();

            for (int i = 0; i < _tileDataModels.Count; i++)
            {
                if (_tileDataModels[i].type != TileDataModel.Type.NormalTile) continue;
                {
                    var targetPath = TileAssetFolderPath + "/" + _tileDataModels[i].id + "/defaultSprite" + ".png";
                    var originalImageTex = UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<Sprite>(targetPath);
                    if (originalImageTex == null) continue;
                    AssetManageImporter.ChangeTextureTypeFix(targetPath, false, false, true);
                }
            }
        }
#endif

#if UNITY_EDITOR
        // 英語、中国語への変換
        public void JsonTranslation() {
            if (!File.Exists(JsonFileTranslation))
                return;

            // AssetDatabaseを一時停止
            AssetDatabase.StartAssetEditing();

            List<TileDataModel> DataModels = GetTileEntities();

            var jsonString = UnityEditorWrapper.AssetDatabaseWrapper.LoadJsonString(JsonFileTranslation);
            var eventJson = JsonHelper.FromJsonArray<TileJsonTranslation>(jsonString);

            foreach (var data in eventJson)
            {
                for (int i = 0; i < DataModels.Count; i++)
                {
                    if (data.id == DataModels[i].id)
                    {
                        DataModels[i].name = data.name;
                        EditorUtility.SetDirty(DataModels[i]);
                    }
                }
            }

            UnityEditorWrapper.AssetDatabaseWrapper.SaveAssets();

            // AssetDatabaseを再開
            AssetDatabase.StopAssetEditing();

            File.Delete(JsonFileTranslation);
        }
#endif
    }
}