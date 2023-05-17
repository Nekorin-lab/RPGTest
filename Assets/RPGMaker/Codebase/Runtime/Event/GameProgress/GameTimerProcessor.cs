using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Runtime.Common;
using RPGMaker.Codebase.Runtime.Common.Component.Hud.GameProgress;
using UnityEngine;

namespace RPGMaker.Codebase.Runtime.Event.GameProgress
{
    public class GameTimerProcessor : AbstractEventCommandProcessor
    {
        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            GameTimer gameTimer = HudDistributor.Instance.NowHudHandler().CreateGameTimer();
            gameTimer.SetGameTimer(command.parameters[0] == "1" ? true : false, int.Parse(command.parameters[1]));
            ProcessEndAction();
        }

        private void ProcessEndAction() {
            SendBackToLauncher.Invoke();
        }
    }
}