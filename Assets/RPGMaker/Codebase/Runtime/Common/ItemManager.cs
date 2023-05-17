using System.Collections.Generic;
using System.Linq;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Armor;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Runtime;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Weapon;
using RPGMaker.Codebase.CoreSystem.Service.DatabaseManagement;
using RPGMaker.Codebase.Runtime.Battle.Objects;
using static RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.SystemSetting.SystemSettingDataModel;

namespace RPGMaker.Codebase.Runtime.Common
{
    public static class ItemManager
    {
        public static void SetActorEquipTypes(RuntimeActorDataModel actorData) {
            //GameActorを取得
            GameActor actor = new GameActor(actorData);

            //二刀流の場合には、1番目を0番目と同じにする
            if (actor.IsDualWield())
            {
                actorData.equips[1].equipType = actorData.equips[0].equipType;
            }
        }

        /// <summary>
        /// 装備固定かどうかの判定を行う
        /// </summary>
        /// <param name="actorData"></param>
        /// <returns></returns>
        public static bool CheckTraitEquipLock(RuntimeActorDataModel actorData, EquipType equipType) {
            //GameActorを取得
            GameActor actor = new GameActor(actorData);

            //装備タイプの場所
            var equipTypeIndex = actorData.equips.IndexOf(actorData.equips.FirstOrDefault(equips => equips.equipType == equipType.id));

            //装備が固定されているかどうかを返却する
            return actor.IsEquipTypeLocked(equipTypeIndex);
        }

        /// <summary>
        /// 装備封印かどうかの判定を行う
        /// 装備封印だった場合、かつなにかを装備中であれば、装備を外す処理も行う
        /// </summary>
        /// <param name="actorData"></param>
        /// <returns></returns>
        public static bool CheckTraitEquipSea(RuntimeActorDataModel actorData, EquipType equipType, int equipIndex) {
            //GameActorを取得
            GameActor actor = new GameActor(actorData);

            //装備が封印されているかどうか
            bool ret = actor.IsEquipTypeSealed(equipIndex);
            if (ret)
            {
                //封印されている場合は、その箇所の装備を外す
                //MVの挙動から、このケースでは外した装備は破棄する
                RemoveEquipment(actorData, equipType, equipIndex, true);
            }

            return ret;
        }

        /// <summary>
        /// 装備を外す
        /// </summary>
        /// <param name="actorData"></param>
        /// <param name="equipType"></param>
        /// <param name="fromGameActor">GameActorから参照する場合,true</param>
        /// <returns></returns>
        public static string RemoveEquipment(RuntimeActorDataModel actorData, EquipType equipType, int equipIndex, bool dispose = false, bool fromGameActor = false) {
            //装備固定または、装備封印の特徴を持っている場合には、装備を外せない
            if (!fromGameActor)
            {
                if (CheckTraitEquipLock(actorData, equipType))
                    return "";
            }

            //必要なデータを取得
            var saveData = DataManager.Self().GetRuntimeSaveDataModel();
            var possession = false;

            //装備を外す
            //外す装備のID
            var removeItemId = actorData.equips[equipIndex].itemId;

            //装備タイプ
            var equipTypes = DataManager.Self().GetSystemDataModel().equipTypes;
            if (actorData.equips[equipIndex].equipType == equipTypes[0].id)
            {
                //武器を外す
                if (!dispose)
                {
                    //外す武器を既に所持している場合は、所持数を加算する
                    for (var i = 0; i < saveData.runtimePartyDataModel.weapons.Count; i++)
                        if (saveData.runtimePartyDataModel.weapons[i].weaponId == removeItemId)
                        {
                            saveData.runtimePartyDataModel.weapons[i].value++;
                            possession = true;
                            break;
                        }

                    //所持していない場合は新規追加する
                    if (!possession)
                    {
                        var weapon = new RuntimePartyDataModel.Weapon();
                        weapon.weaponId = removeItemId;
                        weapon.value = 1;
                        saveData.runtimePartyDataModel.weapons.Add(weapon);
                    }
                }

                // 装備パラメータ減算
                var weapons = new DatabaseManagementService().LoadWeapon();
                WeaponDataModel equipWeapon = null;
                for (int i2 = 0; i2 < weapons.Count; i2++)
                    if (weapons[i2].basic.id == actorData.equips[equipIndex].itemId)
                    {
                        equipWeapon = weapons[i2];
                        break;
                    }

                if (equipWeapon != null)
                {
                    // 0:HP 1:MP 2:ATK 3:DEF 4:MAG 5:MAD 6:SPE 7:LUK 8:LV 
                    actorData.paramPlus.maxHp -= equipWeapon.parameters[0];
                    actorData.paramPlus.maxMp -= equipWeapon.parameters[1];
                    actorData.paramPlus.attack -= equipWeapon.parameters[2];
                    actorData.paramPlus.defense -= equipWeapon.parameters[3];
                    actorData.paramPlus.magicAttack -= equipWeapon.parameters[4];
                    actorData.paramPlus.magicDefence -= equipWeapon.parameters[5];
                    actorData.paramPlus.speed -= equipWeapon.parameters[6];
                    actorData.paramPlus.luck -= equipWeapon.parameters[7];
                }

                //装備を外す
                actorData.equips[equipIndex].itemId = "";
            }
            else
            {
                //防具を外す
                if (!dispose)
                {
                    //外す武器を既に所持している場合は、所持数を加算する
                    for (var i = 0; i < saveData.runtimePartyDataModel.armors.Count; i++)
                        if (saveData.runtimePartyDataModel.armors[i].armorId == removeItemId)
                        {
                            saveData.runtimePartyDataModel.armors[i].value++;
                            possession = true;
                            break;
                        }

                    //所持していない場合は新規追加する
                    if (!possession)
                    {
                        var armor = new RuntimePartyDataModel.Armor();
                        armor.armorId = removeItemId;
                        armor.value = 1;
                        saveData.runtimePartyDataModel.armors.Add(armor);
                    }
                }

                // 装備パラメータ減算
                var armors = new DatabaseManagementService().LoadArmor();
                ArmorDataModel equipArmor = null;
                for (int i2 = 0; i2 < armors.Count; i2++)
                    if (armors[i2].basic.id == actorData.equips[equipIndex].itemId)
                    {
                        equipArmor = armors[i2];
                        break;
                    }

                if (equipArmor != null)
                {
                    // 0:HP 1:MP 2:ATK 3:DEF 4:MAG 5:MAD 6:SPE 7:LUK 8:LV 
                    actorData.paramPlus.maxHp -= equipArmor.parameters[0];
                    actorData.paramPlus.maxMp -= equipArmor.parameters[1];
                    actorData.paramPlus.attack -= equipArmor.parameters[2];
                    actorData.paramPlus.defense -= equipArmor.parameters[3];
                    actorData.paramPlus.magicAttack -= equipArmor.parameters[4];
                    actorData.paramPlus.magicDefence -= equipArmor.parameters[5];
                    actorData.paramPlus.speed -= equipArmor.parameters[6];
                    actorData.paramPlus.luck -= equipArmor.parameters[7];
                }

                //装備を外す
                actorData.equips[equipIndex].itemId = "";
            }

            //外した装備のIDを返却する
            return removeItemId;
        }

        /// <summary>
        /// 装備変更
        /// </summary>
        /// <param name="actorData"></param>
        /// <param name="equipType"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static bool ChangeEquipment(RuntimeActorDataModel actorData, EquipType equipType, string itemId, int equipIndex) {
            //装備固定または、装備封印の特徴を持っている場合には、装備を外せない
            if (CheckTraitEquipLock(actorData, equipType) || CheckTraitEquipSea(actorData, equipType, equipIndex))
                return false;

            //必要なデータを取得
            var saveData = DataManager.Self().GetRuntimeSaveDataModel();
            var possession = false;

            //装備を変更する
            //装備タイプ
            var equipTypes = DataManager.Self().GetSystemDataModel().equipTypes;
            if (actorData.equips[equipIndex].equipType == equipTypes[0].id)
            {
                //武器を装備する
                //装備する武器を既に所持しているかどうかを確認する
                for (var i = 0; i < saveData.runtimePartyDataModel.weapons.Count; i++)
                    if (saveData.runtimePartyDataModel.weapons[i].weaponId == itemId)
                    {
                        //所持していたため減算する
                        saveData.runtimePartyDataModel.weapons[i].value--;
                        //装備を行った結果、アイテム所持数が0になった場合は、削除する
                        if (saveData.runtimePartyDataModel.weapons[i].value == 0)
                            saveData.runtimePartyDataModel.weapons.RemoveAt(i);
                        possession = true;
                        //装備を外す
                        RemoveEquipment(actorData, equipType, equipIndex);
                        //装備する
                        actorData.equips[equipIndex].itemId = itemId;

                        // 装備パラメータに追加
                        var weapons = new DatabaseManagementService().LoadWeapon();
                        WeaponDataModel weapon = null;
                        for (int i2 = 0; i2 < weapons.Count; i2++)
                            if (weapons[i2].basic.id == itemId)
                            {
                                weapon = weapons[i2];
                                break;
                            }
                        if (weapon != null)
                        {
                            // 0:HP 1:MP 2:ATK 3:DEF 4:MAG 5:MAD 6:SPE 7:LUK 8:LV 
                            actorData.paramPlus.maxHp += weapon.parameters[0];
                            actorData.paramPlus.maxMp += weapon.parameters[1];
                            actorData.paramPlus.attack += weapon.parameters[2];
                            actorData.paramPlus.defense += weapon.parameters[3];
                            actorData.paramPlus.magicAttack += weapon.parameters[4];
                            actorData.paramPlus.magicDefence += weapon.parameters[5];
                            actorData.paramPlus.speed += weapon.parameters[6];
                            actorData.paramPlus.luck += weapon.parameters[7];
                        }

                        break;
                    }
            }
            else
            {
                //防具を外す
                //外す武器を既に所持している場合は、所持数を加算する
                for (var i = 0; i < saveData.runtimePartyDataModel.armors.Count; i++)
                    if (saveData.runtimePartyDataModel.armors[i].armorId == itemId)
                    {
                        //所持していたため減算する
                        saveData.runtimePartyDataModel.armors[i].value--;
                        //装備を行った結果、アイテム所持数が0になった場合は、削除する
                        if (saveData.runtimePartyDataModel.armors[i].value == 0)
                            saveData.runtimePartyDataModel.armors.RemoveAt(i);
                        possession = true;
                        //装備を外す
                        RemoveEquipment(actorData, equipType, equipIndex);
                        //装備する
                        actorData.equips[equipIndex].itemId = itemId;

                        // 装備パラメータ加算
                        var armors = new DatabaseManagementService().LoadArmor();
                        ArmorDataModel armor = null;
                        for (int i2 = 0; i2 < armors.Count; i2++)
                            if (armors[i2].basic.id == itemId)
                            {
                                armor = armors[i2];
                                break;
                            }

                        if (armor != null)
                        {
                            // 0:HP 1:MP 2:ATK 3:DEF 4:MAG 5:MAD 6:SPE 7:LUK 8:LV 
                            actorData.paramPlus.maxHp += armor.parameters[0];
                            actorData.paramPlus.maxMp += armor.parameters[1];
                            actorData.paramPlus.attack += armor.parameters[2];
                            actorData.paramPlus.defense += armor.parameters[3];
                            actorData.paramPlus.magicAttack += armor.parameters[4];
                            actorData.paramPlus.magicDefence += armor.parameters[5];
                            actorData.paramPlus.speed += armor.parameters[6];
                            actorData.paramPlus.luck += armor.parameters[7];
                        }

                        break;
                    }
            }

            //装備を変更出来たかどうかを返却する
            return possession;
        }

        //装備を全て外す
        public static bool RemoveAllEquipment(RuntimeActorDataModel actorData) {
            var equipTypes = DataManager.Self().GetSystemDataModel().equipTypes;
            for (var i = 0; i < actorData.equips.Count; i++)
            {
                //装備を外す
                RemoveEquipment(actorData, equipTypes[i], i);
            }

            return true;
        }

        //最強装備
        public static bool StrongestEquipment(RuntimeActorDataModel actorData) {
            //職業リスト
            var classData = DataManager.Self().GetClassDataModels();
            var classIndex = classData.IndexOf(classData.FirstOrDefault(data => data.id == actorData.classId));

            //装備を一通り外す
            RemoveAllEquipment(actorData);

            //必要なデータを取得
            var saveData = DataManager.Self().GetRuntimeSaveDataModel();
            var systemSettingDataModel = DataManager.Self().GetSystemDataModel();

            //現在所持しているアイテムの中から、最も数値の高い武器と防具を装備する
            var nowMaxParamSum = -1;
            var itemId = "";
            var paramSum = -1;

            //GameActorを取得
            GameActor actor = new GameActor(actorData);

            //武器
            nowMaxParamSum = 0;
            var itemCt = 0;
            var weaponCount = actor.IsDualWield() == true ? 2 : 1;

            //装備種別
            var equipTypes = DataManager.Self().GetSystemDataModel().equipTypes;
            for (var typeIndex = 0; typeIndex < weaponCount; typeIndex++)
            {
                for (var i = 0; i < saveData.runtimePartyDataModel.weapons.Count; i++)
                {
                    paramSum = 0;
                    //比較処理
                    var weaponDataModel = SumWeaponParameter(saveData.runtimePartyDataModel.weapons[i].weaponId);
                    if (weaponDataModel == null) continue;
                    //数があるか
                    itemCt = saveData.runtimePartyDataModel.weapons[i].value;
                    if (itemCt <= 0) continue;

                    //この武器を装備可能かどうかのチェック
                    for (var j = 0; j < classData[classIndex].weaponTypes.Count; j++)
                        if (classData[classIndex].weaponTypes[j] == weaponDataModel.basic.weaponTypeId)
                        {
                            //装備可能
                            for (var k = 0; k < weaponDataModel.parameters.Count; k++)
                                paramSum += weaponDataModel.parameters[k];

                            if (nowMaxParamSum < paramSum)
                            {
                                nowMaxParamSum = paramSum;
                                itemId = weaponDataModel.basic.id;
                            }

                            break;
                        }
                }

                //装備する
                ChangeEquipment(actorData, equipTypes[0], itemId, typeIndex);
            }

            //防具
            for (var typeIndex = weaponCount; typeIndex < equipTypes.Count; typeIndex++)
            {
                itemId = "";
                nowMaxParamSum = 0;
                for (var i = 0; i < saveData.runtimePartyDataModel.armors.Count; i++)
                {
                    //対象の防具の装備部位を調べる
                    var armorDataModel = SumArmorParameter(saveData.runtimePartyDataModel.armors[i].armorId);
                    if (armorDataModel == null) continue;
                    if (armorDataModel.basic.equipmentTypeId != equipTypes[typeIndex].id) continue;
                    
                    //数があるか
                    itemCt = saveData.runtimePartyDataModel.armors[i].value;
                    if(itemCt <= 0) continue;

                    //この防具を装備可能かどうかのチェック
                    for (var j = 0; j < classData[classIndex].armorTypes.Count; j++)
                        if (classData[classIndex].armorTypes[j] == armorDataModel.basic.armorTypeId)
                        {
                            //装備可能
                            paramSum = 0;
                            for (var k = 0; k < armorDataModel.parameters.Count; k++)
                                paramSum += armorDataModel.parameters[k];

                            if (nowMaxParamSum < paramSum)
                            {
                                nowMaxParamSum = paramSum;
                                itemId = armorDataModel.basic.id;
                            }

                            break;
                        }
                }

                //装備する
                ChangeEquipment(actorData, equipTypes[typeIndex], itemId, typeIndex);
            }

            return true;
        }

        public static bool CanEquip(GameActor actor, GameItem gameItem) {
            bool flg = false;

            if (gameItem == null) return false;
            
            //職業リスト
            var classData = DataManager.Self().GetClassDataModels();
            var classIndex = classData.IndexOf(classData.FirstOrDefault(data => data.id == actor.Actor.classId));
            
            //武器
            var weaponCount = actor.IsDualWield() == true ? 2 : 1;
            //装備種別
            var equipTypes = DataManager.Self().GetSystemDataModel().equipTypes;

            if (gameItem.IsWeapon())
            {
                //比較処理
                var weaponDataModel = SumWeaponParameter(gameItem.ItemId);
                if (weaponDataModel == null)
                {
                    return false;
                }

                //この武器を装備可能かどうかのチェック
                for (var j = 0; j < classData[classIndex].weaponTypes.Count; j++)
                    if (classData[classIndex].weaponTypes[j] == weaponDataModel.basic.weaponTypeId)
                    {
                        flg = true;
                        break;
                    }
            }
            else if (gameItem.IsArmor())
            {
                //防具
                for (var typeIndex = weaponCount; typeIndex < equipTypes.Count; typeIndex++)
                {
                    //対象の防具の装備部位を調べる
                    var armorDataModel = SumArmorParameter(gameItem.ItemId);
                    if (armorDataModel == null) continue;
                    if (armorDataModel.basic.equipmentTypeId != equipTypes[typeIndex].id) continue;

                    //この防具を装備可能かどうかのチェック
                    for (var j = 0; j < classData[classIndex].armorTypes.Count; j++)
                        if (classData[classIndex].armorTypes[j] == armorDataModel.basic.armorTypeId)
                        {
                            flg = true;
                            break;
                        }
                }
            }

            return flg;
        }
        

        private static WeaponDataModel SumWeaponParameter(string id) {
            var weaponDataModels = DataManager.Self().GetWeaponDataModels();
            for (var i = 0; i < weaponDataModels.Count; i++)
                if (weaponDataModels[i].basic.id == id)
                    return weaponDataModels[i];
            return null;
        }

        private static ArmorDataModel SumArmorParameter(string id) {
            var armorDataModels = DataManager.Self().GetArmorDataModels();
            for (var i = 0; i < armorDataModels.Count; i++)
                if (armorDataModels[i].basic.id == id)
                    return armorDataModels[i];
            return null;
        }
    }
}