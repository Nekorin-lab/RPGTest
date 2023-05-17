using System;
using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Outline;
using UnityEngine;

namespace RPGMaker.Codebase.Editor.OutlineEditor.Model
{
    [Serializable]
    public class SectionNodeModel : OutlineNodeModel
    {
        // application entity
        [SerializeField] public List<SwitchSubDataModel> belongingSwitches;
        [SerializeField] public string                   chapterID;

        // object properties
        [SerializeField] public string                   id;
        [SerializeField] public List<MapSubDataModel>    maps;
        [SerializeField] public string                   memo;
        [SerializeField] public string                   name;
        [SerializeField] public float                    posX;
        [SerializeField] public float                    posY;
        [SerializeField] public List<SwitchSubDataModel> referringSwitches;

        public SectionDataModel SectionDataModel { get; private set; }

        public void Init(SectionDataModel sectionDataModel) {
            SectionDataModel = sectionDataModel;
            SetPropertiesFromEntity();
        }

        public string ReplaceChapterId(string id) {
            var prevChapterID = chapterID;
            chapterID = id;
            return prevChapterID;
        }

        private void SetPropertiesFromEntity() {
            id = SectionDataModel.ID;
            chapterID = SectionDataModel.ChapterID;
            name = SectionDataModel.Name;
            maps = SectionDataModel.Maps;
            belongingSwitches = SectionDataModel.BelongingSwitches;
            referringSwitches = SectionDataModel.ReferringSwitches;
            posX = SectionDataModel.PosX;
            posY = SectionDataModel.PosY;
            memo = SectionDataModel.Memo;
        }

        public override string GetEntityID() {
            return SectionDataModel.ID;
        }

        public override void UpdateEntity() {
            base.UpdateEntity();
            SectionDataModel.ID = id;
            SectionDataModel.ChapterID = chapterID;
            SectionDataModel.Name = name;
            SectionDataModel.Maps = maps ?? new List<MapSubDataModel>();
            SectionDataModel.BelongingSwitches = belongingSwitches;
            SectionDataModel.ReferringSwitches = referringSwitches;
            SectionDataModel.PosX = posX;
            SectionDataModel.PosY = posY;
            SectionDataModel.Memo = memo;

            OutlineEditor.SaveOutline();
        }

        public override void UpdatePosition() {
            base.UpdatePosition();

            SectionDataModel.PosX = posX = Position.x;
            SectionDataModel.PosY = posY = Position.y;

            OutlineEditor.SaveOutline();
        }

        public override void SetUpToInspector() {
            base.SetUpToInspector();
            OutlineEditor.SetDataModelToInspector(SectionDataModel);
        }

        public override void RenewEntity() {
            base.RenewEntity();
            SectionDataModel = OutlineEditor.AddNewSectionDataModel(string.Empty);
            id = SectionDataModel.ID;
            UpdateEntity();
        }
    }
}