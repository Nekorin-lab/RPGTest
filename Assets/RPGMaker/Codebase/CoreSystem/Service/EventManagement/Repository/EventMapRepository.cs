using System.Collections.Generic;
using System.IO;
using System.Text;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.EventMap;

namespace RPGMaker.Codebase.CoreSystem.Service.EventManagement.Repository
{
    public class EventMapRepository
    {
        private const string JsonFile = "Assets/RPGMaker/Storage/Event/JSON/eventMap.json";
        private const string SO_PATH  = "Assets/RPGMaker/Storage/Event/SO/eventMap.asset";
#if UNITY_EDITOR
        private const string JsonFileTranslation = "Assets/RPGMaker/Storage/Event/JSON/eventMapTranslation.txt";
#endif

        private static List<EventMapDataModel> _eventMapDataModels;

        public List<EventMapDataModel> Load() {
            if (_eventMapDataModels != null) return _eventMapDataModels;
#if UNITY_EDITOR
            var jsonString = UnityEditorWrapper.AssetDatabaseWrapper.LoadJsonString(JsonFile);
            _eventMapDataModels = JsonHelper.FromJsonArray<EventMapDataModel>(jsonString);
#else
            _eventMapDataModels =
 ScriptableObjectOperator.GetClass<EventMapDataModel>(JsonFile) as List<EventMapDataModel>;
#endif

            //ID重複のデータが存在した場合には、後勝ちとする
            for (int i = 0; i < _eventMapDataModels.Count; i++)
            {
                for (int j = i + 1; j < _eventMapDataModels.Count; j++)
                {
                    if (_eventMapDataModels[i].mapId == _eventMapDataModels[j].mapId &&
                        _eventMapDataModels[i].eventId == _eventMapDataModels[j].eventId)
                    {
                        _eventMapDataModels.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }

            SetSerialNumbers();

            return _eventMapDataModels;
        }

        public void Save() {
            File.WriteAllText(JsonFile, JsonHelper.ToJsonArray(_eventMapDataModels));

            SetSerialNumbers();
        }

        public void Save(EventMapDataModel eventMapDataModel) {
            var eventMapLists = Load();

            var edited = false;
            for (var index = 0; index < eventMapLists.Count; index++)
            {
                if (eventMapLists[index].mapId != eventMapDataModel.mapId) continue;
                if (eventMapLists[index].eventId != eventMapDataModel.eventId) continue;

                eventMapLists[index] = eventMapDataModel;
                edited = true;
                break;
            }

            if (!edited) eventMapLists.Add(eventMapDataModel);

            File.WriteAllText(JsonFile, JsonHelper.ToJsonArray(eventMapLists));

            _eventMapDataModels = eventMapLists;

            SetSerialNumbers();
        }

        /**
         * マップに紐づくイベントを取得する
         */
        public List<EventMapDataModel> LoadEventMapEntitiesByMapId(string mapId) {
            Load();

            return _eventMapDataModels.FindAll(eventEntity => eventEntity.mapId == mapId);
        }

        public List<EventMapDataModel> LoadEventMapEntitiesByMapIdFromJson(string mapId) {
#if UNITY_EDITOR
            var jsonString = UnityEditorWrapper.AssetDatabaseWrapper.LoadJsonString(JsonFile);
            var eventMapDataModels = JsonHelper.FromJsonArray<EventMapDataModel>(jsonString);
#else
            var eventMapDataModels =
 ScriptableObjectOperator.GetClass<EventMapDataModel>(JsonFile) as List<EventMapDataModel>;
#endif

            return eventMapDataModels.FindAll(eventMapDataModel => eventMapDataModel.mapId == mapId);
        }

        public void Delete(EventMapDataModel eventMapDataModel) {
            var eventMapLists = Load();

            eventMapLists.RemoveAll(eventMap =>
                eventMap.mapId == eventMapDataModel.mapId && eventMap.eventId == eventMapDataModel.eventId);

            File.WriteAllText(JsonFile, JsonHelper.ToJsonArray(eventMapLists));

            _eventMapDataModels = eventMapLists;

            SetSerialNumbers();
        }

        private void SetSerialNumbers() {
            // serial numberフィールドがあるモデルには連番を設定する
            for (var i = 0; i < _eventMapDataModels.Count; i++) _eventMapDataModels[i].SerialNumber = i + 1;
        }

#if UNITY_EDITOR
        // 英語、中国語への変換
        public void JsonTranslation() {
            if (!File.Exists(JsonFileTranslation))
                return;

            Load();

            //翻訳用ファイルを1行ずつ読込置換する
            foreach (string line in System.IO.File.ReadLines(JsonFileTranslation, Encoding.UTF8))
            {
                string[] lineSplit = line.Split("\t");
                for (var index = 0; index < _eventMapDataModels.Count; index++)
                {
                    if (_eventMapDataModels[index].name == lineSplit[0])
                    {
                        _eventMapDataModels[index].name = lineSplit[1];
                    }
                }
            }

            for (var index = 0; index < _eventMapDataModels.Count; index++)
            {
                Save(_eventMapDataModels[index]);
            }

            File.Delete(JsonFileTranslation);
        }
#endif
    }
}