using System;
using System.Collections.Generic;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Runtime
{
    [Serializable]
    public class RuntimeTitleDataModel
    {
        public Bgm             bgm;
        public string          gameTitle;
        public GameTitleCommon gameTitleCommon;
        public GameTitleImage  gameTitleImage;
        public GameTitleText   gameTitleText;
        public string          note;
        public StartMenu       startMenu;
        public string          titleBackgroundImage;
        public TitleFront      titleFront;

        [Serializable]
        public class TitleFront
        {
            public string image;
            public int[]  position;
            public float  scale;
        }

        [Serializable]
        public class GameTitleCommon
        {
            public int   gameTitleType;
            public int[] position;
        }

        [Serializable]
        public class GameTitleText
        {
            public int[]  color;
            public string font;
            public int    size;
        }

        [Serializable]
        public class GameTitleImage
        {
            public string image;
            public float  scale;
        }

        [Serializable]
        public class Bgm
        {
            public string name;
            public int    pan;
            public int    pitch;
            public int    volume;
        }

        [Serializable]
        public class StartMenu
        {
            public MenuContinue    menuContinue;
            public MenuFontSetting menuFontSetting;
            public MenuNewGame     menuNewGame;
            public MenuOption      menuOption;
            public MenuUiSetting   menuUiSetting;
        }

        [Serializable]
        public class MenuNewGame
        {
            public bool   enabled;
            public string value;
        }

        [Serializable]
        public class MenuContinue
        {
            public bool   enabled;
            public string value;
        }

        [Serializable]
        public class MenuOption
        {
            public bool   enabled;
            public string value;
        }

        [Serializable]
        public class MenuFontSetting
        {
            public List<int> color;
            public string    font;
            public int       size;
        }

        [Serializable]
        public class MenuUiSetting
        {
            public List<int> color;
            public string    frame;
            public List<int> position;
            public float     scale;
            public string    window;
        }
    }
}