using System.Collections.Generic;
using System.Linq;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.CharacterActor;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Runtime;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.State;
using RPGMaker.Codebase.CoreSystem.Service.DatabaseManagement;
using static RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Flag.FlagDataModel;
using static RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Runtime.RuntimeActorDataModel;
using static RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Runtime.RuntimeSaveDataModel;

namespace RPGMaker.Codebase.Runtime.Common.Component.Hud.Actor
{
    public class ActorChangeState
    {
        private List<Variable> _databaseVariables;

        // データプロパティ
        //--------------------------------------------------------------------------------------------------------------
        private List<RuntimeActorDataModel> _runtimeActorDataModel;
        private SaveDataVariablesData _saveDataVariablesData;
        private List<CharacterActorDataModel> _characterActorData;

        /**
         * 初期化
         */
        public void Init(
            List<RuntimeActorDataModel> runtimeActorData,
            SaveDataVariablesData saveDataVariablesData,
            List<CharacterActorDataModel> characterActorData
        ) {
            _runtimeActorDataModel = runtimeActorData;
            _saveDataVariablesData = saveDataVariablesData;
            _characterActorData = characterActorData;

            var databaseManagementService = new DatabaseManagementService();
            _databaseVariables = databaseManagementService.LoadFlags().variables;
        }

        public void ChangeState(StateDataModel state, EventDataModel.EventCommand command) {
            var isFixedValue = command.parameters[0] == "0" ? true : false;
            var actorId = command.parameters[1];
            var isAddValue = command.parameters[2] == "0" ? true : false;
            var index = 0;

            if (isFixedValue)
            {
                if (actorId == "-1") //パーティ全体
                {
                    for (var i = 0; i < _runtimeActorDataModel.Count; i++)
                        ChangeStateProcess(isAddValue, i, state);
                }
                else //個々のキャラクター
                {
                    index = _runtimeActorDataModel.IndexOf(
                        _runtimeActorDataModel.FirstOrDefault(c => c.actorId == actorId));
                    if (index != -1)
                        ChangeStateProcess(isAddValue, index, state);
                }
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
                            index = _runtimeActorDataModel.IndexOf(_runtimeActorDataModel.FirstOrDefault(c => c.actorId == _characterActorData[indexActor].uuId));
                            if (index >= 0)
                            {
                                ChangeStateProcess(isAddValue, index, state);
                            }
                        }
                    }
                }
            }
        }

        private void ChangeStateProcess(bool isAddValue, int index, StateDataModel value) {
            if (isAddValue)
            {
                //GameActorを検索する
                var actors = DataManager.Self().GetGameParty().Actors;
                for (int i = 0; i < actors.Count; i++)
                {
                    if (actors[i].ActorId == _runtimeActorDataModel[index].actorId)
                    {
                        //ステートが付与可能なタイミングかどうかのチェック
                        if (actors[i].IsStateTiming(value.id))
                        {
                            //ステート付与
                            actors[i].AddState(value.id);
                        }
                    }
                }
            }
            else
            {
                //GameActorを検索する
                var actors = DataManager.Self().GetGameParty().Actors;
                for (int i = 0; i < actors.Count; i++)
                {
                    if (actors[i].ActorId == _runtimeActorDataModel[index].actorId)
                    {
                        //ステート解除
                        actors[i].RemoveState(value.id);
                    }
                }
            }
        }
    }
}