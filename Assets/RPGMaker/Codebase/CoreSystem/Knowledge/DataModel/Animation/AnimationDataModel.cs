using System;
using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Animation
{
    [Serializable]
    public class AnimationDataModel : WithSerialNumberDataModel
    {
        public int         expansion;
        public List<Flash> flashList;
        public string      id;
        public string      offset;
        public string      particleId;
        public string      particleName;
        public int         particlePos;
        public int         particleType;
        public int         playSpeed;
        public string      rotation;
        public List<Se>    seList;
        public string      targetImageName;

        public AnimationDataModel(
            string id,
            string particleName,
            int particleType,
            int particlePos,
            string particleId,
            string targetImageName,
            int expansion,
            int playSpeed,
            string rotation,
            string offset,
            List<Se> seList,
            List<Flash> flashList
        ) {
            this.id = id;
            this.particleName = particleName;
            this.particleType = particleType;
            this.particlePos = particlePos;
            this.particleId = particleId;
            this.targetImageName = targetImageName;
            this.expansion = expansion;
            this.playSpeed = playSpeed;
            this.rotation = rotation;
            this.offset = offset;
            this.seList = seList;
            this.flashList = flashList;
        }

        public static AnimationDataModel CreateDefault(string id) {
            return new AnimationDataModel(
                id,
                "",
                0,
                0,
                "",
                "",
                0,
                0,
                "",
                "",
                new List<Se>(),
                new List<Flash>());
        }


        [Serializable]
        public class Flash
        {
            public string color;
            public int    flashId;
            public int    flashType;
            public int    frame;
            public int    time;

            public Flash(int flashId, int frame, int time, string color, int flashType) {
                this.flashId = flashId;
                this.frame = frame;
                this.time = time;
                this.color = color;
                this.flashType = flashType;
            }
        }

        [Serializable]
        public class Se
        {
            public int    frame;
            public int    seId;
            public string seName;

            public Se(int seId, string seName, int frame) {
                this.seId = seId;
                this.seName = seName;
                this.frame = frame;
            }
        }
    }
}