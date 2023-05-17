using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Map;
using RPGMaker.Codebase.CoreSystem.Service.DatabaseManagement;
using UnityEngine;

namespace RPGMaker.Codebase.CoreSystem.Service.MapManagement.Repository
{
    public class TileImageRepository
    {
        private static bool                     _cacheUsable; // falseの場合キャッシュを利用しない
        private static List<TileImageDataModel> _tileImageDataModels;

        /**
         * タイル用画像をインポートする
         */
        public void ImportTileImageFile() {
            AssetManageImporter.StartToFile("png", PathManager.MAP_TILE_IMAGE);
            _cacheUsable = false;
        }

        /**
         * インポート済みのタイル用画像をエンティティとして一覧取得する
         */
        public List<TileImageDataModel> GetTileImageEntities() {
            if (_tileImageDataModels != null && _cacheUsable) return _tileImageDataModels;

            _tileImageDataModels = Directory.GetFiles(PathManager.MAP_TILE_IMAGE)
                .Select(Path.GetFileName)
                .Where(filename =>
                    filename.EndsWith(".gif") || filename.EndsWith(".jpg") || filename.EndsWith(".png"))
                .Select(filename =>
                {
                    var imagePath = PathManager.MAP_TILE_IMAGE + filename;
                    var texture = ReadImage(imagePath);
                    return new TileImageDataModel(texture, filename);
                })
                .ToList();

            _cacheUsable = true;

            return _tileImageDataModels;
        }

        /**
         * インポート済みのタイルエンティティを削除する
         */
        public void RemoveTileImageEntity(TileImageDataModel tileImageDataModel) {
            var destPath = PathManager.MAP_TILE_IMAGE + Path.GetFileName(tileImageDataModel.filename);

            if (!File.Exists(destPath)) throw new Exception("ファイルが見つかりません " + destPath);

            UnityEditorWrapper.FileUtilWrapper.DeleteFileOrDirectory(destPath);

            _cacheUsable = false;
        }

        public void ResetTileImageEntity() {
            _tileImageDataModels = null;
        }

        public static Texture2D ReadImageFromPath(string path) {
            return ReadImage(PathManager.MAP_TILE_IMAGE + path);
        }

        //-------------------------------------------------------------------------------------
        // private methods
        //-------------------------------------------------------------------------------------
        private static Texture2D ReadImage(string path) {
            return ReadPng(path);
        }

        private static Texture2D ReadPng(string path) {
            var readBinary = ReadPngFile(path);

            var pos = 16; // 16バイトから開始

            var width = 0;
            for (var i = 0; i < 4; i++) width = width * 256 + readBinary[pos++];

            var height = 0;
            for (var i = 0; i < 4; i++) height = height * 256 + readBinary[pos++];

            var texture = new Texture2D(width, height);
            texture.LoadImage(readBinary);

            return texture;
        }

        private static byte[] ReadPngFile(string path) {
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var bin = new BinaryReader(fileStream);
            var values = bin.ReadBytes((int) bin.BaseStream.Length);

            bin.Close();

            return values;
        }

        private static List<Texture2D> SliceTexture(Texture2D originalTexture, int x, int y) {
            var slicedW = originalTexture.width / x;
            var slicedH = originalTexture.height / y;

            var sliceBitmaps = new List<Texture2D>();
            for (var yNum = y; yNum > 0; yNum--)
            for (var xNum = 0; xNum < x; xNum++)
            {
                var slicedTexture = new Texture2D(slicedW, slicedH, TextureFormat.RGBA32, false);
                slicedTexture.SetPixels(ReadPixelsFromTexture(originalTexture, xNum, yNum, slicedW, slicedH));
                slicedTexture.Apply();

                sliceBitmaps.Add(slicedTexture);
            }

            return sliceBitmaps;
        }

        private static Color[] ReadPixelsFromTexture(
            Texture2D texture,
            int xIndex,
            int yIndex,
            int targetW,
            int targetH
        ) {
            var pixels = new Color[targetW * targetH];
            var offsetX = xIndex * targetW;
            var offsetY = yIndex * targetH;

            for (int y = offsetY, y_ = targetH - 1; y > offsetY - targetH; y--, y_--)
            for (int x = offsetX, x_ = 0; x < offsetX + targetW; x++, x_++)
                pixels[y_ * targetW + x_] = texture.GetPixel(x, y);

            return pixels;
        }
    }
}