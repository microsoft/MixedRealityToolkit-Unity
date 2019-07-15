// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    [CustomEditor(typeof(GridObjectCollection), true)]
    public class GridObjectCollectionInspector : BaseCollectionInspector
    {
        private SerializedProperty surfaceType;
        private SerializedProperty orientType;
        private SerializedProperty layout;
        private SerializedProperty radius;
        private SerializedProperty radialRange;
        private SerializedProperty distance;
        private SerializedProperty rows;
        private SerializedProperty cellWidth;
        private SerializedProperty cellHeight;


        protected override void OnEnable()
        {
            base.OnEnable();
            surfaceType = serializedObject.FindProperty("surfaceType");
            orientType = serializedObject.FindProperty("orientType");
            layout = serializedObject.FindProperty("layout");
            radius = serializedObject.FindProperty("radius");
            distance = serializedObject.FindProperty("distance");
            radialRange = serializedObject.FindProperty("radialRange");
            rows = serializedObject.FindProperty("rows");
            cellWidth = serializedObject.FindProperty("cellWidth");
            cellHeight = serializedObject.FindProperty("cellHeight");
        }

        protected override void OnInspectorGUIInsertion()
        {
            EditorGUILayout.PropertyField(surfaceType);
            EditorGUILayout.PropertyField(orientType);
            EditorGUILayout.PropertyField(layout);
            if ((ObjectOrientationSurfaceType) surfaceType.enumValueIndex == ObjectOrientationSurfaceType.Plane)
            {
                EditorGUILayout.PropertyField(distance);
            }
            else
            {
                EditorGUILayout.PropertyField(radius);
                EditorGUILayout.PropertyField(radialRange);
            }

            LayoutOrder layoutTypeIndex = (LayoutOrder) layout.enumValueIndex;
            if (layoutTypeIndex != LayoutOrder.Horizontal && layoutTypeIndex != LayoutOrder.Vertical)
            {
                EditorGUILayout.PropertyField(rows);
            }
            if (layoutTypeIndex != LayoutOrder.Vertical)
            {
                EditorGUILayout.PropertyField(cellWidth);
            }
            if (layoutTypeIndex != LayoutOrder.Horizontal)
            {
                EditorGUILayout.PropertyField(cellHeight);
            }
        }
    }
}
