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

        protected virtual void UseStart()
        {
            //Child do something
        }

        protected virtual void UseEnd()
        {
            //Child do something
        }

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

        private void UseInputStart(InteractionSourcePressedEventArgs obj)
        {
            Debug.Log("Base Usable input start");
            if (obj.pressType == pressType && (handedness == InteractionSourceHandedness.Unknown || handedness == obj.state.source.handedness))
            {
                Debug.Log("Base Usable one step deeper");

                //if (GetComponent<BaseGrabbable>().GrabState == GrabStateEnum.Single)
               // {
                    state = UseStateEnum.Active;
                    UseStart();
               // }
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