using RPGMaker.Codebase.Runtime.Common;

namespace RPGMaker.Codebase.Runtime.Map.Component.Character
{
    public class ActorOnMap : CharacterOnMap
    {
        private void Start() {
            //フレーム単位での処理
            TimeHandler.Instance.AddTimeActionEveryFrame(UpdateTimeHandler);
        }

        private void OnDestroy() {
            TimeHandler.Instance?.RemoveTimeAction(UpdateTimeHandler);
        }
    }
}