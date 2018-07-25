// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Extensions.EditorClassExtensions;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(MixedRealityManager))]
    public class MixedRealityManagerInspector : Editor
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
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(activeProfile);
            bool changed = EditorGUI.EndChangeCheck();
            string commandName = Event.current.commandName;
            var allConfigProfiles = ScriptableObjectExtensions.GetAllInstances<MixedRealityConfigurationProfile>();

            if (activeProfile.objectReferenceValue == null && currentPickerWindow == -1 && checkChange)
            {
                if (allConfigProfiles.Length > 1)
                {
                    EditorUtility.DisplayDialog("Attention!", "You must choose a profile for the Mixed Reality Manager.", "OK");
                    currentPickerWindow = GUIUtility.GetControlID(FocusType.Passive);
                    EditorGUIUtility.ShowObjectPicker<MixedRealityConfigurationProfile>(null, false, string.Empty, currentPickerWindow);
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
                    if (EditorUtility.DisplayDialog("Attention!", "No profiles were found for the Mixed Reality Manager.\n\n" +
                                                                  "Would you like to create one now?", "OK", "Later"))
                    {
                        ScriptableObject profile = CreateInstance(nameof(MixedRealityConfigurationProfile));
                        profile.CreateAsset();
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
                MixedRealityManager.Instance.ResetConfiguration((MixedRealityConfigurationProfile)activeProfile.objectReferenceValue);
            }
        }

        [MenuItem("Mixed Reality Toolkit/Configure...")]
        public static void CreateMixedRealityManagerObject()
        {
            Selection.activeObject = MixedRealityManager.Instance;
            EditorGUIUtility.PingObject(MixedRealityManager.Instance);
        }
    }
}
