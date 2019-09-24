// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    public class MixedRealityProfileCloneWindow : EditorWindow
    {
        public enum ProfileCloneBehavior
        {
            UseExisting,        // Use the existing reference
            CloneExisting,      // Create a clone of the sub-profile
            UseSubstitution,    // Manually select a profile
            LeaveEmpty,         // Set the reference to null
        }

        private struct SubProfileAction
        {
            public SubProfileAction(
                ProfileCloneBehavior behavior,
                SerializedProperty property,
                Object substitutionReference, 
                System.Type profileType)
            {
                Behavior = behavior;
                Property = property;
                SubstitutionReference = substitutionReference;
                ProfileType = profileType;
                TargetFolder = null;

                CloneName = (SubstitutionReference != null) ? "New " + SubstitutionReference.name : "New " + profileType.Name;
            }

            public ProfileCloneBehavior Behavior;
            public SerializedProperty Property;
            public string CloneName;
            public Object SubstitutionReference;
            public System.Type ProfileType;
            internal Object TargetFolder;
        }

        private const string AdvancedModeKey = "MRTK_ProfileCloneWindow_AdvancedMode_Key";
        private static bool AdvancedMode = false;
        private const string DefaultCustomProfileFolder = "Assets/MixedRealityToolkit.Generated/CustomProfiles";
        private const string IsCustomProfileProperty = "isCustomProfile";
        private static readonly Vector2 MinWindowSizeBasic = new Vector2(500, 180);
        private const float SubProfileSizeMultiplier = 95f;
        private static MixedRealityProfileCloneWindow cloneWindow;

        private BaseMixedRealityProfile parentProfile;
        private BaseMixedRealityProfile childProfile;
        private SerializedProperty childProperty;
        private SerializedObject childSerializedObject;
        private Object targetFolder;
        private Object selectionTarget;
        private string childProfileTypeName;
        private string childProfileAssetName;
        private List<SubProfileAction> subProfileActions = new List<SubProfileAction>();

        public static void OpenWindow(BaseMixedRealityProfile parentProfile, BaseMixedRealityProfile childProfile, SerializedProperty childProperty, Object selectionTarget = null)
        {
            if (cloneWindow != null)
            {
                cloneWindow.Close();
            }

            cloneWindow = (MixedRealityProfileCloneWindow)GetWindow<MixedRealityProfileCloneWindow>(true, "Clone Profile", true);
            cloneWindow.Initialize(parentProfile, childProfile, childProperty, selectionTarget);
            cloneWindow.Show(true);
        }

        private void Initialize(BaseMixedRealityProfile parentProfile, BaseMixedRealityProfile childProfile, SerializedProperty childProperty, Object selectionTarget)
        {
            this.childProperty = childProperty;
            this.parentProfile = parentProfile;
            this.childProfile = childProfile;
            this.selectionTarget = selectionTarget;

            childSerializedObject = new SerializedObject(childProfile);
            childProfileTypeName = childProfile.GetType().Name;
            childProfileAssetName = "New " + childProfileTypeName;

            // Find all the serialized properties for sub-profiles
            SerializedProperty iterator = childSerializedObject.GetIterator();
            System.Type basePropertyType = typeof(BaseMixedRealityProfile);

            while (iterator.Next(true))
            {
                SerializedProperty subProfileProperty = childSerializedObject.FindProperty(iterator.name);

                if (subProfileProperty == null)
                {
                    continue;
                }

                if (!subProfileProperty.type.Contains("PPtr<$")) // Not an object reference type
                {
                    continue;
                }

                string subProfileTypeName = subProfileProperty.type.Replace("PPtr<$", string.Empty).Replace(">", string.Empty).Trim();
                System.Type subProfileType = FindProfileType(subProfileTypeName);
                if (subProfileType == null)
                {
                    continue;
                }

                if (!basePropertyType.IsAssignableFrom(subProfileType))
                {
                    continue;
                }

                subProfileActions.Add(new SubProfileAction(
                    ProfileCloneBehavior.UseExisting,
                    subProfileProperty,
                    subProfileProperty.objectReferenceValue,
                    subProfileType));
            }

            cloneWindow.maxSize = MinWindowSizeBasic;

            targetFolder = EnsureTargetFolder(targetFolder);
        }

        private void OnGUI()
        {
            if (cloneWindow == null || childProfile == null)
            {
                Close();
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.ObjectField("Cloning profile", childProfile, typeof(BaseMixedRealityProfile), false);
            if (parentProfile != null)
            {   // Only show this if we're initiating this from a parent profile
                EditorGUILayout.ObjectField("from parent profile", parentProfile, typeof(BaseMixedRealityProfile), false);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            if (subProfileActions.Count > 0)
            {
                AdvancedMode = EditorGUILayout.Foldout(SessionState.GetBool(AdvancedModeKey, false), "Advanced Options", true, MixedRealityStylesUtility.BoldFoldoutStyle);
                SessionState.SetBool(AdvancedModeKey, AdvancedMode);

                if (AdvancedMode)
                {
                    EditorGUILayout.HelpBox("This profile has sub-profiles. By default your clone will reference the existing profiles. If you want to specify a different profile, or if you want to clone the sub-profile, use the options below.", MessageType.Info);

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    for (int i = 0; i < subProfileActions.Count; i++)
                    {
                        GUI.color = Color.white;
                        EditorGUILayout.Space();

                        SubProfileAction action = subProfileActions[i];

                        action.Behavior = (ProfileCloneBehavior)EditorGUILayout.EnumPopup(action.Property.displayName, action.Behavior);

                        switch (action.Behavior)
                        {
                            case ProfileCloneBehavior.UseExisting:
                                GUI.color = Color.Lerp(Color.white, Color.clear, 0.5f);
                                EditorGUILayout.ObjectField("Existing", action.Property.objectReferenceValue, action.ProfileType, false);
                                break;

                            case ProfileCloneBehavior.UseSubstitution:
                                action.SubstitutionReference = EditorGUILayout.ObjectField("Substitution", action.SubstitutionReference, action.ProfileType, false);
                                break;

                            case ProfileCloneBehavior.CloneExisting:
                                if (action.Property.objectReferenceValue == null)
                                {
                                    EditorGUILayout.LabelField("Can't clone profile - none is set.");
                                }
                                else
                                {
                                    action.CloneName = EditorGUILayout.TextField("Clone name", action.CloneName);
                                }
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    if (action.TargetFolder == null)
                                    {
                                        action.TargetFolder = targetFolder;
                                    }
                                    action.TargetFolder = EditorGUILayout.ObjectField("Target Folder", action.TargetFolder, typeof(DefaultAsset), false);
                                    if (GUILayout.Button("Put in original folder", EditorStyles.miniButton, GUILayout.MaxWidth(120)))
                                    {
                                        string profilePath = AssetDatabase.GetAssetPath(action.Property.objectReferenceValue);
                                        action.TargetFolder = AssetDatabase.LoadAssetAtPath<Object>(System.IO.Path.GetDirectoryName(profilePath));
                                    }
                                }
                                break;

                            case ProfileCloneBehavior.LeaveEmpty:
                                // Add one line for formatting reasons
                                EditorGUILayout.LabelField(" ");
                                break;
                        }
                        subProfileActions[i] = action;
                    }

                    EditorGUILayout.EndVertical();
                }
            }

            GUI.color = Color.white;
            // Space between props and buttons at bottom
            GUILayout.FlexibleSpace();

            // Get the selected folder in the project window
            using (new EditorGUILayout.HorizontalScope())
            {
                targetFolder = EditorGUILayout.ObjectField("Target Folder", targetFolder, typeof(DefaultAsset), false);
                if (GUILayout.Button("Put in original folder", EditorStyles.miniButton, GUILayout.MaxWidth(120)))
                {
                    string profilePath = AssetDatabase.GetAssetPath(childProfile);
                    targetFolder = AssetDatabase.LoadAssetAtPath<Object>(System.IO.Path.GetDirectoryName(profilePath));
                }
            }

            EditorGUILayout.HelpBox("If no folder is provided, the profile will be cloned to the Assets/MixedRealityToolkit.Generated/CustomProfiles folder.", MessageType.Info);
            childProfileAssetName = EditorGUILayout.TextField("Profile Name", childProfileAssetName);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Clone"))
                {
                    targetFolder = EnsureTargetFolder(targetFolder);
                    CloneMainProfile();
                }

                if (GUILayout.Button("Cancel"))
                {
                    cloneWindow.Close();
                }
            }   

            // If there are no sub profiles, limit the max so the window isn't spawned too large
            if (subProfileActions.Count <= 0 || !AdvancedMode)
            {
                cloneWindow.minSize = MinWindowSizeBasic;
                cloneWindow.maxSize = MinWindowSizeBasic;
            }
            else
            {
                Vector2 minWindowSize = MinWindowSizeBasic;
                minWindowSize.y = Mathf.Max(minWindowSize.y, subProfileActions.Count * SubProfileSizeMultiplier);
                cloneWindow.minSize = minWindowSize;
                cloneWindow.maxSize = minWindowSize;
            }

            Repaint();
        }

        private void CloneMainProfile()
        {
            var newChildProfile = CloneProfile(parentProfile, childProfile, childProfileTypeName, childProperty, targetFolder, childProfileAssetName);
            SerializedObject newChildSerializedObject = new SerializedObject(newChildProfile);
            // First paste all values outright
            PasteProfileValues(parentProfile, childProfile, newChildSerializedObject);

            // Then over-write with substitutions or clones
            foreach (SubProfileAction action in subProfileActions)
            {
                SerializedProperty actionProperty = newChildSerializedObject.FindProperty(action.Property.name);

                switch (action.Behavior)
                {
                    case ProfileCloneBehavior.UseExisting:
                        // Do nothing
                        break;

                    case ProfileCloneBehavior.UseSubstitution:
                        // Apply the chosen reference to the new property
                        actionProperty.objectReferenceValue = action.SubstitutionReference;
                        break;

                    case ProfileCloneBehavior.CloneExisting:
                        // Clone the profile, then apply the new reference

                        // If the property reference is null, skip this step, the user was warned
                        if (action.Property.objectReferenceValue == null)
                        {
                            break;
                        }

                        // If for some reason it's the wrong type, bail now
                        BaseMixedRealityProfile subProfileToClone = (BaseMixedRealityProfile)action.Property.objectReferenceValue;
                        if (subProfileToClone == null)
                        {
                            break;
                        }

                        // Clone the sub profile
                        Object subTargetFolder = (action.TargetFolder == null) ? targetFolder : action.TargetFolder;
                        var newSubProfile = CloneProfile(newChildProfile, subProfileToClone, action.ProfileType.Name, actionProperty, subTargetFolder, action.CloneName);
                        SerializedObject newSubProfileSerializedObject = new SerializedObject(newSubProfile);
                        // Paste values from existing profile
                        PasteProfileValues(newChildProfile, subProfileToClone, newSubProfileSerializedObject);
                        newSubProfileSerializedObject.ApplyModifiedProperties();
                        break;

                    case ProfileCloneBehavior.LeaveEmpty:
                        actionProperty.objectReferenceValue = null;
                        break;
                }
            }

            newChildSerializedObject.ApplyModifiedProperties();

            // If we're not working with a parent profile, select the newly created profile
            // UNLESS we've been given a selection target
            if (selectionTarget != null)
            {
                Selection.activeObject = selectionTarget;
            }
            else
            {
                if (parentProfile == null)
                {
                    Selection.activeObject = newChildProfile;
                }
            }

            cloneWindow.Close();
        }

        private static BaseMixedRealityProfile CloneProfile(BaseMixedRealityProfile parentProfile, BaseMixedRealityProfile profileToClone, string childProfileTypeName, SerializedProperty childProperty, Object targetFolder, string profileName)
        {
            ScriptableObject instance = CreateInstance(childProfileTypeName);
            instance.name = string.IsNullOrEmpty(profileName) ? childProfileTypeName : profileName;

            string fileName = instance.name;
            string path = AssetDatabase.GetAssetPath(targetFolder);
            Debug.Log("Creating asset in path " + targetFolder);

            var newChildProfile = instance.CreateAsset(path, fileName) as BaseMixedRealityProfile;

            if (childProperty != null)
            {
                childProperty.objectReferenceValue = newChildProfile;
                childProperty.serializedObject.ApplyModifiedProperties();
            }

            return newChildProfile;
        }

        private static void PasteProfileValues(BaseMixedRealityProfile parentProfile, BaseMixedRealityProfile profileToCopy, SerializedObject targetProfile)
        {
            if (parentProfile != null)
            {
                Undo.RecordObject(parentProfile, "Paste Profile Values");
            }

            bool targetIsCustom = targetProfile.FindProperty(IsCustomProfileProperty).boolValue;
            string originalName = targetProfile.targetObject.name;
            EditorUtility.CopySerialized(profileToCopy, targetProfile.targetObject);
            targetProfile.Update();
            targetProfile.FindProperty(IsCustomProfileProperty).boolValue = targetIsCustom;
            targetProfile.ApplyModifiedProperties();
            targetProfile.targetObject.name = originalName;

            AssetDatabase.SaveAssets();
        }

        private static System.Type FindProfileType(string profileTypeName)
        {
            System.Type type = null;
            foreach (Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (System.Type checkType in assembly.GetLoadableTypes())
                {
                    if (checkType.Name == profileTypeName)
                    {
                        type = checkType;
                        break;
                    }
                }
            }

            return type;
        }

        /// <summary>
        /// If the targetFolder is invalid asset folder, this will create the CustomProfiles
        /// folder and use that as the default target.
        /// </summary>
        private static Object EnsureTargetFolder(Object targetFolder)
        {
            if (targetFolder != null && AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(targetFolder)))
            {
                return targetFolder;
            }

            if (!AssetDatabase.IsValidFolder(DefaultCustomProfileFolder))
            {
                // AssetDatabase.CreateFolder must be called to create each child of the asset folder
                // path individually. 
                // Calling AssetDatabase.CreateFolder("Assets", "MixedRealityToolkit.Generated/CustomProfiles")
                // generates a folder that looks like "Assets/MixedRealityToolkit.Generated_CustomProfiles".
                AssetDatabase.CreateFolder("Assets", "MixedRealityToolkit.Generated");
                AssetDatabase.CreateFolder("Assets/MixedRealityToolkit.Generated", "CustomProfiles");
            }
            return AssetDatabase.LoadAssetAtPath(DefaultCustomProfileFolder, typeof(Object));
        }
    }
}