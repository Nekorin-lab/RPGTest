using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Runtime.Common;
using RPGMaker.Codebase.Runtime.Map;

//バトルでは本コマンドは利用しない

//using RPGMaker.Codebase.CoreSystem.Helper;

namespace RPGMaker.Codebase.Runtime.Event.Screen
{
    public class OpenSaveWindowProcessor : AbstractEventCommandProcessor
    {
        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            //一時的にイベントを中断し、メニューへ遷移する
            MapEventExecutionController.Instance.PauseEvent(ProcessEndAction);
            //最初にセーブのWindowをオープンさせる
            MenuBase.ObjName = "_saveObject";
            MenuBase.IsEventToSave = true;
            MapManager.menu.SaveOpenToEvent();
        }

        private void ProcessEndAction() {
            SendBackToLauncher.Invoke();
        }
    }
}