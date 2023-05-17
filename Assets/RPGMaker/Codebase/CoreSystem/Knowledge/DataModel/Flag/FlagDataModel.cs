using System;
using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common;
using RPGMaker.Codebase.CoreSystem.Knowledge.Misc;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Flag
{
    [Serializable]
    public class FlagDataModel
    {
        public List<Switch>   switches;
        public List<Variable> variables;

        public FlagDataModel(List<Switch> switches, List<Variable> variables) {
            this.switches = switches;
            this.variables = variables;
        }

        [Serializable]
        public class Switch : WithSerialNumberDataModel
        {
            public List<Event> events;
            public string      id;
            public string      name;

            public Switch(string id, string name, List<Event> events) {
                this.id = id;
                this.name = name;
                this.events = events;
            }

            public static Switch CreateDefault() {
#if UNITY_EDITOR
                return new Switch(Guid.NewGuid().ToString(), CoreSystemLocalize.LocalizeText("WORD_1518"), new List<Event>());
#else
                return new Switch(Guid.NewGuid().ToString(), "", new List<Event>());
#endif
            }
        }

        [Serializable]
        public class Variable : WithSerialNumberDataModel
        {
            public List<Event> events;
            public string      id;
            public string      name;

            public Variable(string id, string name, List<Event> events) {
                this.id = id;
                this.name = name;
                this.events = events;
            }

            public static Variable CreateDefault() {
#if UNITY_EDITOR
                return new Variable(Guid.NewGuid().ToString(), CoreSystemLocalize.LocalizeText("WORD_1518"), new List<Event>());
#else
                return new Variable(Guid.NewGuid().ToString(), "", new List<Event>());
#endif
            }
        }

        [Serializable]
        public class Event
        {
            public int eventId;
            public int mapId;

            public Event(int mapId, int eventId) {
                this.mapId = mapId;
                this.eventId = eventId;
            }
        }
    }
}