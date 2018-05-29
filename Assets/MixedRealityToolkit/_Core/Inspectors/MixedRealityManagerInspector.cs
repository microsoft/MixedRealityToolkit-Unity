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
                EditorUtility.DisplayDialog("Attention!", "You must choose a profile for the Mixed Reality Manager.", "OK");
                if (allConfigProfiles.Length > 1)
                {
                    currentPickerWindow = GUIUtility.GetControlID(FocusType.Passive);
                    EditorGUIUtility.ShowObjectPicker<MixedRealityConfigurationProfile>(null, false, string.Empty, currentPickerWindow);
                }
                else if (allConfigProfiles.Length == 1)
                {
                    Debug.Log("No Mixed Reality Configuration Profile was set, so we set the default one for you.");
                    activeProfile.objectReferenceValue = allConfigProfiles[0];
                    changed = true;
                }
                else
                {
                    Debug.LogError("No Mixed Reality Configuration Profiles exist!");
                    // TODO, create one and set it?
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
                        break;
                }
            }

            if (changed)
            {
                serializedObject.ApplyModifiedProperties();
                MixedRealityManager.Instance.ResetConfiguration((MixedRealityConfigurationProfile)activeProfile.objectReferenceValue);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
