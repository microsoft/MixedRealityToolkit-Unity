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

        public static void RenderEventSettings(SerializedProperty eventItem, int index, InteractableTypesContainer options, InspectorUIUtility.MultiListButtonEvent changeEvent, InspectorUIUtility.ListButtonEvent removeEvent)
        {
            EditorGUILayout.BeginVertical("Box");
            SerializedProperty uEvent = eventItem.FindPropertyRelative("Event");
            SerializedProperty eventName = eventItem.FindPropertyRelative("Name");
            SerializedProperty className = eventItem.FindPropertyRelative("ClassName");
            SerializedProperty assemblyQualifiedName = eventItem.FindPropertyRelative("AssemblyQualifiedName");
            SerializedProperty hideEvents = eventItem.FindPropertyRelative("HideUnityEvents");

            // show event dropdown
            int id = InspectorUIUtility.ReverseLookup(className.stringValue, options.ClassNames);
            int newId = EditorGUILayout.Popup("Select Event Type", id, options.ClassNames);

            if (id != newId || String.IsNullOrEmpty(className.stringValue))
            {
                className.stringValue = options.ClassNames[newId];
                assemblyQualifiedName.stringValue = options.AssemblyQualifiedNames[newId];

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
                bool isEvent = InspectorFieldsUtility.IsPropertyType(propertyField, InspectorField.FieldTypes.Event);

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
            eventOptions = InteractableEvent.GetEventTypes();
        }
    }
}
