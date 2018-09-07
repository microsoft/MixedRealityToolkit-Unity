using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    public abstract class ReceiverBase
    {
        public string Name;
        protected UnityEvent uEvent;

        public ReceiverBase(UnityEvent ev)
        {
            uEvent = ev;
        }

        public abstract void OnUpdate(InteractableStates state, Interactable source);
    }
}
