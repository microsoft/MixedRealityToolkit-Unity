namespace MixedRealityToolkit.UX.Lines
{
    [UnityEditor.CustomEditor(typeof(LineObjectCollection))]
    public class LineObjectCollectionEditor : UnityEditor.Editor
    {
        public void OnSceneGUI()
        {
            LineObjectCollection loc = (LineObjectCollection)target;

            for (int i = 0; i < loc.Objects.Count; i++)
            {
                if (loc.Objects[i] != null)
                {
                    UnityEditor.Handles.Label(loc.Objects[i].position, "Index: " + i.ToString("000") + "\nOffset: " + loc.GetOffsetFromObjectIndex(i).ToString("00.00"));
                }
            }
        }
    }
}