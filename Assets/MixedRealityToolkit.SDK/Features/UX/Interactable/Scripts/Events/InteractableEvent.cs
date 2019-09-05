// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Event base class for events attached to Interactables.
    /// </summary>
    [System.Serializable]
    public class InteractableEvent
    {
        public UnityEvent Event = new UnityEvent();

        /// <summary>
        /// Defines the type of Receiver to associate. Type must be a class that extends ReceiverBase
        /// </summary>
        public Type ReceiverType
        {
            get
            {
                if (receiverType == null)
                {
                    if (string.IsNullOrEmpty(AssemblyQualifiedName))
                    {
                        return null;
                    }

                    receiverType = Type.GetType(AssemblyQualifiedName);
                }

                return receiverType;
            }
            set
            {
                if (!value.IsSubclassOf(typeof(ReceiverBase)))
                {
                    Debug.LogWarning($"Cannot assign type {value} that does not extend {typeof(ReceiverBase)} to ThemeDefinition");
                    return;
                }

                if (receiverType != value)
                {
                    receiverType = value;
                    ClassName = receiverType.Name;
                    AssemblyQualifiedName = receiverType.AssemblyQualifiedName;
                }
            }
        }

        // Unity cannot serialize System.Type, thus must save AssemblyQualifiedName
        // Field here for Runtime use
        [NonSerialized]
        private Type receiverType;

        [SerializeField]
        private string ClassName;

        [SerializeField]
        private string AssemblyQualifiedName;

        // TODO: Troy Make serialized field?
        public ReceiverBase Receiver;

        public List<InspectorPropertySetting> Settings;

        /// <summary>
        /// Create the event and setup the values from the inspector
        /// </summary>
        /// <param name="iEvent"></param>
        /// <returns></returns>
        public static ReceiverBase GetReceiver(InteractableEvent iEvent)
        {
            Type eventType = Type.GetType(iEvent.AssemblyQualifiedName);

            ReceiverBase newEvent = (ReceiverBase)Activator.CreateInstance(eventType, iEvent.Event);
            InspectorGenericFields<ReceiverBase>.LoadSettings(newEvent, iEvent.Settings);

            return newEvent;
        }
    }
}
