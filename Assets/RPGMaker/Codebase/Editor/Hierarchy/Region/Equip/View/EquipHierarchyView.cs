using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Armor;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Item;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Weapon;
using RPGMaker.Codebase.Editor.Common;
using RPGMaker.Codebase.Editor.Common.View;
using RPGMaker.Codebase.Editor.Hierarchy.Region.Base.View.Component;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.Hierarchy.Region.Equip.View
{
    /// <summary>
    /// 装備のpHierarchyView
    /// </summary>
    public class EquipHierarchyView : AbstractHierarchyView
    {
        // const
        //--------------------------------------------------------------------------------------------------------------
        protected override string MainUxml { get { return "Assets/RPGMaker/Codebase/Editor/Hierarchy/Region/Equip/Asset/database_equip.uxml"; } }

        // ヒエラルキー本体クラス
        //--------------------------------------------------------------------------------------------------------------
        private readonly EquipHierarchy        _equipHierarchy;

        // 利用するデータ
        //--------------------------------------------------------------------------------------------------------------
        private List<WeaponDataModel> _weaponDataModels;

        // 状態
        //--------------------------------------------------------------------------------------------------------------

        // UI要素
        //--------------------------------------------------------------------------------------------------------------
        private HierarchyItemListView _weaponListView;
        private List<ArmorDataModel> _armorDataModels;
        private HierarchyItemListView _armorListView;
        private List<ItemDataModel> _itemDataModels;
        private HierarchyItemListView _itemListView;
        private const int foldoutCount = 1;

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
        /// <param name="equipHierarchy"></param>
        public EquipHierarchyView(EquipHierarchy equipHierarchy) {
            _equipHierarchy = equipHierarchy;
            InitUI();
        }

        /// <summary>
        /// 各コンテンツデータの初期化
        /// </summary>
        override protected void InitContentsData() {
            SetFoldout("weaponFoldout");
            _weaponListView = new HierarchyItemListView(ViewName + "Weapon");
            ((VisualElement) UxmlElement.Query<VisualElement>("weapon_item_list")).Add(_weaponListView);

            SetFoldout("armorFoldout");
            _armorListView = new HierarchyItemListView(ViewName + "Armor");
            ((VisualElement) UxmlElement.Query<VisualElement>("armor_item_list")).Add(_armorListView);

            SetFoldout("itemFoldout");
            _itemListView = new HierarchyItemListView(ViewName + "Item");
            ((VisualElement) UxmlElement.Query<VisualElement>("item_list")).Add(_itemListView);

            //Foldoutの開閉状態保持用
            for (int i = 0; i < foldoutCount; i++)
                SetFoldout("foldout_" + (i + 1));

            InitEventHandlers();
        }

        /// <summary>
        /// イベントの初期設定
        /// </summary>
        private void InitEventHandlers() {
            WeaponDataModel weaponDataModel = null;
            ArmorDataModel armorDataModel = null;
            ItemDataModel itemDataModel = null;

            // 武器Foldout右クリック時
            BaseClickHandler.ClickEvent(GetFoldout("weaponFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0512")), false,
                    _equipHierarchy.CreateWeaponDataModel);
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0513")), false, () =>
                {
                    if (weaponDataModel != null) _equipHierarchy.DuplicateWeaponDataModel(weaponDataModel);
                });
                menu.ShowAsContext();
            });
            // 武器リストアイテムクリック時
            _weaponListView.SetEventHandler(
                (i, value) => { _equipHierarchy.OpenWeaponInspector(_weaponDataModels[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0514")), false,
                        () => { weaponDataModel = _weaponDataModels[i]; });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0515")), false,
                        () =>
                        {
                            if (weaponDataModel != null && weaponDataModel.basic.id == _weaponDataModels[i].basic.id) weaponDataModel = null;
                            _equipHierarchy.DeleteWeaponDataModel(_weaponDataModels[i]);
                        });
                    menu.ShowAsContext();
                });

            // 防具Foldout右クリック時
            BaseClickHandler.ClickEvent(GetFoldout("armorFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0516")), false,
                    _equipHierarchy.CreateArmorDataModel);
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0517")), false, () =>
                {
                    if (armorDataModel != null) _equipHierarchy.DuplicateArmorDataModel(armorDataModel);
                });
                menu.ShowAsContext();
            });
            // 防具リストアイテムクリック時
            _armorListView.SetEventHandler(
                (i, value) => { _equipHierarchy.OpenArmorInspector(_armorDataModels[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0518")), false,
                        () => { armorDataModel = _armorDataModels[i]; });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0519")), false,
                        () =>
                        {
                            if (armorDataModel != null && armorDataModel.basic.id == _armorDataModels[i].basic.id) armorDataModel = null;
                            _equipHierarchy.DeleteArmorDataModel(_armorDataModels[i]);
                        });
                    menu.ShowAsContext();
                });

            // アイテムFoldout右クリック時
            BaseClickHandler.ClickEvent(GetFoldout("itemFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0520")), false,
                    _equipHierarchy.CreateItemDataModel);
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0521")), false, () =>
                {
                    if (itemDataModel != null) _equipHierarchy.DuplicateItemDataModel(itemDataModel);
                });
                menu.ShowAsContext();
            });
            // アイテムリストアイテムクリック時
            _itemListView.SetEventHandler(
                (i, value) => { _equipHierarchy.OpenItemInspector(_itemDataModels[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0522")), false,
                        () => { itemDataModel = _itemDataModels[i]; });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0523")), false,
                        () =>
                        {
                            if (itemDataModel != null && itemDataModel.basic.id == _itemDataModels[i].basic.id) itemDataModel = null;
                            _equipHierarchy.DeleteItemDataModel(_itemDataModels[i]);
                        });
                    menu.ShowAsContext();
                });
        }

        /// <summary>
        /// データ更新
        /// </summary>
        /// <param name="weaponDataModels"></param>
        /// <param name="armorDataModels"></param>
        /// <param name="itemDataModels"></param>
        public void Refresh(
            [CanBeNull] List<WeaponDataModel> weaponDataModels = null,
            [CanBeNull] List<ArmorDataModel> armorDataModels = null,
            [CanBeNull] List<ItemDataModel> itemDataModels = null
        ) {
            _weaponDataModels = weaponDataModels ?? _weaponDataModels;
            _armorDataModels = armorDataModels ?? _armorDataModels;
            _itemDataModels = itemDataModels ?? _itemDataModels;
            base.Refresh();
        }

        /// <summary>
        /// データ更新
        /// </summary>
        protected override void RefreshContents() {
            base.RefreshContents();
            _weaponListView.Refresh(_weaponDataModels.Select(item => item.basic.name).ToList());
            _armorListView.Refresh(_armorDataModels.Select(item => item.basic.name).ToList());
            _itemListView.Refresh(_itemDataModels.Select(item => item.basic.name).ToList());
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
        /// 最終選択していた防具を返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastArmorIndex() {
            var elements = new List<VisualElement>();
            _armorListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }

        /// <summary>
        /// 最終選択していたアイテムを返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastItemIndex() {
            var elements = new List<VisualElement>();
            _itemListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }
    }
}