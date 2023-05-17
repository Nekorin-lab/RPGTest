using System;
using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.EventMap
{
    [Serializable]
    public class EventMapDataModel : WithSerialNumberDataModel
    {
        public string             eventId;
        public string             mapId;
        public string             name;
        public string             note;
        public List<EventMapPage> pages;
        public int                temporaryErase;
        public int                x;
        public int                y;

        [Serializable]
        public class EventMapPage
        {
            // 表示プライオリティ。
            public enum PriorityType
            {
                Under,      // 下。
                Normal,     // 通常。
                Upper,      // 上。
            }

            public const int                   DefaultPage = 0;
            public       string                chapterId;
            public       EventMapPageCondition condition;
            public       int                   eventTrigger;
            public       EventMapPageImage     image;
            public       EventMapPageMove      move;

            public int              page;
            public int              priority;
            public string           sectionId;
            public EventMapPageWalk walk;

            public EventMapPage(
                int page,
                EventMapPageCondition condition,
                EventMapPageImage image,
                EventMapPageWalk walk,
                EventMapPageMove move,
                int priority,
                int eventTrigger
            ) {
                this.page = page;
                this.condition = condition;
                this.image = image;
                this.walk = walk;
                this.move = move;
                this.priority = priority;
                this.eventTrigger = eventTrigger;
            }

            public PriorityType Priority => (PriorityType)priority;

            public static EventMapPage CreateDefault() {
                return new EventMapPage(DefaultPage, EventMapPageCondition.CreateDefault(),
                    EventMapPageImage.CreateDefault(),
                    EventMapPageWalk.CreateDefault(), EventMapPageMove.CreateDefault(), 1, 0);
            }
        }

        [Serializable]
        public class EventMapPageImage
        {
            public int    direction;
            public string faceName;
            public string name;
            public string pictureName;
            public string sdName;

            public EventMapPageImage(string faceName, string sdName, string pictureName, int direction, string name) {
                this.faceName = faceName;
                this.sdName = sdName;
                this.pictureName = pictureName;
                this.direction = direction;
                this.name = name;
            }

            public static EventMapPageImage CreateDefault() {
                return new EventMapPageImage("", "", "", 0, "");
            }
        }

        [Serializable]
        public class EventMapPageCondition
        {
            public EventMapPageConditionActor      actor;
            public EventMapPageConditionImage      image;
            public EventMapPageConditionItem       item;
            public EventMapPageConditionSelfSwitch selfSwitch;
            public EventMapPageConditionSwitchItem switchItem;
            public EventMapPageConditionSwitch     switchOne;
            public EventMapPageConditionSwitch     switchTwo;
            public EventMapPageConditionVariable   variables;

            public EventMapPageCondition(
                EventMapPageConditionActor actor,
                EventMapPageConditionItem item,
                EventMapPageConditionSelfSwitch selfSwitch,
                EventMapPageConditionSwitch switchOne,
                EventMapPageConditionSwitch switchTwo,
                EventMapPageConditionVariable variables,
                EventMapPageConditionImage image,
                EventMapPageConditionSwitchItem switchItem
            ) {
                this.actor = actor;
                this.item = item;
                this.selfSwitch = selfSwitch;
                this.switchOne = switchOne;
                this.switchTwo = switchTwo;
                this.variables = variables;
                this.image = image;
                this.switchItem = switchItem;
            }

            public static EventMapPageCondition CreateDefault() {
                return new EventMapPageCondition(EventMapPageConditionActor.CreateDefault(),
                    EventMapPageConditionItem.CreateDefault(), EventMapPageConditionSelfSwitch.CreateDefault(),
                    EventMapPageConditionSwitch.CreateDefault(), EventMapPageConditionSwitch.CreateDefault(),
                    EventMapPageConditionVariable.CreateDefault(), EventMapPageConditionImage.CreateDefault(),
                    EventMapPageConditionSwitchItem.CreateDefault());
            }
        }

        [Serializable]
        public class EventMapPageConditionActor
        {
            public string actorId;
            public int    enabled;

            public EventMapPageConditionActor(int enabled, string actorId) {
                this.enabled = enabled;
                this.actorId = actorId;
            }

            public static EventMapPageConditionActor CreateDefault() {
                return new EventMapPageConditionActor(0, "");
            }
        }

        [Serializable]
        public class EventMapPageConditionImage
        {
            public enum EnabledType
            {
                Character,
                SelectedImage,
                None
            }

            public int    enabled;
            public string imageName;

            public EventMapPageConditionImage(int enabled, string imageName) {
                this.enabled = enabled;
                this.imageName = imageName;
            }

            public EnabledType Enabled
            {
                get { return (EnabledType)enabled; } 
                set { enabled = (int)value; } 
            }

            public static EventMapPageConditionImage CreateDefault() {
                return new EventMapPageConditionImage(0, "");
            }
        }

        [Serializable]
        public class EventMapPageConditionItem
        {
            public int    enabled;
            public string itemId;

            public EventMapPageConditionItem(int enabled, string itemId) {
                this.enabled = enabled;
                this.itemId = itemId;
            }

            public static EventMapPageConditionItem CreateDefault() {
                return new EventMapPageConditionItem(0, "");
            }
        }

        [Serializable]
        public class EventMapPageConditionSwitchItem
        {
            public int    enabled;
            public string switchItemId;

            public EventMapPageConditionSwitchItem(int enabled, string switchItemId) {
                this.enabled = enabled;
                this.switchItemId = switchItemId;
            }

            public static EventMapPageConditionSwitchItem CreateDefault() {
                return new EventMapPageConditionSwitchItem(0, "");
            }
        }

        [Serializable]
        public class EventMapPageConditionSelfSwitch
        {
            public int    enabled;
            public string selfSwitch;

            public EventMapPageConditionSelfSwitch(int enabled, string selfSwitch) {
                this.enabled = enabled;
                this.selfSwitch = selfSwitch;
            }

            public static EventMapPageConditionSelfSwitch CreateDefault() {
                return new EventMapPageConditionSelfSwitch(0, "A");
            }
        }

        [Serializable]
        public class EventMapPageConditionSwitch
        {
            public int    enabled;
            public string switchId;

            public EventMapPageConditionSwitch(int enabled, string switchId) {
                this.enabled = enabled;
                this.switchId = switchId;
            }

            public static EventMapPageConditionSwitch CreateDefault() {
                return new EventMapPageConditionSwitch(0, "");
            }
        }

        [Serializable]
        public class EventMapPageConditionVariable
        {
            public int    enabled;
            public int    value;
            public string variableId;

            public EventMapPageConditionVariable(int enabled, string variableId, int value) {
                this.enabled = enabled;
                this.variableId = variableId;
                this.value = value;
            }

            public static EventMapPageConditionVariable CreateDefault() {
                return new EventMapPageConditionVariable(0, "", 0);
            }
        }

        [Serializable]
        public class EventMapPageWalk
        {
            public int direction;
            public int directionFix;
            public int stepping;
            public int through;
            public int walking;

            public EventMapPageWalk(int walking, int stepping, int direction, int directionFix, int through) {
                this.walking = walking;
                this.stepping = stepping;
                this.direction = direction;
                this.directionFix = directionFix;
                this.through = through;
            }

            public static EventMapPageWalk CreateDefault() {
                return new EventMapPageWalk(0, 0, -1, 0, 1);
            }
        }

        [Serializable]
        public class EventMapPageMove
        {
            public int                                        frequency;
            public int                                        moveType;
            public int                                        repeat;
            public List<EventDataModel.EventCommandMoveRoute> route;
            public int                                        skip;
            public int                                        speed;


            public EventMapPageMove(
                int moveType,
                int speed,
                int frequency,
                List<EventDataModel.EventCommandMoveRoute> route,
                int repeat,
                int skip
            ) {
                this.moveType = moveType;
                this.speed = speed;
                this.frequency = frequency;
                this.route = route;
                this.repeat = repeat;
                this.skip = skip;
            }

            public static EventMapPageMove CreateDefault() {
                return new EventMapPageMove(0, 3, 2, new List<EventDataModel.EventCommandMoveRoute>(), 0, 0);
            }
        }
    }
}