using System;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common
{
    [Serializable]
    public class SoundCommonDataModel
    {
        public string name;
        public int    pan;
        public int    pitch;
        public int    volume;

        public SoundCommonDataModel(string name, int pan, int pitch, int volume) {
            this.name = name;
            this.pan = pan;
            this.pitch = pitch;
            this.volume = volume;
        }

        public static SoundCommonDataModel CreateDefault() {
            return new SoundCommonDataModel("", 0, 0, 100);
        }
    }
}