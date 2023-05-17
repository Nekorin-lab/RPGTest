using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.EventCommon;
using RPGMaker.Codebase.Editor.Common;
using RPGMaker.Codebase.Editor.Common.View;
using RPGMaker.Codebase.Editor.Hierarchy.Region.Base.View.Component;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.Hierarchy.Region.CommonEvent.View
{
    /// <summary>
    /// コモンイベントのHierarchyView
    /// </summary>
    public class CommonEventHierarchyView : AbstractHierarchyView
    {
        // const
        //--------------------------------------------------------------------------------------------------------------
        protected override string MainUxml { get { return "Assets/RPGMaker/Codebase/Editor/Hierarchy/Region/CommonEvent/Asset/database_common.uxml"; } }

        // ヒエラルキー本体クラス
        //--------------------------------------------------------------------------------------------------------------
        private readonly CommonEventHierarchy _commonEventHierarchy;

        // 利用するデータ
        //--------------------------------------------------------------------------------------------------------------
        private List<EventCommonDataModel> _eventCommonDataModels;

        // UI要素
        //--------------------------------------------------------------------------------------------------------------
        private HierarchyItemListView _eventCommonListView;
        private string _tagClassName;

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
        /// <param name="commonEventHierarchy"></param>
        public CommonEventHierarchyView(CommonEventHierarchy commonEventHierarchy) {
            _commonEventHierarchy = commonEventHierarchy;
            InitUI();
        }

        /// <summary>
        /// 各コンテンツデータの初期化
        /// </summary>
        override protected void InitContentsData() {
            SetFoldout("eventCommonFoldout");
            _eventCommonListView = new HierarchyItemListView(ViewName);
            ((VisualElement) UxmlElement.Query<VisualElement>("common_event_list")).Add(_eventCommonListView);

            InitEventHandlers();
        }

        /// <summary>
        /// イベントの初期設定
        /// </summary>
        private void InitEventHandlers() {
            EventCommonDataModel eventCommonDataModel = null;

            BaseClickHandler.ClickEvent(GetFoldout("eventCommonFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0883")), false,
                    _commonEventHierarchy.CreateEventCommonDataModel);
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0884")), false, () =>
                {
                    if (eventCommonDataModel != null)
                        _commonEventHierarchy.DuplicateEventCommonDataModel(eventCommonDataModel);
                });
                menu.ShowAsContext();
            });
            _eventCommonListView.SetEventHandler(
                (i, value) => { _commonEventHierarchy.OpenEventCommonInspector(_eventCommonDataModels[i]); },
                (i, value) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0015")), false,
                        () => { eventCommonDataModel = _eventCommonDataModels[i]; });
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_0886")), false,
                        () =>
                        {
                            eventCommonDataModel = null;
                            _commonEventHierarchy.DeleteEventCommonDataModel(_eventCommonDataModels[i]);
                        });
                    menu.ShowAsContext();
                });
        }

        /// <summary>
        /// データ更新
        /// </summary>
        /// <param name="eventCommonDataModels"></param>
        /// <param name="tagClassName"></param>
        public void Refresh(
            [CanBeNull] List<EventCommonDataModel> eventCommonDataModels = null, string tagClassName = null)
        {
            _eventCommonDataModels = eventCommonDataModels ?? _eventCommonDataModels;
            _tagClassName = tagClassName;
            base.Refresh();
        }

        /// <summary>
        /// データ更新
        /// </summary>
        protected override void RefreshContents() {
            base.RefreshContents();
            _eventCommonListView.Refresh(_eventCommonDataModels.Select(item => item.name).ToList());

            // ボタンの種類判別用に、未定義のクラス名をタグとして追加する。
            if (_tagClassName != null)
            {
                _eventCommonListView.Query<Button>().ForEach(button => { button.AddToClassList(_tagClassName); });
            }
        }

        public VisualElement LastCommonEventIndex() {
            var elements = new List<VisualElement>();
            _eventCommonListView.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }
    }
}