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
        private bool checkChange = false;

        private void OnEnable()
        {
            activeProfile = serializedObject.FindProperty("activeProfile");
            currentPickerWindow = -1;
            checkChange = activeProfile.objectReferenceValue == null;
        }

        public override void OnInspectorGUI()
        {
            MixedRealityToolkit instance = (MixedRealityToolkit)target;

            if (MixedRealityToolkit.Instance == null)
            {   // See if an active instance exists at all. If it doesn't register this instance preemptively.
                MixedRealityToolkit.SetActiveInstance(instance);
            }

            if (!instance.IsActiveInstance)
            {
                EditorGUILayout.HelpBox("This instance of the toolkit is inactive. There can only be one active instance loaded at any time.", MessageType.Warning);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select Active Instance"))
                {
                    UnityEditor.Selection.activeGameObject = MixedRealityToolkit.Instance.gameObject;
                }
                if (GUILayout.Button("Make this the Active Instance"))
                {
                    MixedRealityToolkit.SetActiveInstance(instance);
                }
                EditorGUILayout.EndHorizontal();
                return;
            }

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(activeProfile);
            bool changed = EditorGUI.EndChangeCheck();
            string commandName = Event.current.commandName;
            var allConfigProfiles = ScriptableObjectExtensions.GetAllInstances<MixedRealityToolkitConfigurationProfile>();

            if (activeProfile.objectReferenceValue == null && currentPickerWindow == -1 && checkChange && !BuildPipeline.isBuildingPlayer)
            {
                if (allConfigProfiles.Length > 1)
                {
                    EditorUtility.DisplayDialog("Attention!", "You must choose a profile for the Mixed Reality Toolkit.", "OK");
                    currentPickerWindow = GUIUtility.GetControlID(FocusType.Passive);
                    // Shows the list of MixedRealityToolkitConfigurationProfiles in our project,
                    // selecting the default profile by default (if it exists).
                    EditorGUIUtility.ShowObjectPicker<MixedRealityToolkitConfigurationProfile>(MixedRealityInspectorUtility.GetDefaultConfigProfile(allConfigProfiles), false, string.Empty, currentPickerWindow);
                }
                else if (allConfigProfiles.Length == 1)
                {
                    activeProfile.objectReferenceValue = allConfigProfiles[0];
                    changed = true;
                    Selection.activeObject = allConfigProfiles[0];
                    EditorGUIUtility.PingObject(allConfigProfiles[0]);
                }
                else
                {
                    if (EditorUtility.DisplayDialog("Attention!", "No profiles were found for the Mixed Reality Toolkit.\n\n" +
                                                                  "Would you like to create one now?", "OK", "Later"))
                    {
                        ScriptableObject profile = CreateInstance(nameof(MixedRealityToolkitConfigurationProfile));
                        profile.CreateAsset("Assets/MixedRealityToolkit.Generated/CustomProfiles");
                        activeProfile.objectReferenceValue = profile;
                        Selection.activeObject = profile;
                        EditorGUIUtility.PingObject(profile);
                    }
                }

                checkChange = false;
            }

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
                        Selection.activeObject = activeProfile.objectReferenceValue;
                        EditorGUIUtility.PingObject(activeProfile.objectReferenceValue);
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
            EditorGUIUtility.PingObject(MixedRealityToolkit.Instance);
        }
    }
}
