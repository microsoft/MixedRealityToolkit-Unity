// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloToolkit.Unity
{
    // Displays a drop-down menu of GameObjects that are limited to the target object
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DropDownGameObjectAttribute : DrawOverrideAttribute
    {
        public string CustomLabel { get; private set; }

        public DropDownGameObjectAttribute(string customLabel = null)
        {
            CustomLabel = customLabel;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            Transform transform = (target as Component).transform;

            GameObject fieldValue = field.GetValue(target) as GameObject;
            fieldValue = DropDownGameObjectField(
                SplitCamelCase(field.Name),
                fieldValue,
                transform);
            field.SetValue(target, fieldValue);
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            Transform transform = (target as Component).transform;

            GameObject propValue = prop.GetValue(target, null) as GameObject;
            propValue = DropDownGameObjectField(
                SplitCamelCase(prop.Name),
                propValue,
                transform);
            prop.SetValue(target, propValue, null);
        }

        private static GameObject DropDownGameObjectField(string label, GameObject obj, Transform transform)
        {
            Transform[] optionObjects = transform.GetComponentsInChildren<Transform>(true);
            int selectedIndex = 0;
            string[] options = new string[optionObjects.Length + 1];
            options[0] = "(None)";
            for (int i = 0; i < optionObjects.Length; i++)
            {
                options[i + 1] = optionObjects[i].name;
                if (obj == optionObjects[i].gameObject)
                {
                    selectedIndex = i + 1;
                }
            }

            EditorGUILayout.BeginHorizontal();
            int newIndex = EditorGUILayout.Popup(label, selectedIndex, options);
            if (newIndex == 0)
            {
                // Zero means '(None)'
                obj = null;
            }
            else
            {
                obj = optionObjects[newIndex - 1].gameObject;
            }

            //draw the object field so people can click it
            obj = (GameObject)EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            return obj;
        }
#endif

    }
}