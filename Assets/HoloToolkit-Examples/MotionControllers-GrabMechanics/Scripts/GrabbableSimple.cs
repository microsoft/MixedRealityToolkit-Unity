using System.Collections;
using UnityEngine;
namespace MRTK.Grabbables
{
    /// <summary>
    /// This type of grab makes the grabbed object follow the position and rotation of the grabber, but does not create a parent child relationship
    /// </summary>

    public class GrabbableSimple : BaseGrabbable
    {
        protected override void Start()
        {
            base.Start();
            rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Specify the target and turn off gravity. Otherwise gravity will interfere with desired grab effect
        /// </summary>
        protected override void StartGrab(BaseGrabber grabber)
        {
            base.StartGrab(grabber);
            if (rb)
                rb.useGravity = false;
        }

        /// <summary>
        /// On release turn garvity back on the so the object falls and set the target back to null
        /// </summary>
        protected override void EndGrab()
        {
            if (rb)
            {
                rb.useGravity = true;
            }
            base.EndGrab();
        }

        protected override void OnGrabStay()
        {
            if(matchPosition)
                transform.position = GrabberPrimary.GrabHandle.position;

            if (matchRotation)
                transform.rotation = GrabberPrimary.GrabHandle.rotation;
        }

        //the next three functions provide basic behaviour. Extend from this base script in order to provide more specific functionality.
        protected override void AttachToGrabber(BaseGrabber grabber)
        {
            Debug.Log("Attaching to Grabber --from Grabbable Simple");
            GetComponent<Rigidbody>().isKinematic = true;
            if (!activeGrabbers.Contains(grabber))
                activeGrabbers.Add(grabber);
        }

        protected override void DetachFromGrabber(BaseGrabber grabber)
        {
            Debug.Log("Detaching form grabber -- from Grabbable Simple");
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;
        }

        private Rigidbody rb;

        [SerializeField]
        private bool matchPosition = true;
        [SerializeField]
        private bool matchRotation = false;
    }
}