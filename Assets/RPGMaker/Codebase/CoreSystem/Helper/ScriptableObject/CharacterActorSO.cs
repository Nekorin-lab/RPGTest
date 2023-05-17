using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.CharacterActor;
using UnityEngine;

namespace RPGMaker.Codebase.CoreSystem.Helper.SO
{
    public class CharacterActorSO : ScriptableObject
    {
        public List<CharacterActorDataModel> dataModels;
    }
}