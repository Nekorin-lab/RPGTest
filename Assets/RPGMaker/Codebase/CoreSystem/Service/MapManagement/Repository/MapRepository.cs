using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Map;
using RPGMaker.Codebase.CoreSystem.Knowledge.JsonStructure;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RPGMaker.Codebase.CoreSystem.Service.MapManagement.Repository
{
    public class MapRepository
    {
        private const string JsonFile	       = "Assets/RPGMaker/Storage/Map/JSON/Map/";
        private const string JsonBaseFile      = "Assets/RPGMaker/Storage/Map/JSON/mapbase.json";
        private const string OldJsonFile       = "Assets/RPGMaker/Storage/Map/JSON/map.json";
        private const string JsonFileTranslation = "Assets/RPGMaker/Storage/Map/JSON/mapname.json";
        private const string JsonFileSample    = "Assets/RPGMaker/Storage/Map/JSON/MapSample/";
        private const string OldJsonFileSample = "Assets/RPGMaker/Storage/Map/JSON/mapSample.json";

        private static List<MapDataModel> _mapDataModels;
        private static List<MapBaseDataModel> _mapBaseDataModels;
        private static List<MapDataModel> _mapSampleDataModels;

        public enum SaveType
        {
            NO_PREFAB = 0,
            SAVE_PREFAB,
            SAVE_PREFAB_FORCE
        }

        public List<MapDataModel> LoadMapDataModels() {
#if UNITY_EDITOR
            // 再生中でなければ
            if (!EditorApplication.isPlaying)
            {
                if (_mapDataModels != null)
                {
                    // キャッシュがあればそれを返す
                    return _mapDataModels;
                }
            }

            _mapDataModels = GetJsons(JsonFile).Select(ConvertJsonToEntity).ToList();
#else
            //UnityEngine.Debug.LogError("Don't call.");
#endif

            SetSerialNumbers();

            return _mapDataModels;
        }

        public List<MapBaseDataModel> LoadMapBaseDataModels() {
#if UNITY_EDITOR
            // 再生中でなければ
            if (!EditorApplication.isPlaying)
            {
                if (_mapBaseDataModels != null)
                {
                    // キャッシュがあればそれを返す
                    return _mapBaseDataModels;
                }
            }

            _mapBaseDataModels = GetBaseJsons(JsonFile).Select(ConvertJsonToEntityBase).ToList();
            SetBaseSerialNumbers();
#else
            _mapBaseDataModels = ScriptableObjectOperator.GetClass<MapBaseDataModel>(JsonBaseFile) as List<MapBaseDataModel>;
#endif

            return _mapBaseDataModels;
        }

        public MapDataModel LoadMapDataModel(string id, bool isSampleMap) {
#if UNITY_EDITOR
            if (!isSampleMap)
            {
                if (_mapDataModels != null)
                {
                    for (int i = 0; i < _mapDataModels.Count; i++)
                    {
                        if (_mapDataModels[i].id == id)
                        {
                            SetSerialNumbers();
                            return _mapDataModels[i];
                        }
                    }
                }

                return ConvertJsonToEntity(
                    JsonHelper.FromJson<MapJson>(
                        UnityEditorWrapper.AssetDatabaseWrapper.LoadJsonString(JsonFile + id + ".json")));
            }
            else
            {
                return ConvertJsonToEntity(
                    JsonHelper.FromJson<MapJson>(
                        UnityEditorWrapper.AssetDatabaseWrapper.LoadJsonString(JsonFileSample + id + ".json")));
            }
#else
            return ScriptableObjectOperator.GetClass<MapDataModel>(JsonFile + id + ".json") as MapDataModel;
#endif
        }

        public List<MapDataModel> LoadMapDataModels(bool reload) {
#if UNITY_EDITOR
            _mapDataModels = GetJsons(JsonFile).Select(ConvertJsonToEntity).ToList();

            SetSerialNumbers();

            return _mapDataModels;
#else
            //UnityEngine.Debug.LogError("Don't call.");
            return null;
#endif
        }

#if UNITY_EDITOR
        private List<MapJson> GetJsons(string path) {
            var mapJson = new List<MapJson>();
            var files = Directory.GetFiles(path, "*.json", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
                mapJson.Add(
                    JsonHelper.FromJson<MapJson>(UnityEditorWrapper.AssetDatabaseWrapper.LoadJsonString(file)));
            
            mapJson = mapJson.OrderBy(item => item.index).ToList();

            return mapJson;
        }

        private List<MapBaseJson> GetBaseJsons(string path) {
            var mapJson = new List<MapBaseJson>();
            var files = Directory.GetFiles(path, "*.json", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
                mapJson.Add(
                    JsonHelper.FromJson<MapBaseJson>(UnityEditorWrapper.AssetDatabaseWrapper.LoadJsonString(file)));

            mapJson = mapJson.OrderBy(item => item.index).ToList();

            return mapJson;
        }
#endif

        public void SaveMapDataModelForEditor(MapDataModel mapDataModel, SaveType savePrefab) {
            // マッププレハブ保存 (存在している場合のみ)
            if (savePrefab == SaveType.SAVE_PREFAB)
            {
                //Unload時に保存する
                mapDataModel.changePrefab = true;
                mapDataModel.isSampleMap = false;
            }
            else if (savePrefab == SaveType.SAVE_PREFAB_FORCE)
            {
                //強制的に保存する
                var mapPrefab = mapDataModel.MapPrefabManagerForEditor.mapPrefab;
                if (mapPrefab != null)
                {
                    UnityEditorWrapper.PrefabUtilityWrapper.SaveAsPrefabAsset(mapPrefab, mapDataModel.GetPrefabPath());
                }
                mapDataModel.changePrefab = true;
                mapDataModel.isSampleMap = false;
            }

            // その他パラメータをJSONに保存
            var json = ConvertEntityToJsonForEditor(mapDataModel);
            File.WriteAllText(JsonFile + mapDataModel.id + ".json", JsonHelper.ToJson(json));

            // JSONに保存したデータを改めて読込、キャッシュする
            if (_mapDataModels == null)
                _mapDataModels = LoadMapDataModels();

            bool flg = false;
            for (int i = 0; i < _mapDataModels.Count; i++)
            {
                if (_mapDataModels[i].id == mapDataModel.id)
                {
                    _mapDataModels[i] = MapDataModel.CopyData(mapDataModel);
                    flg = true;
                    break;
                }
            }
            if (!flg)
            {
                _mapDataModels.Add(MapDataModel.CopyData(mapDataModel));
            }

#if UNITY_EDITOR
            if (!flg)
            {
                AddressableManager.Path.SetAddressToAsset(mapDataModel.GetPrefabPath());
            }
#endif

            SetSerialNumbers();
        }

        public void RemoveMapEntity(MapDataModel mapDataModel) {
            // マッププレハブ削除
            UnityEditorWrapper.PrefabUtilityWrapper.RemovePrefabAsset(mapDataModel.GetPrefabPath());

            if (_mapDataModels == null)
                _mapDataModels = LoadMapDataModels();

            var targetIndex = _mapDataModels.FindIndex(item => item.id == mapDataModel.id);
            if (targetIndex != -1)
            {
                _mapDataModels.RemoveAt(targetIndex);
            }

            File.Delete(JsonFile + mapDataModel.id + ".json");

            SetSerialNumbers();
        }

        public static void RemoveCache(string id) {
            if (_mapDataModels == null) return;
            for (int i = 0; i < _mapDataModels.Count; i++)
            {
                if (_mapDataModels[i].id == id)
                {
                    _mapDataModels[i] = ConvertJsonToEntity(
                        JsonHelper.FromJson<MapJson>(
                            UnityEditorWrapper.AssetDatabaseWrapper.LoadJsonString(JsonFile + id + ".json")));
                    break;
                }
            }
        }

        public void ResetMapEntity() {
            _mapDataModels = null;
        }

        public static MapDataModel ConvertJsonToEntity(MapJson json) {
            return new MapDataModel(
                json.mapId,
                json.index,
                json.name,
                json.displayName,
                json.width,
                json.height,
                json.scrollType,

                json.autoPlayBGM,
                json.bgmID,
                new MapDataModel.SoundState(json.bgmState.pan,json.bgmState.pitch,json.bgmState.volume),
                json.autoPlayBgs,
                json.bgsID,
                new MapDataModel.SoundState(json.bgsState.pan,json.bgsState.pitch,json.bgsState.volume),

                json.forbidDash,

                json.memo,
                json.layers,
                new MapDataModel.Background(
                    json.background.imageName,
                    (MapDataModel.ImageZoomIndex)json.background.imageZoomIndex,
                    json.background.showInEditor),
                new MapDataModel.parallax(json.Parallax.loopX, json.Parallax.loopY, json.Parallax.name,
                    json.Parallax.show, json.Parallax.sx, json.Parallax.sy, json.Parallax.zoom0, json.Parallax.zoom2,
                    json.Parallax.zoom4)
            );
        }

        public MapBaseDataModel ConvertJsonToEntityBase(MapBaseJson json) {
            return new MapBaseDataModel(
                json.mapId,
                json.name,
                json.SerialNumber
            );
        }

        private MapJson ConvertEntityToJsonForEditor(MapDataModel dataModel) {
            var backgroundLayer = new Background(
                dataModel.background.imageName,
                (int)dataModel.background.imageZoomIndex,
                dataModel.background.showInEditor);

            var parallax = new parallax(
                dataModel.Parallax.loopX,
                dataModel.Parallax.loopY,
                dataModel.Parallax.name,
                dataModel.Parallax.show,
                dataModel.Parallax.sx,
                dataModel.Parallax.sy,
                dataModel.Parallax.zoom0,
                dataModel.Parallax.zoom2,
                dataModel.Parallax.zoom4
            );

            var BGMState = new SoundState(
                dataModel.bgmState.pan,
                dataModel.bgmState.pitch,
                dataModel.bgmState.volume
            );

            var BGSState = new SoundState(
                dataModel.bgsState.pan,
                dataModel.bgsState.pitch,
                dataModel.bgsState.volume
            );

            return new MapJson(
                dataModel.id,
                dataModel.index,
                dataModel.name,
                dataModel.displayName,
                dataModel.MapPrefabManagerForEditor.layers,
                dataModel.width,
                dataModel.height,
                dataModel.scrollType,
                dataModel.autoPlayBGM,
                dataModel.bgmID,
                BGMState,
                dataModel.autoPlayBgs,
                dataModel.bgsID,
                BGSState,
                dataModel.forbidDash,
                backgroundLayer,
                parallax,
                dataModel.memo
            );
        }
        
        private void SetSerialNumbers() {
            // serial numberフィールドがあるモデルには連番を設定する
            for (var i = 0; i < _mapDataModels.Count; i++)
            {
                _mapDataModels[i].SerialNumber = i + 1;
            }
        }

        private void SetBaseSerialNumbers() {
            // serial numberフィールドがあるモデルには連番を設定する
            for (var i = 0; i < _mapBaseDataModels.Count; i++)
            {
                _mapBaseDataModels[i].SerialNumber = i + 1;
            }
        }

        // SampleMap
        //--------------------------------------------------------------------------------------------------------------
        public List<MapDataModel> LoadMapSampleDataModels() {
#if UNITY_EDITOR
            // 再生中でなければ
            if (!EditorApplication.isPlaying)
            {
                if (_mapSampleDataModels != null)
                {
                    // キャッシュがあればそれを返す
                    return _mapSampleDataModels;
                }
            }

            _mapSampleDataModels = GetJsons(JsonFileSample).Select(ConvertJsonToEntity).ToList();
#else
            _mapSampleDataModels = ScriptableObjectOperator.GetClass<MapDataModel>(JsonFile) as List<MapDataModel>;
#endif

            SetSampleSerialNumbers();

            return _mapSampleDataModels;
        }

        public void SaveMapSampleDataModelForEditor(MapDataModel mapDataModel) {
            // マッププレハブ保存 (存在している場合のみ)
            var mapPrefab = mapDataModel.MapPrefabManagerForEditor.mapPrefab;
            if (mapPrefab != null)
            {
                UnityEditorWrapper.PrefabUtilityWrapper.SaveAsPrefabAsset(
                    mapPrefab, mapDataModel.GetPrefabPath(true));
            }

            // その他パラメータをJSONに保存
            if (_mapSampleDataModels == null)
                _mapSampleDataModels = LoadMapSampleDataModels();

            var targetIndex = _mapSampleDataModels.FindIndex(item => item.id == mapDataModel.id);
            if (targetIndex != -1)
            {
                _mapSampleDataModels[targetIndex] = mapDataModel;
            }
            else
            {
                _mapSampleDataModels.Add(mapDataModel);
            }

            var json = ConvertEntityToJsonForEditor(mapDataModel);
            File.WriteAllText(JsonFileSample + mapDataModel.id + ".json", JsonHelper.ToJson(json));

#if UNITY_EDITOR
            if (targetIndex == -1)
            {
                AddressableManager.Path.SetAddressToAsset(mapDataModel.GetPrefabPath(true));
            }
#endif

            SetSampleSerialNumbers();
        }

        public void RemoveMapSampleEntity(MapDataModel mapDataModel) {
            // マッププレハブ削除
            UnityEditorWrapper.PrefabUtilityWrapper.RemovePrefabAsset(mapDataModel.GetPrefabPath(true));

            if (_mapSampleDataModels == null)
                _mapSampleDataModels = LoadMapSampleDataModels();

            var targetIndex = _mapSampleDataModels.FindIndex(item => item.id == mapDataModel.id);
            if (targetIndex != -1)
            {
                _mapSampleDataModels.RemoveAt(targetIndex);
            }

            var json = _mapSampleDataModels.Select(ConvertEntityToJsonForEditor);
            File.WriteAllText(JsonFileSample + mapDataModel.id + ".json", JsonHelper.ToJson(json));

#if UNITY_EDITOR
            AddressableManager.Path.SetAddressToAsset(mapDataModel.GetPrefabPath(true));
#endif

            SetSampleSerialNumbers();
        }

        public void ResetMapSampleEntity() {
            _mapSampleDataModels = null;
        }

        private void SetSampleSerialNumbers() {
            // serial numberフィールドがあるモデルには連番を設定する
            for (var i = 0; i < _mapSampleDataModels.Count; i++)
            {
                _mapSampleDataModels[i].SerialNumber = i + 1;
            }
        }

#if ENABLE_DEVELOPMENT_FIX
        //リリース時不要
#if false
        // MapScale、サイズ修正
        public void MapFixForEditor() {
            if (File.Exists("Assets/initialize.txt"))
                return;

            foreach (var mapDataModel in LoadMapDataModels())
            {
                //if(i > 20) return;

                var mapPrefabManager = mapDataModel.MapPrefabManagerForEditor;

                var mapPrefab = mapPrefabManager.LoadPrefab();
                bool isFix = false;
                if (mapPrefab.transform.localPosition != UnityEngine.Vector3.zero ||
                    mapPrefab.transform.localScale != UnityEngine.Vector3.one ||
                    mapPrefab.transform.GetComponent<UnityEngine.Grid>().cellGap != UnityEngine.Vector3.zero)
                {
                    mapPrefab.transform.localPosition = UnityEngine.Vector3.zero;
                    mapPrefab.transform.localScale = UnityEngine.Vector3.one;
                    mapPrefab.transform.GetComponent<UnityEngine.Grid>().cellGap = UnityEngine.Vector3.zero;
                    isFix = true;
                }

                var layers = mapPrefabManager.layers;
                for (int i2 = 0; i2 < layers.Count; i2++)
                {
                    var pos = mapPrefab.transform.GetChild(i2).gameObject.transform.localPosition;
                    var scale = mapPrefab.transform.GetChild(i2).gameObject.transform.localScale;

                    if ((int) MapDataModel.Layer.LayerType.Shadow == i2)
                    {
                        if(layers[(int) MapDataModel.Layer.LayerType.Shadow].tilemap.gameObject.
                            GetComponent<UnityEngine.Grid>().cellSize != new UnityEngine.Vector3(0.5f, 0.5f, 1))
                        {
                            layers[(int) MapDataModel.Layer.LayerType.Shadow].tilemap.gameObject.
                                GetComponent<UnityEngine.Grid>().cellSize = new UnityEngine.Vector3(0.5f, 0.5f, 1);
                        }
                        isFix = true;
                    }

                    if((int) MapDataModel.Layer.LayerType.Background == i2 || (int) MapDataModel.Layer.LayerType.DistantView == i2)
                    {
                        if (pos != new UnityEngine.Vector3(0, 1, pos.z) ||
                            scale != new UnityEngine.Vector3(UnityEngine.Mathf.Round(scale.x), UnityEngine.Mathf.Round(scale.y), 1))
                        {
                            mapPrefab.transform.GetChild(i2).gameObject.transform.localPosition = new UnityEngine.Vector3(0, 1, pos.z);
                            mapPrefab.transform.GetChild(i2).gameObject.transform.localScale = new UnityEngine.Vector3(UnityEngine.Mathf.Round(scale.x), UnityEngine.Mathf.Round(scale.y), 1);
                            isFix = true;
                        }
                    }
                    else
                    {
                        if (pos != new UnityEngine.Vector3(0, 0, pos.z) ||
                            scale != UnityEngine.Vector3.one)
                        {
                            mapPrefab.transform.GetChild(i2).gameObject.transform.localPosition = new UnityEngine.Vector3(0, 0, pos.z);
                            mapPrefab.transform.GetChild(i2).gameObject.transform.localScale = UnityEngine.Vector3.one;
                            isFix = true;
                        }
                    }
                }

                // 更新があれば保存
                if (isFix == true) SaveMapDataModelForEditor(mapDataModel, true);
            }

            UnityEditorWrapper.AssetDatabaseWrapper.Refresh2();
        }
#endif
        //リリース時不要
#if false
        // MapScale、サイズ修正
        public void MapTileFixForEditor() {
            // 変換用データ
            List<List<string>> data = new List<List<string>>()
            {
                new List<string>(){ "a462b049-06e7-4f91-99e8-a89daa4bef3d", "ad7c12f7-43fe-443a-acff-11c4d5b1b022" },
                new List<string>(){ "cbe9edeb-307f-46ef-885f-72fc6d0e12e7", "ad7c12f7-43fe-443a-acff-11c4d5b1b022" },
                new List<string>(){ "d583216a-a313-43a4-8792-f81433be8072", "ad7c12f7-43fe-443a-acff-11c4d5b1b022" },
                new List<string>(){ "3a7bfa29-a6c2-4b20-a890-122825a51450", "bcabfb36-6bfa-4812-a979-98a0312e3b76" },
                new List<string>(){ "93f7f597-30ef-4d26-a27e-a033026730b0", "14e88f23-833e-4db1-9428-25f3c0b77a68" },
                new List<string>(){ "c95d2358-7028-428e-ab24-c4398c258d77", "14e88f23-833e-4db1-9428-25f3c0b77a68" },
                new List<string>(){ "5cec51c0-1c30-4937-a758-2da15aed2c84", "c3dbc62e-7b00-4b2b-a9bf-6da7935d0943" },
                new List<string>(){ "714c51c8-104a-47b9-916c-a59753a13691", "04cf12a4-068d-4520-8473-6a85bf58e48f" },
                new List<string>(){ "9f21913b-9ff5-4695-976a-d5b2bfda9e15", "59658247-e40f-4fbd-a009-fd7f9aca35a6" },
                new List<string>(){ "20f75faa-70c5-4b2b-b70c-99e904acc618", "59658247-e40f-4fbd-a009-fd7f9aca35a6" },
                new List<string>(){ "c75bcf33-03e3-404a-bb2c-f45931b61e42", "c3dbc62e-7b00-4b2b-a9bf-6da7935d0943" },
                new List<string>(){ "2ee48d8b-7be2-4a9e-9a79-d18b44fb3636", "d5b9eb5d-16e9-4870-ae50-fa359cd5c3b7" },
                new List<string>(){ "146d7e8b-72b2-4cf9-a8d9-64936364003e", "ad25228d-8b38-4395-9bc3-bc6296a6a8f5" },
                new List<string>(){ "15b03874-0e47-43c5-97f5-333dd4b41fec", "2cdc7411-6d2c-4bd5-8d33-83976ddf0fc4" },
                new List<string>(){ "7d20de22-cd67-4b52-ad80-1ddf92c83350", "2cdc7411-6d2c-4bd5-8d33-83976ddf0fc4" },
                new List<string>(){ "9c752a46-7d37-48e6-a9aa-c2f554ef2c64", "ad25228d-8b38-4395-9bc3-bc6296a6a8f5" },
                new List<string>(){ "d05814c4-7afd-49a6-a52a-cb2adbd476ab", "508f0b3a-4655-4738-9149-14138b286e3b" },
                new List<string>(){ "f7de3e43-0efe-4d0c-8be4-df1a77a8d22e", "1db82de3-8fc3-4142-b887-4adaac5d5b39" },
                new List<string>(){ "4129b09c-d21a-4503-9878-4f1369707d92", "f2742a50-ff8a-4633-bc5a-d1c145c9e2e6" },
                new List<string>(){ "173e36d9-6934-4680-b3ce-02930a9e9195", "e6fdc143-db08-44d9-bcec-8d62a006dedc" },
                new List<string>(){ "4cf4e151-7db7-47b9-97e4-d6245d33e45e", "daa9cf74-b598-4829-aca8-828fd5b5b30a" },
                new List<string>(){ "747b251e-9071-4a9e-827b-ecbad6cf65e3", "419ef89e-9daa-4757-b795-bcd86b654610" },
                new List<string>(){ "ca887bff-a561-411e-86ac-b48d332ad009", "3ef325c3-678b-42c0-9749-3ca758fe452d" },
                new List<string>(){ "58c973d9-6c34-419b-9f53-97123d35bfbd", "6d6b8ded-4476-41dc-8fff-418004873a44" },
                new List<string>(){ "a7795796-8f3e-4591-9479-f1b2814574f4", "dd6e59f0-dc90-4b39-b7ed-97e83cd9c001" },
                new List<string>(){ "67742344-c1ca-4729-99c8-d378a15ed5e1", "100b7cee-ab16-4d15-8ecc-d887b606f122" },
                new List<string>(){ "c14eba5c-0d41-4526-8d1c-91fef424f4c9", "d10b7bf7-817b-4a33-858f-68a53b7d005b" },
                new List<string>(){ "044517ba-25de-4ea6-a3e5-d8ef909cf3c8", "7be6e83f-a98c-46d3-a970-24241e03ef7a" },
                new List<string>(){ "4356565c-d53d-41ae-884f-2fea51600f17", "5f670c48-ec7f-4f70-b4c3-3615a55fdd34" },
                new List<string>(){ "1f10ec5f-66e7-4432-9e8e-d8b1ef8d0394", "c777cee3-6992-4436-9844-03e9776d7f63" },
                new List<string>(){ "d072715b-b58e-4205-9090-89c2682f3572", "7df89e58-a1f7-4527-932c-f344180002aa" },
                new List<string>(){ "29942e80-07af-49a2-8db0-feff1a203b3f", "e8d2cbdb-5a2f-464d-8ca8-24ae6c1acb43" },
                new List<string>(){ "141044c8-b89f-446a-b9b7-7ae25b49c9c5", "fc7f7fab-6326-4611-b361-c35888aaca65" },
                new List<string>(){ "8a7e5b84-fdef-423e-80bb-9a34415949eb", "783be011-afb0-4dfc-8d45-c5e3597f0ef5" },
                new List<string>(){ "dfe724c1-9c47-4aa7-91c0-6d704f88886b", "1ed713e0-7752-4a4d-b901-98052573f0e1" },
                new List<string>(){ "c0d4a753-3f0c-4027-bb43-69888d8124c3", "e24f4ada-7467-484c-ada1-6edf17336f80" },
                new List<string>(){ "4a8c4078-861a-4dd0-a5b7-f80e1e871b5b", "b1c48090-e67a-48d2-8bdb-2c7eee5e71f4" },
                new List<string>(){ "062d3372-e565-4bfb-8a47-27b2edd55f41", "e24f4ada-7467-484c-ada1-6edf17336f80" },
                new List<string>(){ "225287ed-871b-4b98-947c-a028e9d9f161", "b6a4e29c-4701-4785-9f78-507cae14743e" },
                new List<string>(){ "553d4935-2055-4f78-8794-a4e12ffad6c9", "e1d0ca52-99b6-43e2-a8c4-db496a4adf10" },
                new List<string>(){ "863e8a97-6532-4f54-83de-4f87e3f3c3bc", "3305faa6-ce56-4c9b-ba27-7f4e8a1f6af9" },
                new List<string>(){ "e9e02672-5908-4172-9489-5159ae26b7c5", "ced385e2-bc6d-4381-9356-2bdf23dd8662" },
            };
            
            if (File.Exists("Assets/tileinitialize.txt"))
                return;

            File.WriteAllText("Assets/tileinitialize.txt", "");

            var maps = LoadMapDataModels();
            for (int i = 0; i < maps.Count; i++)
            {
                bool isFix = false;
                var layers = maps[i].MapPrefabManagerForEditor.layers;
                for (int i2 = 0; i2 < layers.Count; i2++)
                {
                    if ((int) MapDataModel.Layer.LayerType.Shadow == i2 ||
                        (int) MapDataModel.Layer.LayerType.Background == i2 || 
                        (int) MapDataModel.Layer.LayerType.DistantView == i2 ||
                        (int) MapDataModel.Layer.LayerType.BackgroundCollision == i2 ||
                        (int) MapDataModel.Layer.LayerType.Region == i2)
                    {
                        continue;
                    }
                    
                    for (int y = 0; y < maps[i].height; y++)
                    {
                        for (int x = 0; x < maps[i].width; x++)
                        {
                            var tile = layers[i2].GetTileDataModelByPosition(new UnityEngine.Vector2(x,-y));
                            if (tile == null) continue;
                            for(int d = 0; d < data.Count; d++)
                            {
                                if (data[d][0] == tile.id)
                                {
                                    TileDataModel rep = null;
                                    var tiles = new TileRepository().GetTileEntities();
                                    for (int i3 = 0; i3 < tiles.Count; i3++)
                                        if (tiles[i3].id == data[d][1])
                                        {
                                            rep = tiles[i3];
                                            break;
                                        }
                                    layers[i2].tilemap.SetTile(new UnityEngine.Vector3Int(x, -y, 0), null);
                                    layers[i2].tilemap.SetTile(new UnityEngine.Vector3Int(x, -y, 0), rep);
                                    isFix = true;
                                }
                            }
                        }
                    }
                }

                // 更新があれば保存
                // 補正を行っているため、ここで強制的にPrefabも保存する
                if (isFix == true) SaveMapDataModelForEditor(maps[i], SaveType.SAVE_PREFAB_FORCE);
            }

            string path = "Assets/RPGMaker/Storage/Map/TileAssets/";
            // タイル削除
            for (int d = 0; d < data.Count; d++)
            {
                Directory.Delete(path + data[d][0], true);
                File.Delete(path + data[d][0] + ".meta");
                File.Delete(path + data[d][0] + ".asset");
                File.Delete(path + data[d][0] + ".asset.meta");
            }
            UnityEditorWrapper.AssetDatabaseWrapper.Refresh2();
        }
#endif

        // 旧形式から修正
        public void MapJsonFix() {
            if (!File.Exists(OldJsonFile))
                return;

            if (Directory.Exists(JsonFile) == false)
                Directory.CreateDirectory(JsonFile);

            var jsonString = UnityEditorWrapper.AssetDatabaseWrapper.LoadJsonString(OldJsonFile);
            var eventJson = JsonHelper.FromJsonArray<MapJson>(jsonString);
            var eventList = eventJson.Select(ConvertJsonToEntity).ToList();

            int index = 0;
            foreach (var data in eventList)
            {
                data.index = index;
                File.WriteAllText(JsonFile + data.id + ".json", JsonHelper.ToJson(ConvertEntityToJsonForEditor(data)));
                index++;
            }

            File.Delete(OldJsonFile);
        }

        public void MapSampleJsonFix() {
            if (!File.Exists(OldJsonFileSample))
                return;

            if (Directory.Exists(JsonFileSample) == false)
                Directory.CreateDirectory(JsonFileSample);

            var jsonString = UnityEditorWrapper.AssetDatabaseWrapper.LoadJsonString(OldJsonFileSample);
            var eventJson = JsonHelper.FromJsonArray<MapJson>(jsonString);
            var eventList = eventJson.Select(ConvertJsonToEntity).ToList();

            int index = 0;
            foreach (var data in eventList)
            {
                data.index = index;
                File.WriteAllText(JsonFileSample + data.id + ".json", JsonHelper.ToJson(ConvertEntityToJsonForEditor(data)));
                index++;
            }

            File.Delete(OldJsonFileSample);
        }

        // 英語、中国語へのマップ名変換
        public void MapJsonTranslation() {
            if (!File.Exists(JsonFileTranslation))
                return;

            if (Directory.Exists(JsonFile) == false)
                Directory.CreateDirectory(JsonFile);

            var jsonString = UnityEditorWrapper.AssetDatabaseWrapper.LoadJsonString(JsonFileTranslation);
            var eventJson = JsonHelper.FromJsonArray<MapJsonTranslation>(jsonString);

            foreach (var data in eventJson)
            {
                try
                {
                    var jsonStringOrg = UnityEditorWrapper.AssetDatabaseWrapper.LoadJsonString(JsonFile + data.mapId + ".json");
                    var eventJsonOrg = JsonHelper.FromJson<MapJson>(jsonStringOrg);
                    eventJsonOrg.name = data.name;
                    File.WriteAllText(JsonFile + data.mapId + ".json", JsonHelper.ToJson(eventJsonOrg));

                    if (_mapDataModels != null)
                    {
                        for (int i = 0; i < _mapDataModels.Count; i++)
                        {
                            if (_mapDataModels[i].id == data.mapId)
                            {
                                _mapDataModels[i].name = data.name;
                                break;
                            }
                        }
                    }
                } catch (Exception) { }
            }

            File.Delete(JsonFileTranslation);
        }
#endif
    }
}