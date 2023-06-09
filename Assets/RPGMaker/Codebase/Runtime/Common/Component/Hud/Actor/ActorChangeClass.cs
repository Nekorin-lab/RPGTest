using System.Collections.Generic;
using System.Linq;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.CharacterActor;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Runtime;

namespace RPGMaker.Codebase.Runtime.Common.Component.Hud.Actor
{
    public class ActorChangeClass
    {
        public void ChangeClass(CharacterActorDataModel actorData, EventDataModel.EventCommand command) {
            var classId = command.parameters[1];
            var save = command.parameters[2] == "1" ? true : false;

            var party = DataManager.Self().GetGameParty();
            var index = party.Actors.IndexOf(party.Actors.FirstOrDefault(c => c.ActorId == actorData.uuId));
            if(index >= 0)
                party.Actors[index].ChangeClass(classId, save);
        }
    }
}