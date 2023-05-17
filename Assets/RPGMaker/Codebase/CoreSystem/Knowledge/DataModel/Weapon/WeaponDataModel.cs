using System;
using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Weapon
{
    [Serializable]
    public class WeaponDataModel : WithSerialNumberDataModel
    {
        public Basic                      basic;
        public string                     memo;
        public List<int>                  parameters;
        public List<TraitCommonDataModel> traits;

        public WeaponDataModel(Basic basic, List<int> parameters, List<TraitCommonDataModel> traits, string memo) {
            this.basic = basic;
            this.parameters = parameters;
            this.traits = traits;
            this.memo = memo;
        }

        public static WeaponDataModel CreateDefault(string id) {
            return new WeaponDataModel(Basic.CreateDefault(id), new List<int>(), new List<TraitCommonDataModel>(), "");
        }

        [Serializable]
        public class Basic
        {
            public string animationId;
            public int    canSell;
            public string description;
            public string equipmentTypeId;
            public string iconId;
            public string id;
            public string name;
            public int    price;
            public int    sell;
            public int    switchItem;
            public string weaponTypeId;

            public Basic(
                string id,
                string name,
                string description,
                string equipmentTypeId,
                string animationId,
                string iconId,
                string weaponTypeId,
                int price,
                int sell,
                int canSell,
                int switchItem
            ) {
                this.id = id;
                this.name = name;
                this.description = description;
                this.equipmentTypeId = equipmentTypeId;
                this.weaponTypeId = weaponTypeId;
                this.animationId = animationId;
                this.iconId = iconId;
                this.price = price;
                this.sell = sell;
                this.canSell = canSell;
                this.switchItem = switchItem;
            }

            public static Basic CreateDefault(string id) {
                return new Basic(id, "", "", "", "", "IconSet_000", "", 0, 0, 0, 0);
            }
        }
    }
}