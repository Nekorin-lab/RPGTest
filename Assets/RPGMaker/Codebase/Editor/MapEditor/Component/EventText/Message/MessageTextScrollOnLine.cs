#define USE_LONG_TEXT_FIELD

using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
#if USE_LONG_TEXT_FIELD
using RPGMaker.Codebase.Editor.Common.View;
#endif
using System.Linq;
using UnityEngine.UIElements;

namespace RPGMaker.Codebase.Editor.MapEditor.Component.EventText.Message
{
    public class MessageTextScrollOnLine : AbstractEventText, IEventCommandView
    {
#if USE_LONG_TEXT_FIELD
        private readonly LongTextField _textField = new();
#else
        private readonly ImTextField _textField = new();
#endif

        public override VisualElement Invoke(string indent, EventDataModel.EventCommand eventCommand) {
            ret = indent;
            Element.style.flexDirection = FlexDirection.Row;
            Element.style.alignItems = Align.Center;

            ret += "◇#";
            LabelElement.text = ret;
            _textField.multiline = true;
            _textField.style.width = 400;

            _textField.value = eventCommand.parameters[0];
            _textField.RegisterCallback<FocusOutEvent>(o =>
            {
                ExecutionContentsWindow.IsSaveWait = true;
                var eventDataModels = ExecutionContentsWindow.EventDataModels;
                var dataModelsIndex = int.Parse(eventCommand.parameters[1]);

                var data = eventDataModels[dataModelsIndex].eventCommands.FirstOrDefault(c =>
                    c.code == eventCommand.code &&
                    c.parameters[0] == eventCommand.parameters[0] &&
                    c.parameters[1] == eventCommand.parameters[1]);

                var index = eventDataModels[dataModelsIndex].eventCommands.IndexOf(data);
                eventCommand.parameters[0] = _textField.value;
                eventDataModels[dataModelsIndex].eventCommands[index].parameters[0] = _textField.value;
                //EventManagementService.SaveEvent(eventDataModels[dataModelsIndex]);
                ExecutionContentsWindow.SetUpData();
            });

            Element.Add(LabelElement);
            Element.Add(_textField);
            return Element;
        }
    }
}