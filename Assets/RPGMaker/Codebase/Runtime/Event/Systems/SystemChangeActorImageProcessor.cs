using RPGMaker.Codebase.Runtime.Common;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Runtime.Map;
using RPGMaker.Codebase.Runtime.Map.Component.Character;

namespace RPGMaker.Codebase.Runtime.Event.Systems
{
    public class SystemChangeActorImageProcessor : AbstractEventCommandProcessor
    {
        protected override void Process(string eventId, EventDataModel.EventCommand command) {
            var runtimeActorDataModels = DataManager.Self().GetRuntimeSaveDataModel().runtimeActorDataModels;

            //IDの一致したActorを探す
            for (var i = 0; i < runtimeActorDataModels.Count; i++)
                if (runtimeActorDataModels[i].actorId == command.parameters[0])
                {
                    DataManager.Self().GetRuntimeSaveDataModel().runtimeActorDataModels[i].faceImage =
                        command.parameters[1];
                    DataManager.Self().GetRuntimeSaveDataModel().runtimeActorDataModels[i].characterImage =
                        command.parameters[2];
                    DataManager.Self().GetRuntimeSaveDataModel().runtimeActorDataModels[i].battlerImage =
                        command.parameters[3];
                    DataManager.Self().GetRuntimeSaveDataModel().runtimeActorDataModels[i].advImage =
                        command.parameters[4];

                    //バトルはUpdateで切り替わる
                    if (GameStateHandler.IsMap())
                    {
                        //MAP上のキャラクターの再描画
                        var actorObj = MapManager.OperatingActor;
                        actorObj.GetComponent<CharacterOnMap>().ReloadCharacterImage(command.parameters[2]);
                    }

                    break;
                }

            //次へ
            ProcessEndAction();
        }

        private void ProcessEndAction() {
            SendBackToLauncher.Invoke();
        }
    }
}