// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Inspectors.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(MixedRealityPose))]
    public class MixedRealityPosePropertyDrawer : PropertyDrawer
    {
        private readonly GUIContent positionContent = new GUIContent("Position");
        private readonly GUIContent rotationContent = new GUIContent("Rotation");
        private Quaternion rotationCache = Quaternion.identity;
        private Vector3 rotations = Vector3.zero;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool lastMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.indentLevel++;

            var fieldHeight = position.height / 3f;
            var positionRect = new Rect(position.x, position.y + fieldHeight, position.width, fieldHeight);
            var rotationRect = new Rect(position.x, position.y + fieldHeight * 2, position.width, fieldHeight);

            EditorGUI.PropertyField(positionRect, property.FindPropertyRelative("position"), positionContent);

            var rotationProperty = property.FindPropertyRelative("rotation");
            rotationCache = rotationProperty.quaternionValue;
            Debug.Assert(rotationCache.w.Equals(1f));
            EditorGUI.BeginChangeCheck();
            rotations.x = rotationCache.x;
            rotations.y = rotationCache.y;
            rotations.z = rotationCache.z;
            rotations = EditorGUI.Vector3Field(rotationRect, rotationContent, rotations);

            if (EditorGUI.EndChangeCheck())
            {
                rotationCache.w = 1f;
                rotationCache.x = rotations.x;
                rotationCache.y = rotations.y;
                rotationCache.z = rotations.z;
                rotationProperty.quaternionValue = rotationCache;
            }

            EditorGUI.indentLevel--;
            EditorGUIUtility.wideMode = lastMode;
            EditorGUI.EndProperty();
        }
    }
}