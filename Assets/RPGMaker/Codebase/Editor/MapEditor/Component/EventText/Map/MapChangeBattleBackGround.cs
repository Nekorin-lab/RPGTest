using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Editor.Common;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.MapEditor.Component.EventText.Map
{
    public class MapChangeBattleBackGround : AbstractEventText, IEventCommandView
    {
        public override VisualElement Invoke(string indent, EventDataModel.EventCommand eventCommand) {
            ret = indent;
            ret += "◆" + EditorLocalize.LocalizeText("WORD_1179") + " : " + eventCommand.parameters[0] + "," +
                   eventCommand.parameters[1];

            LabelElement.text = ret;
            Element.Add(LabelElement);
            return Element;
        }
    }
}