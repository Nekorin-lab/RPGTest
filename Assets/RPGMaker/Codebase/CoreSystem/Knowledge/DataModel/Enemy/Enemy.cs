using System;
using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Enemy
{
    [Serializable]
    public class EnemyDataModel : WithSerialNumberDataModel
    {
        public List<EnemyAction>          actions;
        public AutoGuideEnemy             autoGuide;
        public int                        battlerHue;
        public int                        deleted;
        public List<DropItem>             dropItems;
        public List<int>                  elements;
        public int                        exp;
        public int                        gold;
        public string                     id;
        public ImageEnemy                 images;
        public int                        level;
        public string                     memo;
        public string                     name;
        public List<int>                  param;
        public List<TraitCommonDataModel> traits;

        public EnemyDataModel(
            string id,
            string name,
            List<int> elements,
            int level,
            AutoGuideEnemy autoGuide,
            List<int> param,
            List<EnemyAction> actions,
            int battlerHue,
            List<DropItem> dropItems,
            int exp,
            int gold,
            string memo,
            List<TraitCommonDataModel> traits,
            ImageEnemy images
        ) {
            this.id = id;
            this.name = name;
            this.elements = elements;
            this.level = level;
            this.autoGuide = autoGuide;
            this.param = param;
            this.actions = actions;
            this.battlerHue = battlerHue;
            this.dropItems = dropItems;
            this.exp = exp;
            this.gold = gold;
            this.memo = memo;
            this.traits = traits;
            this.images = images;
        }

        public static EnemyDataModel CreateDefault(string id, string name) {
            return new EnemyDataModel(
                id,
                name,
                new List<int>(new[] {0, 0, 0}),
                1,
                new AutoGuideEnemy(),
                new List<int>(new[] {0, 0, 0, 0, 0, 0, 0, 0}),
                new List<EnemyAction>(),
                255,
                new List<DropItem>(),
                0,
                0,
                "",
                //敵キャラは固定特徴の追加を行う
                enemyDefaultTraits(),
                new ImageEnemy()
            );

            List<TraitCommonDataModel> enemyDefaultTraits() {
                var returnList = new List<TraitCommonDataModel>();
                //命中率
                returnList.Add(new TraitCommonDataModel(2, 2, 0, 950));
                //回避率
                returnList.Add(new TraitCommonDataModel(2, 2, 1, 50));
                //攻撃時属性
                returnList.Add(new TraitCommonDataModel(3, 1, 0, 0));
                return returnList;
            }
        }


        [Serializable]
        public class AutoGuideEnemy
        {
            public float attackTurn;
            public float guardTurn;
            public int   level;
            public int   magicGuardSetting;
            public int   magicPowerSetting;

            public AutoGuideEnemy() {
                level = 1;
                attackTurn = 4;
                guardTurn = 3;
                magicPowerSetting = 1;
                magicGuardSetting = 1;
            }
        }

        [Serializable]
        public class EnemyAction
        {
            public int    conditionParam1;
            public int    conditionParam2;
            public int    conditionType;
            public int    rating;
            public string skillId;

            public EnemyAction() {
                conditionParam1 = 0;
                conditionParam2 = 0;
                conditionType = 0;
                rating = 5;
                skillId = "1";
            }
        }

        [Serializable]
        public class DropItem
        {
            public string dataId;
            public int    denominator;
            public int    kind;

            public DropItem() {
                dataId = "1";
                denominator = 1;
                kind = 1;
            }
        }

        [Serializable]
        public class ImageEnemy
        {
            public int    autofit;
            public int    autofitPattern;
            public int    battleAlignment;
            public int    high;
            public string image;
            public int    scale;
            public int    wide;

            public ImageEnemy() {
                image = "Enemy1";
                autofit = 0;
                autofitPattern = 0;
                scale = 100;
                battleAlignment = 1;
                high = 100;
                wide = 100;
            }
        }
    }
}