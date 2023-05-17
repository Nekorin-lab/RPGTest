using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.EventMap;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Map;
using RPGMaker.Codebase.CoreSystem.Service.MapManagement;
using RPGMaker.Codebase.CoreSystem.Service.MapManagement.Repository;
using RPGMaker.Codebase.Editor.Common;
using RPGMaker.Codebase.Editor.Common.View;
using RPGMaker.Codebase.Editor.Common.Window.ModalWindow;
using RPGMaker.Codebase.Editor.Hierarchy.Common;
using RPGMaker.Codebase.Editor.Hierarchy.Region.Base.View.Component;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.Hierarchy.Region.Map.View
{
    /// <summary>
    ///     データベースヒエラルキーのマップ部分
    /// </summary>
    public class MapHierarchyView : AbstractHierarchyView
    {
        protected override string MainUxml { get { return ""; } }

        private Button _addTileButton;
        private readonly Dictionary<string, Foldout> _eventFoldouts = new Dictionary<string, Foldout>();
        private List<EventMapDataModel> _eventMapDataModels;
        private string _updateData;

        // const
        //--------------------------------------------------------------------------------------------------------------
        public ExecEventType _execEventType = ExecEventType.None;

        // 利用するデータ
        //--------------------------------------------------------------------------------------------------------------
        private List<MapDataModel> _mapEntities;
        //private HierarchyItemListView _mapListView;

        private readonly Dictionary<string, Foldout> _mapFoldouts = new Dictionary<string, Foldout>();

        // ヒエラルキー本体
        //--------------------------------------------------------------------------------------------------------------
        private readonly MapHierarchy _mapHierarchy;

        // 状態
        //--------------------------------------------------------------------------------------------------------------
        private List<TileGroupDataModel> _tileGroupEntities;
        private HierarchyItemListView _tileGroupListView;

        private bool isInit;


        // UI要素
        //--------------------------------------------------------------------------------------------------------------


        //--------------------------------------------------------------------------------------------------------------
        //
        // methods
        //
        //--------------------------------------------------------------------------------------------------------------

        // 初期化・更新系
        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mapHierarchy"></param>
        public MapHierarchyView(MapHierarchy mapHierarchy) {
            _mapHierarchy = mapHierarchy;
            InitUI();
        }

        /// <summary>
        /// 各コンテンツデータの初期化
        /// </summary>
        override protected void InitContentsData() {
            // マップ設定Foldout
            Foldout mapSettingFoldout = new Foldout {text = EditorLocalize.LocalizeText("WORD_0732")};
            mapSettingFoldout.name = "mapSettingFoldout";
            Add(mapSettingFoldout);
            SetFoldout("mapSettingFoldout");

            // タイルデータの登録ボタン
            _addTileButton = new Button {text = EditorLocalize.LocalizeText("WORD_0733")};
            _addTileButton.AddToClassList("button-transparent");
            _addTileButton.AddToClassList("AnalyticsTag__page_view__map_tile");
            GetFoldout("mapSettingFoldout").Add(_addTileButton);

            // タイルグループFoldout
            Foldout tileGroupListFoldout = new Foldout {text = EditorLocalize.LocalizeText("WORD_0734")};
            tileGroupListFoldout.name = "tileGroupListFoldout";
            tileGroupListFoldout.AddToClassList("AnalyticsTag__page_view__map_tilegroup");
            GetFoldout("mapSettingFoldout").Add(tileGroupListFoldout);
            SetFoldout("tileGroupListFoldout");

            // タイルグループリスト
            _tileGroupListView = new HierarchyItemListView(ViewName);
            GetFoldout("tileGroupListFoldout").Add(_tileGroupListView);

            // "マップリスト"Foldout
            Foldout mapListFoldout = new Foldout {text = EditorLocalize.LocalizeText("WORD_0739")};
            mapListFoldout.name = "mapListFoldout";
            GetFoldout("mapSettingFoldout").Add(mapListFoldout);
            SetFoldout("mapListFoldout");

            // マップリスト
            //_mapListView = new StringListView();
            //_mapListFoldout.Add(_mapListView);

            InitEventHandlers();
        }

        /// <summary>
        /// イベントの初期設定
        /// </summary>
        private void InitEventHandlers() {
            TileGroupDataModel tileGroupDataModel = null;

            // タイルデータの登録ボタン
            Editor.Hierarchy.Hierarchy.AddSelectableElementAndAction(_addTileButton,
                MapEditor.MapEditor.LaunchTileEditMode);
            _addTileButton.clicked += () =>
            {
                Editor.Hierarchy.Hierarchy.InvokeSelectableElementAction(_addTileButton);
            };

            // タイルグループFoldout
            BaseClickHandler.ClickEvent(GetFoldout("tileGroupListFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;

                // コンテキストメニュー。
                var menu = new GenericMenu();

                // 『タイルグループの新規作成』。
                menu.AddItem(new GUIContent(
                    EditorLocalize.LocalizeText("WORD_0735")),
                    false,
                    () =>
                    {
                        var newTileGroupDataModel = MapEditor.MapEditor.LaunchTileGroupEditMode(null);
                        MapEditor.MapEditor.SaveTileGroup(newTileGroupDataModel);
                        MapEditor.MapEditor.ReloadTileGroups();
                    });

                // 『タイルグループの貼り付け』。
                menu.AddItem(new GUIContent(
                    EditorLocalize.LocalizeText("WORD_0736")),
                    false,
                    () =>
                    {
                        if (tileGroupDataModel != null)
                        {
                            tileGroupDataModel.id = Guid.NewGuid().ToString();
                            MapEditor.MapEditor.LaunchTileGroupEditMode(tileGroupDataModel);
                            MapEditor.MapEditor.SaveTileGroup(tileGroupDataModel);
                            MapEditor.MapEditor.ReloadTileGroups();
                            tileGroupDataModel = null;
                        }
                    });

                menu.ShowAsContext();
            });

            // タイルグループリスト
            _tileGroupListView.SetEventHandler(
                (i, value) => { MapEditor.MapEditor.LaunchTileGroupEditMode(_tileGroupEntities[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0737")), false, () =>
                    {
                        tileGroupDataModel = _tileGroupEntities[i].Clone();
                        tileGroupDataModel.name =
                            _tileGroupEntities[i].Clone().name + EditorLocalize.LocalizeText("WORD_1462");
                    });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0738")), false, () =>
                    {
                        MapEditor.MapEditor.RemoveTileGroup(_tileGroupEntities[i]);
                        MapEditor.MapEditor.ReloadTileGroups();
                        Refresh();
                    });
                    menu.ShowAsContext();
                });

            // マップFoldout
            BaseClickHandler.ClickEvent(GetFoldout("mapListFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                var menu = new GenericMenu();

                // サンプルマップから作成
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_3009")), false,
                    () =>
                    {
                        var modal = new MapCreateForSampleMapModalWindow();
                        modal.ShowWindow();
                    });
                //マップの新規作成
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0025")), false,
                    () =>
                    {
                        isInit = true;
                        var map = MapEditor.MapEditor.LaunchMapEditMode(null);
                        //マップ新規作成時は、Prefabを強制的に保存する
                        MapEditor.MapEditor.SaveMap(map, MapRepository.SaveType.SAVE_PREFAB_FORCE);
                        MapEditor.MapEditor.ReloadMap(map);
                    });
                //マップの貼り付け
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0026")), false,
                    () =>
                    {
                        if (CommonMapHierarchyView.MapDataModel == null) return;

                        isInit = true;

                        // 指定のマップデータのIDのみ変更&Prefab複製
                        var mapData = MapDataModel.CopyData(CommonMapHierarchyView.MapDataModel);
                        mapData.id = Guid.NewGuid().ToString();
                        mapData.index = Editor.Hierarchy.Hierarchy.mapManagementService.LoadMaps()[Editor.Hierarchy.Hierarchy.mapManagementService.LoadMaps().Count - 1].index++;
                        MapDataModel.CopyMapPrefabForEditor(CommonMapHierarchyView.MapDataModel, mapData.id);

                        // 名前設定
                        mapData.name += " - " + EditorLocalize.LocalizeText("WORD_1462");
                        mapData.name = MapEditor.MapEditor.MapNameDuplicateCheck(mapData.name);

                        // データ更新
                        // 新規作成時は、Prefabを強制的に保存する
                        MapEditor.MapEditor.SaveMap(mapData, MapRepository.SaveType.SAVE_PREFAB_FORCE);
                        MapEditor.MapEditor.ReloadMap(mapData);
                    });
                menu.ShowAsContext();
            });
        }

        /// <summary>
        /// 初期化済みかどうかの設定
        /// </summary>
        public void SetInit() {
            isInit = true;
        }

        /// <summary>
        /// データ更新
        /// </summary>
        /// <param name="mapEntities"></param>
        /// <param name="tileGroupEntities"></param>
        /// <param name="eventMapDataModels"></param>
        public void Refresh(
            string updateData = null,
            List<MapDataModel> mapEntities = null,
            List<TileGroupDataModel> tileGroupEntities = null,
            List<EventMapDataModel> eventMapDataModels = null
        ) {
            _updateData = updateData;
            _mapEntities = mapEntities ?? _mapEntities;
            _tileGroupEntities = tileGroupEntities ?? _tileGroupEntities;
            _eventMapDataModels = eventMapDataModels ?? _eventMapDataModels;
            base.Refresh();
        }

        /// <summary>
        /// データ更新
        /// </summary>
        protected override void RefreshContents() {
            base.RefreshContents();
            _tileGroupListView.Refresh(_tileGroupEntities.Select(item => item.name).ToList());
            if (_updateData == null)
            {
                SetMap();
            }
            else
            {
                UpdateMap();
            }
        }

        /// <summary>
        ///     全マップのヒエラルキーを設定。
        /// </summary>
        private void SetMap() {
            var mapHierarchyInfo =
                new MapHierarchyInfo(
                    GetFoldout("mapListFoldout"),
                    _mapFoldouts,
                    _eventFoldouts,
                    _eventMapDataModels,
                    _mapHierarchy,
                    this);

            GetFoldout("mapListFoldout").Clear();
            _mapEntities?.ForEach(mapEntity =>
            {
                CommonMapHierarchyView.AddMapFoldout(mapEntity, mapHierarchyInfo, this);
            });

            InvokeSelectableElementAction();
        }
        
        /// <summary>
        /// 特定のマップのヒエラルキーを更新
        /// </summary>
        private void UpdateMap() {
            var mapHierarchyInfo =
                new MapHierarchyInfo(
                    GetFoldout("mapListFoldout"),
                    _mapFoldouts,
                    _eventFoldouts,
                    _eventMapDataModels,
                    _mapHierarchy,
                    this);

            for (int i = 0; i < _mapEntities.Count; i++)
            {
                if (_mapEntities[i].id == _updateData)
                {
                    CommonMapHierarchyView.EditMapFoldout(_mapEntities[i], mapHierarchyInfo, this);
                }
            }

            InvokeSelectableElementAction();
        }

        /// <summary>
        /// 最終選択していたマップを返却（待ち時間あり）
        /// </summary>
        private async void InvokeSelectableElementAction() {
            await Task.Delay(200);
            if (isInit)
            {
                if (LastMapIndex() != null) Editor.Hierarchy.Hierarchy.InvokeSelectableElementAction(LastMapIndex());
                isInit = false;
            }
        }

        /// <summary>
        /// 最終選択していたマップを返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastMapIndex() {
            var elements = new List<VisualElement>();
            //マップ編集ボタンの名前に設定あるIDで詰めていく
            foreach (var mapEntity in _mapEntities)
                GetFoldout("mapListFoldout").Query<Button>().ForEach(button =>
                {
                    if (button.name == CommonMapHierarchyView.GetMapEditButtonName(mapEntity.id)) elements.Add(button);
                });

            return elements[elements.Count - 1];
        }

        /// <summary>
        ///     データベースのマップリスト用のマップヒエラルキー情報クラス。
        /// </summary>
        private class MapHierarchyInfo : IMapHierarchyInfo {
            private readonly MapHierarchy _mapHierarchy;
            private readonly MapHierarchyView _mapHierarchyView;

            public MapHierarchyInfo(
                VisualElement parentVe,
                Dictionary<string, Foldout> mapFoldouts,
                Dictionary<string, Foldout> eventFoldouts,
                List<EventMapDataModel> eventMapDataModels,
                MapHierarchy mapHierarchy,
                MapHierarchyView mapHierarchyView
            ) {
                ParentVe = parentVe;

                MapFoldouts = mapFoldouts;
                EventFoldouts = eventFoldouts;

                _mapHierarchy = mapHierarchy;
                _mapHierarchyView = mapHierarchyView;

                EventMapDataModels = eventMapDataModels;
            }

            public int ReplaceIndex { get; } = -1;

            public VisualElement ParentVe { get; }
            public string Name { get; } = null;

            public Dictionary<string, Foldout> MapFoldouts { get; }
            public Dictionary<string, Foldout> EventFoldouts { get; }

            public ExecEventType ExecEventType
            {
                get => _mapHierarchyView._execEventType;
                set => _mapHierarchyView._execEventType = value;
            }

            public List<EventMapDataModel> EventMapDataModels { get; }

            public AbstractHierarchyView ParentClass { get { return _mapHierarchyView; } }

            public void RefreshMapHierarchy(string[] mapIds = null) {
                // データベースヒエラルキーのマップリストは、マップidでの絞り込みはしないはず。
                DebugUtil.Assert(mapIds == null);

                _mapHierarchy.Refresh();
            }

            public void RefreshEventHierarchy(string updateData) {
                _mapHierarchyView.Refresh(updateData);
            }
        }
    }
}