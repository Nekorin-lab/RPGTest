using System;
using System.Collections.Generic;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.EventBattle
{
    [Serializable]
    public class EventBattleDataModel
    {
        public string                eventId;
        public List<EventBattlePage> pages;

        public EventBattleDataModel(string eventId, List<EventBattlePage> pages) {
            this.eventId = eventId;
            this.pages = pages;
        }

        public static EventBattleDataModel CreateDefault() {
            return new EventBattleDataModel(null, new List<EventBattlePage>());
        }

        public static EventBattlePage CreateDefaultEventBattlePage(int page) {
            return new EventBattlePage(
                page,
                "",
                new EventBattleCondition(1, 0,
                    new EventBattlePageConditionTurn(0, 0, 0),
                    new EventBattlePageConditionActorHp(0, "", 0),
                    new EventBattlePageConditionEnemyHp(0, "", 0),
                    new EventBattlePageConditionSwitchData(0, ""),
                    0));
        }

        [Serializable]
        public class EventBattlePage
        {
            public EventBattleCondition condition;
            public string               eventId;
            public int                  page;

            public EventBattlePage(int page, string eventId, EventBattleCondition condition) {
                this.page = page;
                this.eventId = eventId;
                this.condition = condition;
            }
        }

        [Serializable]
        public class EventBattleCondition
        {
            public EventBattlePageConditionActorHp    actorHp;
            public EventBattlePageConditionEnemyHp    enemyHp;
            public int                                run;
            public int                                span;
            public EventBattlePageConditionSwitchData switchData;
            public EventBattlePageConditionTurn       turn;
            public int                                turnEnd;

            public EventBattleCondition(
                int run,
                int turnEnd,
                EventBattlePageConditionTurn turn,
                EventBattlePageConditionActorHp actorHp,
                EventBattlePageConditionEnemyHp enemyHp,
                EventBattlePageConditionSwitchData switchData,
                int span
            ) {
                this.run = run;
                this.turnEnd = turnEnd;
                this.turn = turn;
                this.actorHp = actorHp;
                this.enemyHp = enemyHp;
                this.switchData = switchData;
                this.span = span;
            }
        }

        [Serializable]
        public class EventBattlePageConditionActorHp
        {
            public string actorId;
            public int    enabled;
            public int    value;

            public EventBattlePageConditionActorHp(int enabled, string actorId, int value) {
                this.enabled = enabled;
                this.actorId = actorId;
                this.value = value;
            }
        }

        [Serializable]
        public class EventBattlePageConditionEnemyHp
        {
            public int    enabled;
            public string enemyId;
            public int    value;

            public EventBattlePageConditionEnemyHp(int enabled, string enemyId, int value) {
                this.enabled = enabled;
                this.enemyId = enemyId;
                this.value = value;
            }
        }

        [Serializable]
        public class EventBattlePageConditionSwitchData
        {
            public int    enabled;
            public string switchId;

            public EventBattlePageConditionSwitchData(int enabled, string switchId) {
                this.enabled = enabled;
                this.switchId = switchId;
            }
        }

        [Serializable]
        public class EventBattlePageConditionTurn
        {
            public int enabled;
            public int end;
            public int start;

            public EventBattlePageConditionTurn(int enabled, int start, int end) {
                this.enabled = enabled;
                this.start = start;
                this.end = end;
            }
        }
    }
}