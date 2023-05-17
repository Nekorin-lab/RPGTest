using System;
using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Common;
using RPGMaker.Codebase.CoreSystem.Lib.RepositoryCore;

namespace RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Troop
{
    [Serializable]
    public class TroopDataModel : WithSerialNumberDataModel
    {
        //オーとマッチング用の固定ID
        public static string                TROOP_AUTOMATCHING = "AUTOMATCH";
        public        string                backImage1;
        public        string                backImage2;
        public        string                battleEventId;
        public        int                   deleted;
        public        List<FrontViewMember> frontViewMembers;

        //戦闘シーンのプレビュー用の固定ID
        public static string TROOP_PREVIEW = "BATTLE_PREVIEW";
        
        public static string TROOP_BTATLE_TEST = "BATTLE_TEST";

        
        public string               id;
        public string               name;
        public List<SideViewMember> sideViewMembers;

        public static TroopDataModel CreateDefault() {
            return new TroopDataModel
            {
                id = Guid.NewGuid().ToString(),
                name = null,
                backImage1 = null,
                backImage2 = null,
                battleEventId = null,
                deleted = 0,
                frontViewMembers = null,
                sideViewMembers = null
            }; // FIXME
        }

        [Serializable]
        public class FrontViewMember
        {
            public int    appearanceTurn;
            public int    conditions;
            public string enemyId;
            public int    position;

            public FrontViewMember(string enemyId, int position, int conditions, int appearanceTurn) {
                this.enemyId = enemyId;
                this.position = position;
                this.conditions = conditions;
                this.appearanceTurn = appearanceTurn;
            }

            public static FrontViewMember CreateDefault(string id) {
                return new FrontViewMember(id, 1, 1, 3);
            }
        }

        [Serializable]
        public class SideViewMember
        {
            public int    appearanceTurn;
            public int    conditions;
            public string enemyId;
            public int    position1;
            public int    position2;

            public SideViewMember(string enemyId, int position1, int position2, int conditions, int appearanceTurn) {
                this.enemyId = enemyId;
                this.position1 = position1;
                this.position2 = position2;
                this.conditions = conditions;
                this.appearanceTurn = appearanceTurn;
            }

            public static SideViewMember CreateDefault(string id) {
                return new SideViewMember(id, 1, 1, 0, 3);
            }
        }
    }
}