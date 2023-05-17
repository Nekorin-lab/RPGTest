using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.CharacterActor;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.EventMap;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Flag;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Item;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Map;
using RPGMaker.Codebase.CoreSystem.Service.DatabaseManagement;
using RPGMaker.Codebase.CoreSystem.Service.EventManagement;
using RPGMaker.Codebase.CoreSystem.Service.MapManagement;
using RPGMaker.Codebase.Runtime.Map;
using RPGMaker.Codebase.Editor.Common;
using RPGMaker.Codebase.Editor.Common.Window;
using RPGMaker.Codebase.Editor.Hierarchy.Enum;
using RPGMaker.Codebase.Editor.Inspector.Map.View;
using RPGMaker.Codebase.Editor.MapEditor.Component.Canvas;
using RPGMaker.Codebase.Editor.MapEditor.Window.EventEdit;
using RPGMaker.Codebase.Editor.MapEditor.Window.MapEdit;
using UnityEngine;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Armor;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Weapon;
using RPGMaker.Codebase.CoreSystem.Service.MapManagement.Repository;
using RPGMaker.Codebase.Editor.Inspector;

namespace RPGMaker.Codebase.Editor.MapEditor
{
    public static class MapEditor
    {
        //uss
        public static string UssDarkLayout = "Assets/RPGMaker/Codebase/Editor/MapEditor/Asset/Uxml/MapEdit/uss/dark/layout.uss";
        public static string UssDarkMapEdit = "Assets/RPGMaker/Codebase/Editor/MapEditor/Asset/Uxml/MapEdit/uss/dark/map_edit.uss";
        public static string UssDarkLayerInventory = "Assets/RPGMaker/Codebase/Editor/MapEditor/Asset/Uxml/MapEdit/uss/dark/layer_inventory.uss";
        public static string UssDarkTileInventory = "Assets/RPGMaker/Codebase/Editor/MapEditor/Asset/Uxml/MapEdit/uss/dark/tile_inventory.uss";
        public static string UssDarkTileImageInventory = "Assets/RPGMaker/Codebase/Editor/MapEditor/Asset/Uxml/MapEdit/uss/dark/tile_image_inventory.uss";
        public static string UssLightLayout = "Assets/RPGMaker/Codebase/Editor/MapEditor/Asset/Uxml/MapEdit/uss/light/layout.uss";
        public static string UssLightMapEdit = "Assets/RPGMaker/Codebase/Editor/MapEditor/Asset/Uxml/MapEdit/uss/light/map_edit.uss";
        public static string UssLightLayerInventory = "Assets/RPGMaker/Codebase/Editor/MapEditor/Asset/Uxml/MapEdit/uss/light/layer_inventory.uss";
        public static string UssLightTileInventory = "Assets/RPGMaker/Codebase/Editor/MapEditor/Asset/Uxml/MapEdit/uss/light/tile_inventory.uss";
        public static string UssLightTileImageInventory = "Assets/RPGMaker/Codebase/Editor/MapEditor/Asset/Uxml/MapEdit/uss/light/tile_image_inventory.uss";

        // data properties
        //-----------------------------------------------------------------------
        private static MapDataModel _mapDataModel;
        private static EventDataModel _eventDataModelEntity;
        private static EventMapDataModel _eventMapDataModelEntity;
        private static TileGroupDataModel _tileGroupDataModel;
        private static List<TileDataModel> _tileEntities;
        private static List<TileImageDataModel> _tileImageEntities;
        private static List<TileGroupDataModel> _tileGroupEntities;
        private static FlagDataModel _flags;
        private static List<ItemDataModel> _items;
        private static List<ArmorDataModel> _armors;
        private static List<WeaponDataModel> _weapons;
        private static List<CharacterActorDataModel> _actors;
        private static bool _isSampleMap;
        private static int _page;

        // windows
        //-----------------------------------------------------------------------
        private static MenuWindow              _menuWindow;
        private static MapEditWindow           _mapEditWindow;
        private static BattleEditWindow        _battleEditWindow;
        private static TileEditWindow          _tileEditWindow;
        private static TileGroupEditWindow     _tileGroupEditWindow;
        private static EventEditWindow         _eventEditWindow;

        // usecases
        //-----------------------------------------------------------------------
        private static MapManagementService      _mapManagementService;
        private static EventManagementService    _eventManagementService;
        private static DatabaseManagementService _databaseManagementService;

        /**
         * 初期化
         */
        //public static void Init(MapEntity mapEntity)
        public static void Init() {
            _mapManagementService = Hierarchy.Hierarchy.mapManagementService;
            _eventManagementService = Hierarchy.Hierarchy.eventManagementService;
            _databaseManagementService = Hierarchy.Hierarchy.databaseManagementService;

            // プロパティ初期化
            _tileEntities = _mapManagementService.LoadTiles();
            _tileImageEntities = _mapManagementService.LoadTileImageEntities();
            _tileGroupEntities = _mapManagementService.LoadTileGroups();

            // ウィンドウ初期化
            InitWindows();
        }

        public static void Refresh() {
            InitWindows();
            var inspectorParams = InspectorParams.instance;
            switch (inspectorParams.displayIndex)
            {
                case (int) Common.Enum.Display.MapEdit:
                case (int) Common.Enum.Display.MapBackground:
                case (int) Common.Enum.Display.MapBackgroundCol:
                case (int) Common.Enum.Display.MapDistant:
                    LaunchMapEditMode(_mapDataModel, _isSampleMap);
                    break;
                case (int) Common.Enum.Display.Encounter:
                    LaunchBattleEditMode(_mapDataModel);
                    break;
                case (int) Common.Enum.Display.MapEvent:
                    if (_eventMapDataModelEntity != null)
                    {
                        LaunchEventEditMode(_mapDataModel, _eventMapDataModelEntity, _page);
                    }
                    else
                    {
                        LaunchEventPutMode(_mapDataModel);
                    }
                    break;
            }
        }

        /**
         * ウィンドウ初期化
         */
        private static void InitWindows() {
            // マップ編集ウィンドウ
            var ids = new List<WindowLayoutManager.WindowLayoutId>()
            {
                WindowLayoutManager.WindowLayoutId.MapEditWindow,
                WindowLayoutManager.WindowLayoutId.MapBattleEditWindow,
                WindowLayoutManager.WindowLayoutId.MapTileEditWindow,
                WindowLayoutManager.WindowLayoutId.MapTileGroupEditWindow,
                WindowLayoutManager.WindowLayoutId.MapEventEditWindow
            };

            // InitWindow直後はリストが正常に作られていないため、リストを作りつつ、不要なWindowを消していく
            for (int i = 0; i < ids.Count - 1; i++)
            {
                WindowLayoutManager.SwitchWindows(ids[i + 1], ids[i]);
            }

            // マップ編集Windowをデフォルト表示する
            SwitchToMapEditWindow();
            SetRpgSceneMapAsMapEditWindowTitle();

            // イベント関連のWindowは、マップ編集Windowをデフォルトにしているため閉じる
            // 本Windowもリストが正常に作られていないため、作成して即閉じる
            var commandSettingWindow = WindowLayoutManager.GetOrOpenWindow(
                    WindowLayoutManager.WindowLayoutId.MapEventCommandSettingWindow
                ) as CommandSettingWindow;
            commandSettingWindow.Close();

            var executionContentsWindow = WindowLayoutManager.GetOrOpenWindow(
                    WindowLayoutManager.WindowLayoutId.MapEventExecutionContentsWindow
                ) as ExecutionContentsWindow;
            executionContentsWindow.Close();
        }

        private static void SwitchToMapEditWindow()
        {
            _mapEditWindow = WindowLayoutManager.SwitchWindows(
                WindowLayoutManager.WindowLayoutId.MapEditWindow, GetCurrentMainWindowId()) as MapEditWindow;
        }

        private static void SetRpgSceneMapAsMapEditWindowTitle()
        {
            _mapEditWindow.titleContent =
                new GUIContent(EditorLocalize.LocalizeText("WORD_1563") + " " + EditorLocalize.LocalizeText("WORD_1564"));
        }

        // エディタモード制御
        //------------------------------------------------------------------------------
        /**
         * マップ編集モードに切り替え
         */
        public static MapDataModel LaunchMapEditMode(MapDataModel mapDataModel, bool isSampleMap = false) {
            SwitchToMapEditWindow();
            SetRpgSceneMapAsMapEditWindowTitle();

            _mapDataModel = mapDataModel ?? CreateMap();
            _isSampleMap = isSampleMap;
            _mapEditWindow.Init(_mapDataModel, _tileEntities, isSampleMap);
            
            // MapEditWindowをアクティブにする
            WindowLayoutManager.GetOrOpenWindow(WindowLayoutManager.WindowLayoutId.MapEditWindow);

            return _mapDataModel;
        }

        /**
         * バトル編集モードに切り替え
         */
        public static void LaunchBattleEditMode(MapDataModel mapDataModel) {
            var currentMainWindowId = GetCurrentMainWindowId();
            _mapDataModel = mapDataModel ?? CreateMap();
            _battleEditWindow = WindowLayoutManager.SwitchWindows(
                WindowLayoutManager.WindowLayoutId.MapBattleEditWindow,
                currentMainWindowId
            ) as BattleEditWindow;
            _battleEditWindow.titleContent = new GUIContent(EditorLocalize.LocalizeText("WORD_1563") + " " + EditorLocalize.LocalizeText("WORD_1567"));
            _battleEditWindow.Init(_mapDataModel, _tileEntities);

            // BattleEditWindowをアクティブにする
            WindowLayoutManager.GetOrOpenWindow(WindowLayoutManager.WindowLayoutId.MapBattleEditWindow);
        }

        /**
         * タイル編集モードに切り替え
         */
        public static void LaunchTileEditMode() {
            var currentMainWindowId = GetCurrentMainWindowId();
            Inspector.Inspector.Clear();
            _tileEditWindow = WindowLayoutManager.SwitchWindows(
                WindowLayoutManager.WindowLayoutId.MapTileEditWindow,
                currentMainWindowId
            ) as TileEditWindow;
            _tileEditWindow.titleContent = new GUIContent(EditorLocalize.LocalizeText("WORD_1563") + " " + EditorLocalize.LocalizeText("WORD_1565"));
            _tileEditWindow.Init(_tileEntities, _tileImageEntities);
            _tileEditWindow.RefreshWindowSize(_tileEditWindow.rootVisualElement.resolvedStyle.height);
            // ウィンドウサイズが変わった際に呼ばれる
            _tileEditWindow.rootVisualElement.RegisterCallback<UnityEngine.UIElements.GeometryChangedEvent>(evt =>
            {
                _tileEditWindow.RefreshWindowSize(_tileEditWindow.rootVisualElement.resolvedStyle.height);
            });

            // MapTileEditWindowをアクティブにする
            WindowLayoutManager.GetOrOpenWindow(WindowLayoutManager.WindowLayoutId.MapTileEditWindow);
        }

        /**
         * タイルグループ編集モードに切り替え
         */
        public static TileGroupDataModel LaunchTileGroupEditMode(TileGroupDataModel tileGroupDataModel) {
            var currentMainWindowId = GetCurrentMainWindowId();
            Inspector.Inspector.Clear();

            tileGroupDataModel ??=
                new TileGroupDataModel(
                    Guid.NewGuid().ToString(),
                    EditorLocalize.LocalizeText("WORD_1596"), new List<TileDataModel>());
            _tileGroupDataModel = tileGroupDataModel;

            _tileGroupEditWindow = WindowLayoutManager.SwitchWindows(
                WindowLayoutManager.WindowLayoutId.MapTileGroupEditWindow,
                currentMainWindowId
            ) as TileGroupEditWindow;
            //_tileGroupEditWindow.titleContent = new GUIContent(EditorLocalize.LocalizeWindowTitle("MapEditor TileGroupEdit"));
            _tileGroupEditWindow.titleContent = new GUIContent(EditorLocalize.LocalizeText("WORD_1563") + " " + EditorLocalize.LocalizeText("WORD_1566"));
            _tileGroupEditWindow.Init(tileGroupDataModel, _tileEntities);

            // MapTileGroupEditWindowをアクティブにする
            WindowLayoutManager.GetOrOpenWindow(WindowLayoutManager.WindowLayoutId.MapTileGroupEditWindow);

            return tileGroupDataModel;
        }

        /**
         * イベント設置モードに切り替え
         */
        public static void LaunchEventPutMode(MapDataModel mapDataModel) {
            var eventMapEntities = _eventManagementService.LoadEventMapEntitiesByMapId(mapDataModel.id);

            var currentMainWindowId = GetCurrentMainWindowId();
            _mapDataModel = mapDataModel;
            _eventMapDataModelEntity = null;
            _eventEditWindow = WindowLayoutManager.SwitchWindows(
                WindowLayoutManager.WindowLayoutId.MapEventEditWindow,
                currentMainWindowId
            ) as EventEditWindow;
            _eventEditWindow.titleContent =
                new GUIContent(EditorLocalize.LocalizeText("WORD_1563") + " " + EditorLocalize.LocalizeText("WORD_1564"));
            //new GUIContent(EditorLocalize.LocalizeWindowTitle("MapEditor") + " " + _mapDataModel.name);

            _eventEditWindow.Init(_mapDataModel, eventMapEntities, 0);
            _eventEditWindow.LaunchEventMode(mapDataModel.id);

            // MapEventEditWindowをアクティブにする
            WindowLayoutManager.GetOrOpenWindow(WindowLayoutManager.WindowLayoutId.MapEventEditWindow);
        }

        /**
         * イベント編集モードに切り替え
         */
        public static async void LaunchEventEditMode(
            MapDataModel mapDataModel,
            EventMapDataModel eventMapDataModelEntity,
            int pageNum = 0
        ) {
            DebugUtil.Log(string.Concat(
                $"LaunchEventEditMode(",
                string.Join(
                    ", ",
                    new string[]
                    {
                        $"マップ名: \"{mapDataModel.name}\"",
                        $"イベント名: \"{eventMapDataModelEntity.name}\"",
                        $"座標: ({eventMapDataModelEntity.x}, {eventMapDataModelEntity.y})",
                        $"ページ: {pageNum + 1}",
                    }),
                ")"));

            var eventMapEntities = _eventManagementService.LoadEventMapEntitiesByMapId(mapDataModel.id);
            var dataList = _eventManagementService.LoadEventsById(eventMapDataModelEntity.eventId);
            _eventDataModelEntity = dataList.FirstOrDefault(c => c.page == pageNum);
            if (_eventDataModelEntity == null)
                _eventDataModelEntity = new EventDataModel(
                    eventMapDataModelEntity.eventId,
                    pageNum,
                    1,
                    new List<EventDataModel.EventCommand>()
                );

            var currentMainWindowId = GetCurrentMainWindowId();
            _eventMapDataModelEntity = eventMapDataModelEntity;
            _mapDataModel = mapDataModel;
            if (currentMainWindowId != WindowLayoutManager.WindowLayoutId.MapEventEditWindow)
            {
                _eventEditWindow = WindowLayoutManager.SwitchWindows(
                    WindowLayoutManager.WindowLayoutId.MapEventEditWindow,
                    currentMainWindowId
                ) as EventEditWindow;
                _eventEditWindow.titleContent =
                    new GUIContent(EditorLocalize.LocalizeText("WORD_1563") + " " + EditorLocalize.LocalizeText("WORD_1564"));
                //new GUIContent(EditorLocalize.LocalizeWindowTitle("MapEditor") + " " + _mapDataModel.name);
            }

            if(_eventEditWindow == null){
                _eventEditWindow = WindowLayoutManager.SwitchWindows(
                    WindowLayoutManager.WindowLayoutId.MapEventEditWindow,
                    currentMainWindowId
                ) as EventEditWindow;
                _eventEditWindow.titleContent =
                    new GUIContent(EditorLocalize.LocalizeText("WORD_1563") + " " + EditorLocalize.LocalizeText("WORD_1564"));
            }

            await Task.Delay(50);

            // Delay中にイベントが削除される事がある為エラーチェック
            // そもそも参照するべきイベントが無くなっていた場合は処理終了
            eventMapEntities = _eventManagementService.LoadEventMapEntitiesByMapId(mapDataModel.id);
            EventMapDataModel data = null;
            for (int i = 0; i < eventMapEntities.Count; i++)
                if (eventMapEntities[i].eventId == eventMapDataModelEntity.eventId)
                {
                    data = eventMapEntities[i];
                    break;
                }

            if (eventMapEntities == null || 
                eventMapDataModelEntity == null ||
                data == null ||
                data.pages == null ||
                data.pages.Count == 0 || 
                _eventDataModelEntity == null)
            {
                return;
            }

            // ページ番号が大きい場合は丸める
            if (data.pages.Count <= pageNum)
            {
                pageNum = data.pages.Count - 1;
            }
            _eventEditWindow.Init(_mapDataModel, eventMapEntities, pageNum, eventMapDataModelEntity);

            await Task.Delay(250);

            // Delay中にイベントが削除される事がある為エラーチェック
            // そもそも参照するべきイベントが無くなっていた場合は処理終了
            eventMapEntities = _eventManagementService.LoadEventMapEntitiesByMapId(mapDataModel.id);
            data = null;
            for (int i = 0; i < eventMapEntities.Count; i++)
                if (eventMapEntities[i].eventId == eventMapDataModelEntity.eventId)
                {
                    data = eventMapEntities[i];
                    break;
                }

            if (eventMapEntities == null || 
                eventMapDataModelEntity == null ||
                data == null ||
                data.pages == null ||
                data.pages.Count == 0 ||
                _eventDataModelEntity == null)
            {
                return;
            }

            // ページ番号が大きい場合は丸める
            if (data.pages.Count <= pageNum)
            {
                pageNum = data.pages.Count - 1;
            }

            // イベント位置にカーソルを設定する
            _eventEditWindow.SetCursorPosOnEventPositon();

            // 『イベントコマンド』枠を開く。
            var commandSettingWindow = (CommandSettingWindow)WindowLayoutManager.GetActiveWindow(
                WindowLayoutManager.WindowLayoutId.MapEventCommandSettingWindow);
            if (commandSettingWindow == null)
            {
                commandSettingWindow = WindowLayoutManager.OpenAndDockWindow(
                    WindowLayoutManager.WindowLayoutId.MapEventCommandSettingWindow,
                    WindowLayoutManager.WindowLayoutId.DatabaseInspectorWindow,
                    Docker.DockPosition.Bottom
                ) as CommandSettingWindow;
                commandSettingWindow.titleContent =
                    new GUIContent(EditorLocalize.LocalizeWindowTitle("WORD_1570"));
            }

            // 『イベント実行内容』枠を開く。
            // (『イベントコマンド』枠を参照しているので、『イベントコマンド』枠の後で開く)
            var executionContentsWindow = (ExecutionContentsWindow)WindowLayoutManager.GetActiveWindow(
                WindowLayoutManager.WindowLayoutId.MapEventExecutionContentsWindow);
            if (executionContentsWindow == null)
            {
                executionContentsWindow = WindowLayoutManager.OpenAndDockWindow(
                    WindowLayoutManager.WindowLayoutId.MapEventExecutionContentsWindow,
                    WindowLayoutManager.WindowLayoutId.MapEventEditWindow,
                    Docker.DockPosition.Bottom
                ) as ExecutionContentsWindow;
                executionContentsWindow.titleContent =
                    new GUIContent(EditorLocalize.LocalizeWindowTitle("WORD_1569"));
            }

            executionContentsWindow.Init(_eventDataModelEntity, true, ExecutionContentsWindow.EventType.Normal);

            if (!WindowLayoutManager.IsCharacterShowAnimationEventSelecting())
            {
                // MapEventEditWindowをアクティブにする
                WindowLayoutManager.GetOrOpenWindow(WindowLayoutManager.WindowLayoutId.MapEventEditWindow);
            }

            _page = pageNum;
        }

        /// <summary>
        /// 座標指定
        /// </summary>
        /// <param name="mapDataModel"></param>
        /// <param name="pageNum"></param>
        /// <param name="callBack"></param>
        /// <param name="notCoordinateMode">非座標指定モード。</param>
        public static async void LaunchCommonEventEditMode(
            MapDataModel mapDataModel,
            int pageNum = 0,
            Action<Vector3Int> callBack = null,
            string id = "",
            bool eventMove = false,
            bool notCoordinateMode = false
        ) {
            _mapDataModel = mapDataModel;
            if (_eventEditWindow == null)
            {
                var currentMainWindowId = GetCurrentMainWindowId();
                _eventEditWindow = WindowLayoutManager.SwitchWindows(
                    WindowLayoutManager.WindowLayoutId.MapEventEditWindow,
                    currentMainWindowId
                ) as EventEditWindow;
                _eventEditWindow.titleContent = new GUIContent(EditorLocalize.LocalizeText("WORD_1563") + " " + EditorLocalize.LocalizeText("WORD_1564"));
                //new GUIContent(EditorLocalize.LocalizeWindowTitle("MapEditor") + " " + _mapDataModel.name);
            }

            _eventEditWindow.Init(_mapDataModel, null, pageNum);

            if (!notCoordinateMode)
            {
                _eventEditWindow.LaunchCoordinateMode(callBack, id, eventMove);
            }

            await Task.Delay(250);
            WindowLayoutManager.GetOrOpenWindow(WindowLayoutManager.WindowLayoutId.MapEventEditWindow);
        }
        
        public static async void LaunchCommonEventEditModeEnd(
            MapDataModel mapDataModel,
            int pageNum = 0
        ) {
            _mapDataModel = mapDataModel;
            if (_eventEditWindow == null)
            {
                var currentMainWindowId = GetCurrentMainWindowId();
                _eventEditWindow = WindowLayoutManager.SwitchWindows(
                    WindowLayoutManager.WindowLayoutId.MapEventEditWindow,
                    currentMainWindowId
                ) as EventEditWindow;
                _eventEditWindow.titleContent = new GUIContent(EditorLocalize.LocalizeText("WORD_1563") + " " + EditorLocalize.LocalizeText("WORD_1564"));
                //new GUIContent(EditorLocalize.LocalizeWindowTitle("MapEditor") + " " + _mapDataModel.name);
            }

            _eventEditWindow.Init(_mapDataModel, null, pageNum);
            _eventEditWindow.LaunchCoordinateModeEnd();
            await Task.Delay(250);
            WindowLayoutManager.GetOrOpenWindow(WindowLayoutManager.WindowLayoutId.MapEventEditWindow);
        }

        //プレビューボタンの作成
        public static void CreateEventPreviewButton(Action callBack) {
            if (_eventEditWindow != null)
                _eventEditWindow.PreviewButtonCreate(callBack);
        }

        //別のイベントが開かれた時、ボタンがあったら削除する
        public static void WhenEventClosed() {
            if (_eventEditWindow != null)
                _eventEditWindow.WhenEventClosed();
        }

        private static WindowLayoutManager.WindowLayoutId GetCurrentMainWindowId() {
            var ids = new List<WindowLayoutManager.WindowLayoutId>()
            {
                WindowLayoutManager.WindowLayoutId.MapEditWindow,
                WindowLayoutManager.WindowLayoutId.MapBattleEditWindow,
                WindowLayoutManager.WindowLayoutId.MapTileEditWindow,
                WindowLayoutManager.WindowLayoutId.MapTileGroupEditWindow,
                WindowLayoutManager.WindowLayoutId.MapEventEditWindow
            };

            for (int i = 0; i < ids.Count; i++)
                if (WindowLayoutManager.IsActiveWindow(ids[i]))
                    return ids[i];

            return WindowLayoutManager.WindowLayoutId.None;
        }

        // イベントサブウィンドウ制御
        //------------------------------------------------------------------------------
        public static void LaunchRouteEditMode(
            List<Vector3Int> pos,
            List<int> indexList,
            List<EventDataModel.EventCommandMoveRoute> codeList,
            List<string> nameList,
            Action<List<EventDataModel.EventCommandMoveRoute>> setAndSaveMoveRouteAction,
            int eventIndex = 0,
            int eventCommandIndex = 0
        ) {
            _eventEditWindow?.LaunchRouteMode(
                _eventMapDataModelEntity, eventIndex, eventCommandIndex, codeList, setAndSaveMoveRouteAction, pos);
        }
        
        public static void LaunchRouteDrawingMode(Vector3Int initializePos) {
            _eventEditWindow?.LaunchRouteDrawingMode(initializePos);
        }

        public static void LaunchRouteDrawingModeEnd() {
            _eventEditWindow?.LaunchRouteDrawingModeEnd();
        }

        public static void LaunchDestinationEditMode(
            int eventIndex,
            int eventCommandIndex,
            Vector2Int pos,
            [CanBeNull] Action action = null
        ) {
            _eventEditWindow?.LaunchDestinationMode(_eventMapDataModelEntity, eventIndex, eventCommandIndex, pos, action);
        }

        /// <summary>
        /// 目的地描画モードを開始
        /// </summary>
        public static void BeginDestinationMode(
            int eventIndex,
            int eventCommandIndex,
            Vector2Int pos
        ) {
            _eventEditWindow?.BeginDestinationMode(_eventMapDataModelEntity, eventIndex, eventCommandIndex, pos);
        }

        /// <summary>
        /// 目的地描画モードを終了
        /// </summary>
        public static void EndDestinationMode() {
            _eventEditWindow?.EndDestinationMode();
        }


        // インスペクター制御
        //------------------------------------------------------------------------------
        /**
         * インスペクターにマップを表示
         */
        public static void SetMapEntityToInspector(MapDataModel mapDataModel, bool isSampleMap = false) {
            Inspector.Inspector.MapView(mapDataModel, isSampleMap);
        }

        /**
         * インスペクターに遠景を表示
         */
        public static void SetDistantViewToInspector(MapDataModel mapEntity) {
            Inspector.Inspector.MapDistantView(mapEntity);
        }

        /**
         * インスペクターに背景を表示
         */
        public static void SetBackgroundViewToInspector(MapDataModel mapEntity) {
            Inspector.Inspector.MapBackgroundView(mapEntity);
        }

        /**
         * インスペクターに背景を表示
         */
        public static void SetBackgroundCollisionViewToInspector(TileDataModel tileDataModel) {
            Inspector.Inspector.MapBackgroundCollisionView(tileDataModel);
        }

        /**
         * インスペクターにタイルを表示
         */
        public static void SetTileEntityToInspector(TileDataModel tileDataModel, bool toImage = false) {
            if (toImage)
            {
                Inspector.Inspector.MapTileView(tileDataModel, TileInspector.TYPE.IMAGE);
                return;
            }
            Inspector.Inspector.MapTileView(tileDataModel);
        }

        /**
         * インスペクターにイベントを表示
         */
        public static void SetEventEntityToInspector(EventMapDataModel eventMapDataModelEntity, int pageNum,EventEditWindow element, MapDataModel mapDataModel) {
            _flags = _databaseManagementService.LoadFlags();
            _items = _databaseManagementService.LoadItem().ToList();
            _weapons = _databaseManagementService.LoadWeapon().ToList();
            _armors = _databaseManagementService.LoadArmor().ToList();
            _actors = _databaseManagementService.LoadCharacterActor().ToList();
            Inspector.Inspector.MapEventView(eventMapDataModelEntity, _flags, _items, _weapons, _armors, _actors, pageNum, element, mapDataModel);
        }

        // マップEntity操作
        //------------------------------------------------------------------------------
        /**
         * マップ一覧を再取得する.
         */
        public static void ReloadMaps() {
            _ = Hierarchy.Hierarchy.Refresh(Region.Map, _mapDataModel.id);
        }

        /**
         * マップを読み込みなおす.
         */
        public static void ReloadMap([CanBeNull] MapDataModel mapDataModel = null, string updateData = null) {
            if (mapDataModel != null) _mapDataModel = mapDataModel;
            if (_mapEditWindow != null) _mapEditWindow.Refresh(mapDataModel, null);
            _ = Hierarchy.Hierarchy.Refresh(Region.Map, updateData);
        }

        /**
         * マップを新規作成する
         */
        public static MapDataModel CreateMap() {
            return MapSettingInitForEditor(_mapManagementService.CreateMapForEditor());
        }

        /**
         * マップの初期設定
         */
        private static MapDataModel MapSettingInitForEditor(MapDataModel mapDataModel) {
            // 各コンポーネントを設定
            var mapPrefab = mapDataModel.MapPrefabManagerForEditor.mapPrefab;
            mapPrefab.transform.GetChild((int) MapDataModel.Layer.LayerType.BackgroundCollision).gameObject.AddComponent<BackgroundCollisionViewManager>();
            mapPrefab.transform.GetChild((int) MapDataModel.Layer.LayerType.Background).gameObject.AddComponent<BackgroundManager>();
            mapPrefab.transform.GetChild((int) MapDataModel.Layer.LayerType.DistantView).gameObject.AddComponent<DistantViewManager>();
            return mapDataModel;
        }

        /**
         * マップを編集（現在の状態を保存）する
         */
        public static void SaveMap(MapDataModel mapDataModel, MapRepository.SaveType saveType) {
            _mapEditWindow.SaveCurrentLayerEmphasis();
            _mapManagementService.SaveMap(mapDataModel, saveType);
            _mapEditWindow.UpdateCurrentLayerEmphasis();
        }

        /**
         * マップを削除する
         */
        public static void RemoveMap(MapDataModel mapDataModel) {
            _mapManagementService.RemoveMap(mapDataModel);
        }

        /**
         * マップ名の重複チェック
         */
        public static string MapNameDuplicateCheck(string name) {
            return _mapManagementService.MapNameDuplicateCheck(name);
        }

        /**
         * マップまたはバトルの描画モードを変更する
         */
        public static void ChangeDrawMode(PenButtonMenu.EditTarget editTarget, MapEditCanvas.DrawMode drawMode)
        {
            switch (editTarget)
            {
                case PenButtonMenu.EditTarget.Map:
                    _mapEditWindow.ChangeDrawMode(drawMode);
                    break;
                case Inspector.Map.View.PenButtonMenu.EditTarget.Battle:
                    _battleEditWindow.ChangeDrawMode(drawMode);
                    break;
            }
        }

        /**
         * タイルを取得する
         */
        public static TileDataModel GetTileToDraw() {
            return _mapEditWindow.GetTileToDraw();
        }


        // タイル用画像Entity操作
        //------------------------------------------------------------------------------
        /**
         * インポート済みのタイル用画像一覧を再取得する.
         */
        public static void ReloadTileImageEntities() {
            _tileImageEntities = _mapManagementService.LoadTileImageEntities();
            if (_tileEditWindow != null) _tileEditWindow.Refresh(null, _tileImageEntities);
        }

        /**
         * タイル用画像をインポート（コピー）する
         */
        public static void ImportImageForTile() {
            _mapManagementService.ImportTileImageFile();
        }

        // タイルEntity操作
        //------------------------------------------------------------------------------
        /**
         * タイル一覧を再取得する.
         */
        public static void ReloadTiles() {
            _tileEntities = _mapManagementService.LoadTiles();
            if (_tileEditWindow != null) _tileEditWindow.Refresh(_tileEntities, null);
            if (_tileGroupEditWindow != null) _tileGroupEditWindow.Refresh(null, _tileEntities);
        }

        /**
         * タイルを新規作成する
         */
        public static TileDataModel CreateTile(TileImageDataModel tileImageDataModel, TileDataModel.Type type) {
            var tileEntity = ScriptableObject.CreateInstance<TileDataModel>();
            tileEntity.id = Guid.NewGuid().ToString();
            tileEntity.tileImageDataModel = tileImageDataModel;
            tileEntity.type = type;
            tileEntity.vehicleTypes = new List<VehicleType>();
            return tileEntity;
        }

        /**
         * タイルを保存する.
         */
        public static async Task<bool> SaveTile(TileDataModel tileDataModel) {
            //AssetDatabaseの更新停止が実行されていたら
            if (ApiManager.IsAssetDatabaseStop)
            {
                return false;
            }

            ApiManager.IsTileOrImageAssetDatabase = true;
            
            bool taskBool = await _mapManagementService.SaveTile(tileDataModel);
            
            ApiManager.IsTileOrImageAssetDatabase = false;

            return taskBool;
        }

        /**
         * タイルを保存する.
         */
        public static async Task<List<bool>> SaveTile(List<TileDataModel> tileDataModel) {
            //AssetDatabaseの更新停止が実行されていたら
            var lists = new List<bool>();
            if (ApiManager.IsAssetDatabaseStop)
            {
                for (int i = 0; i < tileDataModel.Count; i++)
                {
                    lists.Add(false);
                }
                return lists;
            }
            
            ApiManager.IsTileOrImageAssetDatabase = true;
            
            lists = await _mapManagementService.SaveTile(tileDataModel);
            
            ApiManager.IsTileOrImageAssetDatabase = false;

            return lists;
        }

        /**
         * インスペクターのタイルを保存する.
         */
        public static void SaveInspectorTile(TileDataModel tileDataModel) {
            _mapManagementService.SaveInspectorTile(tileDataModel);
        }

        // タイルグループEntity操作
        //------------------------------------------------------------------------------
        public static void ReloadTileGroups() {
            _tileGroupEntities = _mapManagementService.LoadTileGroups();
            _ = Hierarchy.Hierarchy.Refresh(Hierarchy.Enum.Region.Map, "");
        }

        /**
         * タイルグループを保存する
         */
        public static void SaveTileGroup(TileGroupDataModel tileGroupDataModel) {
            _mapManagementService.SaveTileGroup(tileGroupDataModel);
        }

        /**
         * タイルグループを削除する
         */
        public static void RemoveTileGroup(TileGroupDataModel tileGroupDataModel) {
            _mapManagementService.RemoveTileGroup(tileGroupDataModel);
        }

        // イベントEntity操作
        //------------------------------------------------------------------------------
        /**
         * マップイベントを新規作成する
         */
        public static EventMapDataModel CreateEvent(MapDataModel mapDataModel, int x, int y, int specialType = 0) {
            var eventMapDataModel = new EventMapDataModel()
            {
                x = x,
                y = y,
                mapId = mapDataModel.id,
                eventId = Guid.NewGuid().ToString(),
                pages = new List<EventMapDataModel.EventMapPage>()
                {
                    EventMapDataModel.EventMapPage.CreateDefault()
                }
            };

            // ページ配列の1つしかない要素のページ番号。
            int page = eventMapDataModel.pages[0].page;
            Debug.Assert(page == EventMapDataModel.EventMapPage.DefaultPage);

            var eventMapPage = eventMapDataModel.pages.Single(eventMapPage => eventMapPage.page == page);

            if (specialType == 1)
            {
                eventMapPage.condition.image.enabled = 2;
                eventMapPage.eventTrigger = 1;
                eventMapPage.priority = 0;
            }

            int type = 0;
            if (eventMapPage.condition.image.imageName == "")
            {
                var actor = _databaseManagementService.LoadCharacterActor().First();
                eventMapPage.condition.image.imageName = actor.uuId;
            }
            
            SaveEventMap(eventMapDataModel);

            var eventDataModel =
                new EventDataModel(eventMapDataModel.eventId, page, type, new List<EventDataModel.EventCommand>());
            SaveEvent(eventDataModel);

            return eventMapDataModel;
        }

        /**
         * マップイベントを削除する
         */
        public static void DeleteEventMap(EventMapDataModel eventMapDataModel) {
            var eventManagementService = new EventManagementService();

            // 対象のEventDataModelをセーブファイルから削除。
            var eventList = eventManagementService.LoadEvent().
                FindAll(eventDataModel => eventDataModel.id == eventMapDataModel.eventId);
            foreach(var eventData in eventList)
                eventManagementService.DeleteEvent(eventData);

            // 対象のEventMapDataModelをセーブファイルから削除。
            eventManagementService.DeleteEventMap(eventMapDataModel);
        }

        /**
         * マップイベントを保存する
         */
        public static void SaveEventMap(EventMapDataModel eventMapDataModelEntity) {
            _eventManagementService.SaveEventMap(eventMapDataModelEntity);
        }

        /**
         * イベントを保存する
         */
        public static void SaveEvent(EventDataModel eventDataModelEntity) {
            _eventManagementService.SaveEvent(eventDataModelEntity);
        }

        //ページの作成
        public static void CreatePage(EventMapDataModel eventMapDataModel, int page, int type) {
            var eventPage = EventMapDataModel.EventMapPage.CreateDefault();
            eventPage.page = page;
            var actor = _databaseManagementService.LoadCharacterActor().First();
            eventPage.condition.image.imageName = actor.uuId;

            eventMapDataModel.pages.Add(eventPage);
            SaveEventMap(eventMapDataModel);
            var eventDataModel = new EventDataModel(eventMapDataModel.eventId, page, type,
                new List<EventDataModel.EventCommand>());

            SaveEvent(eventDataModel);
        }

        //ページの削除
        public static void DeletePage(EventMapDataModel eventMapDataModel, int page) {
            // 対象のEventDataModelをセーブファイルから削除。
            var eventManagementService = new EventManagementService();
            var eventList = eventManagementService.LoadEventsById(eventMapDataModel.eventId);
            // 対象削除、ページを詰める
            foreach(var eventData in eventList)
            {
                if(page == eventData.page)
                    eventManagementService.DeleteEvent(eventData);
                else if(page < eventData.page)
                    eventManagementService.PageStuffing(eventData);
            }

            // 対象のページを削除。
            for(int i = 0; i < eventMapDataModel.pages.Count; i++)
            {
                if(page == eventMapDataModel.pages[i].page)
                {
                    eventMapDataModel.pages.Remove(eventMapDataModel.pages[i]);
                    i--;
                }
                else if(page < eventMapDataModel.pages[i].page)
                    eventMapDataModel.pages[i].page--;
            }

            if (eventMapDataModel.pages.Count > 0)
            {
                SaveEventMap(eventMapDataModel);

                // イベントページ削除時に最初のページを開く
                var executionContentsWindow = (ExecutionContentsWindow) WindowLayoutManager.GetActiveWindow(
                    WindowLayoutManager.WindowLayoutId.MapEventExecutionContentsWindow);
                if (executionContentsWindow == null)
                    return;

                executionContentsWindow ??= (ExecutionContentsWindow) WindowLayoutManager.GetOrOpenWindow(
                    WindowLayoutManager.WindowLayoutId.MapEventExecutionContentsWindow);
                var eventDataList = _eventManagementService.LoadEventsById(_eventDataModelEntity.id);
                var eventEntity = eventDataList.FirstOrDefault(c => c.page == 0);
                executionContentsWindow.Init(eventEntity, true);

                _eventDataModelEntity = null;
            }
            else
            {
                // ページ数が0になったのならマップイベント自体を削除。
                DeleteEventMap(eventMapDataModel);
            }
        }

        //ページのコピー
        public static void CopyPage(
            EventMapDataModel eventMapDataModel,
            EventMapDataModel.EventMapPage originData,
            int page,
            int type,
            EventMapDataModel pasteEventMapDataModel
        ) {
            // Json経由でクローン作成。
            var eventPage = JsonHelper.Clone(originData);

            int pageNum = pasteEventMapDataModel.pages[pasteEventMapDataModel.pages.Count - 1].page + 1;
            eventPage.page = pageNum;
            pasteEventMapDataModel.pages.Add(eventPage);
            SaveEventMap(pasteEventMapDataModel);

            var dataList = _eventManagementService.LoadEventsById(eventMapDataModel.eventId).DataClone();
            _eventDataModelEntity = dataList.FirstOrDefault(c => c.page == originData.page).DataClone();

            var eventDataModel = new EventDataModel(pasteEventMapDataModel.eventId, pageNum, type,
                new List<EventDataModel.EventCommand>());
            eventDataModel.eventCommands = _eventDataModelEntity.eventCommands;
            SaveEvent(eventDataModel);
        }

        /**
        * 編集するeventEntityの変更
        */
        public static void ChangeEvent(EventDataModel data) {
            _eventDataModelEntity = data;
        }
        
        /**
         * eventEntityの更新
         */
        public static void EventRefresh(bool flg = false) {
            //flg = true の場合、Windowがnullの場合は処理しない
            var executionContentsWindow = (ExecutionContentsWindow)WindowLayoutManager.GetActiveWindow(
                WindowLayoutManager.WindowLayoutId.MapEventExecutionContentsWindow);
            if (executionContentsWindow == null && flg)
            {
                return;
            }

            executionContentsWindow ??= (ExecutionContentsWindow)WindowLayoutManager.GetOrOpenWindow(
                WindowLayoutManager.WindowLayoutId.MapEventExecutionContentsWindow);

            var pageNum = _eventDataModelEntity.page;
            var eventDataList = _eventManagementService.LoadEventsById(_eventDataModelEntity.id);
            var eventEntity = eventDataList.FirstOrDefault(c => c.page == pageNum);
            executionContentsWindow.Refresh(eventEntity, flg);
        }

        /**
         * ProcessTextの更新
         */
        public static void ProcessTextRefresh() {
            var executionContentsWindow = (ExecutionContentsWindow)WindowLayoutManager.GetActiveWindow(
                WindowLayoutManager.WindowLayoutId.MapEventExecutionContentsWindow);
            executionContentsWindow.ProcessTextSetting();
        }

        public static EventEditCanvas ReloadEventMap(
            MapDataModel mapDataModel,
            List<EventMapDataModel> eventMapEntities,
            int pageNum,
            [CanBeNull] EventMapDataModel eventMapDataModelEntity = null
        ) {
            return _eventEditWindow.ReloadMap(
                mapDataModel,
                eventMapEntities,
                pageNum,
                eventMapDataModelEntity
            );
        }
    }
}