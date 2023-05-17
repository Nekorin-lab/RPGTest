using System;
using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Item
{
    [Serializable]
    public class ItemDataModel : WithSerialNumberDataModel
    {
        public ItemBasic  basic;
        public string     memo;
        public ItemEffect targetEffect;
        public ItemEffect userEffect;

        public ItemDataModel(
            ItemBasic basic,
            ItemEffect targetEffect,
            ItemEffect userEffect,
            string memo
        ) {
            this.basic = basic;
            this.targetEffect = targetEffect;
            this.userEffect = userEffect;
            this.memo = memo;
        }

        public static ItemDataModel CreateDefault(string id) {
            return new ItemDataModel(ItemBasic.CreateDefault(id), ItemEffect.CreateDefault(),
                ItemEffect.CreateDefault(), "");
        }

        [Serializable]
        public class ItemBasic
        {
            public int    canSell;
            public int    canUseTiming;
            public int    consumable;
            public string description;
            public string iconId;
            public string id;
            public int    itemType;
            public string name;
            public int    price;
            public int    sell;
            public int    switchItem;

            public ItemBasic(
                string id,
                string name,
                string iconId,
                string description,
                int itemType,
                int price,
                int sell,
                int consumable,
                int canUseTiming,
                int canSell,
                int switchItem
            ) {
                this.id = id;
                this.name = name;
                this.iconId = iconId;
                this.description = description;
                this.itemType = itemType;
                this.price = price;
                this.sell = sell;
                this.consumable = consumable;
                this.canUseTiming = canUseTiming;
                this.canSell = canSell;
                this.switchItem = switchItem;
            }

            public static ItemBasic CreateDefault(string id) {
                return new ItemBasic(
                    id,
                    "",
                    "IconSet_000",
                    "",
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                );
            }
        }

        [Serializable]
        public class ItemEffect
        {
            public ItemEffectActivate         activate;
            public ItemEffectDamage           damage;
            public ItemEffectHeal             heal;
            public List<TraitCommonDataModel> otherEffects;
            public int                        randomNumber;
            public int                        targetRange;
            public int                        targetStatus;
            public int                        targetTeam;
            public int                        targetUser;

            public ItemEffect(
                int targetTeam,
                int targetUser,
                int targetRange,
                int randomNumber,
                int targetStatus,
                ItemEffectActivate activate,
                ItemEffectHeal heal,
                ItemEffectDamage damage,
                List<TraitCommonDataModel> otherEffects
            ) {
                this.targetTeam = targetTeam;
                this.targetUser = targetUser;
                this.targetRange = targetRange;
                this.randomNumber = randomNumber;
                this.targetStatus = targetStatus;
                this.activate = activate;
                this.heal = heal;
                this.damage = damage;
                this.otherEffects = otherEffects;
            }

            public static ItemEffect CreateDefault() {
                return new ItemEffect(
                    0,
                    0,
                    0,
                    0,
                    0,
                    ItemEffectActivate.CreateDefault(),
                    ItemEffectHeal.CreateDefault(),
                    new ItemEffectDamage(0, new List<int>(), "", 0, 0, 0, 0, 0, 0, 0),
                    new List<TraitCommonDataModel>()
                );
            }
        }

        [Serializable]
        public class ItemEffectActivate
        {
            public string animationId;
            public int    continuousNumber;
            public int    correctionSpeed;
            public int    getTp;
            public int    hitType;
            public int    successRate;

            public ItemEffectActivate(
                int correctionSpeed,
                int successRate,
                int continuousNumber,
                int hitType,
                string animationId,
                int getTp
            ) {
                this.correctionSpeed = correctionSpeed;
                this.successRate = successRate;
                this.continuousNumber = continuousNumber;
                this.hitType = hitType;
                this.animationId = animationId;
                this.getTp = getTp;
            }

            public static ItemEffectActivate CreateDefault() {
                return new ItemEffectActivate(0, 0, 0, 0, "", 0);
            }
        }

        [Serializable]
        public class ItemEffectDamage
        {
            public int       critical;
            public int       damageType;
            public int       distribute;
            public int       distributeEnabled;
            public List<int> elements;
            public int       max;
            public int       maxEnabled;
            public int       min;
            public int       minEnabled;
            public string    value;

            public ItemEffectDamage(
                int damageType,
                List<int> elements,
                string value,
                int distributeEnabled,
                int distribute,
                int maxEnabled,
                int max,
                int minEnabled,
                int min,
                int critical
            ) {
                this.damageType = damageType;
                this.elements = elements;
                this.value = value;
                this.distributeEnabled = distributeEnabled;
                this.distribute = distribute;
                this.maxEnabled = maxEnabled;
                this.max = max;
                this.minEnabled = minEnabled;
                this.min = min;
                this.critical = critical;
            }

            public static ItemEffectDamage CreateDefault() {
                return new ItemEffectDamage(0, new List<int>(), "", 0, 0, 0, 0, 0, 0, 0);
            }
        }

        [Serializable]
        public class ItemEffectHeal
        {
            public ItemEffectHealParam hp;
            public ItemEffectHealParam mp;
            public ItemEffectHealParam tp;

            public ItemEffectHeal(ItemEffectHealParam hp, ItemEffectHealParam mp, ItemEffectHealParam tp) {
                this.hp = hp;
                this.mp = mp;
                this.tp = tp;
            }

            public static ItemEffectHeal CreateDefault() {
                return new ItemEffectHeal(ItemEffectHealParam.CreateDefault(), ItemEffectHealParam.CreateDefault(),
                    ItemEffectHealParam.CreateDefault());
            }
        }

        [Serializable]
        public class ItemEffectHealParam
        {
            public ItemEffectHealParamCalc   calc;
            public int                       enabled;
            public ItemEffectHealParamFixed  fix;
            public ItemEffectHealParamPerMax perMax;

            public ItemEffectHealParam(
                ItemEffectHealParamPerMax perMax,
                ItemEffectHealParamFixed fix,
                ItemEffectHealParamCalc calc,
                int enabled
            ) {
                this.perMax = perMax;
                this.fix = fix;
                this.calc = calc;
                this.enabled = enabled;
            }

            public static ItemEffectHealParam CreateDefault() {
                return new ItemEffectHealParam(ItemEffectHealParamPerMax.CreateDefault(),
                    ItemEffectHealParamFixed.CreateDefault(), ItemEffectHealParamCalc.CreateDefault(), 0);
            }
        }

        [Serializable]
        public class ItemEffectHealParamCalc
        {
            public int    distribute;
            public int    distributeEnabled;
            public int    enabled;
            public int    max;
            public int    maxEnabled;
            public string value;

            public ItemEffectHealParamCalc(
                int enabled,
                string value,
                int distributeEnabled,
                int distribute,
                int maxEnabled,
                int max
            ) {
                this.enabled = enabled;
                this.value = value;
                this.distributeEnabled = distributeEnabled;
                this.distribute = distribute;
                this.maxEnabled = maxEnabled;
                this.max = max;
            }

            public static ItemEffectHealParamCalc CreateDefault() {
                return new ItemEffectHealParamCalc(0, "", 0, 0, 0, 0);
            }
        }

        [Serializable]
        public class ItemEffectHealParamFixed
        {
            public int enabled;
            public int value;

            public ItemEffectHealParamFixed(int enabled, int value) {
                this.enabled = enabled;
                this.value = value;
            }

            public static ItemEffectHealParamFixed CreateDefault() {
                return new ItemEffectHealParamFixed(0, 0);
            }
        }

        [Serializable]
        public class ItemEffectHealParamPerMax
        {
            public int distribute;
            public int distributeEnabled;
            public int enabled;
            public int max;
            public int maxEnabled;
            public int value;

            public ItemEffectHealParamPerMax(
                int enabled,
                int value,
                int distributeEnabled,
                int distribute,
                int maxEnabled,
                int max
            ) {
                this.enabled = enabled;
                this.value = value;
                this.distributeEnabled = distributeEnabled;
                this.distribute = distribute;
                this.maxEnabled = maxEnabled;
                this.max = max;
            }

            public static ItemEffectHealParamPerMax CreateDefault() {
                return new ItemEffectHealParamPerMax(0, 0, 0, 0, 0, 0);
            }
        }
    }
}