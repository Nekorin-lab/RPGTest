using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.CharacterActor;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Class;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Enemy;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Troop;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Vehicle;
using RPGMaker.Codebase.CoreSystem.Knowledge.Enum;
using RPGMaker.Codebase.Editor.Common;
using RPGMaker.Codebase.Editor.Common.View;
using RPGMaker.Codebase.Editor.Hierarchy.Region.Base.View.Component;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.Hierarchy.Region.Character.View
{
    /// <summary>
    /// キャラクターのHierarchyView
    /// </summary>
    public class CharacterHierarchyView : AbstractHierarchyView
    {
        // const
        //--------------------------------------------------------------------------------------------------------------
        protected override string MainUxml { get { return "Assets/RPGMaker/Codebase/Editor/Hierarchy/Region/Character/Asset/database_characters.uxml"; } }

        // 利用するデータ
        //--------------------------------------------------------------------------------------------------------------
        private List<CharacterActorDataModel> _characterActorDataModels;
        private CharacterActorDataModel _actorDataModel;
        private List<ClassDataModel> _classDataModels;
        private List<CharacterActorDataModel> _npcCharacterActorDataModels;
        private List<VehiclesDataModel> _vehiclesDataModels;

        // ヒエラルキー本体クラス
        //--------------------------------------------------------------------------------------------------------------
        private readonly CharacterHierarchy _characterHierarchy;

        // UI要素
        //--------------------------------------------------------------------------------------------------------------
        private HierarchyItemListView _actorListView;
        private HierarchyItemListView _jobListView;
        private HierarchyItemListView _npcListView;
        private Button _initialPartySettingButton;
        private Button _jobCommonSettingButton;
        private HierarchyItemListView _vehicleListView;

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
        /// <param name="characterHierarchy"></param>
        public CharacterHierarchyView(CharacterHierarchy characterHierarchy) {
            _characterHierarchy = characterHierarchy;
            InitUI();
        }

        /// <summary>
        /// 各コンテンツデータの初期化
        /// </summary>
        override protected void InitContentsData() {
            //アクター
            SetFoldout("characterMasterFoldout");
            SetFoldout("actorFoldout");
            _actorListView = new HierarchyItemListView(ViewName + "Actor");
            ((VisualElement) UxmlElement.Query<VisualElement>("actorListContainer")).Add(_actorListView);

            //NPC
            SetFoldout("npcFoldout");
            _npcListView = new HierarchyItemListView(ViewName + "Npc");
            ((VisualElement) UxmlElement.Query<VisualElement>("npcListContainer")).Add(_npcListView);

            //初期パーティ
            _initialPartySettingButton = UxmlElement.Query<Button>("initialPartySettingButton");

            //乗り物
            SetFoldout("vehicleFoldout");
            _vehicleListView = new HierarchyItemListView(ViewName + "Vehicle");
            ((VisualElement) UxmlElement.Query<VisualElement>("vehicleListContainer")).Add(_vehicleListView);

            //職業
            SetFoldout("jobFoldout");
            _jobCommonSettingButton = UxmlElement.Query<Button>("jobCommonSettingButton");
            _jobListView = new HierarchyItemListView(ViewName + "Class");
            ((VisualElement) UxmlElement.Query<VisualElement>("jobListContainer")).Add(_jobListView);

            InitEventHandlers();
        }

        /// <summary>
        /// イベントの初期設定
        /// </summary>
        private void InitEventHandlers() {
            VehiclesDataModel vehiclesDataModel = null;
            ClassDataModel classDataModel = null;

            // アクターFoldout右クリック時
            BaseClickHandler.ClickEvent(GetFoldout("actorFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0302")), false,
                    () => { _characterHierarchy.CreateCharacterActorDataModel(this); });
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0303")), false, () =>
                {
                    if (_actorDataModel != null) _characterHierarchy.PasteActorOrNpcDataModel(this, _actorDataModel);
                });
                menu.ShowAsContext();
            });

            // アクターリストアイテムクリック時
            _actorListView.SetEventHandler(
                (i, value) =>
                {
                    Inspector.Inspector.CharacterView((int) ActorTypeEnum.ACTOR, _characterActorDataModels[i].uuId,
                        this);
                },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0304")), false,
                        () => { _actorDataModel = _characterActorDataModels[i]; });
                    if (i != 0)
                    {
                        menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0305")), false,
                            () => { _characterHierarchy.DeleteCharacterActorDataModel(_characterActorDataModels[i]); });
                    }

                    menu.ShowAsContext();
                });

            // NPC Foldout右クリック時
            BaseClickHandler.ClickEvent(GetFoldout("npcFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;

                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0307")), false,
                    () => { _characterHierarchy.CreateNpcCharacterActorDataModel(this); });
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0308")), false, () =>
                {
                    if (_actorDataModel != null) _characterHierarchy.PasteActorOrNpcDataModel(this, _actorDataModel);
                });
                menu.ShowAsContext();
            });

            // NPCリストアイテムクリック時
            _npcListView.SetEventHandler(
                (i, value) =>
                {
                    Inspector.Inspector.CharacterView((int) ActorTypeEnum.NPC, _npcCharacterActorDataModels[i].uuId,
                        this);
                },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0309")), false,
                        () => { _actorDataModel = _npcCharacterActorDataModels[i]; });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0310")), false,
                        () =>
                        {
                            _characterHierarchy.DeleteNpcCharacterActorDataModel(_npcCharacterActorDataModels[i]);
                        });
                    menu.ShowAsContext();
                });

            // 初期パーティー設定ボタンクリック時
            Editor.Hierarchy.Hierarchy.AddSelectableElementAndAction(_initialPartySettingButton,
                () => { _characterHierarchy.OpenInitialPartySettingInspector(); });
            _initialPartySettingButton.clickable.clicked += () =>
            {
                Editor.Hierarchy.Hierarchy.InvokeSelectableElementAction(_initialPartySettingButton);
            };


            // 乗り物Foldout右クリック時
            BaseClickHandler.ClickEvent(GetFoldout("vehicleFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;

                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0316")), false,
                    () => { _characterHierarchy.CreateVehicleDataModel(); });
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0317")), false, () =>
                {
                    if (vehiclesDataModel != null) _characterHierarchy.PasteVehicleDataModel(vehiclesDataModel);
                });
                menu.ShowAsContext();
            });

            // 乗り物リストアイテムクリック時
            _vehicleListView.SetEventHandler(
                (i, value) => { _characterHierarchy.OpenVehicleInspector(_vehiclesDataModels[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0318")), false,
                        () => { vehiclesDataModel = _vehiclesDataModels[i]; });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0319")), false,
                        () => { _characterHierarchy.DeleteVehicleDataModel(_vehiclesDataModels[i]); });
                    menu.ShowAsContext();
                });

            // 職業の編集Foldout右クリック時
            BaseClickHandler.ClickEvent(GetFoldout("jobFoldout"),
                evt =>
                {
                    if (evt != (int) MouseButton.RightMouse) return;

                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0330")), false,
                        () => { _characterHierarchy.CreateClassDataModel(); });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0331")), false, () =>
                    {
                        if (classDataModel != null) _characterHierarchy.PasteClassDataModel(classDataModel);
                    });
                    menu.ShowAsContext();
                });

            // 職業リストアイテムクリック時
            _jobListView.SetEventHandler(
                (i, value) => { _characterHierarchy.OpenClassInspector(_classDataModels[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0332")), false,
                        () => { classDataModel = _classDataModels[i]; });
                    if (i != 0)
                    {
                        menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0333")), false,
                            () => { _characterHierarchy.DeleteClassDataModel(_classDataModels[i]); });
                    }

                    menu.ShowAsContext();
                });

            // 職業の共通設定クリック時
            Editor.Hierarchy.Hierarchy.AddSelectableElementAndAction(_jobCommonSettingButton, () =>
            {
                _characterHierarchy.OpenClassInspector(null);
            });
            _jobCommonSettingButton.clickable.clicked += () =>
            {
                Editor.Hierarchy.Hierarchy.InvokeSelectableElementAction(_jobCommonSettingButton);
            };
        }

        // データ更新
        //--------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// データ更新
        /// </summary>
        /// <param name="characterActorDataModels"></param>
        /// <param name="npcCharacterActorDataModels"></param>
        /// <param name="vehiclesDataModels"></param>
        /// <param name="classDataModels"></param>
        /// <param name="enemyDataModels"></param>
        /// <param name="troopDataModels"></param>
        /// <param name="eventBattleDataModels"></param>
        /// <param name="encounterDataModels"></param>
        public void Refresh(
            [CanBeNull] List<CharacterActorDataModel> characterActorDataModels = null,
            [CanBeNull] List<CharacterActorDataModel> npcCharacterActorDataModels = null,
            [CanBeNull] List<VehiclesDataModel> vehiclesDataModels = null,
            [CanBeNull] List<ClassDataModel> classDataModels = null
        ) {
            if (characterActorDataModels != null) _characterActorDataModels = characterActorDataModels;
            if (npcCharacterActorDataModels != null) _npcCharacterActorDataModels = npcCharacterActorDataModels;
            if (vehiclesDataModels != null) _vehiclesDataModels = vehiclesDataModels;
            if (classDataModels != null) _classDataModels = classDataModels;
            base.Refresh();
        }

        /// <summary>
        /// データ更新
        /// </summary>
        protected override void RefreshContents() {
            base.RefreshContents();
            UpdateCharacter();
            UpdateVehicle();
            UpdateClass();
        }

        /// <summary>
        /// キャラクターの更新
        /// </summary>
        public void UpdateCharacter() {
            _actorListView.Refresh(_characterActorDataModels.Select(item => item.basic.name).ToList());
            _npcListView.Refresh(_npcCharacterActorDataModels.Select(item => item.basic.name).ToList());
        }

        /// <summary>
        /// 乗り物の更新
        /// </summary>
        public void UpdateVehicle() {
            _vehicleListView.Refresh(_vehiclesDataModels.Select(item => item.name).ToList());
        }

        /// <summary>
        /// 職業の更新
        /// </summary>
        public void UpdateClass() {
            _jobListView.Refresh(_classDataModels.Select(item => item.basic.name).ToList());
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
        /// 最終選択していたNPCを返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastNpcIndex() {
            var elements = new List<VisualElement>();
            _npcListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }

        /// <summary>
        /// 最終選択していた乗り物を返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastVehicleIndex() {
            var elements = new List<VisualElement>();
            _vehicleListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }

        /// <summary>
        /// 最終選択していた職業を返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastClassIndex() {
            var elements = new List<VisualElement>();
            _jobListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }

        // イベントハンドラ
        //--------------------------------------------------------------------------------------------------------------
    }
}