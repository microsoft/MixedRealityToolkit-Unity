// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

using InfoType = Microsoft.MixedReality.Toolkit.ShowInfoIfAttribute.InfoType;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Conditionally draws an info box based on the value associated
    /// with the <see cref="ShowInfoIfAttribute"/> attribute.
    /// </summary>
    /// <remarks>
    /// Based on: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
    /// </remarks>
    [CustomPropertyDrawer(typeof(ShowInfoIfAttribute))]
    public class ShowInfoIfPropertyDrawer : PropertyDrawer
    {
        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);

            if (ShouldShow(property))
            {
                ShowInfoIfAttribute showInfoIf = attribute as ShowInfoIfAttribute;

                Rect infoRect = new Rect()
                {
                    x = position.x,
                    y = position.y + EditorGUI.GetPropertyHeight(property),
                    width = position.width,
                    height = EditorGUIUtility.singleLineHeight * 2
                };
                EditorGUI.HelpBox(infoRect, showInfoIf.Message, GetMessageType(showInfoIf.InfoBoxType));
            }
        }

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label);

            if (ShouldShow(property))
            {
                height += EditorGUIUtility.singleLineHeight * 2;
            }
            return height;
        }

        private bool ShouldShow(SerializedProperty property)
        {
            ShowInfoIfAttribute showInfoIf = attribute as ShowInfoIfAttribute;
            if (showInfoIf == null) { return true; }

            string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, showInfoIf.ComparedPropertyName) : showInfoIf.ComparedPropertyName;

            SerializedProperty propertyToCheck = property.serializedObject.FindProperty(path);
            if (propertyToCheck == null)
            {
                Debug.LogError($"ShowInfoIfAttribute couldn't find the SerializedProperty to compare against! (property name: {showInfoIf.ComparedPropertyName})");
                return true;
            }

            switch (propertyToCheck.type)
            {
                case "bool":
                    return showInfoIf.ComparisonMode != ShowInfoIfAttribute.ComparisonType.Equal ^ propertyToCheck.boolValue.Equals(showInfoIf.CompareAgainst);
                case "Enum":
                    return showInfoIf.ComparisonMode != ShowInfoIfAttribute.ComparisonType.Equal ^ propertyToCheck.enumValueIndex.Equals((int)showInfoIf.CompareAgainst);
                default:
                    Debug.LogError($"DrawIfAttribute only supports bool and Enum types. Your property '{showInfoIf.ComparedPropertyName}' is a {propertyToCheck.type}");
                    return true;
            }
        }

        private MessageType GetMessageType(InfoType infoType)
        {
            switch (infoType)
            {
                case InfoType.None: return MessageType.None;
                case InfoType.Info: return MessageType.Info;
                case InfoType.Warning: return MessageType.Warning;
                case InfoType.Error: return MessageType.Error;
            }

            return MessageType.None;
        }
    }
}
