using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Runtime.Common.Component.Hud.Actor;
using RPGMaker.Codebase.Runtime.Common;
using UnityEngine;

namespace RPGMaker.Codebase.Runtime.Event.Actor
{
    public class ActorChangeExpProcessor : AbstractEventCommandProcessor
    {
        private ActorChangeExp _actor;

        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            if (_actor == null)
            {
                _actor = new ActorChangeExp();
                _actor.Init(
                    DataManager.Self().GetRuntimeSaveDataModel().runtimeActorDataModels,
                    DataManager.Self().GetRuntimeSaveDataModel().variables,
                    DataManager.Self().GetActorDataModels()
                );
            }

            _actor.ChangeEXP(command, ProcessEndAction);
        }

        private void ProcessEndAction() {
            _actor = null;
            SendBackToLauncher.Invoke();
        }
    }
}