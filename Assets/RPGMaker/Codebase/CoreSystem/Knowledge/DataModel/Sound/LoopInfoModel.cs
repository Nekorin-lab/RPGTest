using System;
using System.Collections.Generic;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Sound
{
    [Serializable]
    public class LoopInfoModel
    {
        public string name;
        public int start;
        public int end;

        public LoopInfoModel(
            string name,
            int start,
            int end
        )
        {
            this.name = name;
            this.start = start;
            this.end = end;
        }

    }
}