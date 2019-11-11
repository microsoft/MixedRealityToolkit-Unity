// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Editor
{
    [CustomEditor(typeof(InteractableToggleCollection))]
    /// <summary>
    /// Custom inspector for InteractableToggleCollection
    /// </summary>
    internal class InteractableToggleCollectionInspector : UnityEditor.Editor
    {
        protected InteractableToggleCollection instance;
        protected SerializedProperty toggleListProperty;
        protected SerializedProperty currentIndexProperty;

        protected virtual void OnEnable()
        {
            instance = (InteractableToggleCollection)target;
            toggleListProperty = serializedObject.FindProperty("toggleList");
            currentIndexProperty = serializedObject.FindProperty("currentIndex");
        }

        public override void OnInspectorGUI()
        {
            RenderCustomInspector();

            if (Application.isPlaying && instance != null && GUI.changed)
            {
                int currentIndex = instance.CurrentIndex;
                currentIndex = Mathf.Clamp(currentIndex, 0, instance.ToggleList.Length - 1);

                if (currentIndex >= instance.ToggleList.Length || currentIndex < 0)
                {
                    Debug.Log("Index out of range: " + currentIndex);
                }
                else
                {
                    instance.SetSelection(currentIndex, true, true);
                }  
            }
        }

        public virtual void RenderCustomInspector()
        {
            serializedObject.Update();

            // Disable ability to edit ToggleList through the inspector if in play mode 
            bool isPlayMode = EditorApplication.isPlaying || EditorApplication.isPaused;
            using (new EditorGUI.DisabledScope(isPlayMode))
            {
                EditorGUILayout.PropertyField(toggleListProperty, true);
            }

            EditorGUILayout.PropertyField(currentIndexProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
