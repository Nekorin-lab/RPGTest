using RPGMaker.Codebase.Runtime.Common;
using RPGMaker.Codebase.CoreSystem.Knowledge.DataModel.Event;

namespace RPGMaker.Codebase.Runtime.Event.Picture
{
    public class PictureShowProcessor : AbstractEventCommandProcessor
    {
        protected override void Process(string eventID, EventDataModel.EventCommand command) {
            //画像の番号
            var pictureNumber = int.Parse(command.parameters[0]);
            HudDistributor.Instance.NowHudHandler().PictureInit();
            //既に他のが画像が表示されていた場合一度消す
            HudDistributor.Instance.NowHudHandler().DeletePicture(pictureNumber);
            //画像の表示
            HudDistributor.Instance.NowHudHandler().AddPicture(pictureNumber, command.parameters[1]);

            //アンカー
            HudDistributor.Instance.NowHudHandler().SetAnchor(pictureNumber, int.Parse(command.parameters[2]));

            //座標なのか、変数なのか
            HudDistributor.Instance.NowHudHandler().SetPosition(pictureNumber,
                int.Parse(command.parameters[3]),
                command.parameters[4], command.parameters[5]);

            //幅,高さ
            HudDistributor.Instance.NowHudHandler().SetPictureSize(pictureNumber,
                int.Parse(command.parameters[6]),
                int.Parse(command.parameters[7]));

            //不透明度
            HudDistributor.Instance.NowHudHandler().SetPictureOpacity(pictureNumber, int.Parse(command.parameters[8]));

            //"通常", "加算", "乗算", "スクリーン";
            HudDistributor.Instance.NowHudHandler().SetProcessingType(pictureNumber, int.Parse(command.parameters[9]));

            //セーブデータへの保存用
            HudDistributor.Instance.NowHudHandler().AddPictureParameter(pictureNumber, command.parameters);

            ProcessEndAction();
        }

        private void ProcessEndAction() {
            SendBackToLauncher.Invoke();
        }
    }
}