// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Custom property drawer for <see cref="TimedFlag"/> properties.
    /// Enables inspector-editing of state value if the property is decorated
    /// with <see cref="EditableTimedFlagAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(TimedFlag), true)]
    [CustomPropertyDrawer(typeof(EditableTimedFlagAttribute), true)]
    public class TimedFlagPropertyDrawer : PropertyDrawer
    {
        bool foldout = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (foldout = EditorGUILayout.Foldout(foldout, label, true))
            {
                if (attribute is EditableTimedFlagAttribute flagAttribute)
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("active"));
                }
                EditorGUILayout.PropertyField(property.FindPropertyRelative("onEntered"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("onExited"));
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;
    }
}