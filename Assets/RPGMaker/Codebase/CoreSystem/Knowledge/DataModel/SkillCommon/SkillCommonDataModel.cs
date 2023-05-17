using System;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.SkillCommon
{
    [Serializable]
    public class SkillCommonDataModel : WithSerialNumberDataModel
    {
        public enum SkillType
        {
            None = -1,
            NormalAttack, //通常攻撃
            MagicAttack, //魔法
            SpecialAttack //必殺技
        }

        public Damage damage;

        public string id;

        public SkillCommonDataModel(string id, Damage damage) {
            this.id = id;
            this.damage = damage;
        }

        [Serializable]
        public class Damage
        {
            public MagicAttack   magicAttack;
            public NormalAttack  normalAttack;
            public SpecialAttack specialAttack;

            public Damage(NormalAttack normalAttack, MagicAttack magicAttack, SpecialAttack specialAttack) {
                this.normalAttack = normalAttack;
                this.magicAttack = magicAttack;
                this.specialAttack = specialAttack;
            }
        }

        [Serializable]
        public class NormalAttack
        {
            public float aMag;
            public float bMag;

            public NormalAttack(float aMag, float bMag) {
                this.aMag = aMag;
                this.bMag = bMag;
            }
        }

        [Serializable]
        public class MagicAttack
        {
            public float aMag;
            public float bMag;
            public float cDmg;

            public MagicAttack(float cDmg, float aMag, float bMag) {
                this.cDmg = cDmg;
                this.aMag = aMag;
                this.bMag = bMag;
            }
        }

        [Serializable]
        public class SpecialAttack
        {
            public float aMag;
            public float bMag;
            public float cDmg;

            public SpecialAttack(float cDmg, float aMag, float bMag) {
                this.cDmg = cDmg;
                this.aMag = aMag;
                this.bMag = bMag;
            }
        }
    }
}