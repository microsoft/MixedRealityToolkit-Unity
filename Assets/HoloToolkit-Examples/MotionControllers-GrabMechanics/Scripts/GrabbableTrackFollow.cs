using UnityEngine;

namespace MRTK.Grabbables
{
    /// <summary>
    /// This type of grab makes the grabbed object track the position of the grabber
    /// The follow can be tight or loose depending on the lagAmount specified.
    /// </summary>

    public class GrabbableTrackFollow : BaseGrabbable
    {
        public float lagAmount = 5;
        public float rotationSpeed = 5;
        private Rigidbody rb;

        protected override void Start()
        {
            base.Start();
            rb = GetComponent<Rigidbody>();
        }

        protected override void AttachToGrabber(BaseGrabber grabber)
        {
            base.AttachToGrabber(grabber);
            rb.useGravity = false;
        }

        protected override void DetachFromGrabber(BaseGrabber grabber)
        {
            base.DetachFromGrabber(grabber);
            rb.useGravity = true;
        }

        protected override void OnGrabStay()
        {
            base.OnGrabStay();
            transform.position = Vector3.Lerp(transform.position, GrabberPrimary.GrabHandle.position, Time.time / (lagAmount * 1000));
        }
    }
}