using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Runtime.Map;

//バトルでは本コマンドは利用しない


namespace RPGMaker.Codebase.Runtime.Event.Screen
{
    public class OpenMenuWindowProcessor : AbstractEventCommandProcessor
    {
        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            //一時的にイベントを中断し、メニューへ遷移する
            MapEventExecutionController.Instance.PauseEvent(ProcessEndAction);
            MapManager.menu.MenuOpen(true);
        }

        private void ProcessEndAction() {
            SendBackToLauncher.Invoke();
        }
    }
}