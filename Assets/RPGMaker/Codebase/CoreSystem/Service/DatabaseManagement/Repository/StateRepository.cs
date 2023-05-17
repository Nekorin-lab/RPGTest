using System.Collections.Generic;
using System.IO;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.State;
using RPGMaker.Codebase.CoreSystem.Knowledge.JsonStructure;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Service.DatabaseManagement.Repository
{
    public class StateRepository : AbstractDatabaseRepository<StateDataModel>
    {
        protected override string JsonPath => "Assets/RPGMaker/Storage/Initializations/JSON/state.json";
#if UNITY_EDITOR
        private const string JsonFileTranslation = "Assets/RPGMaker/Storage/Initializations/JSON/statename.json";
#endif

#if !UNITY_EDITOR
        public new List<StateDataModel> Load() {
            if (DataModels != null)
            {
                // キャッシュがあればそれを返す
                return DataModels;
            }
            DataModels = ScriptableObjectOperator.GetClass<StateDataModel>(JsonPath) as List<StateDataModel>;
            SetSerialNumbers();
            return DataModels;
        }
#endif

#if UNITY_EDITOR
        // 英語、中国語への変換
        public void JsonTranslation() {
            if (!File.Exists(JsonFileTranslation))
                return;

            Load();

            var jsonString = UnityEditorWrapper.AssetDatabaseWrapper.LoadJsonString(JsonFileTranslation);
            var eventJson = JsonHelper.FromJsonArray<StateJsonTranslation>(jsonString);

            foreach (var data in eventJson)
            {
                for (int i = 0; i < DataModels.Count; i++)
                {
                    if (data.id == DataModels[i].id)
                    {
                        DataModels[i].name = data.name;
                        DataModels[i].note = data.note;
                        DataModels[i].message1 = data.message1;
                        DataModels[i].message2 = data.message2;
                        DataModels[i].message3 = data.message3;
                        DataModels[i].message4 = data.message4;
                    }
                }
            }

            Save(DataModels);

            File.Delete(JsonFileTranslation);
        }

        public void OverRayConvert()
        {
            Load();
            for (int i = 0; i < DataModels.Count; i++)
            {
                if (DataModels[i].overlay == "0" || DataModels[i].overlay == "1")
                {
                    DataModels[i].overlay = "";
                }
            }
            
            Save(DataModels);
        }
#endif
    }
}