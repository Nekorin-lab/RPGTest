using System;
using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.State
{
    [Serializable]
    public class StateDataModel : WithSerialNumberDataModel
    {
        public int                        autoRemovalTiming;
        public string                     iconId;
        public string                     id;
        public int                        inBattleRemoveDamage;
        public int                        inBattleRemoveProbability;
        public int                        inBattleRemoveRestriction;
        public int                        maxTurns;
        public string                     message1;
        public string                     message2;
        public string                     message3;
        public string                     message4;
        public int                        minTurns;
        public int                        motion;
        public string                     name;
        public string                     note;
        public int                        occurrenceFrequencyStep;
        public string                     overlay;
        public int                        priority;
        public int                        removeAtBattleEnd;
        public int                        removeAtBattling;
        public int                        removeByDamage;
        public int                        removeByRestriction;
        public int                        removeByWalking;
        public int                        removeProbability;
        public int                        restriction;
        public int                        stateOn;
        public int                        stepGeneration;
        public int                        stepsToRemove;
        public List<TraitCommonDataModel> traits;

        public StateDataModel(
            string id,
            string iconId,
            int priority,
            int motion,
            string overlay,
            int stateOn,
            int restriction,
            string name,
            string note,
            int autoRemovalTiming,
            int removeAtBattleEnd,
            int removeAtBattling,
            int maxTurns,
            int minTurns,
            int removeByDamage,
            int removeByRestriction,
            int removeByWalking,
            int stepsToRemove,
            int stepGeneration,
            int occurrenceFrequencyStep,
            int inBattleRemoveRestriction,
            int inBattleRemoveDamage,
            int inBattleRemoveProbability,
            int removeProbability,
            List<TraitCommonDataModel> traits,
            string message1,
            string message2,
            string message3,
            string message4
        ) {
            this.id = id;
            this.iconId = iconId;
            this.priority = priority;
            this.motion = motion;
            this.overlay = overlay;
            this.stateOn = stateOn;
            this.restriction = restriction;
            this.name = name;
            this.note = note;
            this.autoRemovalTiming = autoRemovalTiming;
            this.removeAtBattleEnd = removeAtBattleEnd;
            this.removeAtBattling = removeAtBattling;
            this.maxTurns = maxTurns;
            this.minTurns = minTurns;
            this.removeByDamage = removeByDamage;
            this.removeByRestriction = removeByRestriction;
            this.removeByWalking = removeByWalking;
            this.stepsToRemove = stepsToRemove;
            this.stepGeneration = stepGeneration;
            this.occurrenceFrequencyStep = occurrenceFrequencyStep;
            this.inBattleRemoveRestriction = inBattleRemoveRestriction;
            this.inBattleRemoveDamage = inBattleRemoveDamage;
            this.inBattleRemoveProbability = inBattleRemoveProbability;
            this.removeProbability = removeProbability;
            this.traits = traits;
            this.message1 = message1;
            this.message2 = message2;
            this.message3 = message3;
            this.message4 = message4;
        }

        public static StateDataModel CreateDefault(string id) {
            return new StateDataModel(
                id,
                "IconSet_000",
                0,
                0,
                "",
                0,
                0,
                "新規ステート",
                "",
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                new List<TraitCommonDataModel>(),
                "",
                "",
                "",
                ""
            );
        }
    }
}