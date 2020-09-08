// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.UI.Interaction;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Custom inspector for the BaseInteractiveElement class. 
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BaseInteractiveElement))]
    public class BaseInteractiveElementInspector : UnityEditor.Editor
    {
        private BaseInteractiveElement instance;
        private SerializedProperty trackedStates;

        protected virtual void OnEnable()
        {
            instance = (BaseInteractiveElement)target;
            trackedStates = serializedObject.FindProperty("trackedStates");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            RenderTrackedStatesScriptable();

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderTrackedStatesScriptable()
        {
            // Draw the States scriptable object
            InspectorUIUtility.DrawScriptableFoldout<TrackedStates>(trackedStates, "Tracked States", true);

            EditorGUILayout.Space();
        }
    }
}
