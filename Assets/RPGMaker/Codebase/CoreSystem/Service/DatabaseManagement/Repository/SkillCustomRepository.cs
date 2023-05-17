using System.Collections.Generic;
using System.IO;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.SkillCustom;
using RPGMaker.Codebase.CoreSystem.Knowledge.JsonStructure;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Service.DatabaseManagement.Repository
{
    public class SkillCustomRepository : AbstractDatabaseRepository<SkillCustomDataModel>
    {
        protected override string JsonPath => "Assets/RPGMaker/Storage/Initializations/JSON/skillCustom.json";
#if UNITY_EDITOR
        private const string JsonFileTranslation = "Assets/RPGMaker/Storage/Initializations/JSON/skillCustomname.json";
#endif

        public void OldSkill() {
            if (DataModels == null) Load();
            if (DataModels?.Count == 0) return;
            var flg = false;
            foreach (var skillCustomDataModel in DataModels)
                if (skillCustomDataModel.targetEffect.targetTeam == 4)
                {
                    flg = true;
                    break;
                }

            if (flg)
                foreach (var skillCustomDataModel in DataModels)
                {
                    if (skillCustomDataModel.targetEffect.targetTeam == 4)
                        skillCustomDataModel.targetEffect.targetTeam = skillCustomDataModel.targetEffect.targetTeam - 1;

                    if (skillCustomDataModel.targetEffect.activate.hitType == 1)
                        skillCustomDataModel.targetEffect.activate.hitType = 0;

                    if (skillCustomDataModel.targetEffect.activate.hitType == 2)
                        skillCustomDataModel.targetEffect.activate.hitType = 1;

                    if (skillCustomDataModel.targetEffect.activate.hitType == 3)
                        skillCustomDataModel.targetEffect.activate.hitType = 2;
                }

            flg = false;
            foreach (var skillCustomDataModel in DataModels)
                if (skillCustomDataModel.userEffect.targetTeam == 4)
                {
                    flg = true;
                    break;
                }

            if (flg)
                foreach (var skillCustomDataModel in DataModels)
                {
                    if (skillCustomDataModel.userEffect.targetTeam == 4)
                        skillCustomDataModel.userEffect.targetTeam = skillCustomDataModel.userEffect.targetTeam - 1;

                    if (skillCustomDataModel.userEffect.activate.hitType == 1)
                        skillCustomDataModel.userEffect.activate.hitType = 0;

                    if (skillCustomDataModel.userEffect.activate.hitType == 2)
                        skillCustomDataModel.userEffect.activate.hitType = 1;

                    if (skillCustomDataModel.userEffect.activate.hitType == 3)
                        skillCustomDataModel.userEffect.activate.hitType = 2;
                }
        }

#if !UNITY_EDITOR
        public new List<SkillCustomDataModel> Load() {
            if (DataModels != null)
            {
                // キャッシュがあればそれを返す
                return DataModels;
            }
            DataModels = ScriptableObjectOperator.GetClass<SkillCustomDataModel>(JsonPath) as List<SkillCustomDataModel>;
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
            var eventJson = JsonHelper.FromJsonArray<SkillCustomJsonTranslation>(jsonString);

            foreach (var data in eventJson)
            {
                for (int i = 0; i < DataModels.Count; i++)
                {
                    if (data.id == DataModels[i].basic.id)
                    {
                        DataModels[i].basic.name = data.name;
                        DataModels[i].basic.description = data.description;
                        DataModels[i].basic.message = data.message;
                        DataModels[i].memo = data.memo;
                    }
                }
            }

            Save(DataModels);

            File.Delete(JsonFileTranslation);
        }
#endif
    }
}