using UnityEngine;
namespace MRTK.Grabbables
{
    /// <summary>
    /// This type of grab creates a temporary fixed joint to attach the grabbed object to the grabber
    /// The fixed joint properties can be assigned here, because the joint will not be attached/visible until runtime
    /// </summary>

    public class GrabbableFixedJoint : BaseGrabbable
    {
        //expose the joint variables here for editing because the joint is added/destroyed at runtime
        // to understand how these variables work in greater depth see documentation for spring joint and fixed joint
        [SerializeField]
        protected float breakForce;
        [SerializeField]
        protected float breakTorque;
        [SerializeField]
        protected float tolerance;
        [SerializeField]
        protected Vector3 joint_anchor;
        [SerializeField]
        protected float minDistance;
        [SerializeField]
        protected float maxDistance;


        protected override void AttachToGrabber(BaseGrabber grabber)
        {
            base.AttachToGrabber(grabber);
            FixedJoint joint = GetComponent<FixedJoint>();
            if (joint == null)
            {
                gameObject.AddComponent<FixedJoint>();
                joint = gameObject.GetComponent<FixedJoint>();
                joint.connectedBody = grabber.GetComponent<Rigidbody>();
                joint.anchor = joint_anchor;
                joint.breakForce = breakForce;
                joint.breakTorque = breakTorque;
            }
        }

        protected override void DetachFromGrabber(BaseGrabber grabber)
        {
            base.DetachFromGrabber(grabber);
            FixedJoint joint = GetComponent<FixedJoint>();
            if (joint != null)
            {
                joint.connectedBody = null;
                Destroy(joint);
            }
        }
    }
}