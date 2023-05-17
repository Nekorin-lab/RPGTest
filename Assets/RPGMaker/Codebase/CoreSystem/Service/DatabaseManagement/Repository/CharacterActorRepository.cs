#if !UNITY_EDITOR
using System.Collections.Generic;
#endif
using System.IO;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.CharacterActor;
using RPGMaker.Codebase.CoreSystem.Knowledge.JsonStructure;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Service.DatabaseManagement.Repository
{
    public class CharacterActorRepository : AbstractDatabaseRepository<CharacterActorDataModel>
    {
        protected override string JsonPath => "Assets/RPGMaker/Storage/Character/JSON/characterActor.json";
#if UNITY_EDITOR
        private const string JsonFileTranslation = "Assets/RPGMaker/Storage/Character/JSON/characterActorname.json";
#endif

#if !UNITY_EDITOR
        public new List<CharacterActorDataModel> Load() {
            if (DataModels != null)
            {
                // キャッシュがあればそれを返す
                return DataModels;
            }
            DataModels = ScriptableObjectOperator.GetClass<CharacterActorDataModel>(JsonPath) as List<CharacterActorDataModel>;
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
            var eventJson = JsonHelper.FromJsonArray<CharacterJsonTranslation>(jsonString);

            foreach (var data in eventJson)
            {
                for (int i = 0; i < DataModels.Count; i++)
                {
                    if (data.uuId == DataModels[i].uuId)
                    {
                        DataModels[i].basic.name = data.name;
                        DataModels[i].basic.secondName = data.secondName;
                        DataModels[i].basic.profile = data.profile;
                        DataModels[i].basic.memo = data.memo;
                    }
                }
            }

            Save(DataModels);

            File.Delete(JsonFileTranslation);
        }
#endif
    }
}