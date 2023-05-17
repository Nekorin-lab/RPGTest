using System.Collections.Generic;
using System.Linq;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.EventMap;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Map;
using RPGMaker.Codebase.CoreSystem.Service.EventManagement;
using RPGMaker.Codebase.CoreSystem.Service.MapManagement;
using RPGMaker.Codebase.Editor.Common.View;
using RPGMaker.Codebase.Editor.Hierarchy.Common;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.Hierarchy.Region.Outline.View.Component
{
    /// <summary>
    ///     アウトラインのチャプターとセクション用のマップヒエラルキー情報クラス。
    /// </summary>
    public class OutlineMapHierarchyInfo : IMapHierarchyInfo
    {
        private List<MapDataModel> _mapDataModels;

        public OutlineMapHierarchyInfo(VisualElement parentVe, string name, AbstractHierarchyView hierarchyView) {
            ParentVe = parentVe;
            Name = name;
            ParentClass = hierarchyView;
        }

        public VisualElement ParentVe { get; }
        public string Name { get; }

        public Dictionary<string, Foldout> MapFoldouts { get; } = new Dictionary<string, Foldout>();
        public Dictionary<string, Foldout> EventFoldouts { get; } = new Dictionary<string, Foldout>();

        public ExecEventType ExecEventType { get; set; } = ExecEventType.None;

        public List<EventMapDataModel> EventMapDataModels { get; private set; }

        public AbstractHierarchyView ParentClass { get; }

        public void RefreshMapHierarchy(string[] mapIds = null) {
            _mapDataModels =
                Editor.Hierarchy.Hierarchy.mapManagementService.LoadMaps()
                    .Where(mapDataModel => mapIds != null && mapIds.Contains(mapDataModel.id)).ToList();
            RefreshEventHierarchy();
        }

        public void RefreshEventHierarchy(string updateData = null) {
            EventMapDataModels = new EventManagementService().LoadEventMap();
            ParentVe.Clear();
            foreach (var mapDataModel in _mapDataModels) CommonMapHierarchyView.AddMapFoldout(mapDataModel, this);
        }
    }
}