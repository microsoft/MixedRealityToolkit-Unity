// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Inspector for themes, and used by Interactable
    /// </summary>

#if UNITY_EDITOR
    [CustomEditor(typeof(InteractableProfileItem))]
    public class ProfileItemInspector : UnityEditor.Editor
    {
        protected SerializedProperty settings;

        protected static InteractableTypesContainer themeOptions;
        protected static string[] shaderOptions;
        protected static State[] themeStates;

        protected GUIStyle boxStyle;
        protected bool layoutComplete = false;
        private const float ThemeStateFontScale = 1.2f;

        protected virtual void OnEnable()
        {
            //settings = serializedObject.FindProperty("Settings");
            //SetupThemeOptions();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("TesT!!!!");
            base.OnInspectorGUI();
        }

    }
#endif
}
