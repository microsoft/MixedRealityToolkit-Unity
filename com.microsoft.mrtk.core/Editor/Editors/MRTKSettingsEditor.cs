// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Main editor that appears in Project Settings -> XR Plug-in Management -> MRTK Settings
    /// This allows the user to edit the per-platform profiles, and the configurations stored therein.
    /// </summary>
    [CustomEditor(typeof(MRTKSettings))]
    public class MRTKSettingsEditor : UnityEditor.Editor
    {
        // Serialized property corresponding to the serialized dictionary of
        // build targets to profile references.
        private SerializedProperty profileDict;

        // Cached profile editor reference to reduce the creation of
        // new editors every frame.
        private UnityEditor.Editor activeProfileEditor;

        private Vector2 scrollPosition = Vector2.zero;

        private GUIContent profileLabel = new GUIContent("Profile");

        private const string MRTKDefaultProfileGUID = "c677e5c4eb85b7849a8da406775c299d";

        private void OnEnable()
        {
            profileDict = serializedObject.FindProperty("settings");
        }

        public override void OnInspectorGUI()
        {
            if (serializedObject == null || serializedObject.targetObject == null)
            {
                return;
            }

            serializedObject.Update();

            // Split the editor by build target.
            BuildTargetGroup buildTargetGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();

            // Retrieve the profile associated with the current build target (as specified by
            // the BuildTargetSelectionGrouping)
            SerializedProperty profile = GetProfileForBuildTarget(buildTargetGroup);

            // If no profile is assigned, then warn user.
            if (profile.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("MixedRealityToolkit cannot initialize unless an Active Profile is assigned!", MessageType.Error);
                if (GUILayout.Button(new GUIContent("Assign MRTK Default")))
                {
                    profile.objectReferenceValue = AssetDatabase.LoadAssetAtPath<MRTKProfile>(AssetDatabase.GUIDToAssetPath(MRTKDefaultProfileGUID));
                }
            }

            EditorGUILayout.PropertyField(profile, profileLabel);

            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollView.scrollPosition;
                if (profile.objectReferenceValue != null)
                {
                    CreateCachedEditor(profile.objectReferenceValue, null, ref activeProfileEditor);

                    if (activeProfileEditor != null)
                    {
                        activeProfileEditor.OnInspectorGUI();
                    }
                }
            }

            EditorGUILayout.EndBuildTargetSelectionGrouping();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Attempts to retrieve the profile associated with the specified build target.
        /// If there is no key in the dictionary for the specified build target,
        /// this will insert a new entry with the build target.
        /// </summary>
        private SerializedProperty GetProfileForBuildTarget(BuildTargetGroup target)
        {
            SerializedProperty keyValArray = profileDict.FindPropertyRelative("entries");
            foreach (SerializedProperty keyVal in keyValArray)
            {
                if ((int)target == keyVal.FindPropertyRelative("key").intValue)
                {
                    return keyVal.FindPropertyRelative("value");
                }
            }

            // No entry in the dict for this build target group. Insert one now!
            keyValArray.InsertArrayElementAtIndex(0);
            keyValArray.GetArrayElementAtIndex(0).FindPropertyRelative("key").intValue = (int)target;

            SerializedProperty newValue = keyValArray.GetArrayElementAtIndex(0).FindPropertyRelative("value");
            newValue.objectReferenceValue = null;

            return newValue;
        }
    }
}
