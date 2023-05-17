using System;
using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.CharacterActor
{
    [Serializable]
    public class CharacterActorDataModel : WithSerialNumberDataModel
    {
        public Basic                      basic;
        public int                        charaType;
        public int                        element;
        public List<Equipment>            equips;
        public Image                      image;
        public int                        initialLevel;
        public int                        maxLevel;
        public string                     name;
        public string                     nickname;
        public string                     profile;
        public List<TraitCommonDataModel> traits;
        public string                     uuId;

        public CharacterActorDataModel(
            string uuId,
            int charaType,
            string name,
            string nickname,
            Basic basic,
            int element,
            int initialLevel,
            int maxLevel,
            string profile,
            Image image,
            List<Equipment> equips,
            List<TraitCommonDataModel> traits
        ) {
            this.uuId = uuId;
            this.charaType = charaType;
            this.name = name;
            this.nickname = nickname;
            this.basic = basic;
            this.element = element;
            this.initialLevel = initialLevel;
            this.maxLevel = maxLevel;
            this.profile = profile;
            this.image = image;
            this.equips = equips;
            this.traits = traits;
        }

        public static CharacterActorDataModel CreateDefault(string id, string name, int type) {
            return new CharacterActorDataModel(id, type, "", "", Basic.CreateDefault(name), 0, 1, 99, "",
                Image.CreateDefault(), new List<Equipment> {new Equipment("", "1")}, new List<TraitCommonDataModel>());
        }

        [Serializable]
        public class Basic
        {
            public string classId;
            public int    initialLevel;
            public int    maxLevel;
            public string memo;
            public string name;
            public string profile;
            public string secondName;

            public Basic(
                string name,
                string classId,
                string secondName,
                int initialLevel,
                int maxLevel,
                string profile,
                string memo
            ) {
                this.name = name;
                this.classId = classId;
                this.secondName = secondName;
                this.initialLevel = initialLevel;
                this.maxLevel = maxLevel;
                this.profile = profile;
                this.memo = memo;
            }

            public static Basic CreateDefault(string name) {
                return new Basic(name, "", "", 1, 99, "", "");
            }
        }

        [Serializable]
        public class Image
        {
            public string adv;
            public string battler;
            public string character;
            public string face;

            public Image(string face, string character, string battler, string adv) {
                this.face = face;
                this.character = character;
                this.battler = battler;
                this.adv = adv;
            }

            public static Image CreateDefault() {
                return new Image(string.Empty, string.Empty, string.Empty, string.Empty);
            }
        }

        [Serializable]
        public class Adv
        {
            public string id;
            public string image;
            public string name;

            public Adv(string id, string name, string image) {
                this.id = id;
                this.name = name;
                this.image = image;
            }

            public static Adv CreateDefault() {
                return new Adv(Guid.NewGuid().ToString(), "AAA", "AAA");
            }
        }

        [Serializable]
        public class Equipment
        {
            public string type;
            public string value;

            public Equipment(string type, string value) {
                this.type = type;
                this.value = value;
            }

            public static Equipment CreateDefault() {
                return new Equipment("1", "1");
            }
        }
    }
}