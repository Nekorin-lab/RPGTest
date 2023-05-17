using RPGMaker.Codebase.Runtime.Common;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;

namespace RPGMaker.Codebase.Runtime.Event.Picture
{
    public class PictureRotateProcessor : AbstractEventCommandProcessor
    {
        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            HudDistributor.Instance.NowHudHandler().PictureInit();
            HudDistributor.Instance.NowHudHandler().PlayRotation(
                int.Parse(command.parameters[0]),
                int.Parse(command.parameters[1]));
            SendBackToLauncher.Invoke();
        }
    }
}