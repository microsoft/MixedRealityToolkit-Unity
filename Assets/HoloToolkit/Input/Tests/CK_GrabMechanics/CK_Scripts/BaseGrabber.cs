using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;


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

/// <summary>
/// Intended usage: scripts that inherit from this can be attached to the controller, or any object with a collider 
/// that needs to grabbing or carrying other objects. 
/// </summary>

public abstract class BaseGrabber : MonoBehaviour
{
    public Transform GrabHandle { get { return grabAttachSpot; } set { grabAttachSpot = value; } }
    public bool GrabActive { get { return grabActive; } set { grabActive = value; } }
    public GameObject HeldObject { get { return heldObject; } set { heldObject = value; } }
    public Vector3 Velocity { get { return velocity; } set { velocity = value; } }
    public float Strength { get { return strength; } set { strength = value; } }

    ///Subscribe GrabStart and GrabEnd to InputEvents for GripPressed
    protected virtual void OnEnable()
    {
        InteractionManager.InteractionSourcePressed += GrabStart;
        InteractionManager.InteractionSourceReleased += GrabEnd;
    }

    protected virtual void OnDisable()
    {
        InteractionManager.InteractionSourcePressed -= GrabStart;
        InteractionManager.InteractionSourceReleased -= GrabEnd;
    }

    /// <summary>
    /// If not grabattachpoint is specified, use the gameobject transform by default
    /// </summary>

    void Start()
    {
        if (GrabHandle == null)
        {
            GrabHandle = transform;
        }
    }

    /// <summary>
    /// If the correct grabbing button is pressed, we set the GrabActive to true.
    /// Grab behaviour depends on the combination of grabactive being true, and a grabbable trigger entered
    /// </summary>
    /// <param name="obj"></param>
    protected virtual void GrabStart(InteractionSourcePressedEventArgs obj)
    {
        grabActive = true;
        Debug.Log("Grab Initiated.");
    }

    /// <summary>
    /// If the correct grabbing button is pressed, we set the GrabActive to true.
    /// Grab behaviour depends on the combination of grabactive being true, and a grabbable trigger entered
    /// </summary>
    protected virtual void GrabEnd(InteractionSourceReleasedEventArgs obj)
    {
        grabActive = false;
        Debug.Log("Grab Ended.");
    }

    public Vector3 GetCurrentPosition()
    {
        return currPos;
    }

    public Vector3 GetPreviousPosition()
    {
        return prevPos;
    }

    void Update()
    {
        currPos = transform.position;
    }

    void LateUpdate()
    {
        prevPos = transform.position;
    }

    //variable declaration
    [SerializeField]
    protected Transform grabAttachSpot;
    protected bool grabActive;
    protected float grabForgivenessRadius;
    private bool holding;
    private GameObject heldObject;

    //for scaling
    private Rigidbody rb;
    private Vector3 velocity;
    private GameObject myGrabbedObject;
    private float scaleMulitplier;
    private Vector3 attachPoint;
    [SerializeField]
    private float strength = 1.0f;
    private Vector3 currPos;
    private Vector3 prevPos;
    
}


