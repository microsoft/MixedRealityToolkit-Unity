namespace MixedRealityToolkit.UX.Lines
{
    [UnityEditor.CustomEditor(typeof(Bezier))]
    public class BezierEditor : LineBaseEditor
    {
        protected override void DrawCustomSceneGUI()
        {
            base.DrawCustomSceneGUI();

            Bezier line = (Bezier)target;

            line.SetPoint(0, SphereMoveHandle(line.GetPoint(0)));
            line.SetPoint(1, SquareMoveHandle(line.GetPoint(1)));
            line.SetPoint(2, SquareMoveHandle(line.GetPoint(2)));
            line.SetPoint(3, SphereMoveHandle(line.GetPoint(3)));

            UnityEditor.Handles.color = handleColorTangent;
            UnityEditor.Handles.DrawLine(line.GetPoint(0), line.GetPoint(1));
            UnityEditor.Handles.DrawLine(line.GetPoint(2), line.GetPoint(3));
        }
    }
}