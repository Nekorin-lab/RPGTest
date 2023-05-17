using System.Collections.Generic;
using System.IO;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Weapon;
using RPGMaker.Codebase.CoreSystem.Knowledge.JsonStructure;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Service.DatabaseManagement.Repository
{
    public class WeaponRepository : AbstractDatabaseRepository<WeaponDataModel>
    {
        protected override string JsonPath => "Assets/RPGMaker/Storage/Initializations/JSON/weapon.json";
#if UNITY_EDITOR
        private const string JsonFileTranslation = "Assets/RPGMaker/Storage/Initializations/JSON/weaponname.json";
#endif

#if !UNITY_EDITOR
        public new List<WeaponDataModel> Load() {
            if (DataModels != null)
            {
                // キャッシュがあればそれを返す
                return DataModels;
            }
            DataModels = ScriptableObjectOperator.GetClass<WeaponDataModel>(JsonPath) as List<WeaponDataModel>;
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
            var eventJson = JsonHelper.FromJsonArray<WeaponJsonTranslation>(jsonString);

            foreach (var data in eventJson)
            {
                for (int i = 0; i < DataModels.Count; i++)
                {
                    if (data.id == DataModels[i].basic.id)
                    {
                        DataModels[i].basic.name = data.name;
                        DataModels[i].basic.description = data.description;
                        DataModels[i].memo = data.memo;
                    }
                }
            }

            Save(DataModels);

            File.Delete(JsonFileTranslation);
        }

        public void SetWeaponEquipType() {
            
            var system = new SystemRepository().Load();
            Load();

            for (int i = 0; i < DataModels.Count; i++)
            {
                if (DataModels[i].basic.equipmentTypeId == "")
                {
                    DataModels[i].basic.equipmentTypeId = system.equipTypes[0].id;
                }
            }

            Save(DataModels);
        }

#endif
    }
}