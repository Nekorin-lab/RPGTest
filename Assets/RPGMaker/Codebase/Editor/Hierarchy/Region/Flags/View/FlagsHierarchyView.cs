using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Flag;
using RPGMaker.Codebase.Editor.Common;
using RPGMaker.Codebase.Editor.Common.View;
using RPGMaker.Codebase.Editor.Hierarchy.Region.Flags.View.Component;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.Hierarchy.Region.Flags.View
{
    /// <summary>
    /// スイッチのHierarchyView
    /// </summary>
    public class FlagsHierarchyView : AbstractHierarchyView
    {
        // const
        //--------------------------------------------------------------------------------------------------------------
        protected override string MainUxml { get { return "Assets/RPGMaker/Codebase/Editor/Hierarchy/Region/Flags/Asset/database_flags.uxml"; } }

        private const int GroupingUnit = 20;

        // ヒエラルキー本体クラス
        //--------------------------------------------------------------------------------------------------------------
        private readonly FlagsHierarchy _flagsHierarchy;
        private List<List<FlagDataModel.Switch>> _chunkedSwitchLists; // GroupingUnit数で分割されたスイッチリスト
        private List<List<FlagDataModel.Variable>> _chunkedVariableLists; // 同上の変数リスト

        // 利用するデータ
        //--------------------------------------------------------------------------------------------------------------
        private FlagDataModel _flagDataModel;
        private VisualElement _switchFoldoutsContainer;
        private List<SwitchListView> _switchListViews;

        // 状態
        //--------------------------------------------------------------------------------------------------------------

        // UI要素
        //--------------------------------------------------------------------------------------------------------------
        private VisualElement _variableFoldoutsContainer;
        private List<VariableListView> _variableListViews;

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
        /// <param name="flagsHierarchy"></param>
        public FlagsHierarchyView(FlagsHierarchy flagsHierarchy) {
            _flagsHierarchy = flagsHierarchy;
            _chunkedSwitchLists = new List<List<FlagDataModel.Switch>>();
            _chunkedVariableLists = new List<List<FlagDataModel.Variable>>();
            InitUI();
        }

        /// <summary>
        /// 各コンテンツデータの初期化
        /// </summary>
        override protected void InitContentsData() {
            SetFoldout("switchSettingFoldout");
            SetFoldout("variableSettingFoldout");

            // スイッチリスト
            _switchFoldoutsContainer = UxmlElement.Query<VisualElement>("switch_area");
            _switchListViews = new List<SwitchListView>();
            var chunkIndex = 0;
            foreach (var switchList in _chunkedSwitchLists)
            {
                var switchFoldout = new Foldout();
                switchFoldout.name = "switch_" + chunkIndex;
                switchFoldout.text = $"[{chunkIndex * GroupingUnit + 1:0000} - {chunkIndex * GroupingUnit + GroupingUnit:0000}]";

                var switchListView =
                    new SwitchListView(null, SelectionType.Single, chunkIndex * GroupingUnit, ViewName);
                _switchListViews.Add(switchListView);

                switchFoldout.Add(switchListView);
                _switchFoldoutsContainer.Add(switchFoldout);

                SetFoldout(switchFoldout.name);
                chunkIndex++;
            }

            // 変数リスト
            _variableFoldoutsContainer = UxmlElement.Query<VisualElement>("variable_area");
            _variableListViews = new List<VariableListView>();
            chunkIndex = 0;
            foreach (var variableList in _chunkedVariableLists)
            {
                var variableFoldout = new Foldout();
                variableFoldout.name = "variables_" + chunkIndex;
                variableFoldout.text = $"[{chunkIndex * GroupingUnit + 1:0000} - {chunkIndex * GroupingUnit + GroupingUnit:0000}]";

                var variableListView = new VariableListView(
                    null,
                    SelectionType.Single,
                    chunkIndex * GroupingUnit,
                    ViewName
                );
                _variableListViews.Add(variableListView);

                variableFoldout.Add(variableListView);
                _variableFoldoutsContainer.Add(variableFoldout);

                SetFoldout(variableFoldout.name);
                chunkIndex++;
            }

            InitEventHandlers();
        }

        /// <summary>
        /// イベントの初期設定
        /// </summary>
        private void InitEventHandlers() {
            // スイッチの設定Foldout 右クリック設定
            BaseClickHandler.ClickEvent(GetFoldout("switchSettingFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1520")), false, _flagsHierarchy.CreateSwitch);

                bool isDelete = _switchListViews.Count == 1 && _switchListViews[0].childCount == 1;

                if(!isDelete)
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1521")), false,
                        _flagsHierarchy.DeleteSwitchAtTail);
                

                menu.ShowAsContext();
            });

            // スイッチのリストビュークリック設定
            var switchListIndex = 0;
            foreach (var switchListView in _switchListViews)
            {
                switchListView.SetEventHandler(
                    sw => { _flagsHierarchy.OpenSwitchInspector(sw); },
                    sw =>
                    {
                        // 右クリックは何もなし
                    });
                switchListIndex++;
            }

            // 変数の設定Foldoutクリック設定
            BaseClickHandler.ClickEvent(GetFoldout("variableSettingFoldout"), evt =>
            {
                if (evt != (int) MouseButton.RightMouse) return;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1520")), false, _flagsHierarchy.CreateVariable);
                bool isDelete = _variableListViews.Count == 1 && _variableListViews[0].childCount == 1;
                if (!isDelete)
                {
                    menu.AddItem(new GUIContent(EditorLocalize.LocalizeText("WORD_1521")), false,
                        _flagsHierarchy.DeleteVariableAtTail);
                }

                menu.ShowAsContext();
            });

            // 変数のリストビュークリック設定
            foreach (var variableListView in _variableListViews)
                variableListView.SetEventHandler(
                    variable => { _flagsHierarchy.OpenVariableInspector(variable); },
                    variable =>
                    {
                        // 右クリックは何もなし
                    });
        }

        /// <summary>
        /// データ更新
        /// </summary>
        /// <param name="flagDataModel"></param>
        public void Refresh([CanBeNull] FlagDataModel flagDataModel = null) {
            if (flagDataModel != null)
                _flagDataModel = flagDataModel;
            base.Refresh();
        }

        /// <summary>
        /// データ更新
        /// </summary>
        protected override void RefreshContents() {
            base.RefreshContents();
            _chunkedSwitchLists = Chunk(_flagDataModel.switches, GroupingUnit);
            _chunkedVariableLists = Chunk(_flagDataModel.variables, GroupingUnit);

            RefreshSwitchLists();
            RefreshVariableLists();
        }

        /// <summary>
        /// スイッチ一覧更新
        /// </summary>
        private void RefreshSwitchLists() {
            // スイッチ一覧をリフレッシュ
            var switchChunkIndex = 0;
            foreach (var switchList in _chunkedSwitchLists)
            {
                var targetListView = _switchListViews.ElementAtOrDefault(switchChunkIndex);
                if (targetListView == null)
                {
                    // スイッチが追加され、新たなリストビューが必要になった場合の追加処理
                    targetListView = new SwitchListView(null, SelectionType.Single, switchChunkIndex * GroupingUnit,
                        ViewName);
                    targetListView.SetEventHandler(
                        _flagsHierarchy.OpenSwitchInspector,
                        sw => { });
                    _switchListViews.Add(targetListView);

                    var newSwitchFoldout = new Foldout();
                    newSwitchFoldout.name = "switch_" + switchChunkIndex;
                    newSwitchFoldout.Add(targetListView);

                    //各項目を20個づつ振り分けて置くときの項目名の表示
                    newSwitchFoldout.text =
                        $"[{switchChunkIndex * GroupingUnit + 1:0000} - {switchChunkIndex * GroupingUnit + GroupingUnit:0000}]";

                    _switchFoldoutsContainer.Add(newSwitchFoldout);
                    SetFoldout(newSwitchFoldout.name);
                }

                targetListView.Refresh(switchList);

                switchChunkIndex++;
            }

            if (_switchListViews.Count > switchChunkIndex)
                // スイッチが削除され、当該リストビューが不要になった場合の削除処理
                for (var i = _switchListViews.Count - 1; i >= switchChunkIndex; i--)
                {
                    for (var j = _switchListViews[i].childCount - 1; j >= 0; j--) _switchListViews[i].RemoveAt(j);
                    for (var j = _switchFoldoutsContainer[i].childCount - 1; j >= 0; j--)
                        _switchFoldoutsContainer[i].RemoveAt(j);
                    _switchListViews.RemoveAt(i);
                    _switchFoldoutsContainer.RemoveAt(i);
                }
        }

        /// <summary>
        /// 変数一覧更新
        /// </summary>
        private void RefreshVariableLists() {
            // 変数一覧をリフレッシュ
            var variableChunkIndex = 0;
            foreach (var variableList in _chunkedVariableLists)
            {
                var targetListView = _variableListViews.ElementAtOrDefault(variableChunkIndex);
                if (targetListView == null)
                {
                    // 変数が追加され、新たなリストビューが必要になった場合の追加処理
                    targetListView =
                        new VariableListView(null, SelectionType.Single, variableChunkIndex * GroupingUnit, ViewName);
                    targetListView.SetEventHandler(
                        _flagsHierarchy.OpenVariableInspector,
                        variable => { });
                    _variableListViews.Add(targetListView);

                    var newvariableFoldout = new Foldout();
                    newvariableFoldout.name = "variables_" + variableChunkIndex;
                    newvariableFoldout.Add(targetListView);

                    //各項目を20個づつ振り分けて置くときの項目名の表示
                    newvariableFoldout.text =
                        $"[{variableChunkIndex * GroupingUnit + 1:0000} - {variableChunkIndex * GroupingUnit + GroupingUnit:0000}]";

                    _variableFoldoutsContainer.Add(newvariableFoldout);
                    SetFoldout(newvariableFoldout.name);
                }

                targetListView.Refresh(variableList);

                variableChunkIndex++;
            }

            if (_variableListViews.Count > variableChunkIndex)
                // 変数が削除され、当該リストビューが不要になった場合の削除処理
                for (var i = _variableListViews.Count - 1; i >= variableChunkIndex; i--)
                {
                    for (var j = _variableListViews[i].childCount - 1; j >= 0; j--) _variableListViews[i].RemoveAt(j);
                    for (var j = _variableFoldoutsContainer[i].childCount - 1; j >= 0; j--)
                        _variableFoldoutsContainer[i].RemoveAt(j);
                    _variableListViews.RemoveAt(i);
                    _variableFoldoutsContainer.RemoveAt(i);
                }
        }

        private static List<List<T>> Chunk<T>(IEnumerable<T> original, int unit) {
            return original.Select((v, i) => new {v, i})
                .GroupBy(x => x.i / unit)
                .Select(g => g.Select(x => x.v).ToList()).ToList();
        }
        
        /// <summary>
        /// 最終選択していたスイッチを返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastSwitchesIndex() {
            var elements = new List<VisualElement>();
            _switchFoldoutsContainer.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }

        /// <summary>
        /// 最終選択していた変数を返却
        /// </summary>
        /// <returns></returns>
        public VisualElement LastVariableIndex() {
            var elements = new List<VisualElement>();
            _variableFoldoutsContainer.Query<Button>().ForEach(button => { elements.Add(button); });

            return elements[elements.Count - 1];
        }
    }
}