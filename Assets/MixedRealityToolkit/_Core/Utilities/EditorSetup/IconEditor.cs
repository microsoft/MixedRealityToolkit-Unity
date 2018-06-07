using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DefaultAsset))]
public class IconEditor : Editor
{
    private Texture2D icon;

    private bool overwriteIcons;

    public override void OnInspectorGUI()
    {
        GUI.enabled = true;
        icon = (Texture2D)EditorGUILayout.ObjectField("Icon Texture", icon, typeof(Texture2D), false);

        overwriteIcons = EditorGUILayout.Toggle("Overwrite Icon?", overwriteIcons);

        if (GUILayout.Button("Set Icons for child script assets"))
        {
            Object[] selectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            for (int i = 0; i < selectedAsset.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Updating Icons...", $"{selectedAsset[i].name}", i / (float)selectedAsset.Length);
                var path = AssetDatabase.GetAssetPath(selectedAsset[i]);
                if (path.Contains(".cs"))
                {
                    SetIcon(selectedAsset[i], icon);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            EditorUtility.ClearProgressBar();
        }

        GUI.enabled = false;
    }

    private void SetIcon(Object selectedObject, Texture2D texture)
    {
        var getIconForObject = typeof(EditorGUIUtility).GetMethod("GetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
        var setIcon = (Texture2D)getIconForObject.Invoke(null, new object[] { selectedObject });

        if (setIcon != null && !overwriteIcons)
        {
            return;
        }

        var setIconForObject = typeof(EditorGUIUtility).GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
        setIconForObject.Invoke(null, new object[] { selectedObject, texture });

        var forceReloadInspectors = typeof(EditorUtility).GetMethod("ForceReloadInspectors", BindingFlags.NonPublic | BindingFlags.Static);
        forceReloadInspectors.Invoke(null, null);

        var copyMonoScriptIconToImporters = typeof(MonoImporter).GetMethod("CopyMonoScriptIconToImporters", BindingFlags.NonPublic | BindingFlags.Static);
        copyMonoScriptIconToImporters.Invoke(null, new object[] { selectedObject as MonoScript });
    }
}