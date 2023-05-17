using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.EventMap;
using UnityEngine;

namespace RPGMaker.Codebase.CoreSystem.Helper.SO
{
    public class EventMapSO : ScriptableObject
    {
        public List<EventMapDataModel> dataModels;
    }
}