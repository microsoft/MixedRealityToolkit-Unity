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
    /// <summary>
    /// Which button is associated with grabbing behaviour
    /// </summary>
    public enum ButtonChoice
    {
        None,
        Trigger,
        Grip,
        Touchpad
    }

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
            GrabStart();
        }

        private void InteractionSourceReleased(InteractionSourceReleasedEventArgs obj)
        {
            GrabEnd();
        }

        /// <summary>
        /// Controller grabbers find available grabbable objects via triggers
        /// </summary>
        /// <param name="other"></param>
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != grabbableLayers.value)
                return;

            BaseGrabbable bg = other.GetComponent<BaseGrabbable>();
            if (bg == null && other.attachedRigidbody != null)
                bg = other.attachedRigidbody.GetComponent<BaseGrabbable>();

            if (bg == null)
                return;

            AddContact(bg);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer != grabbableLayers.value)
                return;

            BaseGrabbable bg = other.GetComponent<BaseGrabbable>();
            if (bg == null && other.attachedRigidbody != null)
                bg = other.attachedRigidbody.GetComponent<BaseGrabbable>();

            if (bg == null)
                return;

            RemoveContact(bg);
        }

        [SerializeField]
        private LayerMask grabbableLayers = ~0;
    }
}