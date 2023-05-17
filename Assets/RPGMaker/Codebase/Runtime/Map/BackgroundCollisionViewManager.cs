using UnityEngine;
using UnityEngine.Tilemaps;

namespace RPGMaker.Codebase.Runtime.Map
{
    // 開始時にCollisionレイヤーを非表示にするだけのスクリプト
    public class BackgroundCollisionViewManager : MonoBehaviour
    {
        //==============================================================
        // 開始
        private void Start() {
            // TilemapRendererを非表示に設定
            transform.GetComponent<TilemapRenderer>().enabled = false;
        }
    }
}