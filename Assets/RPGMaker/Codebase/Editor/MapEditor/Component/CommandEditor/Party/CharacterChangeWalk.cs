using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Editor.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.MapEditor.Component.CommandEditor.Party
{
    public class CharacterChangeWalk : AbstractCommandEditor
    {
        private const string SettingUxml =
            "Assets/RPGMaker/Codebase/Editor/MapEditor/Asset/Uxml/EventCommand/inspector_mapEvent_character_change_walk.uxml";


        public CharacterChangeWalk(
            VisualElement rootElement,
            List<EventDataModel> eventDataModels,
            int eventIndex,
            int eventCommandIndex
        )
            : base(rootElement, eventDataModels, eventIndex, eventCommandIndex) {
        }

        public override void Invoke() {
            var commandTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(SettingUxml);
            VisualElement commandFromUxml = commandTree.CloneTree();
            EditorLocalize.LocalizeElements(commandFromUxml);
            commandFromUxml.style.flexGrow = 1;
            RootElement.Add(commandFromUxml);

            UnityEditorWrapper.AssetDatabaseWrapper.Refresh();


            if (EventDataModels[EventIndex].eventCommands[EventCommandIndex].parameters.Count == 0)
            {
                EventDataModels[EventIndex].eventCommands[EventCommandIndex].parameters.Add("0");
                Save(EventDataModels[EventIndex]);
                UnityEditorWrapper.AssetDatabaseWrapper.Refresh();
            }


            RadioButton on = RootElement.Q<VisualElement>("command_formationWalkingChange").Query<RadioButton>("radioButton-eventCommand-display23");
            RadioButton off = RootElement.Q<VisualElement>("command_formationWalkingChange").Query<RadioButton>("radioButton-eventCommand-display24");
            if (EventDataModels[EventIndex].eventCommands[EventCommandIndex].parameters[0] == "0")
                on.value = true;
            else
                off.value = true;
            
            var defaultSelect = EventDataModels[EventIndex].eventCommands[EventCommandIndex].parameters[0] == "0" ? 0 : 1;
            new CommonToggleSelector().SetRadioSelector(
                new List<RadioButton> {on, off},
                defaultSelect, new List<System.Action>
                {
                    //ON
                    () =>
                    {
                        EventDataModels[EventIndex].eventCommands[EventCommandIndex].parameters[0] = "0";
                        Save(EventDataModels[EventIndex]);
                    },
                    //OFF
                    () =>
                    {
                        EventDataModels[EventIndex].eventCommands[EventCommandIndex].parameters[0] = "1";
                        Save(EventDataModels[EventIndex]);
                    }
                });
        }
    }
}