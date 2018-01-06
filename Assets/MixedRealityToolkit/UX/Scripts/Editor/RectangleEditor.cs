using UnityEngine;

namespace MixedRealityToolkit.UX.Lines
{
    [UnityEditor.CustomEditor(typeof(Rectangle))]
    public class RectangleEditor : LineBaseEditor
    {
        // Use FromSource step mode for rectangles since interpolated looks weird
        protected override StepModeEnum EditorStepMode { get { return StepModeEnum.FromSource; } }
    }

}