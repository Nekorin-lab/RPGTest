using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.SystemSetting;
using RPGMaker.Codebase.Editor.Common;
using RPGMaker.Codebase.Editor.Common.View;
using RPGMaker.Codebase.Editor.Hierarchy.Region.Base.View.Component;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.Hierarchy.Region.Type.View
{
    public class TypeHierarchyView : AbstractHierarchyView
    {
        // const
        //--------------------------------------------------------------------------------------------------------------
        protected override string MainUxml { get { return "Assets/RPGMaker/Codebase/Editor/Hierarchy/Region/Type/Asset/database_type.uxml"; } }

        // 状態
        //--------------------------------------------------------------------------------------------------------------
        //コピーしたデータの保持
        private int _attributeIndex;

        // UI要素
        //--------------------------------------------------------------------------------------------------------------
        private HierarchyItemListView _attributeTypeListView;
        private HierarchyItemListView _equipmentTypeListView;
        private int _equipmentIndex;
        private HierarchyItemListView _skillTypeListView;
        private int _skillIndex;
        private HierarchyItemListView _armorTypeListView;
        private int _armorIndex;
        private HierarchyItemListView _weaponTypeListView;
        private int _weaponIndex;
        private const int foldoutCount = 1;

        // 利用するデータ
        //--------------------------------------------------------------------------------------------------------------
        private SystemSettingDataModel _systemSettingDataModel;

        // ヒエラルキー本体
        //--------------------------------------------------------------------------------------------------------------
        private readonly TypeHierarchy         _typeHierarchy;


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
        /// <param name="typeHierarchy"></param>
        public TypeHierarchyView(TypeHierarchy typeHierarchy) {
            _typeHierarchy = typeHierarchy;
            InitUI();
            InitEventHandlers();
        }

        /// <summary>
        /// 各コンテンツデータの初期化
        /// </summary>
        override protected void InitContentsData() {
            SetFoldout("element_type");
            _attributeTypeListView = new HierarchyItemListView(ViewName + "Element");
            ((VisualElement) UxmlElement.Query<VisualElement>("attribute_type_list")).Add(_attributeTypeListView);

            SetFoldout("skill_type");
            _skillTypeListView = new HierarchyItemListView(ViewName + "Skill");
            ((VisualElement) UxmlElement.Query<VisualElement>("skill_type_list")).Add(_skillTypeListView);

            SetFoldout("weapon_type");
            _weaponTypeListView = new HierarchyItemListView(ViewName + "Weapon");
            ((VisualElement) UxmlElement.Query<VisualElement>("weapon_type")).Add(_weaponTypeListView);

            SetFoldout("armor_type");
            _armorTypeListView = new HierarchyItemListView(ViewName + "Armor");
            ((VisualElement) UxmlElement.Query<VisualElement>("armor_type")).Add(_armorTypeListView);

            SetFoldout("equip_type");
            _equipmentTypeListView = new HierarchyItemListView(ViewName + "Equip");
            ((VisualElement) UxmlElement.Query<VisualElement>("equip_type")).Add(_equipmentTypeListView);

            //Foldoutの開閉状態保持用
            for (int i = 0; i < foldoutCount; i++)
                SetFoldout("foldout_" + (i + 1));
        }

        /// <summary>
        /// イベントの初期設定
        /// </summary>
        private void InitEventHandlers() {
            // 属性
            BaseClickHandler.ClickEvent(GetFoldout("element_type"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                if (_systemSettingDataModel.elements.Count >= 99) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1231")), false,
                    _typeHierarchy.CreateAttributeType);
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1232")), false,
                    () =>
                    {
                        _typeHierarchy.DuplicateAttributeType(_systemSettingDataModel.elements[_attributeIndex]);
                    });
                menu.ShowAsContext();
            });
            _attributeTypeListView.SetEventHandler(
                //属性表示時、「なし」「通常攻撃」を除外して表示する(前から二つを非表示)
                (i, value) => { _typeHierarchy.OpenAttributeTypeInspector(_systemSettingDataModel.elements[i + 2]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    //通常攻撃、なしで+2
                    if (_systemSettingDataModel.elements.Count < 99)
                        menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1233")), false,
                            () => { _attributeIndex = i + 2; });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1234")), false,
                        () => { _typeHierarchy.DeleteAttributeType(_systemSettingDataModel.elements[i + 2]); });
                    menu.ShowAsContext();
                });

            // スキルタイプ
            BaseClickHandler.ClickEvent(GetFoldout("skill_type"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                if (_systemSettingDataModel.skillTypes.Count >= 99) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1244")), false,
                    _typeHierarchy.CreateSkillType);
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1245")), false,
                    () => { _typeHierarchy.DuplicateSkillType(_systemSettingDataModel.skillTypes[_skillIndex]); });
                menu.ShowAsContext();
            });
            _skillTypeListView.SetEventHandler(
                //属性表示時、「なし」「通常攻撃」を除外して表示する(前から二つを非表示)
                (i, value) => { _typeHierarchy.OpenSkillTypeInspector(_systemSettingDataModel.skillTypes[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    if (_systemSettingDataModel.skillTypes.Count < 99) 
                        menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1246")), false,
                            () => { _skillIndex = i; });
                    //先頭のものは消さないようにする
                    if(i > 0)
                        menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1247")), false, 
                            () => { _typeHierarchy.DeleteSkillType(_systemSettingDataModel.skillTypes[i]); });
                    menu.ShowAsContext();
                });

            // 武器タイプ
            BaseClickHandler.ClickEvent(GetFoldout("weapon_type"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                if (_systemSettingDataModel.weaponTypes.Count >= 99) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1250")), false,
                    _typeHierarchy.CreateWeaponType);
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1251")), false,
                    () => { _typeHierarchy.DuplicateWeaponType(_systemSettingDataModel.weaponTypes[_weaponIndex]); });
                menu.ShowAsContext();
            });
            _weaponTypeListView.SetEventHandler(
                (i, value) => { _typeHierarchy.OpenWeaponTypeInspector(_systemSettingDataModel.weaponTypes[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    if (_systemSettingDataModel.weaponTypes.Count < 99)
                        menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1252")), false,
                            () => { _weaponIndex = i; });
                    //先頭のものは消さないようにする
                    if(i > 0)
                        menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1253")), false,
                        () => { _typeHierarchy.DeleteWeaponType(_systemSettingDataModel.weaponTypes[i]); });
                    menu.ShowAsContext();
                });

            // 防具タイプ
            BaseClickHandler.ClickEvent(GetFoldout("armor_type"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                if (_systemSettingDataModel.armorTypes.Count >= 99) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1266")), false,
                    _typeHierarchy.CreateArmorType);
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1267")), false,
                    () => { _typeHierarchy.DuplicateArmorType(_systemSettingDataModel.armorTypes[_armorIndex]); });
                menu.ShowAsContext();
            });
            _armorTypeListView.SetEventHandler(
                //属性表示時、「なし」「通常攻撃」を除外して表示する(前から二つを非表示)
                (i, value) => { _typeHierarchy.OpenArmorTypeInspector(_systemSettingDataModel.armorTypes[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    if (_systemSettingDataModel.armorTypes.Count < 99)
                        menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1268")), false,
                            () => { _armorIndex = i; });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1269")), false,
                        () => { _typeHierarchy.DeleteArmorType(_systemSettingDataModel.armorTypes[i]); });


                    menu.ShowAsContext();
                });

            // 装備タイプ
            BaseClickHandler.ClickEvent(GetFoldout("equip_type"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                if (_systemSettingDataModel.equipTypes.Count >= 99) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1276")), false,
                    _typeHierarchy.CreateEquipmentType);
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1277")), false,
                    () =>
                    {
                        _typeHierarchy.DuplicateEquipmentType(_systemSettingDataModel.equipTypes[_equipmentIndex]);
                    });
                menu.ShowAsContext();
            });
            _equipmentTypeListView.SetEventHandler(
                //属性表示時、「なし」「通常攻撃」を除外して表示する(前から二つを非表示)
                (i, value) => { _typeHierarchy.OpenEquipmentTypeInspector(_systemSettingDataModel.equipTypes[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    if (_systemSettingDataModel.equipTypes.Count < 99)
                        menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1278")), false,
                            () => { _equipmentIndex = i; });
                    //4つ目までは、削除不可
                    if (i > 3)
                        menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1279")), false,
                            () => { _typeHierarchy.DeleteEquipmentType(_systemSettingDataModel.equipTypes[i]); });

                    menu.ShowAsContext();
                });
        }

        /// <summary>
        /// データ更新
        /// </summary>
        /// <param name="systemSettingDataModel"></param>
        public void Refresh([CanBeNull] SystemSettingDataModel systemSettingDataModel = null) {
            _systemSettingDataModel = systemSettingDataModel ?? _systemSettingDataModel;
            base.Refresh();
        }

        /// <summary>
        /// データ更新
        /// </summary>
        protected override void RefreshContents() {
            base.RefreshContents();
            //属性表示時、「なし」「通常攻撃」を除外して表示する(前から二つを非表示)
            var elementViewList = new List<SystemSettingDataModel.Element>();
            for (var i = 2; i < _systemSettingDataModel.elements.Count; i++)
                elementViewList.Add(_systemSettingDataModel.elements[i]);


            _attributeTypeListView.Refresh(elementViewList.Select(item => item.value).ToList());
            _skillTypeListView.Refresh(_systemSettingDataModel.skillTypes.Select(item => item.value).ToList());
            _weaponTypeListView.Refresh(_systemSettingDataModel.weaponTypes.Select(item => item.value).ToList());
            _armorTypeListView.Refresh(_systemSettingDataModel.armorTypes.Select(item => item.name).ToList());
            _equipmentTypeListView.Refresh(_systemSettingDataModel.equipTypes.Select(item => item.name).ToList());
        }

        /// <summary>
        /// 最終選択していた属性を返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastAttributeTypeIndex() {
            var elements = new List<VisualElement>();
            _attributeTypeListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }

        /// <summary>
        /// 最終選択していたスキルタイプを返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastSkillTypeIndex() {
            var elements = new List<VisualElement>();
            _skillTypeListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }

        /// <summary>
        /// 最終選択していた武器タイプを返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastWeaponTypeIndex() {
            var elements = new List<VisualElement>();
            _weaponTypeListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }

        /// <summary>
        /// 最終選択していた防具タイプを返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastArmorTypeIndex() {
            var elements = new List<VisualElement>();
            _armorTypeListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }

        /// <summary>
        /// 最終選択していた装備タイプを返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastEquipmentTypeIndex() {
            var elements = new List<VisualElement>();
            _equipmentTypeListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }
    }
}