// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    [CustomEditor(typeof(InteractableReceiverList))]
    public class InteractableReceiverListInspector : UnityEditor.Editor
    {
        protected List<InteractableEvent> eventList;
        protected InteractableTypesContainer eventOptions;

        // indent tracker
        protected static int indentOnSectionStart = 0;

        private static readonly GUIContent SelectEventLabel = new GUIContent("Select Event Type", "Select the event type from the list");

        protected virtual void OnEnable()
        {
            eventList = ((InteractableReceiverList)target).Events;
            SetupEventOptions();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            RenderInspectorHeader();

            SerializedProperty events = serializedObject.FindProperty("Events");

            if(events.arraySize < 1)
            {
                AddEvent(0);
            }
            else
            {
                for (int i = 0; i < events.arraySize; i++)
                {
                    SerializedProperty eventItem = events.GetArrayElementAtIndex(i);

                    InspectorUIUtility.ListButtonEvent removeEventRef = null;
                    if (i > 0)
                    {
                        removeEventRef = RemoveEvent;
                    }

                    if (RenderEventSettings(eventItem, i, eventOptions, ChangeEvent, removeEventRef))
                    {
                        // If removed, skip rendering rest of list till next redraw
                        break;
                    }
                }

                if (eventOptions.ClassNames.Length > 1)
                {
                    if (GUILayout.Button(new GUIContent("Add Event")))
                    {
                        AddEvent(events.arraySize);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void RenderInspectorHeader()
        {
            SerializedProperty interactable = serializedObject.FindProperty("Interactable");
            SerializedProperty searchScope = serializedObject.FindProperty("InteractableSearchScope");

            EditorGUILayout.PropertyField(interactable, new GUIContent("Interactable","The Interactable that will be monitored"));

            ReceiverBaseMonoBehavior.SearchScopes scope = (ReceiverBaseMonoBehavior.SearchScopes)searchScope.intValue;
            scope = (ReceiverBaseMonoBehavior.SearchScopes)EditorGUILayout.EnumPopup(new GUIContent("Search Scope", "Where to look for an Interactable if one is not assigned"), scope);

            if ((int)scope != searchScope.intValue)
            {
                searchScope.intValue = (int)scope;
            }
        }

        protected virtual void RemoveEvent(int index, SerializedProperty prop = null)
        {
            SerializedProperty events = serializedObject.FindProperty("Events");
            if (events.arraySize > index)
            {
                events.DeleteArrayElementAtIndex(index);
            }
        }

        protected virtual void AddEvent(int index)
        {
            SerializedProperty events = serializedObject.FindProperty("Events");
            events.InsertArrayElementAtIndex(events.arraySize);
        }

        /// <summary>
        /// Invoked when the event is changed.
        /// </summary>
        /// <param name="indexArray">
        /// A two-element sized index array where the first element is the index of the
        /// event in the event list, and the second is the new event handler class that
        /// was selected.
        /// </param>
        protected virtual void ChangeEvent(int[] indexArray, SerializedProperty prop = null)
        {
            SerializedProperty className = prop.FindPropertyRelative("ClassName");
            SerializedProperty name = prop.FindPropertyRelative("Name");
            SerializedProperty assemblyQualifiedName = prop.FindPropertyRelative("AssemblyQualifiedName");
            SerializedProperty settings = prop.FindPropertyRelative("Settings");
            SerializedProperty hideEvents = prop.FindPropertyRelative("HideUnityEvents");

            if (!String.IsNullOrEmpty(className.stringValue))
            {
                InteractableEvent.ReceiverData data = eventList[indexArray[0]].AddReceiver(eventOptions.Types[indexArray[1]]);
                name.stringValue = data.Name;
                // Technically not necessary due to how this is set in RenderEventSettings, nevertheless included to
                // make sure that wherever we set Name/ClassName, we always set AssemblyQualifiedName as well.
                // Performance wise this is not a huge deal due to how this is only triggered on changes in the inspector
                // in the editor (i.e. dropdown selection has changed, which requires explicit user input).
                assemblyQualifiedName.stringValue = eventOptions.AssemblyQualifiedNames[indexArray[1]];
                hideEvents.boolValue = data.HideUnityEvents;

                InspectorFieldsUtility.PropertySettingsList(settings, data.Fields);
            }
        }

        /// <summary>
        /// Render event properties for the given event item. If item has been removed, returns true. False otherwise
        /// </summary>
        /// <param name="eventItem">serialized property of the event item to render properties from</param>
        /// <param name="index">index of event item in higher order list</param>
        /// <param name="options">Event type options</param>
        /// <param name="changeEvent">Function to call if event properties have changed</param>
        /// <param name="removeEvent">Function to call if event requested to be removed</param>
        /// <returns>If item has been removed, returns true. False otherwise</returns>
        public static bool RenderEventSettings(SerializedProperty eventItem, int index, InteractableTypesContainer options, InspectorUIUtility.MultiListButtonEvent changeEvent, InspectorUIUtility.ListButtonEvent removeEvent)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                SerializedProperty uEvent = eventItem.FindPropertyRelative("Event");
                SerializedProperty eventName = eventItem.FindPropertyRelative("Name");
                SerializedProperty className = eventItem.FindPropertyRelative("ClassName");
                SerializedProperty assemblyQualifiedName = eventItem.FindPropertyRelative("AssemblyQualifiedName");
                SerializedProperty hideEvents = eventItem.FindPropertyRelative("HideUnityEvents");

                // show event dropdown
                int id = InspectorUIUtility.ReverseLookup(className.stringValue, options.ClassNames);

                using (new EditorGUILayout.HorizontalScope())
                {
                    Rect position = EditorGUILayout.GetControlRect();
                    EditorGUI.BeginProperty(position, SelectEventLabel, className);
                    {
                        int newId = EditorGUI.Popup(position, id, options.ClassNames);

                        if (id != newId || String.IsNullOrEmpty(className.stringValue))
                        {
                            className.stringValue = options.ClassNames[newId];
                            assemblyQualifiedName.stringValue = options.AssemblyQualifiedNames[newId];

                            changeEvent(new int[] { index, newId }, eventItem);
                        }

                    }
                    EditorGUI.EndProperty();

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
        
        protected virtual void SetupEventOptions()
        {
            eventOptions = InteractableEvent.GetEventTypes();
        }
    }
}
