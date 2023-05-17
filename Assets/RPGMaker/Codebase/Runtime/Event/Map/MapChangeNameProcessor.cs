using RPGMaker.Codebase.Runtime.Common;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Runtime;

namespace RPGMaker.Codebase.Runtime.Event.Map
{
    /// <summary>
    ///     マップ名の表示/非表示
    /// </summary>
    public class MapChangeNameProcessor : AbstractEventCommandProcessor
    {
        private RuntimePlayerDataModel _runtimePlayerDataModel;

        protected override void Process(string eventId, EventDataModel.EventCommand command) {
            //表示切り替え
            DataManager.Self().GetRuntimeSaveDataModel().runtimePlayerDataModel.map.nameDisplay =
                int.Parse(command.parameters[0]);
            //マップ名の表示
            HudDistributor.Instance.NowHudHandler().PlayChangeMapName();

            ProcessEndAction();
        }

        private void ProcessEndAction() {
            SendBackToLauncher.Invoke();
        }
    }
}