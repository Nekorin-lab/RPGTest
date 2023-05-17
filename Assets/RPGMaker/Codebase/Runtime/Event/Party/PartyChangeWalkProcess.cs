using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Runtime.Map;

//バトルでは本コマンドは利用しない

namespace RPGMaker.Codebase.Runtime.Event.Party
{
    public class PartyChangeWalkProcess : AbstractEventCommandProcessor
    {
        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            var isPlayerFollow = command.parameters[0] == "0";
            MapManager.ChangePlayerFollowers(isPlayerFollow);

            ProcessEndAction();
        }

        private void ProcessEndAction() {
            SendBackToLauncher.Invoke();
        }
    }
}