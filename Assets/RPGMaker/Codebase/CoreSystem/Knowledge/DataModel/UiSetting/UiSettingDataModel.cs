using System;
using System.Collections.Generic;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.UiSetting
{
    [Serializable]
    public class UiSettingDataModel
    {
        public BattleMenu       battleMenu;
        public List<CommonMenu> commonMenus;
        public GameMenu         gameMenu;
        public TalkMenu         talkMenu;

        public UiSettingDataModel(
            List<CommonMenu> commonMenus,
            GameMenu gameMenu,
            BattleMenu battleMenu,
            TalkMenu talkMenu
        ) {
            this.commonMenus = commonMenus;
            this.gameMenu = gameMenu;
            this.battleMenu = battleMenu;
            this.talkMenu = talkMenu;
        }

        [Serializable]
        public class MenuItem
        {
            public int enabled;

            public MenuItem(int enabled) {
                this.enabled = enabled;
            }
        }

        [Serializable]
        public class CommonMenu
        {
            public BackgroundImage backgroundImage;
            public BackgroundImage buttonFrameImage;
            public BackgroundImage buttonImage;
            public List<int>       buttonImageHighlight;
            public int             characterType;
            public string          id;
            public MenuFontSetting menuFontSetting;
            public BackgroundImage windowBackgroundImage;
            public BackgroundImage windowFrameImage;
            public List<int>       windowFrameImageHighlight;

            public CommonMenu(
                string id,
                int characterType,
                MenuFontSetting menuFontSetting,
                BackgroundImage backgroundImage,
                BackgroundImage windowBackgroundImage,
                BackgroundImage windowFrameImage,
                List<int> windowFrameImageHighlight,
                BackgroundImage buttonImage,
                List<int> buttonImageHighlight,
                BackgroundImage buttonFrameImage
            ) {
                this.id = id;
                this.characterType = characterType;
                this.menuFontSetting = menuFontSetting;
                this.backgroundImage = backgroundImage;
                this.windowBackgroundImage = windowBackgroundImage;
                this.windowFrameImage = windowFrameImage;
                this.windowFrameImageHighlight = windowFrameImageHighlight;
                this.buttonImage = buttonImage;
                this.buttonImageHighlight = buttonImageHighlight;
                this.buttonFrameImage = buttonFrameImage;
            }

            public static CommonMenu CreateDefault() {
                return new CommonMenu(
                    Guid.NewGuid().ToString(),
                    0,
                    MenuFontSetting.CreateDefault(),
                    BackgroundImage.CreateDefault(),
                    BackgroundImage.CreateDefault(),
                    BackgroundImage.CreateDefault(),
                    new List<int> {255, 255, 255, 255},
                    BackgroundImage.CreateDefault(),
                    new List<int> {255, 255, 255, 255},
                    BackgroundImage.CreateDefault()
                );
            }
        }

        [Serializable]
        public class BackgroundImage
        {
            public List<int> color;
            public string    image;
            public int       type;

            public BackgroundImage(int type, string image, List<int> color) {
                this.type = type;
                this.image = image;
                this.color = color;
            }

            public static BackgroundImage CreateDefault() {
                return new BackgroundImage(1, "", new List<int> {255, 255, 255, 255});
            }
        }

        [Serializable]
        public class GameMenu
        {
            public MenuItem        categoryArmor;
            public MenuItem        categoryImportant;
            public MenuItem        categoryItem;
            public MenuItem        categoryWeapon;
            public MenuItem        menuEquipment;
            public MenuFontSetting menuFontSetting;
            public MenuItem        menuGameEnd;
            public MenuItem        menuItem;
            public MenuItem        menuOption;
            public MenuItem        menuSave;
            public MenuItem        menuSkill;
            public MenuItem        menuSort;
            public MenuItem        menuStatus;

            public GameMenu(
                MenuItem menuItem,
                MenuItem menuSkill,
                MenuItem menuEquipment,
                MenuItem menuStatus,
                MenuItem menuSort,
                MenuItem menuSave,
                MenuItem menuOption,
                MenuItem menuGameEnd,
                MenuItem categoryItem,
                MenuItem categoryWeapon,
                MenuItem categoryArmor,
                MenuItem categoryImportant,
                MenuFontSetting menuFontSetting
            ) {
                this.menuItem = menuItem;
                this.menuSkill = menuSkill;
                this.menuEquipment = menuEquipment;
                this.menuStatus = menuStatus;
                this.menuSort = menuSort;
                this.menuSave = menuSave;
                this.menuOption = menuOption;
                this.menuGameEnd = menuGameEnd;
                this.categoryItem = categoryItem;
                this.categoryWeapon = categoryWeapon;
                this.categoryArmor = categoryArmor;
                this.categoryImportant = categoryImportant;
                this.menuFontSetting = menuFontSetting;
            }
        }

        [Serializable]
        public class BattleMenu
        {
            public MenuItem menuHp;
            public MenuItem menuMp;
            public MenuItem menuTp;

            public BattleMenu(MenuItem menuHp, MenuItem menuMp, MenuItem menuTp) {
                this.menuHp = menuHp;
                this.menuMp = menuMp;
                this.menuTp = menuTp;
            }
        }

        [Serializable]
        public class TalkMenu
        {
            public TalkCharacterMenu  characterMenu;
            public TalkItemSelectMenu itemSelectMenu;
            public TalkNumberMenu     numberMenu;
            public TalkSelectMenu     selectMenu;

            public TalkMenu(
                TalkCharacterMenu characterMenu,
                TalkSelectMenu selectMenu,
                TalkItemSelectMenu itemSelectMenu,
                TalkNumberMenu numberMenu
            ) {
                this.characterMenu = characterMenu;
                this.selectMenu = selectMenu;
                this.itemSelectMenu = itemSelectMenu;
                this.numberMenu = numberMenu;
            }
        }

        [Serializable]
        public class TalkCharacterMenu
        {
            public int             characterEnabled;
            public int             nameEnabled;
            public MenuFontSetting nameFontSetting;
            public MenuFontSetting talkFontSetting;

            public TalkCharacterMenu(
                int nameEnabled,
                int characterEnabled,
                MenuFontSetting nameFontSetting,
                MenuFontSetting talkFontSetting
            ) {
                this.nameEnabled = nameEnabled;
                this.characterEnabled = characterEnabled;
                this.nameFontSetting = nameFontSetting;
                this.talkFontSetting = talkFontSetting;
            }

            public static TalkCharacterMenu CreateDefault() {
                return new TalkCharacterMenu(1, 0, MenuFontSetting.CreateDefault(), MenuFontSetting.CreateDefault());
            }
        }

        [Serializable]
        public class MenuFontSetting
        {
            public List<int> color;
            public string    font;
            public int       size;

            public MenuFontSetting(string font, int size, List<int> color) {
                this.font = font;
                this.size = size;
                this.color = color;
            }

            public static MenuFontSetting CreateDefault() {
                return new MenuFontSetting("", 16, new List<int> {255, 255, 255, 255});
            }
        }

        [Serializable]
        public class TalkSelectMenu
        {
            public MenuFontSetting menuFontSetting;

            public TalkSelectMenu(MenuFontSetting menuFontSetting) {
                this.menuFontSetting = menuFontSetting;
            }
        }

        [Serializable]
        public class TalkItemSelectMenu
        {
            public MenuFontSetting menuFontSetting;
            public int             positionItemWindow;

            public TalkItemSelectMenu(int positionItemWindow, MenuFontSetting menuFontSetting) {
                this.positionItemWindow = positionItemWindow;
                this.menuFontSetting = menuFontSetting;
            }

            public TalkItemSelectMenu CreateDefault() {
                return new TalkItemSelectMenu(0, MenuFontSetting.CreateDefault());
            }
        }

        [Serializable]
        public class TalkNumberMenu
        {
            public MenuFontSetting menuFontSetting;
            public int             numberEnabled;
            public List<int>       positionNumberWindow;

            public TalkNumberMenu(int numberEnabled, List<int> positionNumberWindow, MenuFontSetting menuFontSetting) {
                this.numberEnabled = numberEnabled;
                this.positionNumberWindow = positionNumberWindow;
                this.menuFontSetting = menuFontSetting;
            }

            public static TalkNumberMenu CreateDefault() {
                return new TalkNumberMenu(1, new List<int> {100, 200}, MenuFontSetting.CreateDefault());
            }
        }
    }
}