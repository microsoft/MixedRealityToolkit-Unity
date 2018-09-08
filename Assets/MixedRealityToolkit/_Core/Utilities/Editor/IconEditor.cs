using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Editor
{
    [CustomEditor(typeof(DefaultAsset))]
    public class IconEditor : UnityEditor.Editor
    {
        private Texture2D icon;
        private string filter;
        private string[] filters;
        private bool filterFlag;
        private bool overwriteIcons;
        private MethodInfo getIconForObject;
        private MethodInfo setIconForObject;
        private MethodInfo forceReloadInspectors;
        private MethodInfo copyMonoScriptIconToImporters;

        private void OnEnable()
        {
            if (getIconForObject == null)
            {
                getIconForObject = typeof(EditorGUIUtility).GetMethod("GetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
            }

            if (setIconForObject == null)
            {
                setIconForObject = typeof(EditorGUIUtility).GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
            }

            if (forceReloadInspectors == null)
            {
                forceReloadInspectors = typeof(EditorUtility).GetMethod("ForceReloadInspectors", BindingFlags.NonPublic | BindingFlags.Static);
            }

            if (copyMonoScriptIconToImporters == null)
            {
                copyMonoScriptIconToImporters = typeof(MonoImporter).GetMethod("CopyMonoScriptIconToImporters", BindingFlags.NonPublic | BindingFlags.Static);
            }
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = true;
            icon = (Texture2D)EditorGUILayout.ObjectField("Icon Texture", icon, typeof(Texture2D), false);
            filter = EditorGUILayout.TextField(new GUIContent("Partial name filters", "Use comma separated values for each partial name search."), filter);
            filterFlag = EditorGUILayout.Toggle(filterFlag ? "Skipping filter results" : "Targeting filter results", filterFlag);

            EditorGUI.BeginChangeCheck();
            overwriteIcons = EditorGUILayout.Toggle("Overwrite Icon?", overwriteIcons);

            if (GUILayout.Button("Set Icons for child script assets"))
            {
                filters = !string.IsNullOrEmpty(filter) ? filter.Split(',') : null;

                Object[] selectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
                for (int i = 0; i < selectedAsset.Length; i++)
                {
                    EditorUtility.DisplayProgressBar("Updating Icons...", $"{i} of {selectedAsset.Length} {selectedAsset[i].name}", i / (float)selectedAsset.Length);
                    var path = AssetDatabase.GetAssetPath(selectedAsset[i]);
                    if (!path.Contains(".cs")) { continue; }

                    if (filters != null)
                    {
                        bool matched = filterFlag;
                        for (int j = 0; j < filters.Length; j++)
                        {
                            if (selectedAsset[i].name.ToLower().Contains(filters[j].ToLower()))
                            {
                                matched = !filterFlag;
                            }
                        }

                        if (overwriteIcons && !matched ||
                           !overwriteIcons && matched)
                        {
                            continue;
                        }
                    }

                    SetIcon(selectedAsset[i], icon, overwriteIcons);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                EditorUtility.ClearProgressBar();
            }

            GUI.enabled = false;
        }

        private void SetIcon(Object selectedObject, Texture2D texture, bool overwrite)
        {
            var setIcon = (Texture2D)getIconForObject.Invoke(null, new object[] { selectedObject });

            if (setIcon != null && !overwrite)
            {
                return;
            }

            setIconForObject.Invoke(null, new object[] { selectedObject, texture });
            forceReloadInspectors.Invoke(null, null);
            copyMonoScriptIconToImporters.Invoke(null, new object[] { selectedObject as MonoScript });
        }
    }
}