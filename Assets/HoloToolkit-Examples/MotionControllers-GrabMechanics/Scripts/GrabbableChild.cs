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

            base.EndGrab();
            transform.SetParent(null);
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}