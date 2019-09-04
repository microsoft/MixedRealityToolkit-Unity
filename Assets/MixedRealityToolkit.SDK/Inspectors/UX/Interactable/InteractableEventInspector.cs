// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public static class InteractableEventInspector
    {
        private static readonly GUIContent SelectEventLabel = new GUIContent("Select Event Type", "Select the event type from the list");

        /// <summary>
        /// Render event properties for the given event item. If item has been removed, returns true. False otherwise
        /// </summary>
        /// <param name="eventItem">serialized property of the event item to render properties from</param>
        /// <returns>If item has been removed, returns true. False otherwise</returns>
        public static bool RenderEventSettings(SerializedProperty eventItem)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                SerializedProperty uEvent = eventItem.FindPropertyRelative("Event");
                SerializedProperty eventName = eventItem.FindPropertyRelative("Name");
                SerializedProperty className = eventItem.FindPropertyRelative("ClassName");
                SerializedProperty assemblyQualifiedName = eventItem.FindPropertyRelative("AssemblyQualifiedName");
                SerializedProperty hideEvents = eventItem.FindPropertyRelative("HideUnityEvents");

                using (new EditorGUILayout.HorizontalScope())
                {
                    Rect position = EditorGUILayout.GetControlRect();
                    using (new EditorGUI.PropertyScope(position, SelectEventLabel, className))
                    {
                        var receiverTypes = typeof(ReceiverBase).GetAllSubClassesOf();
                        var recevierClassNames = receiverTypes.Select(t => t.Name).ToArray();
                        int id = Array.IndexOf(recevierClassNames, className.stringValue);
                        int newId = EditorGUI.Popup(position, id, recevierClassNames);

                        if (id != newId || String.IsNullOrEmpty(className.stringValue))
                        {
                            // TODO: Troy
                            className.stringValue = recevierClassNames[newId];
                            //assemblyQualifiedName.stringValue = options.AssemblyQualifiedNames[newId];

                            changeEvent(new int[] { index, newId }, eventItem);
                        }

                    }

                    if (removeEvent != null)
                    {
                        if (InspectorUIUtility.FlexButton(new GUIContent("Remove Event"), index, removeEvent))
                        {
                            return true;
                        }
                    }

                }
                EditorGUILayout.Space();

                if (!hideEvents.boolValue)
                {
                    EditorGUILayout.PropertyField(uEvent, new GUIContent(eventName.stringValue));
                }

                // show event properties
                SerializedProperty eventSettings = eventItem.FindPropertyRelative("Settings");
                for (int j = 0; j < eventSettings.arraySize; j++)
                {
                    SerializedProperty propertyField = eventSettings.GetArrayElementAtIndex(j);
                    bool isEvent = InspectorFieldsUtility.IsPropertyType(propertyField, InspectorField.FieldTypes.Event);

                    if (!hideEvents.boolValue || !isEvent)
                    {
                        InspectorFieldsUtility.DisplayPropertyField(eventSettings.GetArrayElementAtIndex(j));
                    }
                }
            }

            return false;
        }
    }
}
