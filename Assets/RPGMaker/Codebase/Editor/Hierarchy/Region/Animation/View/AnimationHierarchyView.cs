using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Animation;
using RPGMaker.Codebase.Editor.Common;
using RPGMaker.Codebase.Editor.Common.View;
using RPGMaker.Codebase.Editor.Hierarchy.Region.Base.View.Component;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.Hierarchy.Region.Animation.View
{
    /// <summary>
    /// アニメーションのHierarchyView
    /// </summary>
    public class AnimationHierarchyView : AbstractHierarchyView
    {
        // const
        //--------------------------------------------------------------------------------------------------------------
        protected override string MainUxml { get { return "Assets/RPGMaker/Codebase/Editor/Hierarchy/Region/Animation/Asset/database_animation.uxml"; } }

        // ヒエラルキー本体クラス
        //--------------------------------------------------------------------------------------------------------------
        private readonly AnimationHierarchy _animationHierarchy;
        private Button _addAnimationButton;

        // 利用するデータ
        //--------------------------------------------------------------------------------------------------------------
        private List<AnimationDataModel> _animationDataModels;
        private VisualElement            _animationListContainer;
        private HierarchyItemListView    _animationListView;

        // コピー時の保持データINDEX
        //--------------------------------------------------------------------------------------------------------------
        private int _index;

        //--------------------------------------------------------------------------------------------------------------
        //
        // methods
        //
        //--------------------------------------------------------------------------------------------------------------

        // 初期化・更新系
        //--------------------------------------------------------------------------------------------------------------
        public AnimationHierarchyView(AnimationHierarchy animationHierarchy) {
            _animationHierarchy = animationHierarchy;
            InitUI();
        }

        /// <summary>
        /// 各コンテンツデータの初期化
        /// </summary>
        override protected void InitContentsData() {
            // 新規作成ボタン初期化
            _addAnimationButton = new Button
            {
                text = EditorLocalize.LocalizeText("WORD_1349")
            };
            _addAnimationButton.RegisterCallback<ClickEvent>(evt => { CreateItem(); });

            SetFoldout("battle_effect_foldout");
            BaseClickHandler.ClickEvent(GetFoldout("battle_effect_foldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1349")), false, CreateItem);
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1463")), false, PasteItem);
                menu.ShowAsContext();
            });

            // リストコンテナ初期化
            _animationListContainer = UxmlElement.Query<VisualElement>("animation_list");

            // リストインスタンス初期化
            _animationListView = new HierarchyItemListView(ViewName);
            _animationListContainer.Add(_animationListView);

            // uxml全体をrootに設置
            Add(UxmlElement);

            InitEventHandlers();
        }

        /// <summary>
        /// イベントの初期設定
        /// </summary>
        private void InitEventHandlers() {
            _animationListView.SetEventHandler(OnClickItem, OnRightClickItem);
        }

        /// <summary>
        /// データ更新
        /// </summary>
        /// <param name="animationDataModels"></param>
        public void Refresh([CanBeNull] List<AnimationDataModel> animationDataModels = null) {
            if (animationDataModels != null)
                _animationDataModels = animationDataModels;
            base.Refresh();
        }

        /// <summary>
        /// データ更新
        /// </summary>
        protected override void RefreshContents() {
            base.RefreshContents();
            var particleNames = _animationDataModels.Where(item => item.id != "54b168ea-5141-48ed-9e42-4336ac58755c").Select(item => item.particleName).ToList();
            _animationListView.Refresh(particleNames);
        }

        // イベントハンドラ
        //--------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// アニメーションデータの新規作成
        /// </summary>
        private void CreateItem() {
            _animationHierarchy.CreateAnimationDataModel();
        }

        /// <summary>
        /// アニメーションデータのコピー＆貼り付け処理
        /// </summary>
        private void PasteItem() {
            _animationHierarchy.DuplicateAnimationDataModel(_animationDataModels[_index]);
        }

        /// <summary>
        /// アニメーションデータのクリック時イベント
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        private void OnClickItem(int index, string value) {
            Inspector.Inspector.AnimEditView(index);
        }

        /// <summary>
        /// アニメーションデータの右クリック
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        private void OnRightClickItem(int index, string value) {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0383")), false,
                () => { _animationHierarchy.DeleteAnimationDataModel(_animationDataModels[index]); });
            menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1462")), false, () => { _index = index; });
            menu.ShowAsContext();
        }

        /// <summary>
        /// 最終選択していたアニメーションデータ返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastAnimationIndex() {
            var elements = new List<VisualElement>();
            _animationListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }
    }
}