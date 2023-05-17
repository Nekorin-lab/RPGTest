using System.Collections.Generic;
using System.Linq;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.CharacterActor;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Runtime;
using RPGMaker.Codebase.CoreSystem.Service.DatabaseManagement;
using UnityEngine;
using static RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Flag.FlagDataModel;
using static RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Runtime.RuntimeSaveDataModel;

namespace RPGMaker.Codebase.Runtime.Common.Component.Hud.Actor
{
    /// <summary>
    ///     [能力値の増減]の適応処理
    /// </summary>
    public class ActorChangeParameter
    {
        // データプロパティ
        //--------------------------------------------------------------------------------------------------------------
        private List<Variable> _databaseVariables;
        private List<RuntimeActorDataModel> _runtimeActorDataModels;
        private SaveDataVariablesData _saveDataVariablesData;
        private List<CharacterActorDataModel> _characterActorData;

        /**
         * 初期化
         */
        public void Init(RuntimeSaveDataModel saveDataModel) {
            _runtimeActorDataModels = saveDataModel.runtimeActorDataModels;
            _saveDataVariablesData = saveDataModel.variables;

            var databaseManagementService = new DatabaseManagementService();
            _databaseVariables = databaseManagementService.LoadFlags().variables;

            _characterActorData = DataManager.Self().GetActorDataModels();
        }

        public void ChangeParameter(EventDataModel.EventCommand command) {
            var isFixedValue = command.parameters[0] == "0";
            var parameter = int.Parse(command.parameters[2]);
            var isAddValue = command.parameters[3] == "0";
            var isConstant = command.parameters[4] == "0";

            var value = 0;
            if (isConstant)
            {
                if (!int.TryParse(command.parameters[5], out value))
                    return;
            }
            else
            {
                var index = _databaseVariables.FindIndex(v => v.id == command.parameters[5]);
                if (index == -1)
                    return;

                if (!int.TryParse(_saveDataVariablesData.data[index], out value))
                    return;
            }

            value = isAddValue ? value : -value;

            if (isFixedValue)
            {
                var actorId = command.parameters[1];
                if (actorId == "-1")
                {
                    // パーティ全体
                    foreach (var actorDataModel in _runtimeActorDataModels)
                        ChangeParameterProcess(parameter, actorDataModel, value);

                    return;
                }

                // 個々のキャラクター
                var actor = _runtimeActorDataModels.FirstOrDefault(c => c.actorId == actorId);
                if(actor != null) ChangeParameterProcess(parameter, actor, value);
            }
            else
            {
                //MVの挙動から
                //変数内の数値によって経験値を変動させるのは、該当のIDのユーザー（=SerialNoが一致するアクター）
                int variableIndex = _databaseVariables.FindIndex(v => v.id == command.parameters[1]);
                if (variableIndex >= 0)
                {
                    int actorSerialNo = int.Parse(_saveDataVariablesData.data[variableIndex]);
                    if (actorSerialNo >= 0)
                    {
                        int indexActor = _characterActorData.IndexOf(_characterActorData.FirstOrDefault(c => c.SerialNumber == actorSerialNo));
                        if (indexActor >= 0)
                        {
                            int index = _runtimeActorDataModels.IndexOf(_runtimeActorDataModels.FirstOrDefault(c => c.actorId == _characterActorData[indexActor].uuId));
                            if (index >= 0)
                            {
                                ChangeParameterProcess(parameter, _runtimeActorDataModels[index], value);
                            }
                        }
                    }
                }
            }
        }

        private void ChangeParameterProcess(int parameter, RuntimeActorDataModel targetDataModel, int value) {
            //パーティに指定したキャラが存在しない
            if (targetDataModel == null) return;
            var actorsWork = DataManager.Self().GetGameParty().Actors;
            var actor = actorsWork.FirstOrDefault(c => c.ActorId == targetDataModel?.actorId);
            if (actor == null) return;
            
            var gameActor = DataManager.Self().GetGameActors().Actor(targetDataModel);
            gameActor.AddParam(parameter, value);
            

            var actors = DataManager.Self().GetGameParty().Actors;
            for (int i = 0; i < actors.Count; i++)
                if (actors[i].ActorId == gameActor.ActorId)
                    actors[i].ResetActorData();
        }
    }
}