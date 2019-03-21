// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
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
        public string AssemblyQualifiedName;
        public ReceiverBase Receiver;
        public List<InspectorPropertySetting> Settings;
        public bool HideUnityEvents;

        public struct ReceiverData
        {
            public string Name;
            public bool HideUnityEvents;
            public List<InspectorFieldData> Fields;
        }

        /// <summary>
        /// The list of base classes whose derived classes will be included in interactable event
        /// selection dropdowns.
        /// </summary>
        private static readonly List<Type> candidateEventTypes = new List<Type>() { typeof(ReceiverBase) };

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
        public static InteractableTypesContainer GetEventTypes()
        {
            return InteractableTypeFinder.Find(candidateEventTypes, TypeRestriction.DerivedOnly);
        }
        
        /// <summary>
        /// Create the event and setup the values from the inspector
        /// </summary>
        /// <param name="iEvent"></param>
        /// <returns></returns>
        public static ReceiverBase GetReceiver(InteractableEvent iEvent, InteractableTypesContainer interactableTypes)
        {
#if UNITY_EDITOR
            int index = InspectorField.ReverseLookup(iEvent.ClassName, interactableTypes.ClassNames);
            Type eventType = interactableTypes.Types[index];
#else
            Type eventType = Type.GetType(iEvent.AssemblyQualifiedName);
#endif
            // apply the settings?
            ReceiverBase newEvent = (ReceiverBase)Activator.CreateInstance(eventType, iEvent.Event);
            InspectorGenericFields<ReceiverBase>.LoadSettings(newEvent, iEvent.Settings);

            return newEvent;
        }
    }
}
