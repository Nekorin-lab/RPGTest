using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace RPGMaker.Codebase.Runtime.Common
{
    [ExecuteInEditMode]
    public class CanvasResolutionManager : ResolutionManager
    {
        List<GameObject> _frameObj;

        // 解像度基準の更新
        protected override void UpdateResolution() {
            var canvasScaler = gameObject.transform.GetComponent<UnityEngine.UI.CanvasScaler>();

            // 高さが大きい場合
            if ((float)_screenHeight / RESOLUTION_RATIO_HEIGHT > (float) _screenWidth / RESOLUTION_RATIO_WIDTH)
                canvasScaler.matchWidthOrHeight = 0;
            else
                canvasScaler.matchWidthOrHeight = 1;

#if UNITY_EDITOR
            if (!EditorApplication.isPlaying) 
            {
                TitleInit();

                if (_frameObj != null)
                {
                    foreach (var obj in _frameObj)
                        UnityEngine.Object.DestroyImmediate(obj);
                    _frameObj = null;
                }
                return;
            }
#endif

            // レターボックスの処理
            if (_frameObj == null)
                CreateLetterBox();
            UpdateLetterBox((int)canvasScaler.matchWidthOrHeight);
        }

        // レターボックスの作成
        private void CreateLetterBox() {
            // アンカー最小、アンカー最大、ピポット（↑→↓←）
            List<List<Vector2>> setting = new List<List<Vector2>>
            {
                new List<Vector2>(){new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1) },
                new List<Vector2>(){new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f) },
                new List<Vector2>(){new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0) },
                new List<Vector2>(){new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f) },
            };

            _frameObj = new List<GameObject>();
            for (int i = 0; i < 4; i++)
            {
                _frameObj.Add(new GameObject());
                _frameObj[i].transform.SetParent(this.transform);
                _frameObj[i].transform.localPosition = Vector3.zero;
                _frameObj[i].transform.localScale = Vector3.one;

                var image = _frameObj[i].AddComponent<UnityEngine.UI.Image>();
                image.color = Color.black;

                var rect = _frameObj[i].gameObject.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(_screenWidth, _screenHeight);
                rect.anchorMin = setting[i][0];
                rect.anchorMax = setting[i][1];
                rect.pivot = setting[i][2];
            }
        }

        // レターボックスの更新
        private void UpdateLetterBox(int match) {
            for (int i = 0; i < 4; i++)
            {
                if (_frameObj == null || _frameObj.Count <= i) continue; 

                float width = 0;
                float height = 0;

                RectTransform rect = _frameObj[i].gameObject.GetComponent<RectTransform>();
                try
                {
                    rect = _frameObj[i].gameObject.GetComponent<RectTransform>();
                } catch (Exception)
                {
                    return;
                }
                if (match == 0)
                {
                    if (i == 0 || i == 2)
                    {
                        width = RESOLUTION_WIDTH;
                        height = RESOLUTION_HEIGHT * ((((float) _screenHeight / RESOLUTION_RATIO_HEIGHT) / ((float) _screenWidth / RESOLUTION_RATIO_WIDTH)) - 1) / 2;
                    }
                }
                else
                {
                    if (i == 1 || i == 3)
                    {
                        width = RESOLUTION_WIDTH * ((((float) _screenWidth / RESOLUTION_WIDTH) / ((float) _screenHeight / RESOLUTION_HEIGHT)) - 1) / 2;
                        height = RESOLUTION_HEIGHT;
                    }
                }
                rect.sizeDelta = new Vector2(width, height);
            }
        }

        // タイトル用初期化
        private void TitleInit() {
            if (gameObject.name != "TitleCanvas") return;

            var menu = gameObject.transform.Find("Menu/TitleMenu/Menus");
            menu.transform.Find("NewGame").transform.gameObject.GetComponent<RectTransform>().sizeDelta = 
                new Vector2(menu.transform.Find("NewGame").transform.gameObject.GetComponent<RectTransform>().rect.width, 65.33334f);
            menu.transform.Find("Continue").transform.gameObject.GetComponent<RectTransform>().sizeDelta =
                new Vector2(menu.transform.Find("Continue").transform.gameObject.GetComponent<RectTransform>().rect.width, 65.33334f);
            menu.transform.Find("Option").transform.gameObject.GetComponent<RectTransform>().sizeDelta =
                new Vector2(menu.transform.Find("Option").transform.gameObject.GetComponent<RectTransform>().rect.width, 65.33334f);
        }
    }
}
