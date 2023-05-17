using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.Enum;
using UnityEngine;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Map
{
    [Serializable]
    public class TileDataModel : RuleTile
    {
        public enum DamageFloorType
        {
            Fix,
            Rate,
            None
        }

        public enum ImageAdjustType
        {
            Scale,
            Split,
            None
        }

        public enum PassType
        {
            CanPassNormally,
            CanPassUnder,
            CannotPass
        }

        // local class / enum
        //-------------------------------------------------------------------------------------------------------
        public enum Type
        {
            AutoTileA, // 地面型オートタイル 長方形
            AutoTileB, // 建物型オートタイル 正方形
            AutoTileC,
            Effect,
            NormalTile,
            Region,
            BackgroundCollision,
            LargeParts
        }

        public const int             TileSize = 96;
        public const int             LargePartsTileSize = 96;
        public       int             animationFrame;
        public       int             animationSpeed;
        public       DamageFloorType damageFloorType;
        public       float           damageFloorValue;
        public       int             terrainTagValue;
        public       string          filename;
        public       bool            hasAnimation;

        public     string              id;
        public     ImageAdjustType     imageAdjustType;
        public     bool                isBush;
        public     bool                isCounter;
        public     bool                isDamageFloor;
        public     bool                isLadder;
        public     bool                isPassBottom;
        public     bool                isPassLeft;
        public     bool                isPassRight;
        public     bool                isPassTop;
        public     LargePartsDataModel largePartsDataModel;
        public new string              name;
        public     PassType            passType = PassType.CannotPass;
        public     int                 regionId;

        // WithSerialNumberDataModelを多重継承できないため致し方なく直接定義
        public int                serialNumber;
        public int                listNumber;
        public TileImageDataModel tileImageDataModel;
        public Type               type;
        public List<VehicleType>  vehicleTypes;
        public string SerialNumberString => listNumber.ToString("0000");

        /**
         * 通れるか？ (vehicleIdがnull以外の場合は乗り物判定)
         */
        private bool CanStepOn(CharacterMoveDirectionEnum moveDirectionEnum, string vehicleId = null) {
            return vehicleId != null ?
                VehiclePassTypeIs(vehicleId, PassType.CanPassNormally) :
                passType == PassType.CanPassNormally && IsAllowedDirection(moveDirectionEnum);
        }

        /**
         * 下を潜って通れるか？ (vehicleIdがnull以外の場合は乗り物判定)
         */
        public bool CanGoThrough(CharacterMoveDirectionEnum moveDirectionEnum, string vehicleId = null) {
            return vehicleId != null ?
                VehiclePassTypeIs(vehicleId, PassType.CanPassUnder) :
                passType == PassType.CanPassUnder && IsAllowedDirection(moveDirectionEnum);
        }

        /**
         * 通れないか？ (vehicleIdがnull以外の場合は乗り物判定)
         */
        public bool CannotEnter(CharacterMoveDirectionEnum moveDirectionEnum, string vehicleId = null) {
            return !(CanStepOn(moveDirectionEnum, vehicleId) || CanGoThrough(moveDirectionEnum, vehicleId));
        }

        /**
         * 許可された移動方向か
         */
        private bool IsAllowedDirection(CharacterMoveDirectionEnum moveDirectionEnum) {
            return moveDirectionEnum switch
            {
                CharacterMoveDirectionEnum.Up => isPassBottom, // 上向きでの進入 = 下からの進入
                CharacterMoveDirectionEnum.Down => isPassTop, // 下向きでの進入 = 上からの進入
                CharacterMoveDirectionEnum.Left => isPassRight, // 左向きでの進入 = 右からの進入
                CharacterMoveDirectionEnum.Right => isPassLeft, // 右向きでの進入 = 左からの進入
                _ => throw new ArgumentOutOfRangeException(nameof(moveDirectionEnum), moveDirectionEnum, null)
            };
        }


        //乗り物判定----------------------------------------------------------------------------------------------------------------------

        /**
         * 指定の通行設定か？
         */
        private bool VehiclePassTypeIs(string vehicleId, PassType passType)
        {
            var vehicleType = vehicleTypes.FirstOrDefault(vehicleType => vehicleType.vehicleId == vehicleId);
            return vehicleType?.vehiclePassType == passType;
        }

        //他----------------------------------------------------------------------------------------------------------------------

        /**
         * 大型パーツの子タイルを新規作成する
         */
        public static TileDataModel CreateLargePartsChildTileDataModel(TileDataModel parentTileDataModel, int num = 0) {
            DebugUtil.Assert(parentTileDataModel.type == Type.LargeParts);

            var tileEntity = CreateInstance<TileDataModel>();

            // TileDataModelの全フィールド値をコピー (継承元のフィールド含まず)。
            var baseTypeFieldNames = typeof(TileDataModel).BaseType.GetFields().Select(f => f.Name).ToList();
            foreach (var field in
                typeof(TileDataModel).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                // 継承元の型に同名フィールドがあれば対象外。
                if (baseTypeFieldNames.Contains(field.Name)) continue;

                DebugUtil.Log($"field.Name={field.Name}");
                field.SetValue(tileEntity, field.GetValue(parentTileDataModel));
            }

            // string以外の参照型のフィールドにインスタンス生成した値を代入。
            tileEntity.tileImageDataModel = new TileImageDataModel(
                parentTileDataModel.tileImageDataModel.texture, parentTileDataModel.tileImageDataModel.filename);
            tileEntity.vehicleTypes = new List<VehicleType>(parentTileDataModel.vehicleTypes);
            tileEntity.largePartsDataModel =
                parentTileDataModel.largePartsDataModel != null
                    ? new LargePartsDataModel(
                        parentTileDataModel.largePartsDataModel.parentId,
                        parentTileDataModel.largePartsDataModel.x,
                        parentTileDataModel.largePartsDataModel.y)
                    : null;

            // idを設定。
            tileEntity.id = Guid.NewGuid().ToString();
            tileEntity.serialNumber += num;

            return tileEntity;
        }
    }
}