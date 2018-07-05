// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(MixedRealityBoundaryProfile))]
    public class MixedRealityBoundaryProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private SerializedProperty trackingSpaceType;
        private SerializedProperty boundaryHeight;
        private SerializedProperty enablePlatformBoundaryRendering;
        private SerializedProperty createInscribedRectangle;

        private void OnEnable()
        {
            if (!CheckMixedRealityManager(false))
            {
                return;
            }

            trackingSpaceType = serializedObject.FindProperty("trackingSpaceType");
            boundaryHeight = serializedObject.FindProperty("boundaryHeight");
            enablePlatformBoundaryRendering = serializedObject.FindProperty("enablePlatformBoundaryRendering");
            createInscribedRectangle = serializedObject.FindProperty("createInscribedRectangle");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            EditorGUILayout.LabelField("Boundary Profile", EditorStyles.boldLabel);

            if (!CheckMixedRealityManager())
            {
                return;
            }

            EditorGUILayout.HelpBox("The Boundary Profile helps configure playspace settings.", MessageType.Info);

            serializedObject.Update();

            EditorGUILayout.PropertyField(trackingSpaceType, new GUIContent("Tracking Space Type:"));
            if ((TrackingSpaceType)trackingSpaceType.intValue == TrackingSpaceType.RoomScale)
            {
                EditorGUILayout.PropertyField(boundaryHeight, new GUIContent("Boundary Height (in m):"));
                EditorGUILayout.PropertyField(enablePlatformBoundaryRendering, new GUIContent("Render via Platform:"));
                EditorGUILayout.PropertyField(createInscribedRectangle, new GUIContent("Inscribed rectangle:"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}