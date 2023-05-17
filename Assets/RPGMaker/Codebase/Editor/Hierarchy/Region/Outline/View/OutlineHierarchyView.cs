using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.EventMap;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Map;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Outline;
using RPGMaker.Codebase.Editor.Common;
using RPGMaker.Codebase.Editor.Common.View;
using RPGMaker.Codebase.Editor.Hierarchy.Region.Outline.View.Component;
using RPGMaker.Codebase.Editor.OutlineEditor.Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.Hierarchy.Region.Outline.View
{
    public class OutlineHierarchyView : AbstractHierarchyView
    {
        protected override string MainUxml { get { return ""; } }
        private List<EventMapDataModel> _eventMapDataModels;
        private List<MapDataModel>      _mapDataModels;

        // 利用するデータ
        //--------------------------------------------------------------------------------------------------------------
        private OutlineDataModel _outlineDataModel;
        // const
        //--------------------------------------------------------------------------------------------------------------

        // ヒエラルキー本体クラス
        //--------------------------------------------------------------------------------------------------------------
        private OutlineHierarchy _outlineHierarchy;

        // 状態
        //--------------------------------------------------------------------------------------------------------------

        // UI要素
        //--------------------------------------------------------------------------------------------------------------
        private Button _searchEventButton;

        //--------------------------------------------------------------------------------------------------------------
        //
        // methods
        //
        //--------------------------------------------------------------------------------------------------------------

        // 初期化・更新系
        //--------------------------------------------------------------------------------------------------------------
        public OutlineHierarchyView(OutlineHierarchy outlineHierarchy) {
            _outlineHierarchy = outlineHierarchy;
            InitUI();
        }

        public Dictionary<string, ChapterFoldout> ChapterFoldoutsByDataModelId { get; private set; }

        /// <summary>
        /// 各コンテンツデータの初期化
        /// </summary>
        override protected void InitContentsData() {
            AddToClassList("AnalyticsTag__page_view__outline");

            //イベント検索のボタン
            _searchEventButton = new Button {text = EditorLocalize.LocalizeText("WORD_1484")};
            _searchEventButton.AddToClassList("button-transparent");
            _searchEventButton.AddToClassList("AnalyticsTag__page_view__event_search");
            Add(_searchEventButton);

            ChapterFoldoutsByDataModelId = new Dictionary<string, ChapterFoldout>();
            InitEventHandlers();
        }

        /// <summary>
        /// 各コンテンツデータの初期化
        /// </summary>
        private void InitEventHandlers() {
            Editor.Hierarchy.Hierarchy.AddSelectableElementAndAction(_searchEventButton,
                Inspector.Inspector.SearchEventView);
            _searchEventButton.clicked += () =>
            {
                Editor.Hierarchy.Hierarchy.InvokeSelectableElementAction(_searchEventButton);
            };
        }

        /// <summary>
        /// データ更新
        /// </summary>
        /// <param name="outlineDataModel"></param>
        /// <param name="mapDataModels"></param>
        /// <param name="eventMapDataModels"></param>
        public void Refresh(
            [CanBeNull] OutlineDataModel outlineDataModel = null,
            [CanBeNull] List<MapDataModel> mapDataModels = null,
            [CanBeNull] List<EventMapDataModel> eventMapDataModels = null
        ) {
            _outlineDataModel = outlineDataModel ?? _outlineDataModel;
            _mapDataModels = mapDataModels ?? _mapDataModels;
            _eventMapDataModels = eventMapDataModels ?? _eventMapDataModels;
            base.Refresh();
        }

        /// <summary>
        /// データ更新
        /// </summary>
        protected override void RefreshContents() {
            base.RefreshContents();
            // チャプター一覧の更新・生成
            foreach (var chapterDataModel in _outlineDataModel.Chapters)
            {
                var sectionDataModels =
                    _outlineDataModel.Sections.FindAll(item => item.ChapterID == chapterDataModel.ID);
                if (!ChapterFoldoutsByDataModelId.ContainsKey(chapterDataModel.ID))
                {
                    // Foldoutがまだ存在しない場合
                    var chapterFoldout = new ChapterFoldout(
                        chapterDataModel,
                        sectionDataModels,
                        _mapDataModels,
                        _eventMapDataModels
                    );
                    ChapterFoldoutsByDataModelId.Add(chapterDataModel.ID, chapterFoldout);
                    Add(chapterFoldout);
                }

                ChapterFoldoutsByDataModelId[chapterDataModel.ID]
                    .Refresh(chapterDataModel, sectionDataModels, _mapDataModels, _eventMapDataModels);
            }

            // 削除されたチャプターがあればそのFoldoutを削除
            var deleteChapterIds = new HashSet<string>();
            foreach (var chapterId in ChapterFoldoutsByDataModelId.Keys)
            {
                if (_outlineDataModel.Chapters.Select(item => item.ID).Contains(chapterId)) continue;

                Remove(ChapterFoldoutsByDataModelId[chapterId]);
                deleteChapterIds.Add(chapterId);
            }

            foreach (var chapterId in deleteChapterIds) ChapterFoldoutsByDataModelId.Remove(chapterId);
        }

        /// <summary>
        /// アウトラインFoldoutのコンテキストメニュー。
        /// </summary>
        /// <param name="parebtVe"></param>
        public static void AddContextMenu(VisualElement parebtVe) {
            BaseClickHandler.ClickEvent(parebtVe, evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;

                var menu = new GenericMenu();

                menu.AddItem(
                    new GUIContent(EditorLocalize.LocalizeText("WORD_0002")),
                    false,
                    () => { OutlineEditor.OutlineEditor.AddNewChapterNode(); });

                menu.AddItem(
                    CopyPasteDataUtil.GetLastCopiedNode<ChapterNodeModel>() == null,
                    new GUIContent(EditorLocalize.LocalizeText("WORD_0003")),
                    false,
                    () =>
                    {
                        // GraphViewのCopyPasteDataから貼り付け。
                        OutlineEditor.OutlineEditor.GetOeGraphView().CallPasteCallback();
                    });

                menu.ShowAsContext();
            });
        }
    }
}