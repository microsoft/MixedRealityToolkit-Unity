using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace MRTK.Grabbables
{
    /// <summary>
    /// A usable object is one that can be "used" or actiavated while being grabbed/carried
    /// A gun and a remote control are examples: first grab, then press a different button to use
    /// </summary>
    public abstract class BaseUsable : MonoBehaviour
    {
        public enum UseStateEnum
        {
            Inactive,
            Active
        }

        public UseStateEnum UseState
        {
            get { return state; }
        }

        protected abstract void UseStart();

        protected abstract void UseEnd();

        //Subscribe GrabStart and GrabEnd to InputEvents for Grip
        protected virtual void OnEnable()
        {
            InteractionManager.InteractionSourcePressed += UseInputStart;
            InteractionManager.InteractionSourceReleased += UseInputEnd;
        }

        protected virtual void OnDisable()
        {
            InteractionManager.InteractionSourcePressed -= UseInputStart;
            InteractionManager.InteractionSourceReleased -= UseInputEnd;
        }

        protected void Awake()
        {
           // handedness
        }

        private void UseInputStart(InteractionSourcePressedEventArgs obj)
        {
            Debug.Log("attempting to start USING. Handedness is : "+obj.state.source.handedness);
            /*&& (obj.state.source.handedness == handedness || obj.state.source.handedness == InteractionSourceHandedness.Unknown)*/
            if (obj.pressType == pressType && (obj.state.source.handedness.Equals(InteractionSourceHandedness.Left) || obj.state.source.handedness.Equals(InteractionSourceHandedness.Right)))
            {
                state = UseStateEnum.Active;
                UseStart();
                Debug.Log("firing USE START");
            }
        }

        private void UseInputEnd(InteractionSourceReleasedEventArgs obj)
        {
            if (obj.pressType == pressType && obj.state.source.handedness == handedness)
            {
                state = UseStateEnum.Inactive;
                UseEnd();
            }
        }

        [SerializeField]
        private InteractionSourceHandedness handedness;
        //assign a controller button to "use" the object
        [SerializeField]
        private InteractionSourcePressType pressType;

        private UseStateEnum state;
    }
}