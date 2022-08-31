// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(MixedRealityToolkit))]
    public class MixedRealityToolkitInspector : UnityEditor.Editor
    {
        private SerializedProperty activeProfile;
        private UnityEditor.Editor activeProfileEditor;
        private Object cachedProfile;

        private void OnEnable()
        {
            activeProfile = serializedObject.FindProperty("activeProfile");
            cachedProfile = activeProfile.objectReferenceValue;
        }

        public override void OnInspectorGUI()
        {
            MixedRealityToolkit instance = (MixedRealityToolkit)target;

            if (MixedRealityToolkit.Instance == null && instance.isActiveAndEnabled)
            {   // See if an active instance exists at all. If it doesn't register this instance preemptively.
                MixedRealityToolkit.SetActiveInstance(instance);
            }

            if (!instance.IsActiveInstance)
            {
                EditorGUILayout.HelpBox("This instance of the toolkit is inactive. There can only be one active instance loaded at any time.", MessageType.Warning);
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Select Active Instance"))
                    {
                        Selection.activeGameObject = MixedRealityToolkit.Instance.gameObject;
                    }

                    if (GUILayout.Button("Make this the Active Instance"))
                    {
                        MixedRealityToolkit.SetActiveInstance(instance);
                    }
                }
                return;
            }

            serializedObject.Update();

            // If no profile is assigned, then warn user
            if (activeProfile.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("MixedRealityToolkit cannot initialize unless an Active Profile is assigned!", MessageType.Error);
            }

            bool changed = MixedRealityInspectorUtility.DrawProfileDropDownList(activeProfile, null, activeProfile.objectReferenceValue, typeof(MixedRealityToolkitConfigurationProfile), false, false) ||
                cachedProfile != activeProfile.objectReferenceValue;

            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                TryResetConfiguration();
            }

            if (activeProfile.objectReferenceValue != null && activeProfileEditor == null)
            {
                // For the configuration profile, show the default inspector GUI
                activeProfileEditor = CreateEditor(activeProfile.objectReferenceValue);
            }

            if (activeProfileEditor != null)
            {
                activeProfileEditor.OnInspectorGUI();
            }
        }

        private void TryResetConfiguration()
        {
            var newProfile = (MixedRealityToolkitConfigurationProfile)activeProfile.objectReferenceValue;
            try
            {
                if (!Application.isPlaying)
                {
                    MixedRealityToolkit.Instance.ResetConfiguration(newProfile);
                }
                else
                {
                    MixedRealityToolkit.Instance.ActiveProfile = newProfile;
                }

                activeProfileEditor = null;
                cachedProfile = activeProfile.objectReferenceValue;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to switch MRTK profile to {newProfile?.name}:\n{e}");
            }
        }

        [MenuItem("Mixed Reality/Toolkit/Add to Scene and Configure...")]
        public static void CreateMixedRealityToolkitGameObject()
        {
            MixedRealityInspectorUtility.AddMixedRealityToolkitToScene();
            Selection.activeObject = MixedRealityToolkit.Instance;
            EditorGUIUtility.PingObject(MixedRealityToolkit.Instance);
        }
    }
}
