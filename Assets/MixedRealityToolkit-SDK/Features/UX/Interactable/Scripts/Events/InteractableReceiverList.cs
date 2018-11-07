// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    public class InteractableReceiverList : ReceiverBaseMonoBehavior
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
            InteractableEvent.EventLists lists = InteractableEvent.GetEventTypes();

            for (int i = 0; i < Events.Count; i++)
            {
                Events[i].Receiver = InteractableEvent.GetReceiver(Events[i], lists);
            }
        }

        /// <summary>
        /// .A state has changed
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        public override void OnStateChange(InteractableStates state, Interactable source)
        {
            base.OnStateChange(state, source);

            for (int i = 0; i < Events.Count; i++)
            {
                if (Events[i].Receiver != null)
                {
                    Events[i].Receiver.OnUpdate(state, source);
                }
            }

        }
    }
}
