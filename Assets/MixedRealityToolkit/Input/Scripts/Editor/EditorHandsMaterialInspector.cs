using UnityEditor;

public class EditorHandsMaterialInspector : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        foreach (MaterialProperty materialProperty in properties)
        {
            if (materialProperty.flags != MaterialProperty.PropFlags.PerRendererData)
            {
                materialEditor.ShaderProperty(materialProperty, materialProperty.displayName);
            }
        }
    }
}
