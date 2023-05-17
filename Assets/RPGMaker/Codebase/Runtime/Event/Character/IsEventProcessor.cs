using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Runtime.Map;

//バトルでは本コマンドは利用しない

namespace RPGMaker.Codebase.Runtime.Event.Character
{
    public class IsEventProcessor : AbstractEventCommandProcessor
    {
        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            var eventOnMap = MapEventExecutionController.Instance.GetEventOnMap(eventID);
            if (eventOnMap != null)
            {
                //一時削除のフラグON
                eventOnMap.MapDataModelEvent.temporaryErase = 1;
            }

            ProcessEndAction();
        }

        private void ProcessEndAction() {
            SendBackToLauncher.Invoke();
        }
    }
}