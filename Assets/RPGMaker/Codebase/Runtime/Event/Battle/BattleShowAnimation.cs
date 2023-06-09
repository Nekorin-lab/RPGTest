using System;
using System.Linq;
using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Runtime.Battle.Objects;
using RPGMaker.Codebase.Runtime.Common;

namespace RPGMaker.Codebase.Runtime.Event.Battle
{
    public class BattleShowAnimation : AbstractEventCommandProcessor
    {
        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            var memberNo = 0;
            if (int.TryParse(command.parameters[0], out memberNo))
            {
                memberNo -= 1; // 1から始まる番号で格納されているのでインデックス用に調整
                IterateEnemyIndex(memberNo, enemy =>
                {
                    if (enemy.IsAlive()) enemy.StartAnimation(command.parameters[1], false, 0);
                });
            }

            //次のイベントへ
            ProcessEndAction();
        }

        private void IterateEnemyIndex(int number, Action<GameBattler> callback) {
            if (number < 0)
            {
                DataManager.Self().GetGameTroop().Members().ForEach(callback);
            }
            else
            {
                var enemy = DataManager.Self().GetGameTroop().Members().ElementAtOrDefault(number);
                if (enemy != null) callback(enemy);
            }
        }

        private void ProcessEndAction() {
            SendBackToLauncher.Invoke();
        }
    }
}