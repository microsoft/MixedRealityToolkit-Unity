/// <summary>
/// Extends its behaviour from BaseGrabber. This is non-abstract script that's actually attached to the gameObject that will
/// be grabbing/carrying the object. 
/// </summary>
/// 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace MRTK.Grabbables
{
    public class Grabber : BaseGrabber
    {
        ///Subscribe GrabStart and GrabEnd to InputEvents for GripPressed
        protected override void OnEnable()
        {
            base.OnEnable();
            InteractionManager.InteractionSourcePressed += InteractionSourcePressed;
            InteractionManager.InteractionSourceReleased += InteractionSourceReleased;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            InteractionManager.InteractionSourcePressed -= InteractionSourcePressed;
            InteractionManager.InteractionSourceReleased -= InteractionSourceReleased;
        }

        private void InteractionSourcePressed(InteractionSourcePressedEventArgs obj)
        {
            if (obj.pressType == pressType && obj.state.source.handedness == handedness)
            {
                GrabStart();
            }
        }

        private void InteractionSourceReleased(InteractionSourceReleasedEventArgs obj)
        {
            if (obj.pressType == pressType && obj.state.source.handedness == handedness)
            {
                GrabEnd();
            }
        }

        /// <summary>
        /// Controller grabbers find available grabbable objects via triggers
        /// </summary>
        /// <param name="other"></param>
        protected virtual void OnTriggerEnter(Collider other)
        {
            Debug.Log("Entered trigger with " + other.name);
            if (((1 << other.gameObject.layer) & grabbableLayers.value) == 0)
                return;

            BaseGrabbable bg = other.GetComponent<BaseGrabbable>();
            if (bg == null && other.attachedRigidbody != null)
                bg = other.attachedRigidbody.GetComponent<BaseGrabbable>();

            if (bg == null)
                return;

            Debug.Log("Adding contact");

            AddContact(bg);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            Debug.Log("Exited trigger with " + other.name);
            if (((1 << other.gameObject.layer) & grabbableLayers.value) == 0)
                return;

            BaseGrabbable bg = other.GetComponent<BaseGrabbable>();
            if (bg == null && other.attachedRigidbody != null)
                bg = other.attachedRigidbody.GetComponent<BaseGrabbable>();

            if (bg == null)
                return;

            Debug.Log("Removing contact");

            RemoveContact(bg);
        }

        [SerializeField]
        private LayerMask grabbableLayers = ~0;
        [SerializeField]
        private InteractionSourceHandedness handedness;
        [SerializeField]
        private InteractionSourcePressType pressType;
    }
}