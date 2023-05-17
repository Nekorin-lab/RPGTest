using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.CoreSystem.Service.DatabaseManagement;

namespace RPGMaker.Codebase.Runtime.Event.Systems
{
    public class SystemShipBgmProcessor : AbstractEventCommandProcessor
    {
        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            var databaseManagementService = new DatabaseManagementService();
            var vehiclesDataModels = databaseManagementService.LoadCharacterVehicles();

            //IDから乗り物を探す
            for (var i = 0; i < vehiclesDataModels.Count; i++)
                if (vehiclesDataModels[i].id == command.parameters[0])
                {
                    //IDの一致した乗り物のデータの変更
                    vehiclesDataModels[i].images = command.parameters[1];
                    break;
                }

            //データの保存
            databaseManagementService.SaveCharacterVehicles(vehiclesDataModels);
            //次へ
            ProcessEndAction();
        }

        private void ProcessEndAction() {
            SendBackToLauncher.Invoke();
        }
    }
}