// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Solvers
{
    /// <summary>
    /// Custom inspector for the Tap to Place class
    /// </summary>
    [CustomEditor(typeof(TapToPlace))]
    internal class TapToPlaceInspector : UnityEditor.Editor
    {
        protected TapToPlace instance;
        protected SerializedProperty autoStart;
        protected SerializedProperty rotateAccordingToSurface;
        protected SerializedProperty keepOrientationVertical;
        protected SerializedProperty defaultPlacementDistance;
        protected SerializedProperty maxRaycastDistance;
        protected SerializedProperty magneticSurfaces;
        protected SerializedProperty debugEnabled;
        protected SerializedProperty onPlacingStarted;
        protected SerializedProperty onPlacingStopped;

        protected virtual void OnEnable()
        {
            instance = (TapToPlace)target;
            rotateAccordingToSurface = serializedObject.FindProperty("rotateAccordingToSurface");
            defaultPlacementDistance = serializedObject.FindProperty("defaultPlacementDistance");
            magneticSurfaces = serializedObject.FindProperty("magneticSurfaces");
            maxRaycastDistance = serializedObject.FindProperty("maxRaycastDistance");
            keepOrientationVertical = serializedObject.FindProperty("keepOrientationVertical");
            autoStart = serializedObject.FindProperty("autoStart");
            debugEnabled = serializedObject.FindProperty("debugEnabled");
            onPlacingStarted = serializedObject.FindProperty("OnPlacingStarted");
            onPlacingStopped = serializedObject.FindProperty("OnPlacingStopped");
        }

        public override void OnInspectorGUI()
        {
            RenderCustomInspector();
        }

        /// <summary>
        /// Render the custom properties for the tap to place inspector
        /// </summary>
        public virtual void RenderCustomInspector()
        {
            serializedObject.Update();

            // Disable ability to edit through the inspector if in play mode 
            bool isPlayMode = EditorApplication.isPlaying || EditorApplication.isPaused;
            using (new EditorGUI.DisabledScope(isPlayMode))
            {
                EditorGUILayout.PropertyField(autoStart);
            }

            if (!instance.IsColliderPresent)
            {
                EditorGUILayout.HelpBox("A collider needs to be attached to your game object, please attach a collider to use tap to place",MessageType.Error);
            }

            RenderProperties();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Allow editing of the following properties during edit and playmode
        /// </summary>
        public virtual void RenderProperties()
        {
            EditorGUILayout.PropertyField(rotateAccordingToSurface);
            EditorGUILayout.PropertyField(keepOrientationVertical);
            EditorGUILayout.PropertyField(defaultPlacementDistance);
            EditorGUILayout.PropertyField(maxRaycastDistance);
            EditorGUILayout.PropertyField(magneticSurfaces,true);  
            EditorGUILayout.PropertyField(onPlacingStarted);  
            EditorGUILayout.PropertyField(onPlacingStopped);
            EditorGUILayout.PropertyField(debugEnabled);
        }
    }
}
