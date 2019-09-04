// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(MixedRealityToolkit))]
    public class MixedRealityToolkitInspector : UnityEditor.Editor
    {
        private SerializedProperty activeProfile;
        private int currentPickerWindow = -1;

        // Utility to show object picker for ActiveProfile property since Show command must be called in OnGUI()
        private static bool forceShowProfilePicker = false;

        private void OnEnable()
        {
            activeProfile = serializedObject.FindProperty("activeProfile");
            currentPickerWindow = -1;
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
                        UnityEditor.Selection.activeGameObject = MixedRealityToolkit.Instance.gameObject;
                    }

                    if (GUILayout.Button("Make this the Active Instance"))
                    {
                        MixedRealityToolkit.SetActiveInstance(instance);
                    }
                }
                return;
            }

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(activeProfile);
            bool changed = EditorGUI.EndChangeCheck();
            string commandName = Event.current.commandName;

            // If not profile is assigned, then warn user
            if (activeProfile.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("MixedRealityToolkit cannot initialize unless an Active Profile is assigned!", MessageType.Error);

                if (GUILayout.Button("Assign MixedRealityToolkit Profile") || forceShowProfilePicker)
                {
                    forceShowProfilePicker = false;

                    var allConfigProfiles = ScriptableObjectExtensions.GetAllInstances<MixedRealityToolkitConfigurationProfile>();

                    // Shows the list of MixedRealityToolkitConfigurationProfiles in our project,
                    // selecting the default profile by default (if it exists).
                    if (allConfigProfiles.Length > 0)
                    {
                        currentPickerWindow = GUIUtility.GetControlID(FocusType.Passive);

                        var defaultMRTKProfile = MixedRealityInspectorUtility.GetDefaultConfigProfile(allConfigProfiles);
                        activeProfile.objectReferenceValue = defaultMRTKProfile;

                        EditorGUIUtility.ShowObjectPicker<MixedRealityToolkitConfigurationProfile>(defaultMRTKProfile, false, string.Empty, currentPickerWindow);
                    }
                    else
                    {
                        if (EditorUtility.DisplayDialog("Attention!", "No profiles were found for the Mixed Reality Toolkit.\n\n" +
                                                                      "Would you like to create one now?", "OK", "Later"))
                        {
                            ScriptableObject profile = CreateInstance(nameof(MixedRealityToolkitConfigurationProfile));
                            profile.CreateAsset("Assets/MixedRealityToolkit.Generated/CustomProfiles");
                            activeProfile.objectReferenceValue = profile;
                            EditorGUIUtility.PingObject(profile);
                        }
                    }
                }

            }

            // If user selects a new MRTK Active Profile, then update configuration
            if (EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow)
            {
                switch (commandName)
                {
                    case "ObjectSelectorUpdated":
                        activeProfile.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
                        changed = true;
                        break;
                    case "ObjectSelectorClosed":
                        activeProfile.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
                        currentPickerWindow = -1;
                        changed = true;
                        break;
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                MixedRealityToolkit.Instance.ResetConfiguration((MixedRealityToolkitConfigurationProfile)activeProfile.objectReferenceValue);
            }

            if (activeProfile.objectReferenceValue != null)
            {
                UnityEditor.Editor activeProfileEditor = CreateEditor(activeProfile.objectReferenceValue);
                activeProfileEditor.OnInspectorGUI();
            }
        }

        [MenuItem("Mixed Reality Toolkit/Add to Scene and Configure...")]
        public static void CreateMixedRealityToolkitGameObject()
        {
            MixedRealityInspectorUtility.AddMixedRealityToolkitToScene();
            Selection.activeObject = MixedRealityToolkit.Instance;
            EditorGUIUtility.PingObject(MixedRealityToolkit.Instance);

            if (!MixedRealityToolkit.Instance.HasActiveProfile)
            {
                forceShowProfilePicker = true;
            }
        }
    }
}
