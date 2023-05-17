#define FIX_LINE_TILE_COUNT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Map;
using RPGMaker.Codebase.Editor.Common;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using Cursor = UnityEngine.UIElements.Box;
using System.Threading.Tasks;

namespace RPGMaker.Codebase.Editor.MapEditor.Component.ListView
{
    /**
     * タイルリストコンポーネント
     */
    public class TileListView : ScrollView
    {
        public enum SelectionTileAfterRefreshType
        {
            FirstTile,
            EndTile,
            Clear,
        }

        // 定数
        private static int BORDER_WIDTH = 3;
        private static Color BORDER_COLOR = Color.red;
        private const int TILE_SIZE = 40;
#if FIX_LINE_TILE_COUNT
        private const int LINE_TILE_COUNT = 20;
#endif
        private const int MAX_HEIGHT = 4000;
        private const float WAIT_TIME = 1f / 60f;

        // データプロパティ
        private List<TileDataModel> _tileEntities;
        private float _windowWidth;
        private bool _isCanSelect;
        private int _lineTileCount;
        private bool _isDoubleCursor;

        private List<VisualElement> _rowElements;
        private double _lastTime;
        private EditorCoroutine _editorCoroutine;

        // コールバック
        private Action<TileDataModel, List<TileDataModel>> _onSelectionChange;
        private Action<TileDataModel> _onRightClickTile;

        private TileDataModel _currentSelectingTile;
        public TileDataModel CurrentSelectingTile
        {
            get { return _currentSelectingTile; }

            private set
            {
                if (value != _currentSelectingTile)
                {
                    RemoveCursor(_currentSelectingTile);
                    _currentSelectingTile = value;
                    PutCursor(_currentSelectingTile);
                }
            }
        }

        [Flags]
        private enum BorderFlag
        {
            Left = 1 << 0,
            Top = 1 << 1,
            Right = 1 << 2,
            Bottom = 1 << 3,
        }

        private const int CursorImageCount = 16;

        private readonly BorderFlag[] BorderFlagMasks = new BorderFlag[]
        {
            BorderFlag.Top | BorderFlag.Left,
            BorderFlag.Top,
            BorderFlag.Top | BorderFlag.Right,
            BorderFlag.Left,
            BorderFlag.Left | BorderFlag.Top | BorderFlag.Right | BorderFlag.Bottom,
            BorderFlag.Right,
            BorderFlag.Bottom | BorderFlag.Left,
            BorderFlag.Bottom,
            BorderFlag.Bottom | BorderFlag.Right,
        };

        /**
         * コンストラクタ
         */
        public TileListView(
            List<TileDataModel> tileEntities,
            Action<TileDataModel, List<TileDataModel>> onSelectionChange,
            Action<TileDataModel> onRightClickTile,
            bool isCanSelect = true,
            bool setSize = false,
            float windowWitdth = TILE_SIZE,
            bool isDoubleCursor = false
        )
            : base(
#if FIX_LINE_TILE_COUNT
                ScrollViewMode.VerticalAndHorizontal)
#else
                  ScrollViewMode.Vertical)
#endif
        {
            _tileEntities = tileEntities;

            _onSelectionChange = onSelectionChange;
            _onRightClickTile = onRightClickTile;

            _isCanSelect = isCanSelect;
            _isDoubleCursor = isDoubleCursor;

            if (setSize == true)
                _windowWidth = windowWitdth;

            style.maxHeight = MAX_HEIGHT;

            CreateData();

#if FIX_LINE_TILE_COUNT
#else
            // 要素のサイズ変更がされた際に呼ばれる
            var unityContentContainer = this.Q<VisualElement>("unity-content-container");
            unityContentContainer.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                _windowWidth = unityContentContainer.resolvedStyle.width;
                CreateData();
            });
#endif
        }

        /**
         * データおよび表示を更新
         */
        public void Refresh(
            [CanBeNull] List<TileDataModel> tileEntities,
            SelectionTileAfterRefreshType selectionTile = SelectionTileAfterRefreshType.EndTile) {
            if (tileEntities != null)
            {
                _tileEntities = tileEntities;
            }

            CreateData();

            if (selectionTile != SelectionTileAfterRefreshType.FirstTile && _tileEntities.Any())
            {
                SelectTile(selectionTile == SelectionTileAfterRefreshType.EndTile ?
                    _tileEntities.Last() : null);
            }
        }

        /**
         * データおよび表示を更新
         */
        private void CreateData() {
            _lastTime = EditorApplication.timeSinceStartup;
            _currentSelectingTile = null;
            _rowElements = new List<VisualElement>();
            try
            {
                Clear();
            } catch (Exception) { }

            if (_tileEntities == null)
                return;

            // 1行のタイル数
            _lineTileCount =
#if FIX_LINE_TILE_COUNT
                LINE_TILE_COUNT;
#else
                Math.Max((int) _windowWidth / TILE_SIZE, 1);
#endif


            var addedLargePartsIds = new HashSet<string>();
            bool findTile = false;
            foreach (TileDataModel tileDataModel in _tileEntities)
            {
                //手動で消される等の理由で、tileDataModelを正常に読み込めていない場合はcontinue
                if (tileDataModel == null) continue;

                findTile = true;
                if (tileDataModel.type == TileDataModel.Type.LargeParts)
                {
                    // 大型パーツタイルの場合の処理。
                    var parentId = tileDataModel.largePartsDataModel.parentId;
                    if (!addedLargePartsIds.Contains(parentId))
                    {
                        addedLargePartsIds.Add(parentId);
                        SetTiles(GetLargePartsTileDataModels(tileDataModel));
                    }
                }
                else
                {
                    // 大型パーツタイル以外の場合の処理。
#if DEBUG
                    Debug.Assert(tileDataModel.type != TileDataModel.Type.LargeParts);
#endif
                    SetTile(tileDataModel);
                }
            }

            if (_tileEntities != null && _tileEntities.Count > 0 && findTile)
            {
                SelectTile((TileDataModel) ElementAt(0).ElementAt(0).userData);
            }

            //タイル初回描画
            InitializeDrawImage();
        }

        /**
         * 初回のタイル画像描画処理
         * タイルが画面に置かれて配置場所が確定後に実施する
         */
        private async void InitializeDrawImage() {
            //若干の待ちが無いと、配置場所が確定しない
            //最も待つ時間が長いのはマップ編集画面下の各レイヤーになるため、そこに合わせた待ち時間とする
            await Task.Delay(100);
            if (_editorCoroutine != null) EditorCoroutineUtility.StopCoroutine(_editorCoroutine);
            _editorCoroutine = null;
            _lastTime = EditorApplication.timeSinceStartup;
            _editorCoroutine = EditorCoroutineUtility.StartCoroutine(ImmageDisplayRefresh(true), this);
        }

        /**
         * 大型パーツのタイル群を設定する。
         */
        private void SetTiles(List<TileDataModel> tileDataModels) {
            var tileRectSize = GetLargePartsTileRectSize(tileDataModels);
            Vector2Int tilePosition = CalculateAddTilePosition(tileRectSize);

            foreach (var t in tileDataModels)
            {
#if DEBUG
                Debug.Assert(t.type == TileDataModel.Type.LargeParts && t.largePartsDataModel != null);
#endif
                SetTile(tilePosition + new Vector2Int(t.largePartsDataModel.x, t.largePartsDataModel.y), t);
            }
        }

        /**
         * 大型パーツ以外のタイルを設定する。
         */
        private void SetTile(TileDataModel tileDataModel) {
            SetTile(CalculateAddTilePosition(new Vector2Int(1, 1)), tileDataModel);
        }

        /**
         * 追加するタイル矩形の左上座標を算出する。
         */
        private Vector2Int CalculateAddTilePosition(Vector2Int tileRectSize) {
#if true
            //var result = new Vector2Int();
            for (int y = 0; y < childCount; y++)
            {
                for (int x = 0; x < _lineTileCount; x++)
                {
                    if (IsThereSpace(x, y))
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }

            return new Vector2Int(0, childCount);

            bool IsThereSpace(int originX, int originY) {
                for (int y = originY; y < originY + tileRectSize.y; y++)
                {
                    for (int x = originX; x < originX + tileRectSize.x; x++)
                    {
                        if (!IsSpace(new Vector2Int(x, y)))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
#else
            int x = -1;
            int y = -1;
            foreach (var (rowParent, rowIndex) in
                Children().Select((ve, index) => ((VisualElement) ve, (int) index)))
            {
                if (rowParent.childCount <= _lineTileCount - tileRectSize.x)
                {
                    if (x < rowParent.childCount)
                    {
                        x = rowParent.childCount;
                    }

                    if (y < 0)
                    {
                        y = rowIndex;
                    }
                }
                else
                {
                    x = y = -1;
                }
            }

            return x >= 0 ? new Vector2Int(x, y) : new Vector2Int(0, childCount);
#endif
        }

        /**
         * タイルを設定する。
         */
        private void SetTile(Vector2Int position, TileDataModel tileDataModel) {
            if (tileDataModel == null)
                return;

            // 要素の表示切替（テクスチャがメモリを食いつぶす対応）
            verticalScroller.valueChanged += f => {
                // 1度に複数回呼ばれ重くなる為非同期で処理
                if (_editorCoroutine != null) EditorCoroutineUtility.StopCoroutine(_editorCoroutine);
                _editorCoroutine = null;
                _lastTime = EditorApplication.timeSinceStartup;
                _editorCoroutine = EditorCoroutineUtility.StartCoroutine(ImmageDisplayRefresh(), this);
            };

            // タイルを設置
            // Texture は設置段階では空とする
            Image tileImage = PrepareTileImage(position);
            tileImage.userData = tileDataModel;
            tileImage.image = null;

            BaseClickHandler.ClickEvent(tileImage, mouseUpEvent =>
            {
                if (mouseUpEvent == (int) MouseButton.RightMouse)
                {
                    // 右クリック時コールバック
                    _onRightClickTile?.Invoke(tileDataModel);
                    return;
                }

                SelectTile(tileDataModel);
            });
        }

        private void SelectTile(TileDataModel tileDataModel) {
            CurrentSelectingTile = tileDataModel;
            _onSelectionChange?.Invoke(tileDataModel, _tileEntities);
        }

        /**
         * 表示切替処理
         */
        private IEnumerator ImmageDisplayRefresh(bool force = false) {
            // ウェイト毎に実行
            if (EditorApplication.timeSinceStartup - _lastTime < WAIT_TIME && !force)
                yield return null;

            _lastTime = EditorApplication.timeSinceStartup;

            if (_rowElements.Count > 0)
            {
                bool flg = false;
                while (!flg)
                {
                    foreach (var elem in _rowElements)
                    {
                        // 描画範囲の表示切替
                        if (worldBound.y < elem.worldBound.y + TILE_SIZE && worldBound.y + worldBound.height > elem.worldBound.y - TILE_SIZE)
                        {
                            flg = true;
                            if (elem.style.visibility == Visibility.Hidden || force)
                            {
                                elem.style.visibility = Visibility.Visible;

                                // この行にある全てのImageを取得
                                List<Image> images = new List<Image>();
                                GetAllImage(images, elem);
                                foreach (Image image in images)
                                {
                                    // Imageに設定している userDataを取得
                                    TileDataModel tileDataModel = (TileDataModel) image.userData;
                                    // Imageに設定されているTextureが空の場合、正式なタイルを設定する
                                    if (image.image == null)
                                    {
                                        try
                                        {
                                            image.image = tileDataModel.m_DefaultSprite.texture;
                                        }
                                        catch (Exception) { }
                                    }
                                }
                            }
                        }
                        else if (elem.style.visibility != Visibility.Hidden)
                        {
                            elem.style.visibility = Visibility.Hidden;
                            // この行にある全てのImageを取得
                            List<Image> images = new List<Image>();
                            GetAllImage(images, elem);
                            foreach (Image image in images)
                            {
                                // 非表示行のTextureを emptyにする
                                image.image = null;
                            }
                        }
                    }
                    if (!flg)
                    {
                        //描画範囲に1つも表示する行が無い、ということはあり得ない
                        //このケースは、worldBound が正常に取得できていない（描画が完了していない）ケースのため、1F待って再描画を試みる
                        yield return null;
                    }
                }
            }
        }
        protected void GetAllImage(List<Image> data, VisualElement me) {
            if (me is Image)
                data.Add((Image) me);

            foreach (VisualElement child in me.Children())
                GetAllImage(data, child);
        }

        /**
         * 座標位置に配置するためにタイル用のImageを用意する。
         * 
         * 必要に応じてタイルの行列を拡大する。
         */
        private Image PrepareTileImage(Vector2Int tilePosition) {
            // 必要な行を追加。
            while (tilePosition.y >= childCount)
            {
                var addRowParentElement = new VisualElement() { name = $"rows[{childCount}]" };
                Add(addRowParentElement);
                addRowParentElement.style.flexDirection = FlexDirection.Row;
                addRowParentElement.style.minHeight = TILE_SIZE;
                if (childCount > 100)
                    addRowParentElement.style.visibility = Visibility.Hidden;
                _rowElements.Add(addRowParentElement);
            }

            // 必要な列のImageを追加。
            VisualElement rowParentElement = ElementAt(tilePosition.y);
            while (tilePosition.x >= rowParentElement.childCount)
            {
                var tileImage =
                    new Image() { name = $"grid[{rowParentElement.childCount}][{IndexOf(rowParentElement)}]" };
                tileImage.style.width =
                    tileImage.style.height = TILE_SIZE;
                tileImage.style.flexShrink = 0;
                rowParentElement.Add(tileImage);
            }

            return GetTileImage(tilePosition);
        }

        /**
         * 座標位置は空いている？
         */
        private bool IsSpace(Vector2Int tilePosition) {
            if (tilePosition.x >= _lineTileCount)
            {
                return false;
            }

            var image = TryGetTileImage(tilePosition);
            return image == null || image.userData == null;
        }

        /**
         * 座標位置のタイル用のImageの取得を試行する。
         */
        private Image TryGetTileImage(Vector2Int tilePosition) {
            return tilePosition.y < childCount && tilePosition.x < ElementAt(tilePosition.y).childCount
                ? (Image) ElementAt(tilePosition.y).ElementAt(tilePosition.x)
                : null;
        }

        /**
         * 座標位置のタイル用のImageを取得する。
         */
        private Image GetTileImage(Vector2Int tilePosition) {
            return (Image) ElementAt(tilePosition.y).ElementAt(tilePosition.x);
        }

        /**
         * カーソルを置く。
         */
        private void PutCursor(TileDataModel tileDataModel) {
            CoreSystem.Helper.DebugUtil.Log($"   PutCursor({tileDataModel})");
            PutOrRemoveCursor(tileDataModel, false);
        }

        /**
         * カーソルを取り除く。
         */
        private void RemoveCursor(TileDataModel tileDataModel) {
            CoreSystem.Helper.DebugUtil.Log($"RemoveCursor({tileDataModel})");
            PutOrRemoveCursor(tileDataModel, true);
        }

        /**
         * カーソルを置くまたは取り除く。
         */
        private void PutOrRemoveCursor(TileDataModel tileDataModel, bool isRemove) {
            if (tileDataModel == null || !_isCanSelect)
            {
                return;
            }

            // 大型パーツタイル矩形のカーソル。
            if (tileDataModel.type == TileDataModel.Type.LargeParts)
            {
                var tileDataModels = GetLargePartsTileDataModels(tileDataModel);
                var tileRect = new RectInt(
                    GetTileImagePosition(GetTileImage(tileDataModel)) -
                    new Vector2Int(tileDataModel.largePartsDataModel.x, tileDataModel.largePartsDataModel.y),
                    GetLargePartsTileRectSize(tileDataModels));

                for (int y = tileRect.yMin; y < tileRect.yMax; y++)
                {
                    for (int x = tileRect.xMin; x < tileRect.xMax; x++)
                    {
                        BorderFlag borderFlags = 0;

                        if (x == tileRect.xMin)
                        {
                            borderFlags = borderFlags | BorderFlag.Left;
                        }

                        if (x == tileRect.xMax - 1)
                        {
                            borderFlags = borderFlags | BorderFlag.Right;
                        }

                        if (y == tileRect.yMin)
                        {
                            borderFlags = borderFlags | BorderFlag.Top;
                        }

                        if (y == tileRect.yMax - 1)
                        {
                            borderFlags = borderFlags | BorderFlag.Bottom;
                        }

                        if (borderFlags != 0)
                        {
                            var tileImage = GetTileImage(new Vector2Int(x, y));
                            if (!isRemove)
                            {
                                tileImage.Add(CreateCursor(borderFlags));
                            }
                            else
                            {
                                tileImage.Clear();
                            }
                        }
                    }
                }
            }

            // 通常タイルのカーソル。
            if (tileDataModel.type != TileDataModel.Type.LargeParts || _isDoubleCursor)
            {
                var tileImage = GetTileImage(tileDataModel);
                if (!isRemove)
                {
                    tileImage.Add(
                        CreateCursor(BorderFlag.Left | BorderFlag.Top | BorderFlag.Right | BorderFlag.Bottom));
                }
                else
                {
                    tileImage.Clear();
                }
            }
        }

        /**
         *  指定TileDataModelが属する大型パーツの全TileDataModelを取得。
         */
        private List<TileDataModel> GetLargePartsTileDataModels(TileDataModel tileDataModel) {
#if DEBUG
            Debug.Assert(tileDataModel.type == TileDataModel.Type.LargeParts);
#endif
            var parentId = tileDataModel.largePartsDataModel.parentId;
            return _tileEntities.Where(t => t.type == TileDataModel.Type.LargeParts &&
                                            t.largePartsDataModel.parentId == parentId).ToList();
        }

        /**
         * カーソルデータを作成。
         */
        private Cursor CreateCursor(BorderFlag borderFlagMask) {
            var cursor = new Cursor() { name = $"cursor {borderFlagMask}" };
            cursor.style.width =
                cursor.style.height = TILE_SIZE;
            cursor.style.position = Position.Absolute;
            cursor.style.borderLeftWidth =
                cursor.style.borderTopWidth =
                    cursor.style.borderRightWidth =
                        cursor.style.borderBottomWidth = 0;
            cursor.style.borderLeftColor =
                cursor.style.borderTopColor =
                    cursor.style.borderRightColor =
                        cursor.style.borderBottomColor = BORDER_COLOR;
            cursor.style.backgroundColor = Color.clear;

            foreach (BorderFlag borderFlag in Enum.GetValues(typeof(BorderFlag)))
            {
                if (borderFlagMask.HasFlag(borderFlag))
                {
                    switch (borderFlag)
                    {
                        case BorderFlag.Left:
                            cursor.style.borderLeftWidth = BORDER_WIDTH;
                            break;

                        case BorderFlag.Top:
                            cursor.style.borderTopWidth = BORDER_WIDTH;
                            break;

                        case BorderFlag.Right:
                            cursor.style.borderRightWidth = BORDER_WIDTH;
                            break;

                        case BorderFlag.Bottom:
                            cursor.style.borderBottomWidth = BORDER_WIDTH;
                            break;
                    }
                }
            }

            return cursor;
        }

        /**
         *  指定大型パーツのTileDataModel群が構成する矩形サイズを取得。
         */
        private Vector2Int GetLargePartsTileRectSize(List<TileDataModel> tileDataModels) {
            if (tileDataModels.Count == 0)
            {
                return new Vector2Int();
            }

            return new Vector2Int(
                tileDataModels.Max(t => t.largePartsDataModel.x) + 1,
                tileDataModels.Max(t => t.largePartsDataModel.y) + 1);
        }

        /**
         *  配置してあるタイルImageの中から、指定TileDataModelのものを取得。
         */
        private Image GetTileImage(TileDataModel tileDataModel) {
            return this.Query<Image>().Where(tileImage => (tileImage.userData as TileDataModel) == tileDataModel).First();
        }

        /**
         *  指定タイルImageの座標を取得。
         */
        private Vector2Int GetTileImagePosition(Image tileImage) {
            VisualElement rowParentElement = tileImage.parent;
            return new Vector2Int(tileImage.parent.IndexOf(tileImage), IndexOf(rowParentElement));
        }
    }
}