using System.Collections.Generic;
using System.IO;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Class;
using RPGMaker.Codebase.CoreSystem.Knowledge.JsonStructure;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Service.DatabaseManagement.Repository
{
    public class ClassRepository : AbstractDatabaseRepository<ClassDataModel>
    {
        protected override string JsonPath => "Assets/RPGMaker/Storage/Character/JSON/class.json";
#if UNITY_EDITOR
        private const string JsonFileTranslation = "Assets/RPGMaker/Storage/Character/JSON/classname.json";
#endif

#if !UNITY_EDITOR
        public new List<ClassDataModel> Load() {
            if (DataModels != null)
            {
                // キャッシュがあればそれを返す
                return DataModels;
            }
            DataModels = ScriptableObjectOperator.GetClass<ClassDataModel>(JsonPath) as List<ClassDataModel>;
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
            var eventJson = JsonHelper.FromJsonArray<ClassJsonTranslation>(jsonString);

            foreach (var data in eventJson)
            {
                for (int i = 0; i < DataModels.Count; i++)
                {
                    if (data.id == DataModels[i].basic.id)
                    {
                        DataModels[i].basic.name = data.name;
                    }
                }
            }

            Save(DataModels);

            File.Delete(JsonFileTranslation);
        }
#endif
    }
}