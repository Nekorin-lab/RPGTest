using RPGMaker.Codebase.Editor.OutlineEditor.Model;
using UnityEditor.GraphToolsFoundation.Overdrive;

namespace RPGMaker.Codebase.Editor.OutlineEditor.Command.CustomOnGTFCommand
{
    public class PasteSerializedDataCommandCustom : PasteSerializedDataCommand
    {
        public static void CustomCommandHandler(GraphToolState graphToolState, PasteSerializedDataCommand command) {
            DefaultCommandHandler(graphToolState, command);

            // ペーストされたノードモデルは直後セレクトされている（という前提）
            foreach (var node in
                graphToolState.SelectionState.GetSelection(graphToolState.GraphViewState.GraphModel))
            {
                if (node is OutlineNodeModel outlineNodeModel)
                {
                    outlineNodeModel.RenewEntity();
                    OutlineEditor.NodeModelsByUuid.Add(outlineNodeModel.GetEntityID(), outlineNodeModel);

                    // ペースト(貼り付け)されたノードが選択状態になっているので、
                    // ヒエラルキーの選択項目とインスペクターの内容をペースト(貼り付け)されたノードのものに変更する。
                    OutlineEditor.SelectElementsCommandProcess(outlineNodeModel);
                }
            }
        }
    }
}