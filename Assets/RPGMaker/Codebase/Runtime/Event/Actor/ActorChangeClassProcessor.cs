using System.Linq;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.Runtime.Common;
using RPGMaker.Codebase.Runtime.Common.Component.Hud.Actor;
using UnityEngine;

namespace RPGMaker.Codebase.Runtime.Event.Actor
{
    public class ActorChangeClassProcessor : AbstractEventCommandProcessor
    {
        private ActorChangeClass _actor;

        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            if (_actor == null)
            {
                _actor = new ActorChangeClass();
            }

            var actorData = DataManager.Self().GetActorDataModels()
                .FirstOrDefault(c => c.uuId == command.parameters[0]);
            _actor.ChangeClass(actorData, command);
            ProcessEndAction();
        }

        private void ProcessEndAction() {
            _actor = null;
            SendBackToLauncher.Invoke();
        }
    }
}