using System;
using RPGMaker.Codebase.Editor.OutlineEditor.Model;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;

namespace RPGMaker.Codebase.Editor.OutlineEditor.GTF
{
    public class OeGraphModel : GraphModel
    {
        protected override Type GetEdgeType(IPortModel toPort, IPortModel fromPort) {
            return typeof(OutlineEdgeModel);
        }
    }
}