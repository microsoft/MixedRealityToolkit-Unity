// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Extensions.EditorClassExtensions;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    public abstract class MixedRealityBaseConfigurationProfileInspector : Editor
    {
        private static readonly GUIContent NewProfileContent = new GUIContent("+", "Create New Profile");

        [SerializeField]
        private Texture2D logoLightTheme = null;

        [SerializeField]
        private Texture2D logoDarkTheme = null;

        protected virtual void Awake()
        {
            if (logoLightTheme == null)
            {
                logoLightTheme = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/MRTK_Logo_Black.png", typeof(Texture2D));
            }

            if (logoDarkTheme == null)
            {
                logoDarkTheme = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/MRTK_Logo_White.png", typeof(Texture2D));
            }
        }

        /// <summary>
        /// Render the Mixed Reality Toolkit Logo.
        /// </summary>
        protected void RenderMixedRealityToolkitLogo()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(EditorGUIUtility.isProSkin ? logoDarkTheme : logoLightTheme, GUILayout.MaxHeight(128f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(12f);
        }

        /// <summary>
        /// Check and make sure we have a Mixed Reality Manager and an active profile.
        /// </summary>
        /// <returns>True if the Mixed Reality Manager is properly initialized.</returns>
        protected bool CheckMixedRealityManager(bool showHelpBox = true)
        {
            if (!MixedRealityManager.IsInitialized)
            {
                // Search the scene for one, in case we've just hot reloaded the assembly.
                var managerSearch = FindObjectsOfType<MixedRealityManager>();

                if (managerSearch.Length == 0)
                {
                    if (showHelpBox)
                    {
                        EditorGUILayout.HelpBox("No Mixed Reality Manager found in scene.", MessageType.Error);
                    }
                    return false;
                }

                MixedRealityManager.ConfirmInitialized();
            }

            if (!MixedRealityManager.HasActiveProfile)
            {
                if (showHelpBox)
                {
                    EditorGUILayout.HelpBox("No Active Profile set on the Mixed Reality Manager.", MessageType.Error);
                }
                return false;
            }

            return true;
        }

        protected static bool RenderProfile(SerializedProperty property)
        {
            bool changed = false;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property);

            if (property.objectReferenceValue == null)
            {
                if (GUILayout.Button(NewProfileContent, EditorStyles.miniButton))
                {
                    var profileTypeName = property.type.Replace("PPtr<$", string.Empty).Replace(">", string.Empty);
                    Debug.Assert(profileTypeName != null, "No Type Found");
                    ScriptableObject profile = CreateInstance(profileTypeName);
                    profile.CreateAsset(AssetDatabase.GetAssetPath(Selection.activeObject));
                    property.objectReferenceValue = profile;
                    changed = true;
                }
            }

            EditorGUILayout.EndHorizontal();
            return changed;
        }
    }
}