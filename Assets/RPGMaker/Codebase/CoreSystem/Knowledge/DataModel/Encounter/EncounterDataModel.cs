using System;
using System.Collections.Generic;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Encounter
{
    [Serializable]
    public class EncounterDataModel
    {
        public string      backImage1;
        public string      backImage2;
        public Bgm         bgm;
        public int         deleted;
        public int         enabled;
        public List<Enemy> enemyList;
        public int         enemyMax;
        public int         lowestHighestLevel;
        public string      mapId;
        public int         minimumAssumedLevel;
        public string      name;
        public int         region;
        public int         step;
        public List<Troop> troopList;
        public int         troopPer;

        public EncounterDataModel(
            string mapId,
            int region,
            string name,
            int minimumAssumedLevel,
            int lowestHighestLevel,
            int step,
            int enabled,
            string backImage1,
            string backImage2,
            Bgm bgm,
            int enemyMax,
            int troopPer,
            List<Enemy> enemyList,
            List<Troop> troopList,
            int deleted
        ) {
            this.mapId = mapId;
            this.region = region;
            this.name = name;
            this.minimumAssumedLevel = minimumAssumedLevel;
            this.lowestHighestLevel = lowestHighestLevel;
            this.step = step;
            this.enabled = enabled;
            this.backImage1 = backImage1;
            this.backImage2 = backImage2;
            this.bgm = bgm;
            this.enemyMax = enemyMax;
            this.troopPer = troopPer;
            this.enemyList = enemyList;
            this.troopList = troopList;
            this.deleted = deleted;
        }

        public static EncounterDataModel CreateDefault(string id) {
            return new EncounterDataModel(
                id,
                1,
                "",
                1,
                1,
                1,
                1,
                "",
                "",
                new Bgm("", 0, 100, 90),
                1,
                1,
                new List<Enemy>(),
                new List<Troop>(),
                0);
        }

        [Serializable]
        public class Bgm
        {
            public string name;
            public int    pan;
            public int    pitch;
            public int    volume;

            public Bgm(string name, int pan, int pitch, int volume) {
                this.name = name;
                this.pan = pan;
                this.pitch = pitch;
                this.volume = volume;
            }
        }

        [Serializable]
        public class Enemy
        {
            public string enemyId;
            public int    maxAppearances;
            public int    weight;

            public Enemy(string enemyId, int weight, int maxAppearances) {
                this.enemyId = enemyId;
                this.weight = weight;
                this.maxAppearances = maxAppearances;
            }
        }

        [Serializable]
        public class Troop
        {
            public string troopId;
            public int    weight;

            public Troop(string troopId, int weight) {
                this.troopId = troopId;
                this.weight = weight;
            }
        }
    }
}