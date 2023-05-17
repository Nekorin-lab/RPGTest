using System.Collections.Generic;
using UnityEngine;

namespace RPGMaker.Codebase.Runtime.Common
{
    public class ResolutionManager : MonoBehaviour
    {
        // 画面比率
        protected readonly int RESOLUTION_RATIO_WIDTH = 16;
        protected readonly int RESOLUTION_RATIO_HEIGHT = 9;
        protected readonly int RESOLUTION_WIDTH = 1920;
        protected readonly int RESOLUTION_HEIGHT = 1080;

        protected int _screenHeight = 0;
        protected int _screenWidth = 0;

        // Start is called before the first frame update
        void Start()
        {
            UpdateResolution();
        }

        // Update is called once per frame
        void Update()
        {
            // 解像度変更時に更新
            if (_screenHeight != Screen.height || _screenWidth != Screen.width)
            {
                _screenHeight = Screen.height;
                _screenWidth = Screen.width;
                UpdateResolution();
            }
        }

        // 解像度の更新
        protected virtual void UpdateResolution() {
        }
    }
}
