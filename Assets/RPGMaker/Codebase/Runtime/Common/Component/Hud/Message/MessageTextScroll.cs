// 以下の例外での停止を回避する為に、長いテキストは複数のTextコンポーネントで表示する。
//   ArgumentException: Mesh can not have more than 65000 vertices
#define SUPPORT_FOR_LONG_TEXT

using RPGMaker.Codebase.CoreSystem.Helper;
using System;
using System.Collections.Generic;
using RPGMaker.Codebase.Runtime.Common.ControlCharacter;
using RPGMaker.Codebase.Runtime.Common.Enum;
using UnityEngine;
using UnityEngine.UI;

namespace RPGMaker.Codebase.Runtime.Common.Component.Hud.Message
{
    /// <summary>
    /// イベントコマンド『文章のスクロール』用コンポーネント。
    /// </summary>
    public class MessageTextScroll : MonoBehaviour
    {
        // const
        //--------------------------------------------------------------------------------------------------------------
        private const string PrefabPath =
            "Assets/RPGMaker/Codebase/Runtime/Map/Asset/Prefab/MessageScroll.prefab";

        private const int FastSpeed = 3;

        // 表示要素プロパティ
        //--------------------------------------------------------------------------------------------------------------
        private GameObject _prefab;
        private GameObject _text;

        // データプロパティ
        //--------------------------------------------------------------------------------------------------------------
        private int activeFrameCount;

        // 状態プロパティ
        //--------------------------------------------------------------------------------------------------------------
        private Action endAction;
        private int scrollSpeed;
        private bool scrollNoFast;

        // initialize methods
        //--------------------------------------------------------------------------------------------------------------
        /**
         * 初期化
         */
        public void Init() {
            var loadPrefab = UnityEditorWrapper.PrefabUtilityWrapper.LoadPrefabContents(PrefabPath);
            _prefab = Instantiate(
                loadPrefab,
                gameObject.transform,
                true
            );
            UnityEditorWrapper.PrefabUtilityWrapper.UnloadPrefabContents(loadPrefab);
            _text = _prefab.transform.Find("Canvas/Panel/Text").gameObject;
        }

        public void SetSpeed(int speed) {
            scrollSpeed = speed;
        }

        public void StartScroll(Action action) {
            endAction = action;
            TimeHandler.Instance.AddTimeActionEveryFrame(ScrollProces);
        }
        
        public void SetScrollText(string scrollText) {
#if SUPPORT_FOR_LONG_TEXT
            var baseTextComponent = _text.GetComponent<Text>();
            var fontColor = new Color(DataManager.Self().GetUiSettingDataModel().talkMenu.characterMenu.talkFontSetting.color[0] / 255f,
                DataManager.Self().GetUiSettingDataModel().talkMenu.characterMenu.talkFontSetting.color[1] / 255f,
                DataManager.Self().GetUiSettingDataModel().talkMenu.characterMenu.talkFontSetting.color[2] / 255f);
            
            var fontSize = new FontSize(DataManager.Self().GetUiSettingDataModel().talkMenu.characterMenu.talkFontSetting.size);


            _text.GetOrAddComponent<VerticalLayoutGroup>();
            foreach (var splitedText in SplitTextForUnityText(scrollText))
            {
                var textComponent = new GameObject("SplitedText").AddComponent<Text>();
                textComponent.text = splitedText[^1] == '\n' ? splitedText[0..^1] : splitedText;
                textComponent.transform.SetParent(_text.transform);
                textComponent.transform.localScale = Vector3.one;

                textComponent.font = baseTextComponent.font;
                textComponent.fontSize = fontSize.ComponentFontSize;
                textComponent.fontStyle =baseTextComponent.fontStyle;
                textComponent.color = fontColor;
            }
            
            
            
#else
            _text.GetComponent<Text>().text = text;
#endif
        }

        public void SetScrollNoFast(bool flg) {
            scrollNoFast = flg;
        }

        private void ScrollProces() {

            var speed = scrollSpeed;
            
            if (InputHandler.OnPress(HandleType.Decide) && !scrollNoFast)
            {
                speed = scrollSpeed / 2 * FastSpeed;
            }
            else
            {
                speed = scrollSpeed;
            }
            
            var rect = _text.GetComponent<RectTransform>();

            if (rect.sizeDelta.y + Screen.height + 40 >
                rect.localPosition.y)
            {
                rect.localPosition += new Vector3(0, 1f * speed);
            }
            else
            {
                TimeHandler.Instance.RemoveTimeAction(ScrollProces);
                endAction.Invoke();
            }
        }

        /// <summary>
        /// テキストを分割する。
        /// </summary>
        /// <param name="text">分割対象テキスト。</param>
        /// <returns>分割後テキスト列。</returns>
        /// <remarks>
        /// 以下の、分割の手順。
        ///   1. 各所のDemiliterStringの直後で分割。
        ///   2. 上記で分割した各テキストを文字数SplitLengthで分割。
        ///      但し、文字数で分割したテキストに改行('\n')が含まれていた場合、
        ///      最後の改行の直後を分割位置とする。
        /// </remarks>
        public static List<string> SplitTextForUnityText(string text)
        {
            const string DemiliterString = "\n\n\n\n";
            const int SplitLength = 5000;
            return CSharpUtil.SplitText(text, SplitLength, null, true);
        }
    }
}