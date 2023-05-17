using System.Linq;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Editor.Common;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.MapEditor.Component.EventText.Actor
{
    public class ActorChangeName : AbstractEventText, IEventCommandView
    {
        public override VisualElement Invoke(string indent, EventDataModel.EventCommand eventCommand) {
            ret = indent;
            ret += "◆" + EditorLocalize.LocalizeText("WORD_0916") + " : ";

            var actorId = eventCommand.parameters[0];
            var characterActorDataModels = DatabaseManagementService.LoadCharacterActor();
            var actor = characterActorDataModels.FirstOrDefault(c => c.uuId == actorId);

            ret += EditorLocalize.LocalizeText("WORD_0039") + " : " + actor?.basic.name + ", ";
            ret += eventCommand.parameters[2];

            LabelElement.text = ret;
            Element.Add(LabelElement);
            return Element;
        }
    }
}