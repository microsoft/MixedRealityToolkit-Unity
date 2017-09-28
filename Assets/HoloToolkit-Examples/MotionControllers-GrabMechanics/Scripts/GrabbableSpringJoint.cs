using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This type of grab creates a temporary spring joint to attach the grabbed object to the grabber
/// The fixed joint properties can be assigned here, because the joint will not be created until runtime
/// </summary>
public class GrabbableSpringJoint : BaseGrabbable
{
    public Color TouchColor;

    //expose the joint variables here for editing because the joint is added/destroyed at runtime
    // to understand how these variables work in greater depth see unity documentation for spring joint and fixed joint
    [SerializeField]
    protected float spring;
    [SerializeField]
    protected float damper;
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
    protected override void CreateTempJoint(Grabber grabber)
    {
        gameObject.AddComponent<SpringJoint>();
        SpringJoint sj = gameObject.GetComponent<SpringJoint>();
        sj.connectedBody = grabber.GetComponent<Rigidbody>();
        sj.anchor = new Vector3(0, 0.01f, 0.01f);
        sj.tolerance = tolerance;
        sj.breakForce = breakForce;
        sj.breakTorque = breakTorque;
        sj.spring = spring;
        sj.damper = damper;
        Debug.Log("Just CREATED a temp JOINT");
    }
    protected override void StartGrab(Grabber grabber)
    {
        base.StartGrab(grabber);
        if (!GetComponent<SpringJoint>())
        {
            CreateTempJoint(grabber);
        }
    }

    protected override void EndGrab(Grabber grabber)
    {
        base.EndGrab(grabber);
        if (GetComponent<SpringJoint>())
        {
            Debug.Log("Trying to destroy this JOINT");
            GetComponent<SpringJoint>().connectedBody = null;
            Destroy(gameObject.GetComponent<SpringJoint>());
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
