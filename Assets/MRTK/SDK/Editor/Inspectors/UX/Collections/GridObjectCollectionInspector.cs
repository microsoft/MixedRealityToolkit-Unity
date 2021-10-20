// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

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
        private SerializedProperty cols;
        private SerializedProperty cellWidth;
        private SerializedProperty cellHeight;
        private SerializedProperty anchor;
        private SerializedProperty anchorAlongAxis;
        private SerializedProperty rowAlignment;
        private SerializedProperty columnAlignment;


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
            cols = serializedObject.FindProperty("columns");
            cellWidth = serializedObject.FindProperty("cellWidth");
            cellHeight = serializedObject.FindProperty("cellHeight");
            anchor = serializedObject.FindProperty("anchor");
            anchorAlongAxis = serializedObject.FindProperty("anchorAlongAxis");
            rowAlignment = serializedObject.FindProperty("rowAlignment");
            columnAlignment = serializedObject.FindProperty("columnAlignment");
        }

        protected override void OnInspectorGUIInsertion()
        {
            EditorGUILayout.PropertyField(surfaceType);
            EditorGUILayout.PropertyField(orientType);
            EditorGUILayout.PropertyField(layout);



            LayoutOrder layoutTypeIndex = (LayoutOrder)layout.intValue;
            if (layoutTypeIndex == LayoutOrder.ColumnThenRow)
            {
                EditorGUILayout.HelpBox("ColumnThenRow will lay out content first horizontally (by column), then vertically (by row). NumColumns specifies number of columns per row.", MessageType.Info);
                EditorGUILayout.PropertyField(cols, new GUIContent("Num Columns", "Number of columns per row."));
                EditorGUILayout.PropertyField(columnAlignment);
            }
            else if (layoutTypeIndex == LayoutOrder.RowThenColumn)
            {
                EditorGUILayout.HelpBox("RowThenColumns will lay out content first vertically (by row), then horizontally (by column). NumRows specifies number of rows per column.", MessageType.Info);
                EditorGUILayout.PropertyField(rows, new GUIContent("Num Rows", "Number of rows per column."));
                EditorGUILayout.PropertyField(rowAlignment);
            }
            else
            {
                // do not show rows / cols field 
            }

            if (layoutTypeIndex != LayoutOrder.Vertical)
            {
                EditorGUILayout.PropertyField(cellWidth);
            }
            if (layoutTypeIndex != LayoutOrder.Horizontal)
            {
                EditorGUILayout.PropertyField(cellHeight);
            }

            ObjectOrientationSurfaceType surfaceTypeIndex = (ObjectOrientationSurfaceType)surfaceType.intValue;
            if (surfaceTypeIndex == ObjectOrientationSurfaceType.Plane)
            {
                EditorGUILayout.PropertyField(distance, new GUIContent("Distance from parent", "Distance from parent object's origin"));
            }
            else
            {
                EditorGUILayout.PropertyField(radius);
                EditorGUILayout.PropertyField(radialRange);
            }

            if (surfaceTypeIndex != ObjectOrientationSurfaceType.Radial)
            {
                // layout anchor has no effect on radial layout, it is always at center.
                EditorGUILayout.PropertyField(anchor);
            }

            if ((LayoutAnchor)anchor.intValue != LayoutAnchor.MiddleCenter)
            {
                EditorGUILayout.PropertyField(anchorAlongAxis);
            }
        }
    }
}
