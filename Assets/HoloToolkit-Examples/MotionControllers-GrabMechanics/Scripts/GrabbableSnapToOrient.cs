using UnityEngine;
namespace MRTK.Grabbables
{
    /// <summary>
    /// This type of grab uses a parent child relationship and also immediately orients the child's forward to the parent's forward position
    /// </summary>

    public class GrabbableSnapToOrient : BaseGrabbable
    {
        protected override void StartGrab(BaseGrabber grabber)
        {
            base.StartGrab(grabber);
            transform.SetParent(grabber.GrabHandle);
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            transform.rotation = transform.parent.rotation;
        }

        protected override void EndGrab()
        {
            base.EndGrab();
            transform.SetParent(null);
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}