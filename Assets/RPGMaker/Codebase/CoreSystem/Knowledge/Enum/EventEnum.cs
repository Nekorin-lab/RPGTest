namespace RPGMaker.Codebase.CoreSystem.Knowledge.Enum
{
    public enum EventEnum
    {
        MOVEMENT_ONE_STEP_FORWARD = 53,
        MOVEMENT_ONE_STEP_BACKWARD,
        MOVEMENT_JUMP,
        MOVEMENT_TURN_DOWN = 57,
        MOVEMENT_TURN_LEFT,
        MOVEMENT_TURN_RIGHT,
        MOVEMENT_TURN_UP,
        MOVEMENT_TURN_90_RIGHT,
        MOVEMENT_TURN_90_LEFT,
        MOVEMENT_TURN_180,
        MOVEMENT_TURN_90_RIGHT_OR_LEFT,
        MOVEMENT_TURN_AT_RANDOM,
        MOVEMENT_TURN_TOWARD_PLAYER,
        MOVEMENT_TURN_AWAY_FROM_PLAYER,
        MOVEMENT_WALKING_ANIMATION_ON = 72,
        MOVEMENT_WALKING_ANIMATION_OFF,
        MOVEMENT_STEPPING_ANIMATION_ON,
        MOVEMENT_STEPPING_ANIMATION_OFF,
        MOVEMENT_CHANGE_IMAGE   = 82,
        EVENT_CODE_MESSAGE_TEXT = 101,
        EVENT_CODE_MESSAGE_INPUT_SELECT,
        EVENT_CODE_MESSAGE_INPUT_NUMBER,
        EVENT_CODE_MESSAGE_INPUT_SELECT_ITEM,
        EVENT_CODE_MESSAGE_TEXT_SCROLL,
        EVENT_CODE_FLOW_ANNOTATION = 108,
        EVENT_CODE_FLOW_COMMON_START,
        EVENT_CODE_FLOW_COMMON_END,
        EVENT_CODE_FLOW_IF         = 111,
        EVENT_CODE_FLOW_LOOP,
        EVENT_CODE_FLOW_LOOP_BREAK,
        EVENT_CODE_FLOW_EVENT_BREAK = 115,
        EVENT_CODE_FLOW_JUMP_COMMON = 117,
        EVENT_CODE_FLOW_LABEL,
        EVENT_CODE_FLOW_JUMP_LABEL,
        EVENT_CODE_GAME_SWITCH = 121,
        EVENT_CODE_GAME_VAL,
        EVENT_CODE_GAME_SELF_SWITCH,
        EVENT_CODE_GAME_TIMER,
        EVENT_CODE_PARTY_GOLD,
        EVENT_CODE_PARTY_ITEM,
        EVENT_CODE_PARTY_WEAPON,
        EVENT_CODE_PARTY_ARMS,
        EVENT_CODE_PARTY_CHANGE,
        EVENT_CODE_SYSTEM_BATTLE_BGM = 132,
        EVENT_CODE_SYSTEM_BATTLE_WIN,
        EVENT_CODE_SYSTEM_IS_SAVE,
        EVENT_CODE_SYSTEM_IS_MENU,
        EVENT_CODE_SYSTEM_IS_ENCOUNT,
        EVENT_CODE_SYSTEM_IS_SORT,
        EVENT_CODE_SYSTEM_WINDOW_COLOR,
        EVENT_CODE_SYSTEM_BATTLE_LOSE,
        EVENT_CODE_SYSTEM_SHIP_BGM,
        EVENT_CODE_MOVE_PLACE = 201,
        EVENT_CODE_MOVE_PLACE_SHIP,
        EVENT_CODE_MOVE_SET_EVENT_POINT,
        EVENT_CODE_MOVE_MAP_SCROLL,
        EVENT_CODE_MOVE_SET_MOVE_POINT,
        MOVEMENT_MOVE_AT_RANDOM,
        MOVEMENT_MOVE_TOWARD_PLAYER,
        MOVEMENT_MOVE_AWAY_FROM_PLAYER,
        EVENT_CODE_MOVE_RIDE_SHIP,
        EVENT_CODE_CHARACTER_CHANGE_ALPHA = 211,
        EVENT_CODE_CHARACTER_SHOW_ANIMATION,
        EVENT_CODE_CHARACTER_SHOW_ICON,
        EVENT_CODE_CHARACTER_IS_EVENT,
        EVENT_CODE_CHARACTER_CHANGE_WALK = 216,
        EVENT_CODE_CHARACTER_CHANGE_PARTY,
        EVENT_CODE_DISPLAY_FADEOUT = 221,
        EVENT_CODE_DISPLAY_FADEIN,
        EVENT_CODE_DISPLAY_CHANGE_COLOR,
        EVENT_CODE_DISPLAY_FLASH,
        EVENT_CODE_DISPLAY_SHAKE,
        EVENT_CODE_TIMING_WAIT = 230,
        EVENT_CODE_PICTURE_SHOW,
        EVENT_CODE_PICTURE_MOVE,
        EVENT_CODE_PICTURE_ROTATE,
        EVENT_CODE_PICTURE_CHANGE_COLOR,
        EVENT_CODE_PICTURE_ERASE,
        EVENT_CODE_DISPLAY_WEATHER,
        EVENT_CODE_AUDIO_BGM_PLAY = 241,
        EVENT_CODE_AUDIO_BGM_FADEOUT,
        EVENT_CODE_AUDIO_BGM_SAVE,
        EVENT_CODE_AUDIO_BGM_CONTINUE,
        EVENT_CODE_AUDIO_BGS_PLAY,
        EVENT_CODE_AUDIO_BGS_FADEOUT,
        EVENT_CODE_AUDIO_ME_PLAY = 249,
        EVENT_CODE_AUDIO_SE_PLAY,
        EVENT_CODE_AUDIO_SE_STOP,
        EVENT_CODE_AUDIO_MOVIE_PLAY = 261,
        EVENT_CODE_MAP_CHANGE_NAME  = 281,
        EVENT_CODE_MAP_CHANGE_TILE_SET,
        EVENT_CODE_MAP_CHANGE_BATTLE_BACKGROUND,
        EVENT_CODE_MAP_CHANGE_DISTANT_VIEW,
        EVENT_CODE_MAP_GET_POINT,
        EVENT_CODE_SCENE_SET_BATTLE_CONFIG = 301,
        EVENT_CODE_SCENE_SET_SHOP_CONFIG,
        EVENT_CODE_SCENE_INPUT_NAME,
        EVENT_CODE_ACTOR_CHANGE_HP = 311,
        EVENT_CODE_ACTOR_CHANGE_MP,
        EVENT_CODE_ACTOR_CHANGE_STATE,
        EVENT_CODE_ACTOR_HEAL,
        EVENT_CODE_ACTOR_CHANGE_EXP,
        EVENT_CODE_ACTOR_CHANGE_LEVEL,
        EVENT_CODE_ACTOR_CHANGE_PARAMETER,
        EVENT_CODE_ACTOR_CHANGE_SKILL,
        EVENT_CODE_ACTOR_CHANGE_EQUIPMENT,
        EVENT_CODE_ACTOR_CHANGE_NAME,
        EVENT_CODE_ACTOR_CHANGE_CLASS,
        EVENT_CODE_SYSTEM_CHANGE_ACTOR_IMAGE,
        EVENT_CODE_SYSTEM_CHANGE_SHIP_IMAGE,
        EVENT_CODE_ACTOR_CHANGE_NICKNAME,
        EVENT_CODE_ACTOR_CHANGE_PROFILE,
        EVENT_CODE_ACTOR_CHANGE_TP,
        EVENT_CODE_BATTLE_CHANGE_STATUS = 331,
        EVENT_CODE_BATTLE_CHANGE_MP,
        EVENT_CODE_BATTLE_CHANGE_STATE,
        EVENT_CODE_BATTLE_HEAL,
        EVENT_CODE_BATTLE_APPEAR,
        EVENT_CODE_BATTLE_TRANSFORM,
        EVENT_CODE_BATTLE_SHOW_ANIMATION, //337
        EVENT_CODE_BATTLE_EXEC_COMMAND = 339,
        EVENT_CODE_BATTLE_STOP,
        EVENT_CODE_BATTLE_CHANGE_TP = 342,
        EVENT_CODE_SCENE_MENU_OPEN  = 351,
        EVENT_CODE_SCENE_SAVE_OPEN,
        EVENT_CODE_SCENE_GAME_OVER,
        EVENT_CODE_SCENE_GOTO_TITLE,
        EVENT_CODE_MESSAGE_TEXT_ONE_LINE = 401,
        EVENT_CODE_MESSAGE_INPUT_SELECT_SELECTED,
        EVENT_CODE_MESSAGE_INPUT_SELECT_CANCELED,
        EVENT_CODE_MESSAGE_INPUT_SELECT_END,
        EVENT_CODE_MESSAGE_TEXT_SCROLL_ONE_LINE,
        EVENT_CODE_FLOW_ANNOTATION_MULTILINE = 408,
        EVENT_CODE_FLOW_ELSE                 = 411,
        EVENT_CODE_FLOW_ENDIF,
        EVENT_CODE_FLOW_LOOP_END,
        EVENT_CODE_FLOW_AND,
        EVENT_CODE_FLOW_OR,
        EVENT_CODE_SCENE_SET_BATTLE_CONFIG_WIN = 601,
        EVENT_CODE_SCENE_SET_BATTLE_CONFIG_ESCAPE,
        EVENT_CODE_SCENE_SET_BATTLE_CONFIG_LOSE,
        EVENT_CODE_SCENE_SET_BATTLE_CONFIG_END,
        EVENT_CODE_SCENE_SET_SHOP_CONFIG_LINE,
        EVENT_CODE_ADDON_COMMAND = 707,
        EVENT_CODE_FLOW_CUSTOM_MOVE,
        EVENT_CODE_FLOW_CUSTOM_MOVE_END,
        EVENT_CODE_STEP_MOVE,
        EVENT_CODE_CHANGE_MOVE_SPEED,
        EVENT_CODE_CHANGE_MOVE_FREQUENCY,
        EVENT_CODE_PASS_THROUGH,
        EVENT_CODE_MAX           = 999
    }
}