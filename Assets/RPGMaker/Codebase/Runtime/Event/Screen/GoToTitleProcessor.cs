
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Runtime.Common;
using RPGMaker.Codebase.Runtime.Map;
using UnityEngine.SceneManagement;

namespace RPGMaker.Codebase.Runtime.Event.Screen
{
    public class GoToTitleProcessor : AbstractEventCommandProcessor
    {
        protected override void Process(string eventId, EventDataModel.EventCommand command) {
            if (GameStateHandler.IsMap())
                //メニューの非表示
                MapManager.menu.MenuClose(false);
            //画面をフェードアウトする
            HudDistributor.Instance.NowHudHandler().DisplayInit();
            HudDistributor.Instance.NowHudHandler().FadeOut(GoTitle, UnityEngine.Color.black);
        }

        //タイトルに戻る
        private void GoTitle() {
            //タイトルシーンへ
            SceneManager.LoadScene("Title");
            //次のイベントへ
            ProcessEndAction();
        }

        private void ProcessEndAction() {
            SendBackToLauncher.Invoke();
        }
    }
}