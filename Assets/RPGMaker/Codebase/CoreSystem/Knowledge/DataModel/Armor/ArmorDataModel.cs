using System;
using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Armor
{
    [Serializable]
    public class ArmorDataModel : WithSerialNumberDataModel
    {
        public Basic                      basic;
        public string                     memo;
        public List<int>                  parameters;
        public List<TraitCommonDataModel> traits;

        public ArmorDataModel(Basic basic, List<int> parameters, List<TraitCommonDataModel> traits, string memo) {
            this.basic = basic;
            this.parameters = parameters;
            this.traits = traits;
            this.memo = memo;
        }

        public static ArmorDataModel CreateDefault(string id) {
            return new ArmorDataModel(Basic.CreateDefault(id), new List<int> {0, 0, 0, 0, 0, 0, 0, 0},
                new List<TraitCommonDataModel>(), "");
        }

        [Serializable]
        public class Basic
        {
            public string animationId;
            public string armorTypeId;
            public int    canSell;
            public string description;
            public string equipmentTypeId;
            public string iconId;
            public string id;
            public string name;
            public int    price;
            public int    sell;
            public int    switchItem;

            public Basic(
                string id,
                string name,
                string description,
                string equipmentTypeId,
                string animationId,
                string iconId,
                string armorTypeId,
                int price,
                int sell,
                int canSell,
                int switchItem
            ) {
                this.id = id;
                this.name = name;
                this.description = description;
                this.equipmentTypeId = equipmentTypeId;
                this.animationId = animationId;
                this.iconId = iconId;
                this.armorTypeId = armorTypeId;
                this.price = price;
                this.sell = sell;
                this.canSell = canSell;
                this.switchItem = switchItem;
            }

            public static Basic CreateDefault(string id) {
                return new Basic(
                    id,
                    "",
                    "",
                    "",
                    "",
                    "IconSet_000",
                    "",
                    0,
                    0,
                    0,
                    0
                );
            }
        }
    }
}