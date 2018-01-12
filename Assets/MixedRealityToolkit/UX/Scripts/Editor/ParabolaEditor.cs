namespace MixedRealityToolkit.UX.Lines
{
    [UnityEditor.CustomEditor(typeof(Parabola))]
    public class ParabolaEditor : LineBaseEditor
    {
        protected override void DrawCustomSceneGUI()
        {
            base.DrawCustomSceneGUI();

            Parabola line = (Parabola)target;

            line.FirstPoint = SquareMoveHandle(line.FirstPoint);
            line.LastPoint = SquareMoveHandle(line.LastPoint);
            // Draw a handle for the parabola height
            line.Height = AxisMoveHandle(line.FirstPoint, line.transform.up, line.Height);
        }
    }
}