using System.Collections.Generic;
using System.IO;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Vehicle;
using RPGMaker.Codebase.CoreSystem.Knowledge.JsonStructure;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Service.DatabaseManagement.Repository
{
    public class VehicleRepository : AbstractDatabaseRepository<VehiclesDataModel>
    {
        protected override string JsonPath => "Assets/RPGMaker/Storage/Character/JSON/vehicles.json";
#if UNITY_EDITOR
        private const string JsonFileTranslation = "Assets/RPGMaker/Storage/Character/JSON/vehiclesname.json";
#endif

#if !UNITY_EDITOR
        public new List<VehiclesDataModel> Load() {
            if (DataModels != null)
            {
                // キャッシュがあればそれを返す
                return DataModels;
            }
            DataModels = ScriptableObjectOperator.GetClass<VehiclesDataModel>(JsonPath) as List<VehiclesDataModel>;
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
            var eventJson = JsonHelper.FromJsonArray<VehicleJsonTranslation>(jsonString);

            foreach (var data in eventJson)
            {
                for (int i = 0; i < DataModels.Count; i++)
                {
                    if (data.id == DataModels[i].id)
                    {
                        DataModels[i].name = data.name;
                    }
                }
            }

            Save(DataModels);

            File.Delete(JsonFileTranslation);
        }
#endif
    }
}