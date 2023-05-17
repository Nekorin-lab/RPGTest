using System;
using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common;
using RPGMaker.Codebase.CoreSystem.Knowledge.Enum;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.AssetManage
{
    [Serializable]
    public class AssetManageDataModel : WithSerialNumberDataModel
    {
        private const int DataSizeDefault       = 1;
        private const int DataSizeMoveCharacter = 5;
        private const int DataSizeSvCharacter   = 18;
        public        int assetTypeId;

        public string             id;
        public List<ImageSetting> imageSettings;
        public string             name;
        public int                sort;
        public int                type;
        public int                weaponTypeId;

        public AssetManageDataModel(
            string id,
            int sort,
            string name,
            int type,
            int weaponTypeId,
            int assetTypeId,
            List<ImageSetting> imageSettings
        ) {
            this.id = id;
            this.sort = sort;
            this.name = name;
            this.type = type;
            this.weaponTypeId = weaponTypeId;
            this.assetTypeId = assetTypeId;
            this.imageSettings = imageSettings;
        }

        public static AssetManageDataModel CreateDefault(int assetTypeId, int otherSameTypeItemAmount) {
            var ret = new AssetManageDataModel(
                Guid.NewGuid().ToString(),
                otherSameTypeItemAmount + 1,
                "",
                0,
                0,
                assetTypeId,
                new List<ImageSetting>()
            );


            switch (assetTypeId)
            {
                case (int) AssetCategoryEnum.MOVE_CHARACTER:
                case (int) AssetCategoryEnum.OBJECT:
                    ret.imageSettings = new List<ImageSetting>();
                    for (var i = 0; i < DataSizeMoveCharacter; i++)
                        ret.imageSettings.Add(new ImageSetting("", 0, 0, 0, 0));

                    break;
                case (int) AssetCategoryEnum.SV_BATTLE_CHARACTER:
                    ret.imageSettings = new List<ImageSetting>();
                    for (var i = 0; i < DataSizeSvCharacter; i++)
                        ret.imageSettings.Add(new ImageSetting("", 0, 0, 0, 0));

                    break;
                case (int) AssetCategoryEnum.POPUP:
                case (int) AssetCategoryEnum.SV_WEAPON:
                case (int) AssetCategoryEnum.SUPERPOSITION:
                    ret.imageSettings = new List<ImageSetting>();
                    for (var i = 0; i < DataSizeDefault; i++) ret.imageSettings.Add(new ImageSetting("", 0, 0, 0, 0));

                    break;
                case (int) AssetCategoryEnum.BATTLE_EFFECT:
                    ret.imageSettings = new List<ImageSetting>();
                    for (var i = 0; i < DataSizeDefault; i++)
                        ret.imageSettings.Add(
                            new ImageSetting("", 0, 0, 0, 0));

                    break;
            }

            return ret;
        }

        [Serializable]
        public class ImageSetting
        {
            public int    animationFrame;
            public int    animationSpeed;
            public string path;
            public int    sizeX;
            public int    sizeY;

            public ImageSetting(string path, int sizeX, int sizeY, int animationFrame, int animationSpeed) {
                this.path = path;
                this.sizeX = sizeX;
                this.sizeY = sizeY;
                this.animationFrame = animationFrame;
                this.animationSpeed = animationSpeed;
            }
        }
    }
}