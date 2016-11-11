using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class OnFocusEvent : MonoBehaviour, IFocusable
    {
        public UnityEvent FocusEnterEvent;
        public UnityEvent FocusLostEvent;

        private void Start()
        {
            // dummy Start function so we can use this.enabled
        }

        public void OnFocusEnter()
        {
            if (this.enabled == false) return;
            if (FocusEnterEvent != null)
            {
                FocusEnterEvent.Invoke();
            }
        }

        public void OnFocusExit()
        {
            if (this.enabled == false) return;
            if (FocusLostEvent != null)
            {
                FocusLostEvent.Invoke();
            }
        }
    }
}
