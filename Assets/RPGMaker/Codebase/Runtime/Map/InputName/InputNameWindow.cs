using System;
using System.Collections;
using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.Runtime.Common;
using RPGMaker.Codebase.Runtime.Common.Enum;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace RPGMaker.Codebase.Runtime.Map.InputName
{
    public class InputNameWindow : WindowBase
    {
        private const int MAX_INPUT_NUM = 16;

        private readonly string[] LATIN1 =
        {
            "A", "B", "C", "D", "E", "a", "b", "c", "d", "e",
            "F", "G", "H", "I", "J", "f", "g", "h", "i", "j",
            "K", "L", "M", "N", "O", "k", "l", "m", "n", "o",
            "P", "Q", "R", "S", "T", "p", "q", "r", "s", "t",
            "U", "V", "W", "X", "Y", "u", "v", "w", "x", "y",
            "Z", "[", "]", "^", "_", "z", "{", "}", "|", "~",
            "0", "1", "2", "3", "4", "!", "#", "$", "%", "&",
            "5", "6", "7", "8", "9", "(", ")", "*", "+", "-",
            "/", "=", "@", "<", ">", ":", ";", " ", "Page", "OK"
        };

        private readonly string[] LATIN2 =

        {
            "Á", "É", "Í", "Ó", "Ú", "á", "é", "í", "ó", "ú",
            "À", "È", "Ì", "Ò", "Ù", "à", "è", "ì", "ò", "ù",
            "Â", "Ê", "Î", "Ô", "Û", "â", "ê", "î", "ô", "û",
            "Ä", "Ë", "Ï", "Ö", "Ü", "ä", "ë", "ï", "ö", "ü",
            "Ā", "Ē", "Ī", "Ō", "Ū", "ā", "ē", "ī", "ō", "ū",
            "Ã", "Å", "Æ", "Ç", "Ð", "ã", "å", "æ", "ç", "ð",
            "Ñ", "Õ", "Ø", "Š", "Ŵ", "ñ", "õ", "ø", "š", "ŵ",
            "Ý", "Ŷ", "Ÿ", "Ž", "Þ", "ý", "ÿ", "ŷ", "ž", "þ",
            "Ĳ", "Œ", "ĳ", "œ", "ß", "«", "»", " ", "Page", "OK"
        };

        private readonly string[] RUSSIA =

        {
            "А", "Б", "В", "Г", "Д", "а", "б", "в", "г", "д",
            "Е", "Ё", "Ж", "З", "И", "е", "ё", "ж", "з", "и",
            "Й", "К", "Л", "М", "Н", "й", "к", "л", "м", "н",
            "О", "П", "Р", "С", "Т", "о", "п", "р", "с", "т",
            "У", "Ф", "Х", "Ц", "Ч", "у", "ф", "х", "ц", "ч",
            "Ш", "Щ", "Ъ", "Ы", "Ь", "ш", "щ", "ъ", "ы", "ь",
            "Э", "Ю", "Я", "^", "_", "э", "ю", "я", "%", "&",
            "0", "1", "2", "3", "4", "(", ")", "*", "+", "-",
            "5", "6", "7", "8", "9", ":", ";", " ", "", "OK"
        };

        private readonly string[] JAPAN1 =

        {
            "あ", "い", "う", "え", "お", "が", "ぎ", "ぐ", "げ", "ご",
            "か", "き", "く", "け", "こ", "ざ", "じ", "ず", "ぜ", "ぞ",
            "さ", "し", "す", "せ", "そ", "だ", "ぢ", "づ", "で", "ど",
            "た", "ち", "つ", "て", "と", "ば", "び", "ぶ", "べ", "ぼ",
            "な", "に", "ぬ", "ね", "の", "ぱ", "ぴ", "ぷ", "ぺ", "ぽ",
            "は", "ひ", "ふ", "へ", "ほ", "ぁ", "ぃ", "ぅ", "ぇ", "ぉ",
            "ま", "み", "む", "め", "も", "っ", "ゃ", "ゅ", "ょ", "ゎ",
            "や", "ゆ", "よ", "わ", "ん", "ー", "～", "・", "＝", "☆",
            "ら", "り", "る", "れ", "ろ", "ゔ", "を", "　", "カナ", "決定"
        };

        private readonly string[] JAPAN2 =

        {
            "ア", "イ", "ウ", "エ", "オ", "ガ", "ギ", "グ", "ゲ", "ゴ",
            "カ", "キ", "ク", "ケ", "コ", "ザ", "ジ", "ズ", "ゼ", "ゾ",
            "サ", "シ", "ス", "セ", "ソ", "ダ", "ヂ", "ヅ", "デ", "ド",
            "タ", "チ", "ツ", "テ", "ト", "バ", "ビ", "ブ", "ベ", "ボ",
            "ナ", "ニ", "ヌ", "ネ", "ノ", "パ", "ピ", "プ", "ペ", "ポ",
            "ハ", "ヒ", "フ", "ヘ", "ホ", "ァ", "ィ", "ゥ", "ェ", "ォ",
            "マ", "ミ", "ム", "メ", "モ", "ッ", "ャ", "ュ", "ョ", "ヮ",
            "ヤ", "ユ", "ヨ", "ワ", "ン", "ー", "～", "・", "＝", "☆",
            "ラ", "リ", "ル", "レ", "ロ", "ヴ", "ヲ", "　", "英数", "決定"
        };

        private readonly string[] JAPAN3 =
        {
            "Ａ", "Ｂ", "Ｃ", "Ｄ", "Ｅ", "ａ", "ｂ", "ｃ", "ｄ", "ｅ",
            "Ｆ", "Ｇ", "Ｈ", "Ｉ", "Ｊ", "ｆ", "ｇ", "ｈ", "ｉ", "ｊ",
            "Ｋ", "Ｌ", "Ｍ", "Ｎ", "Ｏ", "ｋ", "ｌ", "ｍ", "ｎ", "ｏ",
            "Ｐ", "Ｑ", "Ｒ", "Ｓ", "Ｔ", "ｐ", "ｑ", "ｒ", "ｓ", "ｔ",
            "Ｕ", "Ｖ", "Ｗ", "Ｘ", "Ｙ", "ｕ", "ｖ", "ｗ", "ｘ", "ｙ",
            "Ｚ", "［", "］", "＾", "＿", "ｚ", "｛", "｝", "｜", "～",
            "０", "１", "２", "３", "４", "！", "＃", "＄", "％", "＆",
            "５", "６", "７", "８", "９", "（", "）", "＊", "＋", "－",
            "／", "＝", "＠", "＜", "＞", "：", "；", "　", "かな", "決定"
        };




        private readonly string[] _alphabet =
        {
            "A", "B", "C", "D", "E", "a", "b", "c", "d", "e",
            "F", "G", "H", "I", "J", "f", "g", "h", "i", "j",
            "K", "L", "M", "N", "O", "k", "l", "m", "n", "o",
            "P", "Q", "R", "S", "T", "p", "q", "r", "s", "t",
            "U", "V", "W", "X", "Y", "u", "v", "w", "x", "y",
            "X", "[", "]", "^", "_", "z", "{", "}", "|", "~",
            "0", "1", "2", "3", "4", "!", "#", "$", "%", "&",
            "5", "6", "7", "8", "9", "(", ")", "*", "+", "-",
            "/", "=", "@", "<", ">", ":", ";", " ", "ﾋﾗ", "決定"
        };

        public  GameObject _charaItems;
        public  GameObject _DecisionNameButton;
        private Action     _endAction;


        private readonly string[] _hiragana =
        {
            "あ", "い", "う", "え", "お", "が", "ぎ", "ぐ", "げ", "ご",
            "か", "き", "く", "け", "こ", "ざ", "じ", "ず", "ぜ", "ぞ",
            "さ", "し", "す", "せ", "そ", "だ", "ぢ", "づ", "で", "ど",
            "た", "ち", "つ", "て", "と", "ば", "び", "ぶ", "べ", "ぼ",
            "な", "に", "ぬ", "ね", "の", "ぱ", "ぴ", "ぷ", "ぺ", "ぽ",
            "は", "ひ", "ふ", "へ", "ほ", "ぁ", "ぃ", "ぅ", "ぇ", "ぉ",
            "ま", "み", "む", "め", "も", "っ", "ゃ", "ゅ", "ょ", "ゎ",
            "や", "ゆ", "よ", "わ", "ん", "ー", "～", "・", "＝", "*",
            "ら", "り", "る", "れ", "ろ", "ヴ", "を", " ", "ｶﾅ", "決定"
        };

        private readonly string[] _katakana =
        {
            "ア", "イ", "ウ", "エ", "オ", "ガ", "ギ", "グ", "ゲ", "ゴ",
            "カ", "キ", "ク", "ケ", "コ", "ザ", "ジ", "ズ", "ゼ", "ゾ",
            "サ", "シ", "ス", "セ", "ソ", "ダ", "ヂ", "ヅ", "デ", "ド",
            "タ", "チ", "ツ", "テ", "ト", "バ", "ビ", "ブ", "ベ", "ボ",
            "ナ", "ニ", "ヌ", "ネ", "ノ", "パ", "ピ", "プ", "ペ", "ポ",
            "ハ", "ヒ", "フ", "ヘ", "ホ", "ァ", "ィ", "ゥ", "ェ", "ォ",
            "マ", "ミ", "ム", "メ", "モ", "ッ", "ャ", "ュ", "ョ", "ヮ",
            "ヤ", "ユ", "ヨ", "ワ", "ン", "ー", "～", "・", "＝", "*",
            "ラ", "リ", "ル", "レ", "ロ", "ヴ", "ヲ", " ", "英数", "決定"
        };

        private IEnumerator _keyControl;
        public  GameObject  _keyItems;
        private Mode        _nowMode = Mode.ALPHABET;
        public  string      ActorID;

        public string actorName;
        private string defaultActorName;

        private readonly List<string>     actorNameChar = new List<string>();
        public           List<GameObject> Charas;
        public           GameObject       Display;
        public           GameObject       Face;

        //顔画像のファイルパス
        private readonly string           FacePath = "Assets/RPGMaker/Storage/Images/Faces/";
        public           int              InputMaxNum;
        public           GameObject       KeyBoard;
        public           List<GameObject> Keys;

        public void Init(Action endAction, string actorid, int maxCount) {
            ActorID = actorid;
            InputMaxNum = maxCount;
            _endAction = endAction;

            if (InputMaxNum >= MAX_INPUT_NUM)
                InputMaxNum = MAX_INPUT_NUM;

            InitEnumerator();
            //ナビゲーション設定
            SetNav();
            //フォーカス初期位置
            bool flg = false;
            for (int i = 0; i < Keys.Count; i++)
            {
                Keys[i].GetComponent<WindowButtonBase>().SetEnabled(true);
                //元々フォーカスが当たっていた場所に、フォーカスしなおし
                if (i == 0)
                {
                    flg = true;
                    Keys[i].GetComponent<Button>().Select();
                }
                else
                {
                    Keys[i].GetComponent<WindowButtonBase>().SetHighlight(false);
                }
            }

            if (!flg)
            {
                Keys[0].GetComponent<Button>().Select();
            }

            InputDistributor.AddInputHandler(
                GameStateHandler.IsMap() ? GameStateHandler.GameState.EVENT : GameStateHandler.GameState.BATTLE_EVENT,
                HandleType.Back, DeleteName);

            //フレーム単位での処理
            TimeHandler.Instance.AddTimeActionEveryFrame(KeyController);
        }
        
        /// <summary>
        /// 入力する文字
        /// </summary>
        /// <param name="obj"></param>
        public void ClickItemButton(GameObject obj) {
            if (InputMaxNum > actorNameChar.Count)
            {
                actorNameChar.Add(obj.transform.Find("Name").GetComponent<Text>().text);
                DispName();
            }
        }

        /// <summary>
        /// 最後の文字から削除
        /// </summary>
        private void DeleteName() {
            if (actorNameChar.Count > 0)
            {
                actorNameChar.RemoveAt(actorNameChar.Count - 1);
                DispName();
            }
        }
        
        /// <summary>
        /// 決定キー
        /// </summary>
        /// <param name="obj"></param>
        public void ClickDecisionNameButton(GameObject obj) {
            DecisionName();
        }
        
        /// <summary>
        /// 入力文字変更
        /// </summary>
        /// <param name="obj"></param>
        public void ClickModeChangeButton(GameObject obj) {
            InputModeChange();
        }


        private void DecisionName() {
            var saveData = DataManager.Self().GetRuntimeSaveDataModel();
            actorName = string.Join("", actorNameChar);

            //空文字だったらデフォルトの名前に戻す
            if (actorName == "")
            {
                actorName = defaultActorName;
                CutName();
                DispName();
                return;
            }

            foreach (var actorData in saveData.runtimeActorDataModels)
                if (ActorID == actorData.actorId)
                {
                    actorData.name = actorName;
                    break;
                }

            Display.SetActive(false);
            KeyBoard.SetActive(false);
            _endAction.Invoke();
            InputDistributor.RemoveInputHandler(
                GameStateHandler.IsMap() ? GameStateHandler.GameState.EVENT : GameStateHandler.GameState.BATTLE_EVENT,
                HandleType.Back, DeleteName);
            TimeHandler.Instance.RemoveTimeAction(KeyController);
        }


        //キーボードを順番に変更
        //今のキーボードを取得して次のキーボードへ切り替える
        private void InputModeChange() {
            switch (_nowMode)
            {
                case Mode.ALPHABET:
                    ModeChange(Mode.HIRAGANA);
                    break;
                case Mode.HIRAGANA:
                    ModeChange(Mode.KATAKANA);
                    break;
                case Mode.KATAKANA:
                    ModeChange(Mode.ALPHABET);
                    break;
            }
        }

        private void InitEnumerator() {
            foreach (Transform items in _charaItems.transform) Charas.Add(items.gameObject);

            foreach (Transform items in _keyItems.transform) Keys.Add(items.gameObject);

            foreach (var items in Charas)
            {
                items.GetComponent<Text>().text = "";
                items.SetActive(false);
            }

            var saveData = DataManager.Self().GetRuntimeSaveDataModel();
            foreach (var actorData in saveData.runtimeActorDataModels)
            {
                if (ActorID == actorData.actorId)
                {
                    actorName = actorData.name;
                    defaultActorName = actorData.name;
                    //顔の読み込み
                    Face.GetComponent<Image>().sprite =
                        UnityEditorWrapper.AssetDatabaseWrapper.LoadAssetAtPath<Sprite>(
                            FacePath + actorData.faceImage + ".png");
                    SetInputData();
                    break;
                }
            }
        }

        private void SetNav() {
            for (int i = 0; i < Keys.Count; i++)
            {
                var select = Keys[i].GetComponent<Selectable>();
                var nav = Keys[i].GetComponent<Button>().navigation;
                nav.mode = Navigation.Mode.Explicit;

                nav.selectOnRight = Keys[i % 10 != 9 ? i + 1 : i - 9].GetComponent<Button>();
                nav.selectOnLeft = Keys[i % 10 == 0 ? i + 9 : i - 1].GetComponent<Button>();
                nav.selectOnDown = Keys[i + 10 < Keys.Count ? i + 10 : i - 80].GetComponent<Button>();
                nav.selectOnUp = Keys[i - 10 < 0 ? i + 80: i - 10].GetComponent<Button>();
                select.navigation = nav;
                
            }
        }

        private void SetInputData() {
            Display.SetActive(true);
            KeyBoard.SetActive(true);

            InputMax(InputMaxNum);

            CutName();

            DispName();

            ModeChange(Mode.ALPHABET);
        }

        private void InputMax(int cnt) {
            for (var i = 0; i < cnt; i++) Charas[i].gameObject.SetActive(true);
        }


        //画面上部の名前入力欄に今の名前を一文字ずつ切り取って入れる
        private void CutName() {
            int cnt;
            if (actorName.Length > InputMaxNum)
                cnt = InputMaxNum;
            else
                cnt = actorName.Length;

            for (var i = 0; i < cnt; i++) actorNameChar.Add(actorName.Substring(i, 1));
        }

        private void DispName() {
            for (var i = 0; i < InputMaxNum; i++) Charas[i].GetComponent<Text>().text = "";
            int cnt;
            if (actorNameChar.Count < InputMaxNum)
                cnt = actorNameChar.Count;
            else
                cnt = InputMaxNum;

            for (var i = 0; i < cnt; i++) Charas[i].GetComponent<Text>().text = actorNameChar[i];
        }

        //キーボードの切り替え部分
        private void ModeChange(Mode mode) {
            string[] str = { };

#if UNITY_EDITOR
            var assembly = typeof(UnityEditor.EditorWindow).Assembly;
            var localizationDatabaseType = assembly.GetType("UnityEditor.LocalizationDatabase");
            var currentEditorLanguageProperty = localizationDatabaseType.GetProperty("currentEditorLanguage");
            var systemLanguage = (SystemLanguage) currentEditorLanguageProperty.GetValue(null);
            if (systemLanguage == SystemLanguage.Japanese)
#else
            if (Application.systemLanguage == SystemLanguage.Japanese)
#endif
            {
                switch (mode)
                {
                    case Mode.ALPHABET:
                        str = JAPAN3;
                        _nowMode = Mode.ALPHABET;
                        break;
                    case Mode.KATAKANA:
                        str = JAPAN2;
                        _nowMode = Mode.KATAKANA;
                        break;
                    case Mode.HIRAGANA:
                        str = JAPAN1;
                        _nowMode = Mode.HIRAGANA;
                        break;
                }
            }
            else
            {
                
                switch (mode)
                {
                    case Mode.ALPHABET:
                        str = LATIN1;
                        _nowMode = Mode.ALPHABET;
                        break;
                    case Mode.KATAKANA:
                        str = LATIN2;
                        _nowMode = Mode.KATAKANA;
                        break;
                    case Mode.HIRAGANA:
                        str = LATIN1;
                        _nowMode = Mode.HIRAGANA;
                        break;
                }
                
            }

            //キーボードの文字盤の切り替え
            for (var i = 0; i < Keys.Count; i++) Keys[i].transform.Find("Name").GetComponent<Text>().text = str[i];
        }

        private void KeyController() {
            
            if (InputHandler.OnDown(Common.Enum.HandleType.PageLeft))
            {
                var mode = _nowMode - 1;
                if (mode < 0)
                {
                    mode = Mode.ALPHABET;
                }
                ModeChange(mode);
            }
            else if (InputHandler.OnDown(Common.Enum.HandleType.PageRight))
            {
                var mode = _nowMode + 1;
                if (mode > Mode.ALPHABET)
                {
                    mode = Mode.KATAKANA;
                }
                ModeChange(mode);
            }
        }

        private enum Mode
        {
            KATAKANA,
            HIRAGANA,
            ALPHABET
        }
    }
}