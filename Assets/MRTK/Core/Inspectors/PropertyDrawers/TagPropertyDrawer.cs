// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Draws a Unity Tag selector in the Inspector.
    /// </summary>
    /// <example>
    /// <code>
    /// [TagProperty]
    /// public string FindTag;
    /// </code>
    /// </example>
    [CustomPropertyDrawer(typeof(TagPropertyAttribute))]
    public class TagPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Override this method to make your own GUI for the property.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, new GUIContent(property.name), property);
            string tagValue = EditorGUI.TagField(position, label, property.stringValue);
            if (tagValue != property.stringValue)
            {
                property.stringValue = tagValue;
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
            EditorGUI.EndProperty();
        }
    }
}
