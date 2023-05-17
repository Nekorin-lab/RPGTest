using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Map;
using RPGMaker.Codebase.Editor.MapEditor.Component.ListView;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.MapEditor.Component.Inventory
{
    /**
     * タイル用画像インベントリーコンポーネント
     */
    public class TileImageInventory : VisualElement
    {
        // コールバック関数プロパティ
        private readonly Action<TileImageDataModel> _onSelectTileImage;

        private TileImageDataModel _currentSelectingTileImage;

        // データプロパティ
        private List<TileImageDataModel> _tileImageEntities;

        // UI要素プロパティ
        private TileImageListView _tileImageListView;

        /**
         * コンストラクタ
         */
        public TileImageInventory(
            List<TileImageDataModel> tileImageEntities,
            Action<TileImageDataModel> onSelectTileImage
        ) {
            _tileImageEntities = tileImageEntities;
            _onSelectTileImage = onSelectTileImage;
            InitUI();
            Refresh();
        }

        /**
         * データおよび表示を更新
         */
        public void Refresh([CanBeNull] List<TileImageDataModel> tileImageEntities = null) {
            if (tileImageEntities != null) _tileImageEntities = tileImageEntities;

            _tileImageListView.Refresh(_tileImageEntities);
        }

        /**
         * UI初期化
         */
        private void InitUI() {
            // タイル画像リスト
            _tileImageListView = new TileImageListView(_tileImageEntities, SelectTileImage);

            // 要素配置
            Clear();
            style.flexDirection = FlexDirection.Column;
            Add(_tileImageListView);
        }

        /**
         * タイル用画像を選択する
         */
        private void SelectTileImage(TileImageDataModel tileImageDataModel) {
            _currentSelectingTileImage = tileImageDataModel;
            _onSelectTileImage?.Invoke(tileImageDataModel);
        }
    }
}