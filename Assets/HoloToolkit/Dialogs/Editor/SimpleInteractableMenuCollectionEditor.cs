
using UnityEngine;
using UnityEditor;
using HoloToolkit.Unity.Collections;

[CustomEditor(typeof(SimpleInteractableMenuCollection))]
public class SimpleInteractableMenuCollectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default
        base.OnInspectorGUI();

        // Place the button at the bottom
        SimpleInteractableMenuCollection myScript = (SimpleInteractableMenuCollection)target;
        if (GUILayout.Button("Update SimpleInteractableMenuCollection"))
        {
            myScript.UpdateCollection();
        }
    }
}
