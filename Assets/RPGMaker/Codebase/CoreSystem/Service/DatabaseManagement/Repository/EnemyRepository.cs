#if !UNITY_EDITOR
using System.Collections.Generic;
#endif
#if ENABLE_DEVELOPMENT_FIX
using System.Collections.Generic;
#endif
using System.IO;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Enemy;
using RPGMaker.Codebase.CoreSystem.Knowledge.JsonStructure;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Service.DatabaseManagement.Repository
{
    public class EnemyRepository : AbstractDatabaseRepository<EnemyDataModel>
    {
        protected override string JsonPath => "Assets/RPGMaker/Storage/Character/JSON/enemy.json";
#if UNITY_EDITOR
        private const string JsonFileTranslation = "Assets/RPGMaker/Storage/Character/JSON/enemyname.json";
#endif

#if !UNITY_EDITOR
        public new List<EnemyDataModel> Load() {
            if (DataModels != null)
            {
                // キャッシュがあればそれを返す
                return DataModels;
            }
            DataModels = ScriptableObjectOperator.GetClass<EnemyDataModel>(JsonPath) as List<EnemyDataModel>;
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
            var eventJson = JsonHelper.FromJsonArray<EnemyJsonTranslation>(jsonString);

            foreach (var data in eventJson)
            {
                for (int i = 0; i < DataModels.Count; i++)
                {
                    if (data.id == DataModels[i].id)
                    {
                        DataModels[i].name = data.name;
                        DataModels[i].memo = data.memo;
                    }
                }
            }

            Save(DataModels);

            File.Delete(JsonFileTranslation);
        }
#endif

#if ENABLE_DEVELOPMENT_FIX
        public void EnemyRatingFix() {
            if (DataModels == null)
                Load();

            for (int i = 0; i < DataModels.Count; i++)
            {
                for (int i2 = 0; i2 < DataModels[i].actions.Count; i2++)
                {
                    if (DataModels[i].actions[i2].rating > 9)
                    {
                        DataModels[i].actions[i2].rating = 9;
                        Save(DataModels);
                    }
                }
            }
        }
#endif
    }
}