using UnityEngine;

namespace MRTK.Grabbables
{
    /// <summary>
    /// This type of grab makes the grabbed object a child of the grabber.
    /// This ensures a grabbed object perfectly follows the position and rotation of the grabbing object
    /// </summary>

    public class GrabbableChild : BaseGrabbable
    {
        protected override void StartGrab(BaseGrabber grabber)
        {
            base.StartGrab(grabber);

            transform.SetParent(GrabberPrimary.transform);
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }

        protected override void EndGrab()
        {
            transform.SetParent(null);
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            base.EndGrab();
        }

        protected override void AttachToGrabber(BaseGrabber grabber)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            if (!activeGrabbers.Contains(grabber))
                activeGrabbers.Add(grabber);
        }

        protected override void DetachFromGrabber(BaseGrabber grabber)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;
        }
    }
}