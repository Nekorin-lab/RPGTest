using UnityEngine;

namespace RPGMaker.Codebase.Runtime.Battle
{
    public class CameraRefresh : MonoBehaviour
    {
        void Start()
        {
            // 画面に反映されない為有効切り替え
            GetComponent<Camera>().enabled = false;
            GetComponent<Camera>().enabled = true;
        }
    }
}