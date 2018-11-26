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
    // Displays a drop-down menu of Component objects that are limited to the scene (no prefabs)
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SceneComponentAttribute : DrawOverrideAttribute
    {
        public string CustomLabel { get; private set; }

        public SceneComponentAttribute(string customLabel = null)
        {
            CustomLabel = customLabel;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            Component fieldValue = field.GetValue(target) as Component;
            fieldValue = SceneObjectField(
                SplitCamelCase(field.Name),
                fieldValue,
                field.FieldType);
            field.SetValue(target, fieldValue);
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            Component propValue = prop.GetValue(target, null) as Component;
            propValue = SceneObjectField(
                SplitCamelCase(prop.Name),
                propValue,
                prop.PropertyType);
            prop.SetValue(target, propValue, null);
        }

        public static Component SceneObjectField(string label, Component sceneObject, Type componentType)
        {

            EditorGUILayout.BeginHorizontal();
            if (string.IsNullOrEmpty(label))
            {
                sceneObject = (Component)EditorGUILayout.ObjectField(sceneObject, componentType, true);
            }
            else
            {
                sceneObject = (Component)EditorGUILayout.ObjectField(label, sceneObject, componentType, true);
            }
            if (sceneObject != null && sceneObject.gameObject.scene.name == null)
            {
                // Don't allow objects that aren't in the scene!
                sceneObject = null;
            }

            UnityEngine.Object[] objectsInScene = GameObject.FindObjectsOfType(componentType);
            int selectedIndex = 0;
            string[] displayedOptions = new string[objectsInScene.Length + 1];
            displayedOptions[0] = "(None)";
            for (int i = 0; i < objectsInScene.Length; i++)
            {
                displayedOptions[i + 1] = objectsInScene[i].name;
                if (objectsInScene[i] == sceneObject)
                {
                    selectedIndex = i + 1;
                }
            }
            selectedIndex = EditorGUILayout.Popup(selectedIndex, displayedOptions);
            if (selectedIndex == 0)
            {
                sceneObject = null;
            }
            else
            {
                sceneObject = (Component)objectsInScene[selectedIndex - 1];
            }
            EditorGUILayout.EndHorizontal();
            return sceneObject;
        }
#endif

    }
}