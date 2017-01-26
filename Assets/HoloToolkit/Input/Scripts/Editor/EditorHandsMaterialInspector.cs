using UnityEditor;
using UnityEngine;

public class EditorHandsMaterialInspector : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        foreach (MaterialProperty materialProperty in properties)
        {
            if (materialProperty.type != MaterialProperty.PropType.Texture && materialProperty.type != MaterialProperty.PropType.Color || materialProperty.flags != MaterialProperty.PropFlags.PerRendererData)
            {
                materialEditor.ShaderProperty(materialProperty, materialProperty.displayName);
            }
        }

    }
}
