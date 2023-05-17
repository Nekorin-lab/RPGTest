using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Runtime.Common;
using UnityEngine;

namespace RPGMaker.Codebase.Runtime.Event.Systems
{
    public class SystemWindowColorProcessor : AbstractEventCommandProcessor
    {
        protected override void Process(string eventID, EventDataModel.EventCommand command) {

            var dataModel = DataManager.Self().GetRuntimeSaveDataModel().runtimeSystemConfig;
            var r = int.Parse(command.parameters[0]);
            var g = int.Parse(command.parameters[1]);
            var b = int.Parse(command.parameters[2]);
            dataModel.windowTone = new List<int>(){r,g,b};
            ProcessEndAction();
        }

        private void ProcessEndAction() {
            SendBackToLauncher.Invoke();
        }
    }
}