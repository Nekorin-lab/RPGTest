using System;
using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.EventCommon
{
    [Serializable]
    public class EventCommonDataModel : WithSerialNumberDataModel
    {
        public List<EventCommonCondition> conditions;
        public string                     eventId;
        public string                     name;

        public EventCommonDataModel(string eventId, string name, List<EventCommonCondition> conditions) {
            this.eventId = eventId;
            this.name = name;
            this.conditions = conditions;
        }

        public static EventCommonDataModel CreateDefault(string id, string name) {
            return new EventCommonDataModel(id, name, new List<EventCommonCondition>());
        }

        public EventCommonDataModel Clone() {
            return (EventCommonDataModel) MemberwiseClone();
        }

        [Serializable]
        public struct EventCommonCondition
        {
            public int    trigger;
            public string switchId;

            public EventCommonCondition(int trigger, string switchId) {
                this.trigger = trigger;
                this.switchId = switchId;
            }
        }
    }
}