using System;
using System.Collections.Generic;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Class
{
    [Serializable]
    public class AutoGuideDataModel
    {
        public List<AutoGuideRatio> armorRatio;
        public List<AutoGuideRatio> baseParameterRatio;
        public string               id;
        public List<AutoGuideRatio> weaponRatio;

        public AutoGuideDataModel(
            string id,
            List<AutoGuideRatio> baseParameterRatio,
            List<AutoGuideRatio> weaponRatio,
            List<AutoGuideRatio> armorRatio
        ) {
            this.id = id;
            this.baseParameterRatio = baseParameterRatio;
            this.weaponRatio = weaponRatio;
            this.armorRatio = armorRatio;
        }

        [Serializable]
        public class AutoGuideRatio
        {
            public float attack;
            public float defense;
            public float luck;
            public float magicAttack;
            public float magicDefense;
            public float maxHp;
            public float maxMp;
            public float speed;

            public AutoGuideRatio(
                float maxHp,
                float maxMp,
                float attack,
                float defense,
                float magicAttack,
                float magicDefense,
                float speed,
                float luck
            ) {
                this.maxHp = maxHp;
                this.maxMp = maxMp;
                this.attack = attack;
                this.defense = defense;
                this.magicAttack = magicAttack;
                this.magicDefense = magicDefense;
                this.speed = speed;
                this.luck = luck;
            }
        }
    }
}