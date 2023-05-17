using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Runtime.Common;

namespace RPGMaker.Codebase.Runtime.Event.AudioVideo
{
    public class BgmSaveProcessor : AbstractEventCommandProcessor
    {
        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            SoundManager.Self().Init();
            //サウンドの保存
            SoundManager.Self().SaveBgm();
            //次のイベントへ
            ProcessEndAction();
        }

        private void ProcessEndAction() {
            SendBackToLauncher.Invoke();
        }
    }
}