using JetBrains.Annotations;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.JsonStructure;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;
using System;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Map
{
    [Serializable]
    public class MapBaseDataModel
    {
        public string id;
        public string name;
        public int SerialNumber;

        public MapBaseDataModel(string id, string name, int SerialNumber) {
            this.id = id;
            this.name = name;
            this.SerialNumber = SerialNumber;
        }
    }
}