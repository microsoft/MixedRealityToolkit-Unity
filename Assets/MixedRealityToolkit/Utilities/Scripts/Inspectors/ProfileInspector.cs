// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Base class for profile inspectors
    /// Profiles are scriptable objects that contain shared information
    /// To ensure that developers understand that they're editing 'global' data, 
    /// this inspector automatically displays a CONSISTENT warning message and 'profile' color to the controls
    /// It also provides a 'target component' so inspectors can differentiate between local / global editing
    /// See compound button component inspectors for usage examples
    /// </summary>
    #if UNITY_EDITOR
    public abstract class ProfileInspector : MRTKEditor
    {
        public Component targetComponent;

        protected override bool DisplayHeader
        {
            get
            {
                return targetComponent == null;
            }
        }

        protected override void BeginInspectorStyle()
        {
            if (targetComponent == null)
            {
                GUI.color = profileColor;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.color = Color.Lerp(profileColor, Color.red, 0.5f);
                EditorGUILayout.LabelField("(Warning: this section edits the button profile. These changes will affect all objects that use this profile.)", EditorStyles.wordWrappedMiniLabel);
                GUI.color = defaultColor;
            }
        }

        protected override void EndInspectorStyle()
        {
            if (targetComponent == null)
            {
                EditorGUILayout.EndVertical();
            }
        }
    }
    #endif
}