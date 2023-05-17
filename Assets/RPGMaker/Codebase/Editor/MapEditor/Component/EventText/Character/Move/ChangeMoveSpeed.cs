using System.Collections.Generic;
using System.Linq;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Editor.Common;
using RPGMaker.Codebase.Runtime.Event.Character;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.MapEditor.Component.EventText.Character.Move
{
    public class ChangeMoveSpeed : AbstractEventText, IEventCommandView
    {
        public override VisualElement Invoke(string indent, EventDataModel.EventCommand eventCommand) {
            ret = indent;
            ret += EditorLocalize.LocalizeText("WORD_1514") + EditorLocalize.LocalizeText("WORD_1547") + " : ";
            var index = int.Parse(eventCommand.parameters[0]);
            var speedName = EditorLocalize.LocalizeText(CommandEditor.Character.Move.ChangeMoveSpeed._speedNameList[index]);
            ret += speedName;

            LabelElement.text = ret;
            Element.Add(LabelElement);
            return Element;
        }

    }
}
