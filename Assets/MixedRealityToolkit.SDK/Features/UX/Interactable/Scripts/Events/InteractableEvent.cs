// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Utilities.InspectorFields;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.Events
{
    /// <summary>
    /// Event base class for events attached to Interactables.
    /// </summary>
    [System.Serializable]
    public class InteractableEvent
    {
        public string Name;
        public UnityEvent Event;
        public string ClassName;
        public ReceiverBase Receiver;
        public List<InspectorPropertySetting> Settings;
        public bool HideUnityEvents;

        public struct EventLists
        {
            public List<Type> EventTypes;
            public List<String> EventNames;
        }
        
        public struct ReceiverData
        {
            public string Name;
            public bool HideUnityEvents;
            public List<InspectorFieldData> Fields;
        }
        
        public ReceiverData AddOnClick()
        {
            return AddReceiver(typeof(InteractableOnClickReceiver));
        }

        /// <summary>
        /// Add new events/receivers to the list and grab all the InspectorFields so we can render them in the inspector
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ReceiverData AddReceiver(Type type)
        {
            ReceiverBase receiver = (ReceiverBase)Activator.CreateInstance(type, Event);
            // get the settings for the inspector

            List<InspectorFieldData> fields = new List<InspectorFieldData>();

            Type myType = receiver.GetType();
            int index = 0;

            ReceiverData data = new ReceiverData();
            
            foreach (PropertyInfo prop in myType.GetProperties())
            {
                var attrs = (InspectorField[])prop.GetCustomAttributes(typeof(InspectorField), false);
                foreach (var attr in attrs)
                {
                    fields.Add(new InspectorFieldData() { Name = prop.Name, Attributes = attr, Value = prop.GetValue(receiver, null)});
                }

                index++;
            }

            index = 0;
            foreach (FieldInfo field in myType.GetFields())
            {
                var attrs = (InspectorField[])field.GetCustomAttributes(typeof(InspectorField), false);
                foreach (var attr in attrs)
                {
                    fields.Add(new InspectorFieldData() { Name = field.Name, Attributes = attr, Value = field.GetValue(receiver) });
                }

                index++;
            }

            data.Fields = fields;
            data.Name = receiver.Name;
            data.HideUnityEvents = receiver.HideUnityEvents;

            return data;
        }

        /// <summary>
        /// Get the recieverBase types that contain event logic
        /// </summary>
        /// <returns></returns>
        public static EventLists GetEventTypes()
        {
            List<Type> eventTypes = new List<Type>();
            List<string> names = new List<string>();
            
            var assemblys = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblys)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    TypeInfo info = type.GetTypeInfo();
                    if (info.BaseType != null && info.BaseType.Equals(typeof(ReceiverBase)))
                    {
                        eventTypes.Add(type);
                        names.Add(type.Name);
                    }
                }
            }

            EventLists lists = new EventLists();
            lists.EventTypes = eventTypes;
            lists.EventNames = names;
            return lists;
        }
        
        /// <summary>
        /// Create the event and setup the values from the inspector
        /// </summary>
        /// <param name="iEvent"></param>
        /// <param name="lists"></param>
        /// <returns></returns>
        public static ReceiverBase GetReceiver(InteractableEvent iEvent, EventLists lists)
        {
            int index = InspectorField.ReverseLookup(iEvent.ClassName, lists.EventNames.ToArray());
            Type eventType = lists.EventTypes[index];
            // apply the settings?
            ReceiverBase newEvent = (ReceiverBase)Activator.CreateInstance(eventType, iEvent.Event);
            InspectorGenericFields<ReceiverBase>.LoadSettings(newEvent, iEvent.Settings);

            return newEvent;
        }
    }
}
