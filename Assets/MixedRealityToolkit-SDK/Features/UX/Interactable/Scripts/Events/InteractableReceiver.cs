// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    public class InteractableReceiver : ReceiverBaseMonoBehavior
    {
        // list of events added to this interactable
        [HideInInspector]
        public List<InteractableEvent> Events = new List<InteractableEvent>();

        protected virtual void Awake()
        {
            SetupEvents();
        }

        protected virtual void SetupEvents()
        {
            if (Events.Count > 0)
            {
                InteractableEvent.EventLists lists = InteractableEvent.GetEventTypes();
                Events[0].Receiver = InteractableEvent.GetReceiver(Events[0], lists);
            }
        }

        /// <summary>
        /// A state has changed
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        public override void OnStateChange(InteractableStates state, Interactable source)
        {
            base.OnStateChange(state, source);
            if (Events.Count > 0)
            {
                if (Events[0].Receiver != null)
                {
                    Events[0].Receiver.OnUpdate(state, source);
                }
            }
        }
    }
}
