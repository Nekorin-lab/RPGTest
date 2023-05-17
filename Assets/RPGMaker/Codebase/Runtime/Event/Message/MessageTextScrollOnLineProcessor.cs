using RPGMaker.Codebase.Runtime.Common;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;

namespace RPGMaker.Codebase.Runtime.Event.Message
{
    public class MessageTextScrollOnLineProcessor : AbstractEventCommandProcessor
    {
        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            if (!HudDistributor.Instance.NowHudHandler().IsMessageScrollWindowActive())
               HudDistributor.Instance.NowHudHandler().OpenMessageScrollWindow();
            HudDistributor.Instance.NowHudHandler().SetScrollText(command.parameters[0]);
            HudDistributor.Instance.NowHudHandler().StartScroll(ProcessEndAction);
        }

        private void ProcessEndAction() {
            HudDistributor.Instance.NowHudHandler().CloseMessageScrollWindow();
            SendBackToLauncher.Invoke();
        }
    }
}