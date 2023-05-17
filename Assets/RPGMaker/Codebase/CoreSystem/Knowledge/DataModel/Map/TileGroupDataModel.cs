using System;
using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Map
{
    [Serializable]
    public class TileGroupDataModel : WithSerialNumberDataModel
    {
        public string              id;
        public string              name;
        public List<TileDataModel> tileDataModels;

        public TileGroupDataModel(string id, string name, List<TileDataModel> tileDataModels) {
            this.id = id;
            this.name = name;
            this.tileDataModels = tileDataModels;
        }

        public TileGroupDataModel Clone() {
            return (TileGroupDataModel) MemberwiseClone();
        }
    }
}