using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.SkillCustom;
using RPGMaker.Codebase.Editor.Common;
using RPGMaker.Codebase.Editor.Common.View;
using RPGMaker.Codebase.Editor.Hierarchy.Region.Base.View.Component;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.Hierarchy.Region.Skill.View
{
    /// <summary>
    /// スキルのHierarchyView
    /// </summary>
    public class SkillHierarchyView : AbstractHierarchyView
    {
        // const
        //--------------------------------------------------------------------------------------------------------------
        protected override string MainUxml { get { return "Assets/RPGMaker/Codebase/Editor/Hierarchy/Region/Skill/Asset/database_skill.uxml"; } }

        private Button _attackSkillButton;
        private VisualElement _customSkillListContainer;
        private HierarchyItemListView _customSkillListView;
        private Button _defenseSkillButton;

        private const int foldoutCount = 3;

        // 状態
        //--------------------------------------------------------------------------------------------------------------

        // UI要素
        //--------------------------------------------------------------------------------------------------------------
        private Button _skillCommonButton;

        // 利用するデータ
        //--------------------------------------------------------------------------------------------------------------
        private List<SkillCustomDataModel> _skillCustomDataModels;

        // ヒエラルキー本体クラス
        //--------------------------------------------------------------------------------------------------------------
        private readonly SkillHierarchy _skillHierarchy;

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
        /// <param name="skillHierarchy"></param>
        /// <param name="skillCustomDataModels"></param>
        public SkillHierarchyView(SkillHierarchy skillHierarchy, List<SkillCustomDataModel> skillCustomDataModels) {
            _skillHierarchy = skillHierarchy;
            _skillCustomDataModels = skillCustomDataModels;
            InitUI();
        }

        /// <summary>
        /// 各コンテンツデータの初期化
        /// </summary>
        override protected void InitContentsData() {
            _skillCommonButton = UxmlElement.Query<Button>("skill_common_button");
            _attackSkillButton = UxmlElement.Query<Button>("attack_skill_button");
            _defenseSkillButton = UxmlElement.Query<Button>("defense_skill_button");
            SetFoldout("customSkillFoldout");
            _customSkillListContainer = UxmlElement.Query<VisualElement>("skill_custom_list");
            _customSkillListView = new HierarchyItemListView(ViewName);
            _customSkillListContainer.Add(_customSkillListView);

            //Foldoutの開閉状態保持用
            for (int i = 0; i < foldoutCount; i++)
                SetFoldout("foldout_" + (i + 1));

            InitEventHandlers();
        }

        /// <summary>
        /// イベントの初期設定
        /// </summary>
        private void InitEventHandlers() {
            Editor.Hierarchy.Hierarchy.AddSelectableElementAndAction(_skillCommonButton,
                _skillHierarchy.OpenSkillCommonInspector);
            _skillCommonButton.clickable.clicked += () =>
            {
                Editor.Hierarchy.Hierarchy.InvokeSelectableElementAction(_skillCommonButton);
            };

            Editor.Hierarchy.Hierarchy.AddSelectableElementAndAction(_attackSkillButton,
                () => { _skillHierarchy.OpenSkillCustomInspector(_skillCustomDataModels[0]); });
            _attackSkillButton.clickable.clicked += () =>
            {
                Editor.Hierarchy.Hierarchy.InvokeSelectableElementAction(_attackSkillButton);
            };

            Editor.Hierarchy.Hierarchy.AddSelectableElementAndAction(_defenseSkillButton,
                () => { _skillHierarchy.OpenSkillCustomInspector(_skillCustomDataModels[1]); });
            _defenseSkillButton.clickable.clicked += () =>
            {
                Editor.Hierarchy.Hierarchy.InvokeSelectableElementAction(_defenseSkillButton);
            };

            SkillCustomDataModel skillCustomDataModel = null;

            // カスタムスキルFoldout右クリック時
            BaseClickHandler.ClickEvent(GetFoldout("customSkillFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0410")), false,
                    _skillHierarchy.CreateSkillCustomDataModel);
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0411")), false, () =>
                {
                    if (skillCustomDataModel != null)
                        _skillHierarchy.DuplicateSkillCustomDataModel(skillCustomDataModel);
                });
                menu.ShowAsContext();
            });

            _customSkillListView.SetEventHandler(
                (i, value) => { _skillHierarchy.OpenSkillCustomInspector(_skillCustomDataModels[i + 2]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0412")), false,
                        () => { skillCustomDataModel = _skillCustomDataModels[i + 2].DataClone(); });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0413")), false,
                        () =>
                        {
                            skillCustomDataModel = null;
                            _skillHierarchy.DeleteSkillCustomDataModel(_skillCustomDataModels[i + 2]);
                        });
                    menu.ShowAsContext();
                });
        }

        /// <summary>
        /// データ更新
        /// </summary>
        /// <param name="skillCustomDataModels"></param>
        public void Refresh([CanBeNull] List<SkillCustomDataModel> skillCustomDataModels = null) {
            if (skillCustomDataModels != null) _skillCustomDataModels = skillCustomDataModels;
            base.Refresh();
        }

        /// <summary>
        /// データ更新
        /// </summary>
        protected override void RefreshContents() {
            base.RefreshContents();
            _customSkillListView.Refresh(GetOptionalCustomSkillList().Select(item => item.basic.name).ToList());
        }

        /// <summary>
        /// カスタムスキル取得
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SkillCustomDataModel> GetOptionalCustomSkillList() {
            return new ArraySegment<SkillCustomDataModel>(_skillCustomDataModels.ToArray(), 2,
                _skillCustomDataModels.Count - 2);
        }

        /// <summary>
        /// 最終選択していたスキルを返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastSkillIndex() {
            var elements = new List<VisualElement>();
            _customSkillListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }
    }
}