using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Map;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.MapEditor.Component.ListView
{
    /**
     * タイル用画像リストコンポーネント
     */
    public class TileImageListView : UnityEngine.UIElements.ListView
    {
        // データプロパティ
        private List<TileImageDataModel> _tileImageEntities;

        /**
         * コンストラクタ
         */
        public TileImageListView(
            List<TileImageDataModel> tileImageEntities,
            Action<TileImageDataModel> onSelectionChange
        ) {
            _tileImageEntities = tileImageEntities;

            selectionType = SelectionType.Single;
            itemHeight = 16;
            makeItem = () => new Label();
            bindItem = (e, i) =>
            {
                var target = (Label) e;
                target.text = _tileImageEntities[i].filename;
                target.RemoveFromClassList("list-row-even");
                target.RemoveFromClassList("list-row-odd");
                target.AddToClassList(i % 2 == 0 ? "list-row-even" : "list-row-odd");
            };

            style.flexDirection = FlexDirection.Row;

            this.onSelectionChange += obj => { onSelectionChange?.Invoke(_tileImageEntities[selectedIndex]); };
        }

        /**
         * データおよび表示を更新
         */
        public void Refresh([CanBeNull] List<TileImageDataModel> tileImageEntities = null) {
            if (tileImageEntities != null) _tileImageEntities = tileImageEntities;

            itemsSource = _tileImageEntities;
            style.height = Length.Percent(100);
            Rebuild();
        }
    }
}