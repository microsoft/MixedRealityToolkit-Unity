// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.SpatialMapping.SpatialProcessing;
using UnityEditor;

namespace MixedRealityToolkit.SpatialMapping.EditorScript
{
    /// <summary>
    /// Editor extension class to enable multi-selection of the 'Draw Planes' and 'Destroy Planes' options in the Inspector.
    /// </summary>
    [CustomEditor(typeof(SurfaceMeshesToPlanes))]
    public class SurfaceMeshesToPlanesEditor : Editor
    {
        private SerializedProperty drawPlanesMask;
        private SerializedProperty destroyPlanesMask;

        private void OnEnable()
        {
            drawPlanesMask = serializedObject.FindProperty("drawPlanesMask");
            destroyPlanesMask = serializedObject.FindProperty("destroyPlanesMask");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            drawPlanesMask.intValue = (int)((PlaneTypes)EditorGUILayout.EnumMaskField("Draw Planes",
                (PlaneTypes)drawPlanesMask.intValue));

            destroyPlanesMask.intValue = (int)((PlaneTypes)EditorGUILayout.EnumMaskField("Destroy Planes",
                (PlaneTypes)destroyPlanesMask.intValue));

            serializedObject.ApplyModifiedProperties();
        }
    }
}