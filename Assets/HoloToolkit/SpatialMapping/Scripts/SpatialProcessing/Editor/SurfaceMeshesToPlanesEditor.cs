// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace HoloToolkit.Unity.SpatialMapping
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

#if UNITY_2017_3_OR_NEWER
            drawPlanesMask.intValue = (int)((PlaneTypes)EditorGUILayout.EnumFlagsField("Draw Planes",
                (PlaneTypes)drawPlanesMask.intValue));
#else
            drawPlanesMask.intValue = (int)((PlaneTypes)EditorGUILayout.EnumMaskField("Draw Planes",
                (PlaneTypes)drawPlanesMask.intValue));
#endif

#if UNITY_2017_3_OR_NEWER
            destroyPlanesMask.intValue = (int)((PlaneTypes)EditorGUILayout.EnumFlagsField("Destroy Planes",
                (PlaneTypes)destroyPlanesMask.intValue));
#else
            destroyPlanesMask.intValue = (int)((PlaneTypes)EditorGUILayout.EnumMaskField("Destroy Planes",
                (PlaneTypes)destroyPlanesMask.intValue));
#endif

            serializedObject.ApplyModifiedProperties();
        }
    }
}