using System.Collections.Generic;
using System.IO;
using System.Text;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.EventCommon;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;
using JsonHelper = RPGMaker.Codebase.CoreSystem.Helper.JsonHelper;

namespace RPGMaker.Codebase.CoreSystem.Service.EventManagement.Repository
{
    public class EventCommonRepository
    {
        private const string JsonPath = "Assets/RPGMaker/Storage/Event/JSON/eventCommon.json";
#if UNITY_EDITOR
        private const string JsonFileTranslation = "Assets/RPGMaker/Storage/Event/JSON/eventCommonTranslation.txt";
#endif

        private static List<EventCommonDataModel> _eventCommonDataModels;

        public void Save(EventCommonDataModel eventCommonDataModel) {
            if (_eventCommonDataModels == null) Load();

            var edited = false;
            for (var index = 0; index < _eventCommonDataModels.Count; index++)
            {
                if (_eventCommonDataModels[index].eventId != eventCommonDataModel.eventId) continue;

                _eventCommonDataModels[index] = eventCommonDataModel;
                edited = true;
                break;
            }

            if (!edited) _eventCommonDataModels.Add(eventCommonDataModel);

            File.WriteAllText(JsonPath, JsonHelper.ToJsonArray(_eventCommonDataModels));

            SetSerialNumbers();
        }

        public List<EventCommonDataModel> Load() {
            if (_eventCommonDataModels != null) return _eventCommonDataModels;
#if UNITY_EDITOR
            var jsonString = UnityEditorWrapper.AssetDatabaseWrapper.LoadJsonString(JsonPath);
            _eventCommonDataModels = JsonHelper.FromJsonArray<EventCommonDataModel>(jsonString);
#else
            _eventCommonDataModels = ScriptableObjectOperator.GetClass<EventCommonDataModel>(JsonPath) as List<EventCommonDataModel>;
#endif

            SetSerialNumbers();

            return _eventCommonDataModels;
        }

        public void Delete(EventCommonDataModel eventCommonDataModel) {
            var jsonString = UnityEditorWrapper.AssetDatabaseWrapper.LoadJsonString(JsonPath);
            var eventLists = JsonHelper.FromJsonArray<EventCommonDataModel>(jsonString);

            for (var index = 0; index < eventLists.Count; index++)
            {
                if (eventLists[index].eventId != eventCommonDataModel.eventId) continue;

                eventLists.RemoveAt(index);
                break;
            }

            File.WriteAllText(JsonPath, JsonHelper.ToJsonArray(eventLists));

            _eventCommonDataModels = eventLists;

            SetSerialNumbers();
        }

        private void SetSerialNumbers() {
            // serial numberフィールドがあるモデルには連番を設定する
            for (var i = 0; i < _eventCommonDataModels.Count; i++)
                if (_eventCommonDataModels[i] is WithSerialNumberDataModel serialNumberDataModel)
                    serialNumberDataModel.SerialNumber = i + 1;
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
                for (var index = 0; index < _eventCommonDataModels.Count; index++)
                {
                    if (_eventCommonDataModels[index].name == lineSplit[0])
                    {
                        _eventCommonDataModels[index].name = lineSplit[1];
                    }
                }
            }

            for (var index = 0; index < _eventCommonDataModels.Count; index++)
            {
                Save(_eventCommonDataModels[index]);
            }

            File.Delete(JsonFileTranslation);
        }
#endif
    }
}