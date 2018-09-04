using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Unity
{
    public abstract class ReceiverBase
    {
        public string Name;

        protected UnityEvent uEvent;

        public ReceiverBase(UnityEvent ev)
        {
            uEvent = ev;
        }

        public abstract void OnUpdate(InteractableStates state);
    }
}
