using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DefaultAsset))]
public class IconEditor : Editor
{
    private Texture2D icon;

    private bool overwriteIcons;
    private MethodInfo getIconForObject;
    private MethodInfo setIconForObject;
    private MethodInfo forceReloadInspectors;
    private MethodInfo copyMonoScriptIconToImporters;

    private void Awake()
    {
        getIconForObject = typeof(EditorGUIUtility).GetMethod("GetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
        setIconForObject = typeof(EditorGUIUtility).GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
        forceReloadInspectors = typeof(EditorUtility).GetMethod("ForceReloadInspectors", BindingFlags.NonPublic | BindingFlags.Static);
        copyMonoScriptIconToImporters = typeof(MonoImporter).GetMethod("CopyMonoScriptIconToImporters", BindingFlags.NonPublic | BindingFlags.Static);
    }

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
        var setIcon = (Texture2D)getIconForObject.Invoke(null, new object[] { selectedObject });

        if (setIcon != null && !overwriteIcons)
        {
            return;
        }

        setIconForObject.Invoke(null, new object[] { selectedObject, texture });
        forceReloadInspectors.Invoke(null, null);
        copyMonoScriptIconToImporters.Invoke(null, new object[] { selectedObject as MonoScript });
    }
}