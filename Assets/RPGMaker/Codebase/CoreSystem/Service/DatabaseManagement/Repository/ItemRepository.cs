using System;
using System.Collections.Generic;
using System.IO;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Item;
using RPGMaker.Codebase.CoreSystem.Knowledge.JsonStructure;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Service.DatabaseManagement.Repository
{
    public class ItemRepository : AbstractDatabaseRepository<ItemDataModel>
    {
        protected override string JsonPath => "Assets/RPGMaker/Storage/Item/JSON/item.json";
#if UNITY_EDITOR
        private const string JsonFileTranslation = "Assets/RPGMaker/Storage/Item/JSON/itemname.json";
#endif

        public void DeleteItem(int itemId) {
            throw new NotImplementedException();
        }

        public void ChangeMaximum(int maximumNum) {
            throw new NotImplementedException();
        }

        public void OldItem() {
            if (DataModels == null) Load();
            if (DataModels?.Count == 0) return;
            var flg = false;
            foreach (var itemDataModel in DataModels)
                if (itemDataModel.targetEffect.targetTeam == 4)
                {
                    flg = true;
                    break;
                }

            if (flg)
                foreach (var itemDataModel in DataModels)
                {
                    if (itemDataModel.targetEffect.targetTeam == 4)
                        itemDataModel.targetEffect.targetTeam = itemDataModel.targetEffect.targetTeam - 1;

                    if (itemDataModel.targetEffect.activate.hitType == 1)
                        itemDataModel.targetEffect.activate.hitType = 0;

                    if (itemDataModel.targetEffect.activate.hitType == 2)
                        itemDataModel.targetEffect.activate.hitType = 1;

                    if (itemDataModel.targetEffect.activate.hitType == 3)
                        itemDataModel.targetEffect.activate.hitType = 2;
                }

            flg = false;
            foreach (var itemDataModel in DataModels)
                if (itemDataModel.userEffect.targetTeam == 4)
                {
                    flg = true;
                    break;
                }

            if (flg)
                foreach (var itemDataModel in DataModels)
                {
                    if (itemDataModel.userEffect.targetTeam == 4)
                        itemDataModel.userEffect.targetTeam = itemDataModel.userEffect.targetTeam - 1;

                    if (itemDataModel.userEffect.activate.hitType == 1) itemDataModel.userEffect.activate.hitType = 0;

                    if (itemDataModel.userEffect.activate.hitType == 2) itemDataModel.userEffect.activate.hitType = 1;

                    if (itemDataModel.userEffect.activate.hitType == 3) itemDataModel.userEffect.activate.hitType = 2;
                }
        }

#if !UNITY_EDITOR
        public new List<ItemDataModel> Load() {
            if (DataModels != null)
            {
                // キャッシュがあればそれを返す
                return DataModels;
            }
            DataModels = ScriptableObjectOperator.GetClass<ItemDataModel>(JsonPath) as List<ItemDataModel>;
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
            var eventJson = JsonHelper.FromJsonArray<ItemJsonTranslation>(jsonString);

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
#endif
    }
}