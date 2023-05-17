using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Weapon;
using UnityEngine;

namespace RPGMaker.Codebase.CoreSystem.Helper.SO
{
    public class WeaponSO : ScriptableObject
    {
        public List<WeaponDataModel> dataModels;
    }
}