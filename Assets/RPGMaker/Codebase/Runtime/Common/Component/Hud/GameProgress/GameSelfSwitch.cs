using System.Collections.Generic;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Runtime;
using RPGMaker.Codebase.CoreSystem.Service.EventManagement;

namespace RPGMaker.Codebase.Runtime.Common.Component.Hud.GameProgress
{
    public class GameSelfSwitch
    {
        // コアシステムサービス
        // FIXME 本来コンポーネントから直接コアシステムにアクセスするのはよくない Initの引数で受け取るべき
        //--------------------------------------------------------------------------------------------------------------
        private EventManagementService _eventManagementService;

        // initialize methods
        //--------------------------------------------------------------------------------------------------------------
        /**
         * 初期化
         */
        public void Init() {
            _eventManagementService = new EventManagementService();
        }

        public void SetGameSelfSwitch(string eventID, int pageNumber, string switchName, bool toggle) {
            var saveData = DataManager.Self().GetRuntimeSaveDataModel();
            var eventMapDataModels = _eventManagementService.LoadEventMap();

            // データを検索
            RuntimeSaveDataModel.SaveDataSelfSwitchesData swData = null;
            for (int i = 0; i < saveData.selfSwitches.Count; i++)
                if (saveData.selfSwitches[i].id == eventID)
                {
                    swData = saveData.selfSwitches[i];
                    break;
                }

            // データがなければ追加
            if (swData == null)
            {
                saveData.selfSwitches.Add(new RuntimeSaveDataModel.SaveDataSelfSwitchesData());
                saveData.selfSwitches[saveData.selfSwitches.Count - 1].id = eventID;
                saveData.selfSwitches[saveData.selfSwitches.Count - 1].data =
                    new List<bool> {false, false, false, false};
                swData = saveData.selfSwitches[saveData.selfSwitches.Count - 1];
            }

            // スイッチ設定
            switch (switchName)
            {
                case "A":
                    swData.data[0] = toggle;
                    break;
                case "B":
                    swData.data[1] = toggle;
                    break;
                case "C":
                    swData.data[2] = toggle;
                    break;
                case "D":
                    swData.data[3] = toggle;
                    break;
            }
        }
    }
}