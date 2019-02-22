// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.Events
{
    [CustomEditor(typeof(InteractableReceiverList))]
    public class InteractableReceiverListInspector : Editor
    {
        protected List<InteractableEvent> eventList;
        protected string[] eventOptions;
        protected Type[] eventTypes;

        // indent tracker
        protected static int indentOnSectionStart = 0;

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

                    RenderEventSettings(eventItem, i, eventOptions, ChangeEvent, removeEventRef);
                }

                if (eventOptions.Length > 1)
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

        protected virtual void ChangeEvent(int[] indexArray, SerializedProperty prop = null)
        {
            SerializedProperty className = prop.FindPropertyRelative("ClassName");
            SerializedProperty name = prop.FindPropertyRelative("Name");
            SerializedProperty settings = prop.FindPropertyRelative("Settings");
            SerializedProperty hideEvents = prop.FindPropertyRelative("HideUnityEvents");

            if (!String.IsNullOrEmpty(className.stringValue))
            {
                InteractableEvent.ReceiverData data = eventList[indexArray[0]].AddReceiver(eventTypes[indexArray[1]]);
                name.stringValue = data.Name;
                hideEvents.boolValue = data.HideUnityEvents;

                InspectorFieldsUtility.PropertySettingsList(settings, data.Fields);
            }
        }

        public static void RenderEventSettings(SerializedProperty eventItem, int index, string[] options, InspectorUIUtility.MultiListButtonEvent changeEvent, InspectorUIUtility.ListButtonEvent removeEvent)
        {
            EditorGUILayout.BeginVertical("Box");
            SerializedProperty uEvent = eventItem.FindPropertyRelative("Event");
            SerializedProperty eventName = eventItem.FindPropertyRelative("Name");
            SerializedProperty className = eventItem.FindPropertyRelative("ClassName");
            SerializedProperty hideEvents = eventItem.FindPropertyRelative("HideUnityEvents");

            // show event dropdown
            int id = InspectorUIUtility.ReverseLookup(className.stringValue, options);
            int newId = EditorGUILayout.Popup("Select Event Type", id, options);

            if (id != newId || String.IsNullOrEmpty(className.stringValue))
            {
                className.stringValue = options[newId];

                changeEvent(new int[] { index, newId }, eventItem);
            }

            if (!hideEvents.boolValue)
            {
                EditorGUILayout.PropertyField(uEvent, new GUIContent(eventName.stringValue));
            }

            // show event properties
            EditorGUI.indentLevel = indentOnSectionStart + 1;
            SerializedProperty eventSettings = eventItem.FindPropertyRelative("Settings");
            for (int j = 0; j < eventSettings.arraySize; j++)
            {
                SerializedProperty propertyField = eventSettings.GetArrayElementAtIndex(j);
                bool isEvent = InspectorFieldsUtility.IsPropertyType(propertyField, Core.Utilities.InspectorFields.InspectorField.FieldTypes.Event);

                if (!hideEvents.boolValue || !isEvent)
                {
                    InspectorFieldsUtility.DisplayPropertyField(eventSettings.GetArrayElementAtIndex(j));
                }
            }
            EditorGUI.indentLevel = indentOnSectionStart;

            EditorGUILayout.Space();

            if(removeEvent != null)
            {
                InspectorUIUtility.FlexButton(new GUIContent("Remove Event"), index, removeEvent);
            }

            EditorGUILayout.EndVertical();
        }
        
        protected virtual void SetupEventOptions()
        {
            InteractableEvent.EventLists lists = InteractableEvent.GetEventTypes();
            eventTypes = lists.EventTypes.ToArray();
            eventOptions = lists.EventNames.ToArray();
        }
    }
}
