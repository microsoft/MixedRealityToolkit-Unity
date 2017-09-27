using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This type of grab creates a temporary spring joint to attach the grabbed object to the grabber
/// The fixed joint properties can be assigned here, because the joint will not be attached/visible until runtime
/// </summary>
public class GrabbableSpringJoint : BaseGrabbable
{
    public Color TouchColor;

    //expose the joint variables here for editing because the joint is added/destroyed at runtime
    // to understand how these variables work in greater depth see documentation for spring joint and fixed joint
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
        //CreateTempJoint();
    }

    /// <summary>
    /// This function serves as a constructor for the newly created fixed joint
    /// </summary>
    /// <param name="grabber1"></param>
    protected override void CreateTempJoint(Grabber grabber1)
    {
        Debug.Log("creating a temp spring joint");

        gameObject.AddComponent<SpringJoint>();
        SpringJoint sj = gameObject.GetComponent<SpringJoint>();
        sj.connectedBody = grabber1.GetComponent<Rigidbody>();
        sj.anchor = new Vector3(0, 0.01f, 0.01f);
        sj.tolerance = tolerance;
        sj.breakForce = breakForce;
        sj.breakTorque = breakTorque;
        sj.spring = spring;
        sj.damper = damper;
        Debug.Log("SHOULD BE CREATING A TEMP JOINT");
    }
    protected override void StartGrab(Grabber grabber1)
    {
        base.StartGrab(grabber1);
        if (!GetComponent<SpringJoint>())
        {
            CreateTempJoint(grabber1);
        }
        Debug.Log("RAN IMPORTTTTNNNAATT Start GRAB");
    }

    protected override void EndGrab(Grabber grabber1)
    {
        base.EndGrab(grabber1);
        Debug.Log("END GRAB DESTROYING FIXED JOINT");
        if (GetComponent<SpringJoint>())
        {
            GetComponent<SpringJoint>().connectedBody = null;
            Destroy(gameObject.GetComponent<SpringJoint>());
            Debug.Log("DESTRYOED THE SPRING JOINT");

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
