using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.AssetManage;
using RPGMaker.Codebase.CoreSystem.Knowledge.Enum;
using RPGMaker.Codebase.Editor.Common;
using RPGMaker.Codebase.Editor.Common.View;
using RPGMaker.Codebase.Editor.Hierarchy.Region.Base.View.Component;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.Hierarchy.Region.AssetManage.View
{
    /// <summary>
    /// 素材管理のHierarchyView
    /// </summary>
    public class AssetManageHierarchyView : AbstractHierarchyView
    {
        // const
        //--------------------------------------------------------------------------------------------------------------
        protected override string MainUxml { get { return "Assets/RPGMaker/Codebase/Editor/Hierarchy/Region/AssetManage/Asset/database_asset.uxml"; } }

        // ヒエラルキー本体クラス
        //--------------------------------------------------------------------------------------------------------------
        private readonly AssetManageHierarchy _assetManageHierarchy;

        // 利用するデータ
        //--------------------------------------------------------------------------------------------------------------
        private List<AssetManageDataModel> _walkingCharacterAssets;
        private List<AssetManageDataModel> _actorAssets;
        private List<AssetManageDataModel> _battleEffectAssets;
        private List<AssetManageDataModel> _objectAssets;
        private List<AssetManageDataModel> _popupIconAssets;
        private List<AssetManageDataModel> _stateOverlapAssets;
        private List<AssetManageDataModel> _weaponAssets;

        // 状態
        //--------------------------------------------------------------------------------------------------------------

        // UI要素
        //--------------------------------------------------------------------------------------------------------------
        private HierarchyItemListView _actorListView;
        private HierarchyItemListView _battleEffectListView;
        private HierarchyItemListView _objectListView;
        private HierarchyItemListView _popupIconListView;
        private HierarchyItemListView _stateOverlapListView;
        private HierarchyItemListView _walkingCharacterListView;
        private HierarchyItemListView _weaponListView;
        private const int foldoutCount = 2;


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
        /// <param name="assetManageHierarchy"></param>
        public AssetManageHierarchyView(AssetManageHierarchy assetManageHierarchy) {
            _assetManageHierarchy = assetManageHierarchy;
            InitUI();
        }

        /// <summary>
        /// 各コンテンツデータの初期化
        /// </summary>
        override protected void InitContentsData() {
            SetFoldout("walkingCharacterFoldout");
            _walkingCharacterListView = new HierarchyItemListView(ViewName + "Walking");
            ((VisualElement) UxmlElement.Query<VisualElement>("walkingCharacterListContainer")).Add(_walkingCharacterListView);

            SetFoldout("objectFoldout");
            _objectListView = new HierarchyItemListView(ViewName + "Object");
            ((VisualElement) UxmlElement.Query<VisualElement>("objectListContainer")).Add(_objectListView);

            SetFoldout("popupIconFoldout");
            _popupIconListView = new HierarchyItemListView(ViewName + "PopupIcon");
            ((VisualElement) UxmlElement.Query<VisualElement>("popupIconListContainer")).Add(_popupIconListView);

            SetFoldout("actorFoldout");
            _actorListView = new HierarchyItemListView(ViewName + "Actor");
            ((VisualElement) UxmlElement.Query<VisualElement>("actorListContainer")).Add(_actorListView);

            SetFoldout("weaponFoldout");
            _weaponListView = new HierarchyItemListView(ViewName + "Weapon");
            ((VisualElement) UxmlElement.Query<VisualElement>("weaponListContainer")).Add(_weaponListView);

            SetFoldout("stateOverlapFoldout");
            _stateOverlapListView = new HierarchyItemListView(ViewName + "StateOverlap");
            ((VisualElement) UxmlElement.Query<VisualElement>("stateListContainer")).Add(_stateOverlapListView);

            SetFoldout("battleEffectFoldout");
            _battleEffectListView = new HierarchyItemListView(ViewName + "BattleEffect");
            ((VisualElement) UxmlElement.Query<VisualElement>("battleEffectListContainer")).Add(_battleEffectListView);

            SetFoldout("foldout_sv_battle_list");

            //Foldoutの開閉状態保持用
            for (int i = 0; i < foldoutCount; i++)
                SetFoldout("foldout_" + (i + 1));

            InitEventHandlers();
        }

        /// <summary>
        /// イベントの初期設定
        /// </summary>
        private void InitEventHandlers() {
            AssetManageDataModel _walkingCharacterAsset = null;
            AssetManageDataModel _objectAsset = null;
            AssetManageDataModel _popupIconAsset = null;
            AssetManageDataModel _actorAsset = null;
            AssetManageDataModel _weaponAsset = null;
            AssetManageDataModel _stateOverlapAsset = null;
            AssetManageDataModel _battleEffectAsset = null;

            // 歩行キャラ
            BaseClickHandler.ClickEvent(GetFoldout("walkingCharacterFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1322")), false,
                    () => { _assetManageHierarchy.CreateAssetManageDataModel(AssetCategoryEnum.MOVE_CHARACTER); });
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1323")), false, () =>
                {
                    if (_walkingCharacterAsset != null)
                        _assetManageHierarchy.DuplicateAssetManageDataModel(_walkingCharacterAsset);
                });
                menu.ShowAsContext();
            });
            _walkingCharacterListView.SetEventHandler(
                (i, value) => { _assetManageHierarchy.OpenAssetManageInspector(_walkingCharacterAssets[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1462")), false,
                        () => { _walkingCharacterAsset = _walkingCharacterAssets[i]; });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0383")), false,
                        () =>
                        {
                            _walkingCharacterAsset = null;
                            _assetManageHierarchy.DeleteAssetManageDataModel(_walkingCharacterAssets[i]);
                        });
                    menu.ShowAsContext();
                });

            // オブジェクト
            BaseClickHandler.ClickEvent(GetFoldout("objectFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1327")), false,
                    () => { _assetManageHierarchy.CreateAssetManageDataModel(AssetCategoryEnum.OBJECT); });
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1328")), false, () =>
                {
                    if (_objectAsset != null) _assetManageHierarchy.DuplicateAssetManageDataModel(_objectAsset);
                });
                menu.ShowAsContext();
            });
            _objectListView.SetEventHandler(
                (i, value) => { _assetManageHierarchy.OpenAssetManageInspector(_objectAssets[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1462")), false,
                        () => { _objectAsset = _objectAssets[i]; });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0383")), false,
                        () =>
                        {
                            _objectAsset = null;
                            _assetManageHierarchy.DeleteAssetManageDataModel(_objectAssets[i]);
                        });
                    menu.ShowAsContext();
                });

            // フキダシアイコン
            BaseClickHandler.ClickEvent(GetFoldout("popupIconFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1331")), false,
                    () => { _assetManageHierarchy.CreateAssetManageDataModel(AssetCategoryEnum.POPUP); });
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1332")), false, () =>
                {
                    if (_popupIconAsset != null)
                        _assetManageHierarchy.DuplicateAssetManageDataModel(_popupIconAsset);
                });
                menu.ShowAsContext();
            });
            _popupIconListView.SetEventHandler(
                (i, value) => { _assetManageHierarchy.OpenAssetManageInspector(_popupIconAssets[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1462")), false,
                        () => { _popupIconAsset = _popupIconAssets[i]; });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0383")), false,
                        () =>
                        {
                            _popupIconAsset = null;
                            _assetManageHierarchy.DeleteAssetManageDataModel(_popupIconAssets[i]);
                        });
                    menu.ShowAsContext();
                });

            // アクター
            BaseClickHandler.ClickEvent(GetFoldout("actorFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0302")), false,
                    () => { _assetManageHierarchy.CreateAssetManageDataModel(AssetCategoryEnum.SV_BATTLE_CHARACTER); });
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0303")), false, () =>
                {
                    if (_actorAsset != null) _assetManageHierarchy.DuplicateAssetManageDataModel(_actorAsset);
                });
                menu.ShowAsContext();
            });
            _actorListView.SetEventHandler(
                (i, value) => { _assetManageHierarchy.OpenAssetManageInspector(_actorAssets[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1462")), false,
                        () => { _actorAsset = _actorAssets[i]; });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0383")), false,
                        () =>
                        {
                            _actorAsset = null;
                            _assetManageHierarchy.DeleteAssetManageDataModel(_actorAssets[i]);
                        });
                    menu.ShowAsContext();
                });

            // 武器
            BaseClickHandler.ClickEvent(GetFoldout("weaponFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0512")), false,
                    () => { _assetManageHierarchy.CreateAssetManageDataModel(AssetCategoryEnum.SV_WEAPON); });
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0513")), false, () =>
                {
                    if (_weaponAsset != null) _assetManageHierarchy.DuplicateAssetManageDataModel(_weaponAsset);
                });
                menu.ShowAsContext();
            });
            _weaponListView.SetEventHandler(
                (i, value) => { _assetManageHierarchy.OpenAssetManageInspector(_weaponAssets[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1462")), false,
                        () => { _weaponAsset = _weaponAssets[i]; });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0383")), false,
                        () =>
                        {
                            _weaponAsset = null;
                            _assetManageHierarchy.DeleteAssetManageDataModel(_weaponAssets[i]);
                        });
                    menu.ShowAsContext();
                });

            // ステートの重ね合わせ
            BaseClickHandler.ClickEvent(GetFoldout("stateOverlapFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1345")), false,
                    () => { _assetManageHierarchy.CreateAssetManageDataModel(AssetCategoryEnum.SUPERPOSITION); });
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1346")), false, () =>
                {
                    if (_stateOverlapAsset != null)
                        _assetManageHierarchy.DuplicateAssetManageDataModel(_stateOverlapAsset);
                });
                menu.ShowAsContext();
            });
            _stateOverlapListView.SetEventHandler(
                (i, value) => { _assetManageHierarchy.OpenAssetManageInspector(_stateOverlapAssets[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1462")), false,
                        () => { _stateOverlapAsset = _stateOverlapAssets[i]; });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0383")), false,
                        () =>
                        {
                            _stateOverlapAsset = null;
                            _assetManageHierarchy.DeleteAssetManageDataModel(_stateOverlapAssets[i]);
                        });
                    menu.ShowAsContext();
                });

            // バトルエフェクト
            BaseClickHandler.ClickEvent(GetFoldout("battleEffectFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1349")), false,
                    () => { _assetManageHierarchy.CreateAssetManageDataModel(AssetCategoryEnum.BATTLE_EFFECT); });
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1519")), false, () =>
                {
                    if (_battleEffectAsset != null)
                        _assetManageHierarchy.DuplicateAssetManageDataModel(_battleEffectAsset);
                });
                menu.ShowAsContext();
            });
            _battleEffectListView.SetEventHandler(
                (i, value) => { _assetManageHierarchy.OpenAssetManageInspector(_battleEffectAssets[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1462")), false,
                        () => { _battleEffectAsset = _battleEffectAssets[i]; });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0383")), false,
                        () =>
                        {
                            _battleEffectAsset = null;
                            _assetManageHierarchy.DeleteAssetManageDataModel(_battleEffectAssets[i]);
                        });
                    menu.ShowAsContext();
                });
        }

        /// <summary>
        /// データ更新
        /// </summary>
        /// <param name="walkingCharacterAssets"></param>
        /// <param name="objectAssets"></param>
        /// <param name="popupIconAssets"></param>
        /// <param name="actorAssets"></param>
        /// <param name="weaponAssets"></param>
        /// <param name="stateAssets"></param>
        /// <param name="battleEffectAssets"></param>
        public void Refresh(
            [CanBeNull] List<AssetManageDataModel> walkingCharacterAssets = null,
            [CanBeNull] List<AssetManageDataModel> objectAssets = null,
            [CanBeNull] List<AssetManageDataModel> popupIconAssets = null,
            [CanBeNull] List<AssetManageDataModel> actorAssets = null,
            [CanBeNull] List<AssetManageDataModel> weaponAssets = null,
            [CanBeNull] List<AssetManageDataModel> stateAssets = null,
            [CanBeNull] List<AssetManageDataModel> battleEffectAssets = null
        ) {
            _walkingCharacterAssets = walkingCharacterAssets ?? _walkingCharacterAssets;
            _objectAssets = objectAssets ?? _objectAssets;
            _popupIconAssets = popupIconAssets ?? _popupIconAssets;
            _actorAssets = actorAssets ?? _actorAssets;
            _weaponAssets = weaponAssets ?? _weaponAssets;
            _stateOverlapAssets = stateAssets ?? _stateOverlapAssets;
            _battleEffectAssets = battleEffectAssets ?? _battleEffectAssets;
            base.Refresh();
        }

        /// <summary>
        /// データ更新
        /// </summary>
        protected override void RefreshContents() {
            base.RefreshContents();
            _walkingCharacterListView.Refresh(_walkingCharacterAssets.Select(item => item.name).ToList());
            _objectListView.Refresh(_objectAssets.Select(item => item.name).ToList());
            _popupIconListView.Refresh(_popupIconAssets.Select(item => item.name).ToList());
            _actorListView.Refresh(_actorAssets.Select(item => item.name).ToList());
            _weaponListView.Refresh(_weaponAssets.Select(item => item.name).ToList());
            _stateOverlapListView.Refresh(_stateOverlapAssets.Select(item => item.name).ToList());
            _battleEffectListView.Refresh(_battleEffectAssets.Select(item => item.name).ToList());
        }

        /// <summary>
        /// 最終選択していた歩行キャラを返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastWalkingCharacterIndex() {
            var elements = new List<VisualElement>();
            _walkingCharacterListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }

        /// <summary>
        /// 最終選択していたオブジェクトを返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastObjectIndex() {
            var elements = new List<VisualElement>();
            _objectListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }

        /// <summary>
        /// 最終選択していたフキダシを返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastPopupIconIndex() {
            var elements = new List<VisualElement>();
            _popupIconListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }

        /// <summary>
        /// 最終選択していたアクターを返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastActorIndex() {
            var elements = new List<VisualElement>();
            _actorListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }

        /// <summary>
        /// 最終選択していた武器を返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastWeaponIndex() {
            var elements = new List<VisualElement>();
            _weaponListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }

        /// <summary>
        /// 最終選択していたステートを返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastStateOverlapIndex() {
            var elements = new List<VisualElement>();
            _stateOverlapListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }

        /// <summary>
        /// 最終選択していたバトルエフェクトを返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastBattleEffectIndex() {
            var elements = new List<VisualElement>();
            _battleEffectListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }
    }
}