// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Base class for all <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/> Inspectors to inherit from.
    /// </summary>
    public abstract class BaseMixedRealityProfileInspector : UnityEditor.Editor
    {
        private const string IsCustomProfileProperty = "isCustomProfile";
        private static readonly GUIContent NewProfileContent = new GUIContent("+", "Create New Profile");
        private static readonly String BaseMixedRealityProfileClassName = typeof(BaseMixedRealityProfile).Name;

        private static StringBuilder dropdownKeyBuilder = new StringBuilder();

        protected virtual void OnEnable()
        {
            if (target == null)
            {
                // Either when we are recompiling, or the inspector window is hidden behind another one, the target can get destroyed (null) and thereby will raise an ArgumentException when accessing serializedObject. For now, just return.
                return;
            }
        }

        /// <summary>
        /// Renders a non-editable object field and an editable dropdown of a profile.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static void RenderReadOnlyProfile(SerializedProperty property)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(property.objectReferenceValue != null ? "" : property.displayName, property.objectReferenceValue, typeof(BaseMixedRealityProfile), false, GUILayout.ExpandWidth(true));      
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            if (property.objectReferenceValue != null)
            {
                UnityEditor.Editor subProfileEditor = UnityEditor.Editor.CreateEditor(property.objectReferenceValue);

                // If this is a default MRTK configuration profile, ask it to render as a sub-profile
                if (typeof(BaseMixedRealityToolkitConfigurationProfileInspector).IsAssignableFrom(subProfileEditor.GetType()))
                {
                    BaseMixedRealityToolkitConfigurationProfileInspector configProfile = (BaseMixedRealityToolkitConfigurationProfileInspector)subProfileEditor;
                    configProfile.RenderAsSubProfile = true;
                }

                EditorGUILayout.BeginHorizontal();
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        subProfileEditor.OnInspectorGUI();
                        EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel--;
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Renders a <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/>.
        /// </summary>
        /// <param name="property">the <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/> property.</param>
        /// <param name="showAddButton">If true, draw the clone button, if false, don't</param>
        /// <param name="renderProfileInBox">if true, render box around profile content, if false, don't</param>
        /// <param name="serviceType">Optional service type to limit available profile types.</param>
        /// <returns>True, if the profile changed.</returns>
        protected static bool RenderProfile(SerializedProperty property, Type profileType, bool showAddButton = true, bool renderProfileInBox = false, Type serviceType = null)
        {
            return RenderProfileInternal(property, profileType, showAddButton, renderProfileInBox, serviceType);
        }

        /// <summary>
        /// Renders a <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/>.
        /// </summary>
        /// <param name="property">the <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/> property.</param>
        /// <param name="showAddButton">If true, draw the clone button, if false, don't</param>
        /// <param name="renderProfileInBox">if true, render box around profile content, if false, don't</param>
        /// <param name="serviceType">Optional service type to limit available profile types.</param>
        /// <returns>True, if the profile changed.</returns>
        private static bool RenderProfileInternal(SerializedProperty property, Type profileType,
            bool showAddButton, bool renderProfileInBox, Type serviceType = null)
        {
            var profile = property.serializedObject.targetObject as BaseMixedRealityProfile;
            bool changed = false;
            var oldObject = property.objectReferenceValue;

            if (profileType != null && !profileType.IsSubclassOf(typeof(BaseMixedRealityProfile)) && profileType != typeof(BaseMixedRealityProfile))
            {  
                // If they've drag-and-dropped a non-profile scriptable object, set it to null.
                profileType = null;
            }

            // If we're constraining this to a service type, check whether the profile is valid
            // If it isn't, issue a warning.
            if (serviceType != null && oldObject != null)
            {
                if (!IsProfileForService(oldObject.GetType(), serviceType))
                {
                    EditorGUILayout.HelpBox("This profile is not supported for " + serviceType.Name + ". Using an unsupported service may result in unexpected behavior.", MessageType.Warning);
                }
            }

            // Find the profile type so we can limit the available object field options
            if (serviceType != null)
            {
                // If GetProfileTypesForService has a count greater than one, then it won't be possible to use
                // EditorGUILayout.ObjectField to restrict the set of profiles to a single type - in this
                // case all profiles of BaseMixedRealityProfile will be visible in the picker.
                // 
                // However in the case where there is just a single profile type for the service, we can improve
                // upon the user experience by limiting the set of things that show in the picker by restricting
                // the set of profiles listed to only that type.
                profileType = GetProfileTypesForService(serviceType).FirstOrDefault();
            }

            // If the profile type is still null, just set it to base profile type
            if (profileType == null)
            {
                profileType = typeof(BaseMixedRealityProfile);
            }

            // Begin the horizontal group
            EditorGUILayout.BeginHorizontal();

                // Draw the object field with an empty label - label is kept in the foldout
                property.objectReferenceValue = EditorGUILayout.ObjectField(oldObject != null ? "" : property.displayName, oldObject, profileType, false, GUILayout.ExpandWidth(true));
                changed = (property.objectReferenceValue != oldObject);

                // Draw the clone button
                if (property.objectReferenceValue == null)
                {
                    var profileTypeName = property.type.Replace("PPtr<$", string.Empty).Replace(">", string.Empty);
                    if (showAddButton && IsConcreteProfileType(profileTypeName))
                    {
                        if (GUILayout.Button(NewProfileContent, EditorStyles.miniButton, GUILayout.Width(20f)))
                        {
                            Debug.Assert(profileTypeName != null, "No Type Found");

                            ScriptableObject instance = CreateInstance(profileTypeName);
                            var newProfile = instance.CreateAsset(AssetDatabase.GetAssetPath(Selection.activeObject)) as BaseMixedRealityProfile;
                            property.objectReferenceValue = newProfile;
                            property.serializedObject.ApplyModifiedProperties();
                            changed = true;
                        }
                    }
                }
                else
                {
                    var renderedProfile = property.objectReferenceValue as BaseMixedRealityProfile;
                    Debug.Assert(renderedProfile != null);
                    Debug.Assert(profile != null, "No profile was set in OnEnable. Did you forget to call base.OnEnable in a derived profile class?");
                    
                    if (GUILayout.Button(new GUIContent("Clone", "Replace with a copy of the default profile."), EditorStyles.miniButton, GUILayout.Width(42f)))
                    {
                        MixedRealityProfileCloneWindow.OpenWindow(profile, renderedProfile, property);
                    }
                }

            EditorGUILayout.EndHorizontal();

            if (property.objectReferenceValue != null)
            {
                UnityEditor.Editor subProfileEditor = UnityEditor.Editor.CreateEditor(property.objectReferenceValue);

                // If this is a default MRTK configuration profile, ask it to render as a sub-profile
                if (typeof(BaseMixedRealityToolkitConfigurationProfileInspector).IsAssignableFrom(subProfileEditor.GetType()))
                {
                    BaseMixedRealityToolkitConfigurationProfileInspector configProfile = (BaseMixedRealityToolkitConfigurationProfileInspector)subProfileEditor;
                    configProfile.RenderAsSubProfile = true;
                }

                var subProfile = property.objectReferenceValue as BaseMixedRealityProfile;
                if (subProfile != null && !subProfile.IsCustomProfile)
                {
                    EditorGUILayout.HelpBox("Clone this default profile to edit properties below", MessageType.Warning); 
                }

                if (renderProfileInBox)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                }
                else
                {
                    EditorGUILayout.BeginVertical();
                }

                    EditorGUILayout.Space();
                    subProfileEditor.OnInspectorGUI();
                    EditorGUILayout.Space();

                EditorGUILayout.EndVertical();
            }

            return changed;
        }

        /// <summary>
        /// Render Bold/HelpBox style Foldout
        /// </summary>
        /// <param name="currentState">reference bool for current visibility state of foldout</param>
        /// <param name="title">Title in foldout</param>
        /// <param name="renderContent">code to execute to render inside of foldout</param>
        protected static void RenderFoldout(ref bool currentState, string title, Action renderContent)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            currentState = EditorGUILayout.Foldout(currentState, title, true, MixedRealityStylesUtility.BoldFoldoutStyle);
            if (currentState)
            {
                renderContent();
            }

            EditorGUILayout.EndVertical();
        }

        private static string GetSubProfileDropdownKey(SerializedProperty property)
        {
            if (property.objectReferenceValue == null)
            {
                throw new Exception("Can't get sub profile dropdown key for a property that is null.");
            }

            dropdownKeyBuilder.Clear();
            dropdownKeyBuilder.Append("MRTK_SubProfile_ShowDropdown_");
            dropdownKeyBuilder.Append(property.name);
            dropdownKeyBuilder.Append("_");
            dropdownKeyBuilder.Append(property.objectReferenceValue.GetType().Name);
            return dropdownKeyBuilder.ToString();
        }

        protected static BaseMixedRealityProfile CreateCustomProfile(BaseMixedRealityProfile sourceProfile)
        {
            if (sourceProfile == null)
            {
                return null;
            }

            ScriptableObject newProfile = CreateInstance(sourceProfile.GetType().ToString());
            BaseMixedRealityProfile targetProfile = newProfile.CreateAsset("Assets/MixedRealityToolkit.Generated/CustomProfiles") as BaseMixedRealityProfile;
            Debug.Assert(targetProfile != null);

            EditorUtility.CopySerialized(sourceProfile, targetProfile);

            var serializedProfile = new SerializedObject(targetProfile);
            serializedProfile.FindProperty(IsCustomProfileProperty).boolValue = true;
            serializedProfile.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();

            if (!sourceProfile.IsCustomProfile)
            {
                // For now we only replace it if it's the master configuration profile.
                // Sub-profiles are easy to update in the master configuration inspector.
                if (MixedRealityToolkit.Instance.ActiveProfile.GetType() == targetProfile.GetType())
                {
                    UnityEditor.Undo.RecordObject(MixedRealityToolkit.Instance, "Copy & Customize Profile");
                    MixedRealityToolkit.Instance.ActiveProfile = targetProfile as MixedRealityToolkitConfigurationProfile;
                }
            }

            return targetProfile;
        }

        /// <summary>
        /// Given a service type, finds all sub-classes of BaseMixedRealityProfile that are
        /// designed to configure that service.
        /// </summary>
        private static IReadOnlyCollection<Type> GetProfileTypesForService(Type serviceType)
        {
            if (serviceType == null)
            {
                return Array.Empty<Type>();
            }

            // This is a little inefficient in that it has to enumerate all of the mixed reality
            // profiles in order to make this enumeration. It would be possible to cache the results
            // of this, but then it would be necessary to listen to file/asset creation/destruction
            // events in order to refresh the cache. If this ends up being a perf bottleneck
            // in inspectors this would be one possible way to alleviate the issue.
            HashSet<Type> allTypes = new HashSet<Type>();
            BaseMixedRealityProfile[] allProfiles = ScriptableObjectExtensions.GetAllInstances<BaseMixedRealityProfile>();
            for (int i = 0; i < allProfiles.Length; i++)
            {
                BaseMixedRealityProfile profile = allProfiles[i];
                if (IsProfileForService(profile.GetType(), serviceType))
                {
                    allTypes.Add(profile.GetType());
                }
            }
            return allTypes.ToReadOnlyCollection();
        }

        /// <summary>
        /// Returns true if the given profile type is designed to configure the given service.
        /// </summary>
        private static bool IsProfileForService(Type profileType, Type serviceType)
        {
            foreach (MixedRealityServiceProfileAttribute serviceProfileAttribute in profileType.GetCustomAttributes(typeof(MixedRealityServiceProfileAttribute), true))
            {
                if (serviceProfileAttribute.ServiceType.IsAssignableFrom(serviceType))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsConcreteProfileType(String profileTypeName)
        {
            return profileTypeName != BaseMixedRealityProfileClassName;
        }

        /// <summary>
        /// Checks if the profile is locked
        /// </summary>
        /// <param name="target"></param>
        /// <param name="lockProfile"></param>
        protected static bool IsProfileLock(BaseMixedRealityProfile profile)
        {
            return MixedRealityPreferences.LockProfiles && !profile.IsCustomProfile;
        }
    }
}
