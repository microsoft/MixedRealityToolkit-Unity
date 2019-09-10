// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
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
        public static bool CopySerializedObject(SerializedObject source, SerializedObject target, IEnumerable<string> propsToIgnore = null)
        {
            bool madeChanges = false;
            SerializedProperty sourceProp = source.GetIterator();
            while (sourceProp.NextVisible(true))
            {
                if (propsToIgnore != null)
                {
                    foreach (string propToIgnore in propsToIgnore)
                    {
                        if (propToIgnore == sourceProp.name)
                        {
                            continue;
                        }
                    }
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
        /// Iterates through a serialized object's fields and sets any accompanying fields in the supplied struct.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propNamePrefixFilter">Prefix to remove from serialized object field name before searching for match in struct</param>
        public static T CopySerializedObjectToStruct<T>(SerializedObject source, T target, string propNamePrefixFilter = null, bool errorOnFieldNotFound = false) where T : struct
        {
            Type targetType = typeof(T);
            object targetObject = (object)target;

            SerializedProperty sourceProp = source.GetIterator();
            while (sourceProp.NextVisible(true))
            {
                string propName = sourceProp.name;

                if (!string.IsNullOrEmpty(propNamePrefixFilter) && propName.StartsWith(propNamePrefixFilter))
                {
                    propName = propName.Replace(propNamePrefixFilter, "");
                }

                FieldInfo field = targetType.GetField(propName, BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    SetTargetFieldToSerializedPropertyValue(field, ref targetObject, sourceProp);
                }
                if (errorOnFieldNotFound)
                {
                    Debug.LogError("Field " + propName + " not found in struct type " + targetType.Name);
                }
            }

            return (T)targetObject;
        }
        
        /// <summary>
        /// Sets the target field to the value from property based on property type.
        /// </summary>
        public static void SetTargetFieldToSerializedPropertyValue(FieldInfo field, ref object target, SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    field.SetValue(target, property.boolValue);
                    break;
                case SerializedPropertyType.Integer:
                    field.SetValue(target, property.intValue);
                    break;
                case SerializedPropertyType.String:
                    field.SetValue(target, property.stringValue);
                    break;
                case SerializedPropertyType.Enum:
                    field.SetValue(target, property.enumValueIndex);
                    break;
                case SerializedPropertyType.Float:
                    field.SetValue(target, property.floatValue);
                    break;
                case SerializedPropertyType.Color:
                    field.SetValue(target, property.colorValue);
                    break;
                case SerializedPropertyType.Vector2:
                    field.SetValue(target, property.vector2Value);
                    break;
                case SerializedPropertyType.Vector3:
                    field.SetValue(target, property.vector3Value);
                    break;
                case SerializedPropertyType.Vector4:
                    field.SetValue(target, property.vector4Value);
                    break;
                case SerializedPropertyType.ObjectReference:
                    field.SetValue(target, property.objectReferenceValue);
                    break;
                default:
                    throw new NotImplementedException("Type " + property.propertyType + " is not implemented.");
            }
        }

        /// <summary>
        /// Uses reflection to set all public fields of a struct in an accompanying serialized property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void SetStructValue<T>(SerializedProperty serializedProperty, T value) where T : struct
        {
            foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                SerializedProperty sourceProp = serializedProperty.FindPropertyRelative(field.Name);
                if (sourceProp == null)
                {
                    Debug.LogError("Couldn't find field " + field.Name + " in serialized property.");
                }
                else
                {
                    Type sourcePropType = Type.GetType(sourceProp.type);
                    if (!sourcePropType.IsAssignableFrom(field.FieldType))
                    {
                        Debug.LogError("Couldn't assign field " + field.Name + " in serialized property - types do not match.");
                    }
                    else
                    {
                        SetSerializedPropertyByType(field.GetValue(value), sourceProp);
                    }
                }
            }
        }

        /// <summary>
        /// Sets a serialized property value based on type of value object.
        /// </summary>
        public static void SetSerializedPropertyByType(object value, SerializedProperty property)
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