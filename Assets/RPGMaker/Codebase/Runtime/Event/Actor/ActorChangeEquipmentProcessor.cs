using System.Linq;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Runtime.Common;

namespace RPGMaker.Codebase.Runtime.Event.Actor
{
    public class ActorChangeEquipmentProcessor : AbstractEventCommandProcessor
    {
        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            //装備変更イベントは、以下のパラメータで動作する
            //0:アクターID
            //1:装備タイプ（武器、盾、頭…）
            //2:武器または防具のID

            //対象のアクターを取得
            var actorData = DataManager.Self().GetRuntimeSaveDataModel().runtimeActorDataModels
                .FirstOrDefault(c => c.actorId == command.parameters[0]);
            var actors = DataManager.Self().GetGameParty().Actors;
            var actor = actors.FirstOrDefault(c => c.ActorId == actorData?.actorId);
            if (actor == null)
            {
                ProcessEndAction();
                return;
            }

            //装備変更処理実施
            var equipTypes = DataManager.Self().GetSystemDataModel().equipTypes;
            var equipTypeIndex = equipTypes.IndexOf(equipTypes.FirstOrDefault(c => c.id == command.parameters[1]));

            //-1以外の場合は装備変更を行う
            if (command.parameters[2] != "-1")
                ItemManager.ChangeEquipment(actorData, equipTypes[equipTypeIndex], command.parameters[2], equipTypeIndex);
            //-1指定の場合は装備を外す
            else
                ItemManager.RemoveEquipment(actorData, equipTypes[equipTypeIndex], equipTypeIndex);

            for (int i = 0; i < actors.Count; i++)
                if (actors[i].ActorId == actorData.actorId)
                    actors[i].ResetActorData();

            ProcessEndAction();
        }

        private void ProcessEndAction() {
            SendBackToLauncher.Invoke();
        }
    }
}