// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <summary>
        /// Base Event used to initialize EventReceiver class
        /// </summary>
        public UnityEvent Event = new UnityEvent();

        /// <summary>
        /// ReceiverBase instantiation for this InteractableEvent. Used at runtime by Interactable class
        /// </summary>
        [NonSerialized]
        public ReceiverBase Receiver;

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

        [SerializeField]
        private List<InspectorPropertySetting> Settings = new List<InspectorPropertySetting>();

        /// <summary>
        /// Create the event and setup the values from the inspector. If the asset is invalid,
        /// returns null.
        /// </summary>
        public static ReceiverBase CreateReceiver(InteractableEvent iEvent)
        {
            if (string.IsNullOrEmpty(iEvent.ClassName))
            {
                // If the class name of this event is empty, the asset is invalid and loading types will throw errors. Return null.
                return null;
            }

            // Temporary workaround
            // This is to fix a bug in GA where the AssemblyQualifiedName was never actually saved. Functionality would work in editor...but never on device player
            if (iEvent.ReceiverType == null)
            {
                var correctType = TypeCacheUtility.GetSubClasses<ReceiverBase>().Where(s => s?.Name == iEvent.ClassName).First();
                iEvent.ReceiverType = correctType;
            }

            ReceiverBase newEvent = (ReceiverBase)Activator.CreateInstance(iEvent.ReceiverType, iEvent.Event);
            InspectorGenericFields<ReceiverBase>.LoadSettings(newEvent, iEvent.Settings);

            return newEvent;
        }
    }
}
