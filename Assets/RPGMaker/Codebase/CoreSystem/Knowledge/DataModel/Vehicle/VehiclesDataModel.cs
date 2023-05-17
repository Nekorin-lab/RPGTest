using System;
using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Vehicle
{
    [Serializable]
    public class VehiclesDataModel : WithSerialNumberDataModel
    {
        // 移動領域。
        public enum MoveAriaType
        {
            None,   // なし。
            Low,    // 低空域。
            High,   // 高空域。
        }

        public BGM       bgm;
        public string    id;
        public string    images;
        public List<int> initialPos;
        public string    mapId;
        public List<int> moveTags;
        public string    name;
        public int       speed;

        public VehiclesDataModel(
            string id,
            string name,
            List<int> moveTags,
            int speed,
            string mapId,
            List<int> initialPos,
            BGM bgm,
            string images
        ) {
            this.id = id;
            this.name = name;
            this.moveTags = moveTags;
            this.speed = speed;
            this.mapId = mapId;
            this.initialPos = initialPos;
            this.bgm = bgm;
            this.images = images;
        }

        // 移動領域の取得。
        public MoveAriaType MoveAria
        {
            get
            {
                for (var index = 0; index < moveTags.Count; index++)
                {
                    if (moveTags[index] != 0)
                    {
                        return (MoveAriaType)index;
                    }
                }

                return (MoveAriaType)0;
            }
        }

        public static VehiclesDataModel CreateDefault() {
            return new VehiclesDataModel(
                Guid.NewGuid().ToString(),
                "",
                new List<int> {0, 0, 0},
                10,
                "",
                new List<int> {0, 0},
                BGM.CreateDefault(),
                ""
            );
        }

        [Serializable]
        public class BGM
        {
            public string name;
            public int    pan;
            public int    pitch;
            public int    volume;

            public BGM(
                string name,
                int pan,
                int pitch,
                int volume
            ) {
                this.name = name;
                this.pan = pan;
                this.pitch = pitch;
                this.volume = volume;
            }

            public static BGM CreateDefault() {
                return new BGM(
                    "",
                    0,
                    100,
                    90
                );
            }
        }
    }
}