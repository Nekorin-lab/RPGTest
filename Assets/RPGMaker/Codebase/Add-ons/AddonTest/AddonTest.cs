/*:
 * @addondesc Add-on Operation Test
 * @author Keiji Agusa
 * @help As an add-on operation test, the add-on provides add-on parameters and self-switch add-on commands.
 * @version 1.0.0
 * 
 * @param integerValue
 * @text integer value
 * @desc Specify an integer value.
 * @type integer
 * @min 10
 * @max 999
 * @default 123
 * 
 * @param numberValue
 * @text floating point number
 * @desc Specify a floating point number.
 * @type number
 * @min 10.5
 * @max 999.9
 * @default 0.123
 * @decimals 3
 * 
 * @param stringValue
 * @text character string
 * @desc Specify a character string.
 * @type string
 * @default abc
 *
 * @param multilineStringValue
 * @text multiline character string
 * @desc Specify a multiline character string
 * @type multiline_string
 * @default abc
 *
 * @param selectionValue
 * @text selection
 * @desc Specify a selection.
 * @type select
 * @option A
 * @option B
 * @option C
 * @option D
 * @default A
 *
 * @param comboValue
 * @text Editable Option
 * @desc Specify an option.
 * @type combo
 * @option A
 * @option B
 * @option C
 * @default B
 *
 * @param booleanValue
 * @text boolean value
 * @desc Specify a boolean value.
 * @type boolean
 * @default true
 * @on Enable
 * @off Disable
 *
 * @param noteValue
 * @text memo
 * @desc Specify a note.
 * @type note
 * @default "text1\ntext2"
 *
 * @param stringArrayValue
 * @text string array
 * @desc Specify a string array value.
 * @type string[]
 * @default ["123", "abc"]
 *
 * @param structValue
 * @text structure
 * @desc Specify a structure value.
 * @type struct<Color>
 * @default {"Red": 1, "Green": 2, "Blue": 3, "Alpha": 0.4}
 *
 * @param structArrayValue
 * @text structure array.
 * @desc Specify a structure array.
 * @type struct<Color>[]
 *
 * @param commonEventValue
 * @text common event
 * @desc Specify a common event value.
 * @type common_event
 *
 * @param mapEventValue
 * @text map event
 * @desc Specify a map evnet.
 * @type map_event
 *
 * @param switchValue
 * @text Switch
 * @desc Sppecify a switch.
 * @type switch
 *
 * @param variableValue
 * @text Variable
 * @desc Specify a variable.
 * @type variable
 *
 * @param animationValue
 * @text Animation
 * @desc Specify an animation.
 * @type animation
 *
 * @param actor
 * @text Actor
 * @desc Specify an actor.
 * @type actor
 *
 * @param classValue
 * @text Class
 * @desc Specify a class.
 * @type class
 *
 * @param skillValue
 * @text Skill
 * @desc Specify a skill.
 * @type skill
 *
 * @param itemValue
 * @text Item
 * @desc Specify an item.
 * @type item
 *
 * @param weaponValue
 * @text Weapon
 * @desc Specify a weapon.
 * @type weapon
 *
 * @param armorValue
 * @text Armor
 * @desc Specify an armor.
 * @type armor
 *
 * @param enemyValue
 * @text Enemy
 * @desc Specify an enemy.
 * @type enemy
 *
 * @param troopValue
 * @text Enemy group
 * @desc Specify an enemy group.
 * @type troop
 *
 * @param stateValue
 * @text State
 * @desc Specify a state.
 * @type state
 *
 * @param tilesetValue
 * @text Tile group
 * @desc Specify a tile group.
 * @type tileset
 *
 * @param fileValue
 * @text File
 * @desc Specify a file.
 * @type file
 *
 * @param stringArray2Value
 * @text 2-dimensional string array
 * @desc Specify a 2-dimensional string array
 * @type string[][]
 *
 * @param numberArrayValue
 * @text number array
 * @desc Specify a number array
 * @type number[]
 *
 * @param multilineStringArrayValue
 * @text multiline character string array
 * @desc Specify a multiline character string array
 * @type multiline_string[]
 *
 * @command LogSelfSwitchValue
 *      @text Self-Switch Log Output
 *      @desc output a self-switch value to Unity Log Window.
 * 
 *  @arg SelfSwitch
 *      @text Self-Switch
 *      @desc Specify a self-switch.
 *      @type select
 *      @option A
 *      @option B
 *      @option C
 *      @option D
 *      @default A
 *
 *
 * @command SetSelfSwitchValue
 *      @text Change Self-Switch Value
 *      @desc Set a value to a self-switch.
 * 
 *  @arg SelfSwitch
 *      @text Self-Switch
 *      @desc Specify a self-switch.
 *      @type select
 *      @option A
 *      @option B
 *      @option C
 *      @option D
 *      @default A
 * 
 *  @arg Value
 *      @text Value
 *      @desc Specify a value.
 *      @type select
 *      @option ON
 *      @value 1
 *      @option OFF
 *      @value 0
 *      @default ON
 *
 * @command LogMapEvent
 *      @text Output Map Event Value
 *      @desc output a map event value to Unity Log WIndow.
 * 
 *  @arg MapEvent
 *      @text Map Event
 *      @desc Specify a map event.
 *      @type map_event
 * 
 *
 *  @command Wait
 *      @text Pause Command Processing
 *      @desc Stop the command processing for a specified frame time.
 * 
 *  @arg FrameCount
 *      @text Frame Count
 *      @desc Frames(1/60sec.)
 *      @type integer
 *      @default 60
 *      @min 0
 *      @max 999
 *
 * @command CommandWithManyTypes
 *      @text Specify various types
 *      @desc Logs out various types of arguments specified in the command.
 * 
 *  @arg integerValue
 *  @text integer value
 *  @desc Specify an integer value.
 *  @type integer
 *  @min 10
 *  @max 999
 *  @default 123
 * 
 *  @arg numberValue
 *  @text floating point number
 *  @desc Specify a floating point number.
 *  @type number
 *  @min 10.5
 *  @max 999.9
 *  @default 0.123
 *  @decimals 3
 * 
 *  @arg stringValue
 *  @text character string
 *  @desc Specify a character string.
 *  @type string
 *  @default abc
 *
 *  @arg multilineStringValue
 *  @text multiline character string
 *  @desc Specify a multiline character string
 *  @type multiline_string
 *  @default abc
 *
 *  @arg selectionValue
 *  @text selection
 *  @desc Specify a selection.
 *  @type select
 *  @option A
 *  @option B
 *  @option C
 *  @option D
 *  @default B
 *
 *  @arg comboValue
 *  @text Editable Option
 *  @desc Specify an option.
 *  @type combo
 *  @option A
 *  @option B
 *  @option C
 *  @default B
 *
 *  @arg booleanValue
 *  @text boolean value
 *  @desc Specify a boolean value.
 *  @type boolean
 *  @default true
 *  @on Enable
 *  @off Disable
 *
 *  @arg noteValue
 *  @text memo
 *  @desc Specify a note.
 *  @type note
 *  @default "text1\ntext2"
 *
 *  @arg stringArrayValue
 *  @text string array
 *  @desc Specify a string array value.
 *  @type string[]
 *  @default ["123", "abc"]
 *
 *  @arg structValue
 *  @text structure
 *  @desc Specify a structure value.
 *  @type struct<Color>
 *  @default {"Red": 1, "Green": 2, "Blue": 3, "Alpha": 0.4}
 *
 *  @arg structArrayValue
 *  @text structure array.
 *  @desc Specify a structure array.
 *  @type struct<Color>[]
 *
 *  @arg commonEventValue
 *  @text common event
 *  @desc Specify a common event value.
 *  @type common_event
 *
 *  @arg mapEventValue
 *  @text map event
 *  @desc Specify a map evnet.
 *  @type map_event
 *
 *  @arg switchValue
 *  @text Switch
 *  @desc Sppecify a switch.
 *  @type switch
 *
 *  @arg variableValue
 *  @text Variable
 *  @desc Specify a variable.
 *  @type variable
 *
 *  @arg animationValue
 *  @text Animation
 *  @desc Specify an animation.
 *  @type animation
 *
 *  @arg actorValue
 *  @text Actor
 *  @desc Specify an actor.
 *  @type actor
 *
 *  @arg classValue
 *  @text Class
 *  @desc Specify a class.
 *  @type class
 *
 *  @arg skillValue
 *  @text Skill
 *  @desc Specify a skill.
 *  @type skill
 *
 *  @arg itemValue
 *  @text Item
 *  @desc Specify an item.
 *  @type item
 *
 *  @arg weaponValue
 *  @text Weapon
 *  @desc Specify a weapon.
 *  @type weapon
 *
 *  @arg armorValue
 *  @text Armor
 *  @desc Specify an armor.
 *  @type armor
 *
 *  @arg enemyValue
 *  @text Enemy
 *  @desc Specify an enemy.
 *  @type enemy
 *
 *  @arg troopValue
 *  @text Enemy group
 *  @desc Specify an enemy group.
 *  @type troop
 *
 *  @arg stateValue
 *  @text State
 *  @desc Specify a state.
 *  @type state
 *
 *  @arg tilesetValue
 *  @text Tile group
 *  @desc Specify a tile group.
 *  @type tileset
 *
 *  @arg fileValue
 *  @text File
 *  @desc Specify a file.
 *  @type file
 *
 *  @arg stringArray2Value
 *  @text 2-dimensional string array
 *  @desc Specify a 2-dimensional string array
 *  @type string[][]
 *
 *  @arg numberArrayValue
 *  @text number array
 *  @desc Specify a number array
 *  @type number[]
 *
 *  @arg multilineStringArrayValue
 *  @text multiline character string array
 *  @desc Specify a multiline character string array
 *  @type multiline_string[]
 *
 * @command IsPartyMemberAllAlive
 *      @text Whether the whole party is alive or not.
 *      @desc Determine if all party members are alive and store in the sefr switch
 * 
 *  @arg SelfSwitch
 *      @text SelfSwitch
 *      @desc ON when all party members are alive, otherwise OFF is set.
 *      @type select
 *      @option A
 *      @option B
 *      @option C
 *      @option D
 *      @default A
 * 
 */

/*:ja
 * @addondesc アドオンの動作テスト
 * @author Keiji Agusa
 * @help アドオンの動作テストとして、アドオンパラメータの指定、セルフスイッチのアドオンコマンドを提供しています。
 * @version 1.0.0
 * 
 * @param integerValue
 * @text 整数値
 * @desc 整数値を指定します。
 * @type integer
 * @min 10
 * @max 999
 * @default 123
 * 
 * @param numberValue
 * @text 浮動小数点数値
 * @desc 浮動小数点数値を指定します。
 * @type number
 * @min 10.5
 * @max 999.9
 * @default 0.123
 * @decimals 3
 * 
 * @param stringValue
 * @text 文字列
 * @desc 文字列を指定します。
 * @type string
 * @default abc
 *
 * @param multilineStringValue
 * @text 複数行文字列
 * @desc 複数行文字列を指定します。
 * @type multiline_string
 * @default abc
 *
 * @param selectionValue
 * @text 選択肢
 * @desc 選択肢を指定します。
 * @type select
 * @option A
 * @option B
 * @option C
 * @option D
 * @default B
 *
 * @param comboValue
 * @text 編集可能オプション
 * @desc オプションを指定します。
 * @type combo
 * @option A
 * @option B
 * @option C
 * @default B
 *
 * @param booleanValue
 * @text 真偽値
 * @desc 真偽値を指定します。
 * @type boolean
 * @default true
 * @on Enable
 * @off Disable
 *
 * @param noteValue
 * @text ノート値
 * @desc ノート値を指定します。
 * @type note
 * @default "text1\ntext2"
 *
 * @param stringArrayValue
 * @text 文字列配列
 * @desc 文字列配列値を指定します。
 * @type string[]
 * @default ["123", "abc"]
 *
 * @param structValue
 * @text 構造体
 * @desc 構造体値を指定します。
 * @type struct<Color>
 * @default {"Red": 1, "Green": 2, "Blue": 3, "Alpha": 0.4}
 *
 * @param structArrayValue
 * @text 構造体配列
 * @desc 構造体配列値を指定します。
 * @type struct<Color>[]
 *
 * @param commonEventValue
 * @text Commonイベント
 * @desc Commonイベントを指定します。
 * @type common_event
 *
 * @param mapEventValue
 * @text Mapイベント
 * @desc Mapイベントを指定します。
 * @type map_event
 *
 * @param switchValue
 * @text スイッチ
 * @desc スイッチを指定します。
 * @type switch
 *
 * @param variableValue
 * @text 変数
 * @desc 変数を指定します。
 * @type variable
 *
 * @param animationValue
 * @text アニメーション
 * @desc アニメーション値を指定します。
 * @type animation
 *
 * @param actorValue
 * @text アクター
 * @desc アクター値を指定します。
 * @type actor
 *
 * @param classValue
 * @text 職業
 * @desc 職業値を指定します。
 * @type class
 *
 * @param skillValue
 * @text スキル
 * @desc スキル値を指定します。
 * @type skill
 *
 * @param itemValue
 * @text アイテム
 * @desc アイテム値を指定します。
 * @type item
 *
 * @param weaponValue
 * @text 武器
 * @desc 武器値を指定します。
 * @type weapon
 *
 * @param armorValue
 * @text 防具
 * @desc 防具値を指定します。
 * @type armor
 *
 * @param enemyValue
 * @text 敵キャラ
 * @desc 敵キャラ値を指定します。
 * @type enemy
 *
 * @param troopValue
 * @text 敵グループ
 * @desc 敵グループ値を指定します。
 * @type troop
 *
 * @param stateValue
 * @text ステート
 * @desc ステート値を指定します。
 * @type state
 *
 * @param tilesetValue
 * @text タイルグループ
 * @desc タイルグループ値を指定します。
 * @type tileset
 *
 * @param fileValue
 * @text ファイル
 * @desc ファイル値を指定します。
 * @type file
 *
 * @param stringArray2Value
 * @text 2次元文字列配列
 * @desc 2次元文字列配列値を指定します。
 * @type string[][]
 *
 * @param numberArrayValue
 * @text 数値配列
 * @desc 数値配列値を指定します。
 * @type number[]
 *
 * @param multilineStringArrayValue
 * @text 複数行文字列配列
 * @desc 複数行文字列配列値を指定します。
 * @type multiline_string[]
 *
 *  @command LogSelfSwitchValue
 *      @text セルフスイッチのログ出力
 *      @desc セルフスイッチの値をログ出力します。
 * 
 *  @arg SelfSwitch
 *      @text セルフスイッチ
 *      @desc セルフスイッチを指定
 *      @type select
 *      @option A
 *      @option B
 *      @option C
 *      @option D
 *      @default A
 * 
 *
 * @command SetSelfSwitchValue
 *      @text セルフスイッチ変更
 *      @desc セルフスイッチに値を設定します。
 * 
 *  @arg SelfSwitch
 *      @text セルフスイッチ
 *      @desc セルフスイッチを指定
 *      @type select
 *      @option A
 *      @option B
 *      @option C
 *      @option D
 *      @default A
 * 
 *  @arg Value
 *      @text 設定値
 *      @desc セルフスイッチの値を指定
 *      @type select
 *      @option ON
 *      @value 1
 *      @option OFF
 *      @value 0
 *      @default ON
 *
 * @command LogMapEvent
 *      @text マップイベントのログ出力
 *      @desc マップイベントをログに出力します。
 * 
 *  @arg MapEvent
 *      @text マップイベント
 *      @desc マップイベントを指定
 *      @type map_event
 * 
 *
 *  @command Wait
 *      @text コマンド進行の一時停止
 *      @desc 指定フレーム時間コマンド進行を停止します。
 * 
 *  @arg FrameCount
 *      @text フレーム
 *      @desc フレーム(1/60秒)
 *      @type integer
 *      @default 60
 *      @min 0
 *      @max 999
 *
 * @command CommandWithManyTypes
 *      @text いろんなタイプを指定したコマンド
 *      @desc コマンドに指定したいろんなタイプの引数をログ出力します。
 * 
 *  @arg integerValue
 *  @text 整数値
 *  @desc 整数値を指定します。
 *  @type integer
 *  @min 10
 *  @max 999
 *  @default 123
 * 
 *  @arg numberValue
 *  @text 浮動小数点数値
 *  @desc 浮動小数点数値を指定します。
 *  @type number
 *  @min 10.5
 *  @max 999.9
 *  @default 0.123
 *  @decimals 3
 * 
 *  @arg stringValue
 *  @text 文字列
 *  @desc 文字列を指定します。
 *  @type string
 *  @default abc
 *
 *  @arg multilineStringValue
 *  @text 複数行文字列
 *  @desc 複数行文字列を指定します。
 *  @type multiline_string
 *  @default abc
 *
 *  @arg selectionValue
 *  @text 選択肢
 *  @desc 選択肢を指定します。
 *  @type select
 *  @option A
 *  @option B
 *  @option C
 *  @option D
 *  @default B
 *
 *  @arg comboValue
 *  @text 編集可能オプション
 *  @desc オプションを指定します。
 *  @type combo
 *  @option A
 *  @option B
 *  @option C
 *  @default B
 *
 *  @arg booleanValue
 *  @text 真偽値
 *  @desc 真偽値を指定します。
 *  @type boolean
 *  @default true
 *  @on Enable
 *  @off Disable
 *
 *  @arg noteValue
 *  @text ノート値
 *  @desc ノート値を指定します。
 *  @type note
 *  @default "text1\ntext2"
 *
 *  @arg stringArrayValue
 *  @text 文字列配列
 *  @desc 文字列配列値を指定します。
 *  @type string[]
 *  @default ["123", "abc"]
 *
 *  @arg structValue
 *  @text 構造体
 *  @desc 構造体値を指定します。
 *  @type struct<Color>
 *  @default {"Red": 1, "Green": 2, "Blue": 3, "Alpha": 0.4}
 *
 *  @arg structArrayValue
 *  @text 構造体配列
 *  @desc 構造体配列値を指定します。
 *  @type struct<Color>[]
 *
 *  @arg commonEventValue
 *  @text Commonイベント
 *  @desc Commonイベントを指定します。
 *  @type common_event
 *
 *  @arg mapEventValue
 *  @text Mapイベント
 *  @desc Mapイベントを指定します。
 *  @type map_event
 *
 *  @arg switchValue
 *  @text スイッチ
 *  @desc スイッチを指定します。
 *  @type switch
 *
 *  @arg variableValue
 *  @text 変数
 *  @desc 変数を指定します。
 *  @type variable
 *
 *  @arg animationValue
 *  @text アニメーション
 *  @desc アニメーション値を指定します。
 *  @type animation
 *
 *  @arg actorValue
 *  @text アクター
 *  @desc アクター値を指定します。
 *  @type actor
 *
 *  @arg classValue
 *  @text 職業
 *  @desc 職業値を指定します。
 *  @type class
 *
 *  @arg skillValue
 *  @text スキル
 *  @desc スキル値を指定します。
 *  @type skill
 *
 *  @arg itemValue
 *  @text アイテム
 *  @desc アイテム値を指定します。
 *  @type item
 *
 *  @arg weaponValue
 *  @text 武器
 *  @desc 武器値を指定します。
 *  @type weapon
 *
 *  @arg armorValue
 *  @text 防具
 *  @desc 防具値を指定します。
 *  @type armor
 *
 *  @arg enemyValue
 *  @text 敵キャラ
 *  @desc 敵キャラ値を指定します。
 *  @type enemy
 *
 *  @arg troopValue
 *  @text 敵グループ
 *  @desc 敵グループ値を指定します。
 *  @type troop
 *
 *  @arg stateValue
 *  @text ステート
 *  @desc ステート値を指定します。
 *  @type state
 *
 *  @arg tilesetValue
 *  @text タイルグループ
 *  @desc タイルグループ値を指定します。
 *  @type tileset
 *
 *  @arg fileValue
 *  @text ファイル
 *  @desc ファイル値を指定します。
 *  @type file
 *
 *  @arg stringArray2Value
 *  @text 2次元文字列配列
 *  @desc 2次元文字列配列値を指定します。
 *  @type string[][]
 *
 *  @arg numberArrayValue
 *  @text 数値配列
 *  @desc 数値配列値を指定します。
 *  @type number[]
 *
 *  @arg multilineStringArrayValue
 *  @text 複数行文字列配列
 *  @desc 複数行文字列配列値を指定します。
 *  @type multiline_string[]
 *
 * @command IsPartyMemberAllAlive
 *      @text パーティ全員が生きているか
 *      @desc パーティ全員が生きているかを判定し、セルフスイッチに格納します
 * 
 *  @arg SelfSwitch
 *      @text セルフスイッチ
 *      @desc パーティーメンバー全員が生きているときONが、そうでなければOFFが設定されます。
 *      @type select
 *      @option A
 *      @option B
 *      @option C
 *      @option D
 *      @default A
 * 
 */
/*~struct~Color:
 * 
 * @param Red
 * @text 赤
 * @max 255
 * @type number
 * @default 0
 * 
 * @param Green
 * @max 255
 * @type number
 * @default 0
 * 
 * @param Blue
 * @max 255
 * @type number
 * @default 0
 * 
 * @param Alpha
 * @type number
 * @text Alpha(透明率)
 * @desc 1なら不透明、0なら完全に透明になります。
 * @max 1
 * @decimals 2
 * @default 0.5
 *
 * @param MapEvent
 * @type map_event
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.EventMap;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Runtime;
using RPGMaker.Codebase.CoreSystem.Service.EventManagement;
using RPGMaker.Codebase.CoreSystem.Service.MapManagement;
using RPGMaker.Codebase.Runtime.Common;
using RPGMaker.Codebase.Runtime.Addon;
using UnityEngine;

namespace RPGMaker.Codebase.Addon
{
    public class AddonTest
    {
        static List<string> labels = new List<string>() {"A", "B", "C", "D"};

        public AddonTest(int integerValue, double numberValue, string stringValue, string multilineStringValue, int selectionValue, string comboValue, bool booleanValue, string noteValue, string stringArrayValue, string structValue, string structArrayValue, string commonEventValue, string mapEventValue, string switchValue, string variableValue, string animationValue, string actorValue, string classValue, string skillValue, string itemValue, string weaponValue, string armorValue, string enemyValue, string troopValue, string stateValue, string tilesetValue, string fileValue, string stringArray2Value, string numberArrayValue, string multilineStringArrayValue){
            Debug.Log($"Add-on constructor AddonTest(integerValue={integerValue}, numberValue={numberValue}, stringValue={stringValue}, multilineStringValue={multilineStringValue}, selectionValue={selectionValue}, comboValue={comboValue}, booleanValue={booleanValue}, noteValue={noteValue}, stringArrayValue={stringArrayValue}, structValue={structValue}, structArrayValue={structArrayValue}, commonEventValue={commonEventValue}, mapEventValue={mapEventValue}, switchValue={switchValue}, variableValue={variableValue}, animationValue={animationValue}, actorValue={actorValue}, classValue={classValue}, skillValue={skillValue}, itemValue={itemValue}, weaponValue={weaponValue}, armorValue={armorValue}, enemyValue={enemyValue}, troopValue={troopValue}, stateValue={stateValue}, tilesetValue={tilesetValue}, fileValue={fileValue}, stringArray2Value={stringArray2Value}, numberArrayValue={numberArrayValue}, multilineStringArrayValue={multilineStringArrayValue}) called.");
        }

        public void LogSelfSwitchValue(int SelfSwitch){
            //Debug.Log($"LogSelfSwitchValue({SelfSwitch}) called");
            var eventId = AddonManager.Instance.GetCurrentEventId();
            var data = DataManager.Self().GetRuntimeSaveDataModel();
            var swData = data.selfSwitches.Find(sw => sw.id == eventId);
            var val = false;
            if (swData != null){
                val = swData.data[SelfSwitch];
            }
            Debug.Log($"Add-on command AddonTest.LogSelfSwitchValue({labels[SelfSwitch]}) evId={eventId}, {labels[SelfSwitch]}={(val ? "ON" : "OFF")}");
        }

        public void SetSelfSwitchValue(int SelfSwitch, int Value){
            //Debug.Log($"SetSelfSwitchValue({SelfSwitch}, {Value}) called");
            var eventId = AddonManager.Instance.GetCurrentEventId();
            var data = DataManager.Self().GetRuntimeSaveDataModel();
            var swData = data.selfSwitches.Find(sw => sw.id == eventId);
            if (swData == null){
                swData = new RuntimeSaveDataModel.SaveDataSelfSwitchesData();
                swData.id = eventId;
                swData.data = new List<bool>() { false, false, false, false };
                data.selfSwitches.Add(swData);
            }
            var index = SelfSwitch;
            var oldVal = swData.data[index];
            swData.data[index] = (Value == 1);
            Debug.Log($"Add-on command AddonTest.SetSelfSwitchValue({labels[SelfSwitch]}, {Value}) evId={eventId}, {labels[SelfSwitch]}={(oldVal ? "ON" : "OFF")} => {(swData.data[index] ? "ON" : "OFF")}");
        }

        public void LogMapEvent(string MapEvent){
            var ids = DataConverter.GetStringArrayFromJson(MapEvent);
            var mapEntity = new MapManagementService().LoadMapById(ids[0]);
            var eventMapDataModel = new EventManagementService().LoadEventMapEntitiesByMapId(ids[0])?.FirstOrDefault(x => x.eventId == ids[1]);
            Debug.Log($"Add-on command AddonTest.LogMapEvent({MapEvent}: {mapEntity?.SerialNumberString} {mapEntity?.name}-{eventMapDataModel?.SerialNumberString} {eventMapDataModel?.name})");
        }

        public void Wait(int FrameCount){
            if (FrameCount <= 0){
                return;
            }
            var cb = AddonManager.Instance.TakeOutCommandCallback();
            TimeCallback.Register(cb, FrameCount / 60.0f);
        }

        public class TimeCallback {
            private static List<TimeCallback> _timeCallbacks = new List<TimeCallback>();
            private Action _callback;
            private float _seconds;

            public static void Register(Action callback, float seconds){
                Debug.Log($"Register {seconds}sec.");
                _timeCallbacks.Add(new TimeCallback(callback, seconds));
            }

            private TimeCallback(Action callback, float seconds){
                _callback = callback;
                _seconds = seconds;
                TforuUtility.Instance.StartCoroutine(Wait());
            }

            private IEnumerator Wait() {
                yield return new WaitForSeconds(_seconds);
                Debug.Log($"TimeCallback Action.");
                _timeCallbacks.Remove(this);
                _callback();
            }
        }

        public void CommandWithManyTypes(int integerValue, double numberValue, string stringValue, string multilineStringValue, int selectionValue, string comboValue, bool booleanValue, string noteValue, string stringArrayValue, string structValue, string structArrayValue, string commonEventValue, string mapEventValue, string switchValue, string variableValue, string animationValue, string actorValue, string classValue, string skillValue, string itemValue, string weaponValue, string armorValue, string enemyValue, string troopValue, string stateValue, string tilesetValue, string fileValue, string stringArray2Value, string numberArrayValue, string multilineStringArrayValue){
            Debug.Log($"AddonTest.CommandWithManyTypes(integerValue={integerValue}, numberValue={numberValue}, stringValue={stringValue}, multilineStringValue={multilineStringValue}, selectionValue={selectionValue}, comboValue={comboValue}, booleanValue={booleanValue}, noteValue={noteValue}, stringArrayValue={stringArrayValue}, structValue={structValue}, structArrayValue={structArrayValue}, commonEventValue={commonEventValue}, mapEventValue={mapEventValue}, switchValue={switchValue}, variableValue={variableValue}, animationValue={animationValue}, actorValue={actorValue}, classValue={classValue}, skillValue={skillValue}, itemValue={itemValue}, weaponValue={weaponValue}, armorValue={armorValue}, enemyValue={enemyValue}, troopValue={troopValue}, stateValue={stateValue}, tilesetValue={tilesetValue}, fileValue={fileValue}, stringArray2Value={stringArray2Value}, numberArrayValue={numberArrayValue}, multilineStringArrayValue={multilineStringArrayValue}) called.");
        }

        public void IsPartyMemberAllAlive(int SelfSwitch) {
            Debug.Log($"IsPartyMemberAllAlive({SelfSwitch}) called");
            var eventId = AddonManager.Instance.GetCurrentEventId();
            var data = DataManager.Self().GetRuntimeSaveDataModel();
            var swData = data.selfSwitches.Find(sw => sw.id == eventId);
            if (swData == null)
            {
                data.selfSwitches.Add(new RuntimeSaveDataModel.SaveDataSelfSwitchesData());
                data.selfSwitches[0].id = eventId;
                data.selfSwitches[0].data = new List<bool>() { false, false, false, false };
                swData = data.selfSwitches[0];
            }
            var runtimeActorDataModels = DataManager.Self().GetRuntimeSaveDataModel().runtimeActorDataModels;
            var allAlive = true;
            foreach (var actorDataModel in runtimeActorDataModels)
            {
                if (actorDataModel.hp == 0)
                {
                    allAlive = false;
                    break;
                }
            }

            var index = SelfSwitch;
            swData.data[index] = allAlive;
            Debug.Log($"Add-on command AddonTest.IsPartyMemberAllAlive({labels[SelfSwitch]}) evId={eventId}, {(swData.data[index] ? "ON" : "OFF")}");
        }

    }
}
