using RPGMaker.Codebase.Runtime.Common;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;

namespace RPGMaker.Codebase.Runtime.Event.Display
{
    public class DisplayFadeInProcessor : AbstractEventCommandProcessor
    {
        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            HudDistributor.Instance.NowHudHandler().DisplayInit();
            HudDistributor.Instance.NowHudHandler().FadeIn(ProcessEndAction);
        }

        private void ProcessEndAction() {
            SendBackToLauncher.Invoke();
        }
    }
}