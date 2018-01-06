namespace MixedRealityToolkit.UX.Lines
{
    [UnityEditor.CustomEditor(typeof(Line))]
    public class LineEditor : LineBaseEditor
    {
        protected override void DrawCustomSceneGUI()
        {
            base.DrawCustomSceneGUI();

            LineBase line = (LineBase)target;
            line.FirstPoint = SquareMoveHandle(line.FirstPoint);
            line.LastPoint = SquareMoveHandle(line.LastPoint);
        }
    }
}