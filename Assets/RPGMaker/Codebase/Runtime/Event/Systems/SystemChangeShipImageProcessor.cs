using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Runtime.Map;
using RPGMaker.Codebase.Runtime.Map.Component.Character;

//バトルでも利用するが、直接マップ側を編集することで良い

namespace RPGMaker.Codebase.Runtime.Event.Systems
{
    public class SystemChangeShipImageProcessor : AbstractEventCommandProcessor
    {
        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            if (string.IsNullOrEmpty(command.parameters[0]))
            {
                ProcessEndAction();
                return;
            }
            
            //画像の更新更新
            MapManager.GetVehicleGameObject(command.parameters[0]).GetComponent<VehicleOnMap>()
                .ReloadCharacterImage(command.parameters[1]);
            //次へ
            ProcessEndAction();
        }

        private void ProcessEndAction() {
            SendBackToLauncher.Invoke();
        }
    }
}