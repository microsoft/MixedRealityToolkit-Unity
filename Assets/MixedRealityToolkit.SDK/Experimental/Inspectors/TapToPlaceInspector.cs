// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Inspectors
{
    [CustomEditor(typeof(TapToPlace))]
    internal class TapToPlaceInspector : UnityEditor.Editor
    {
        protected TapToPlace instance;
        protected SerializedProperty gameObjectToPlace;
        protected SerializedProperty autoStart;
        protected SerializedProperty rotateAccordingToSurface;
        protected SerializedProperty keepOrientationVertical;
        protected SerializedProperty spatialMeshVisible;
        protected SerializedProperty defaultPlacementDistance;
        protected SerializedProperty maxRaycastDistance;
        protected SerializedProperty magneticSurfaces;

        protected virtual void OnEnable()
        {
            instance = (TapToPlace)target;
            gameObjectToPlace = serializedObject.FindProperty("gameObjectToPlace");
            rotateAccordingToSurface = serializedObject.FindProperty("rotateAccordingToSurface");
            defaultPlacementDistance = serializedObject.FindProperty("defaultPlacementDistance");
            magneticSurfaces = serializedObject.FindProperty("magneticSurfaces");
            maxRaycastDistance = serializedObject.FindProperty("maxRaycastDistance");
            keepOrientationVertical = serializedObject.FindProperty("keepOrientationVertical");
            spatialMeshVisible = serializedObject.FindProperty("spatialMeshVisible");
            autoStart = serializedObject.FindProperty("autoStart");
        }

        public override void OnInspectorGUI()
        {
            RenderCustomInspector();
        }

        public virtual void RenderCustomInspector()
        {
            serializedObject.Update();

            // Disable ability to edit through the inspector if in play mode 
            bool isPlayMode = EditorApplication.isPlaying || EditorApplication.isPaused;
            using (new EditorGUI.DisabledScope(isPlayMode))
            {
                EditorGUILayout.PropertyField(gameObjectToPlace);
                EditorGUILayout.PropertyField(autoStart);
            }

            // If the GameObjectToPlace is null set it to the gameobject
            if (instance.GameObjectToPlace == null)
            {
                instance.GameObjectToPlace = instance.gameObject;
            }

            if (!instance.ColliderPresent)
            {
                Debug.LogError("A collider needs to be attached to your game object, please attach a collider to use tap to place");
            }

            UpdateProperties();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Allow editing of the following properties during edit and playmode
        /// </summary>
        public virtual void UpdateProperties()
        {
            EditorGUILayout.PropertyField(rotateAccordingToSurface);
            EditorGUILayout.PropertyField(keepOrientationVertical);
            EditorGUILayout.PropertyField(spatialMeshVisible);
            EditorGUILayout.PropertyField(defaultPlacementDistance);
            EditorGUILayout.PropertyField(maxRaycastDistance);
            EditorGUILayout.PropertyField(magneticSurfaces,true);  
        }
    }
}
