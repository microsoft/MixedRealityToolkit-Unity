// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public static class SerializedObjectUtils
    {
        /// <summary>
        /// Copies properties of one serialized object to another (without undo)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool CopySerializedObject(SerializedObject source, SerializedObject target)
        {
            bool madeChanges = false;
            SerializedProperty sourceProp = source.GetIterator();
            while (sourceProp.NextVisible(true))
            {
                switch (sourceProp.name)
                {
                    // This is an odd case where the value is constantly modified, resulting in constant changes.
                    // It's not apparent how this affects rendering.
                    case "m_IndirectSpecularColor":
                        continue;

                    default:
                        break;
                }

                madeChanges |= target.CopyFromSerializedPropertyIfDifferent(sourceProp);
            }

            if (madeChanges)
            {
                target.ApplyModifiedPropertiesWithoutUndo();
            }

            return madeChanges;
        }

        /// <summary>
        /// Uses reflection to set all public fields of a struct in an accompanying serialized property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedProperty"></param>
        /// <param name="value"></param>
        public static void SetStructValue<T>(SerializedProperty serializedProperty, T value) where T : struct
        {
            foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                SerializedProperty fieldProperty = serializedProperty.FindPropertyRelative(field.Name);
                if (fieldProperty == null)
                {
                    Debug.LogError("Couldn't find field " + field.Name + " in serialized property.");
                }
                else
                {
                    Type fieldPropertyType = Type.GetType(fieldProperty.type);
                    if (field.FieldType.IsAssignableFrom(fieldPropertyType))
                    {
                        Debug.LogError("Couldn't assign field " + field.Name + " in serialized property - types do not match.");
                    }
                    else
                    {
                        SetSerializedPropertyByType(field.GetValue(value), field.FieldType, fieldProperty);
                    }
                }
            }
        }

        public static void SetSerializedPropertyByType(object value, Type type, SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    property.boolValue = (bool)value;
                    break;
                case SerializedPropertyType.Integer:
                    property.intValue = (int)value;
                    break;
                case SerializedPropertyType.String:
                    property.stringValue = (string)value;
                    break;
                case SerializedPropertyType.Enum:
                    property.enumValueIndex = (int)value;
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = (float)value;
                    break;
                case SerializedPropertyType.ObjectReference:
                    property.objectReferenceValue = (UnityEngine.Object)value;
                    break;
                default:
                    throw new NotImplementedException("Type " + property.propertyType + " is not implemented.");
            }
        }
    }
}
#endif