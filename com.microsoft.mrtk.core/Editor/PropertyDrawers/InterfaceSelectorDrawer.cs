// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomPropertyDrawer(typeof(InterfaceSelectorAttribute))]
    public class InterfaceSelectorDrawer : PropertyDrawer
    {
        private GUIContent dropLabel;
        private GUIStyle labelStyle;

        public InterfaceSelectorDrawer()
        {
            // Cache some items used frequently in OnGUI
            dropLabel = EditorGUIUtility.IconContent("Linked");
            labelStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).label;

            dropLabel.tooltip = "This will search the project for classes that match this property's interface, and create an instance of that class.";
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUI.GetPropertyHeight(property, true);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Get a nice readable name for the property's instance type
            string typeName = property.managedReferenceFullTypename;
            int split = typeName.IndexOf(' ');
            if (split != -1)
            {
                split += 1;
                typeName = typeName.Substring(split, typeName.Length - split);
            }
            else if (string.IsNullOrEmpty(typeName))
            {
                typeName = "(null)";
            }

            // Add a dropdown menu to select an interface, and generate an instance of it
            float labelWidth = labelStyle.CalcSize(label).x + EditorGUIUtility.singleLineHeight;
            Rect dropdownRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, EditorGUIUtility.singleLineHeight);

            Type fieldType = fieldInfo.FieldType;

            if (fieldType.IsArray)
            {
                fieldType = fieldType.GetElementType();
            }
            else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                fieldType = fieldType.GetGenericArguments()[0];
            }

            dropLabel.text = $"{fieldType.Name} - {typeName}";
            if (EditorGUI.DropdownButton(dropdownRect, dropLabel, FocusType.Passive))
            {
                // Grab a list of all Types currently loaded that implement our
                // field's type.
                List<Type> allDerivedTypes = TypeCache.GetTypesDerivedFrom(fieldType)
                    .Where(type => !type.IsAbstract && !type.IsInterface)
                    .ToList();

                GenericMenu menu = new GenericMenu();
                // Add a menu item for each valid type
                for (int i = 0; i < allDerivedTypes.Count; i++)
                {
                    menu.AddItem(new GUIContent(allDerivedTypes[i].Name), false, (t) =>
                    {
                        property.serializedObject.Update();
                        try
                        {
                            property.managedReferenceValue = Activator.CreateInstance((Type)t);
                        }
                        catch (MissingMethodException)
                        {
                            // Activator.CreateInstance only works with constructors that
                            // have no arguments. It's a limitation here, but not really an
                            // onerous one.
                            Debug.LogError($"{((Type)t).FullName} must have a constructor with no arguments to be instantiated through the inspector!");
                        }
                        property.serializedObject.ApplyModifiedProperties();
                    }, allDerivedTypes[i]);
                }

                // Add an item for clearing to null
                if (((InterfaceSelectorAttribute)attribute).AllowNull)
                {
                    menu.AddItem(new GUIContent("(null)"), false, () =>
                    {
                        property.serializedObject.Update();
                        property.managedReferenceValue = null;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            }

            // The default inspector for the property
            EditorGUI.PropertyField(position, property, label, true);

            EditorGUI.EndProperty();
        }
    }
}
