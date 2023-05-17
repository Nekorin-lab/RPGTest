using RPGMaker.Codebase.CoreSystem.Helper;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Runtime.Common;
using RPGMaker.Codebase.Runtime.Common.Component.Hud.Actor;
using UnityEngine;

namespace RPGMaker.Codebase.Runtime.Event.Actor
{
    public class ActorSettingsProcessor : AbstractEventCommandProcessor
    {
        private ActorSettings _actor;

        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            if (_actor == null)
            {
                _actor = new ActorSettings();
                _actor.Init(
                    DataManager.Self().GetRuntimeSaveDataModel().runtimeActorDataModels,
                    DataManager.Self().GetActorDataModels()
                );
            }

            _actor.ChangeActorSetting(command);
            ProcessEndAction();
        }

        private void ProcessEndAction() {
            _actor = null;
            SendBackToLauncher.Invoke();
        }
    }
}