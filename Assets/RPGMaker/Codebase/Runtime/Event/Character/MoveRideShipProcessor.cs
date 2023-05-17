using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Runtime.Map;

//バトルでは本コマンドは利用しない

namespace RPGMaker.Codebase.Runtime.Event.Character
{
    public class MoveRideShipProcessor : AbstractEventCommandProcessor
    {
        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            if (MapManager.GetRideVehicle() == null)
                MapManager.TryToRideFromThePlayerToVehicle();
            else
                MapManager.TryToGetOffFromTheVehicleToPlayer(true);
            ProcessEndAction();
        }

        private void ProcessEndAction() {
            SendBackToLauncher.Invoke();
        }
    }
}