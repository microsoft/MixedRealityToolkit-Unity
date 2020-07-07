// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

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
        public static bool RenderEvent(SerializedProperty eventItem, bool canRemove = true)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                SerializedProperty uEvent = eventItem.FindPropertyRelative("Event");
                SerializedProperty eventName = eventItem.FindPropertyRelative("Name");
                SerializedProperty className = eventItem.FindPropertyRelative("ClassName");
                SerializedProperty assemblyQualifiedName = eventItem.FindPropertyRelative("AssemblyQualifiedName");
                Type receiverType;

                InspectorUIUtility.DrawHeader("Event Receiver Type");
                using (new EditorGUILayout.HorizontalScope())
                {
                    Rect position = EditorGUILayout.GetControlRect();
                    using (new EditorGUI.PropertyScope(position, SelectEventLabel, className))
                    {
                        var receiverTypes = TypeCacheUtility.GetSubClasses<ReceiverBase>();
                        var recevierClassNames = receiverTypes.Select(t => t?.Name).ToArray();
                        int id = Array.IndexOf(recevierClassNames, className.stringValue);
                        int newId = EditorGUI.Popup(position, id, recevierClassNames);
                        if (newId == -1) { newId = 0; }

                        receiverType = receiverTypes[newId];

                        // Temporary workaround to fix bug shipped in GA where assemblyQualifiedName was never set
                        if (string.IsNullOrEmpty(assemblyQualifiedName.stringValue))
                        {
                            assemblyQualifiedName.stringValue = receiverType.AssemblyQualifiedName;
                        }

                        if (id != newId)
                        {
                            EventChanged(receiverType, eventItem);
                        }
                    }

                    if (canRemove)
                    {
                        if (InspectorUIUtility.FlexButton(new GUIContent("Remove Event")))
                        {
                            return true;
                        }
                    }
                }

                EditorGUILayout.Space();
                InspectorUIUtility.DrawHeader("Event Properties");

                ReceiverBase receiver = (ReceiverBase)Activator.CreateInstance(receiverType, new UnityEvent());

                if (!receiver.HideUnityEvents)
                {
                    EditorGUILayout.PropertyField(uEvent, new GUIContent(receiver.Name));
                }

                SerializedProperty eventSettings = eventItem.FindPropertyRelative("Settings");

                // If fields for given receiver class type have been changed, update the related inspector field data
                var fieldList = InspectorFieldsUtility.GetInspectorFields(receiver);
                if (!InspectorFieldsUtility.AreFieldsSame(eventSettings, fieldList))
                {
                    InspectorFieldsUtility.UpdateSettingsList(eventSettings, fieldList);
                }

                for (int index = 0; index < eventSettings.arraySize; index++)
                {
                    SerializedProperty propertyField = eventSettings.GetArrayElementAtIndex(index);
                    bool isEvent = InspectorFieldsUtility.IsPropertyType(propertyField, InspectorField.FieldTypes.Event);

                    if (!receiver.HideUnityEvents || !isEvent)
                    {
                        InspectorFieldsUtility.DisplayPropertyField(eventSettings.GetArrayElementAtIndex(index));
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Update the given InteractableEvent to the new type (which extends ReceiverBase)
        /// </summary>
        /// <param name="newType">new receiverbase subclass type to target</param>
        /// <param name="eventItem">InteractableEvent to target and update</param>
        private static void EventChanged(Type newType, SerializedProperty eventItem)
        {
            SerializedProperty className = eventItem.FindPropertyRelative("ClassName");
            SerializedProperty assemblyQualifiedName = eventItem.FindPropertyRelative("AssemblyQualifiedName");

            className.stringValue = newType.Name;
            assemblyQualifiedName.stringValue = newType.AssemblyQualifiedName;

            SerializedProperty settings = eventItem.FindPropertyRelative("Settings");

            ReceiverBase defaultReceiver = (ReceiverBase)Activator.CreateInstance(newType, new UnityEvent());
            InspectorFieldsUtility.ClearSettingsList(settings, InspectorFieldsUtility.GetInspectorFields(defaultReceiver));
        }
    }
}
