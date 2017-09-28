using UnityEngine;

/// <summary>
/// This type of grab creates a temporary fixed joint to attach the grabbed object to the grabber
/// The fixed joint properties can be assigned here, because the joint will not be attached/visible until runtime
/// </summary>

public class GrabbableFixedJoint : BaseGrabbable
{
    //Touch color
    public Color TouchColor;

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

    protected override void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
        base.Start();
    }

    /// <summary>
    /// This function serves as a constructor for the newly created fixed joint
    /// </summary>
    /// <param name="grabber1"></param>
    protected override void CreateTempJoint(Grabber grabber1)
    {
        if (!GetComponent<FixedJoint>())
        {
            gameObject.AddComponent<FixedJoint>();
            FixedJoint fj = gameObject.GetComponent<FixedJoint>();
            fj.connectedBody = grabber1.GetComponent<Rigidbody>();
            fj.anchor = joint_anchor;
            fj.breakForce = breakForce;
            fj.breakTorque = breakTorque;
        }
    }
    protected override void StartGrab(Grabber grabber1)
    {
        base.StartGrab(grabber1);
        if (!GetComponent<SpringJoint>())
        {
            CreateTempJoint(grabber1);
        }
    }

    protected override void EndGrab(Grabber grabber1)
    {
        base.EndGrab(grabber1);

        if (GetComponent<FixedJoint>() != null)
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(gameObject.GetComponent<FixedJoint>());
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        Renderer rend = GetComponent<Renderer>();
        rend.material.color = TouchColor;
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        Renderer rend = GetComponent<Renderer>();
        rend.material.color = originalColor;
    }

    private Color originalColor;
}
