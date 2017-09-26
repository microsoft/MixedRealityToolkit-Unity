using System.Collections;
using UnityEngine;




/// <summary>
/// //Intended Usage//
/// Attach a "grabbable_x" script (a script that inherits from this) to any object that is meant to be grabbed
/// create more specific grab behavior by adding additional scripts/components to the game object, such as scalableObject, rotatableObject, throwableObject 
/// </summary>

public abstract class BaseGrabbable : MonoBehaviour
{
    public Grabber MyGrabber { get { return myGrabber; } set { myGrabber = value; } }

    //left protected unless we have the occasion to use them publicly, then switch to public access
    protected Grabber myGrabber;
    protected Transform myOriginalParent;
    protected bool multiGrabAvailable;
    protected GameObject GrabSpot;
    protected bool AwaitingGrab;
    protected Texture AwaitingGrabVisual;
    protected bool grabbable;
    protected bool StayAttachedOnTeleport;
    protected GameObject GrabAttachSpot;
    protected bool held;

    //these events for GrabStarted and GrabEnded are subscribed to by scalable, rotatable, and throwable scripts
    public delegate void GrabActive(GameObject grabber);
    public static event GrabActive GrabStarted;

    public delegate void GrabFalse(GameObject grabber);
    public static event GrabFalse GrabEnded;

    //if a grab handle is not specified, assume that the attach point is the grabbable object's transform
    protected virtual void Start()
    {
        if (!GrabSpot)
            GrabSpot = gameObject;
    }

    //the next three functions provide basic behaviour. Extend from this base script in order to provide more specific functionality.
    protected virtual void CreateTempJoint(Grabber grabber) { }
    protected virtual void StartGrab(Grabber grabber)
    {
        held = true;
        Debug.Log("Start Grab -- from grabbbable");
        myGrabber = grabber;
        grabber.HeldObject = gameObject;
        if (GetComponent<BaseScalable>())
        {
            GrabStarted(grabber.gameObject);
        }
        StartCoroutine(StayGrab(grabber));
    }

    /// <summary>
    /// As long as the grabber script (usually attached to the controller, but not always) reports GrabActive as true,
    /// we stay inside of StayGrab. If the grabactive is false, then we transition into GrabEnd baheviour.
    /// </summary>
    /// <param name="grabber"></param>
    /// <returns></returns>

    protected virtual IEnumerator StayGrab(Grabber grabber)
    {
        while (grabber.GrabActive)
        {
            yield return null;
        }
        EndGrab(grabber);
        yield return null;
    }

    /// <summary>
    /// Grab end fires off a GrabEnded event, but also cleans up some of the variables associated with an active grab, such
    /// as which grabber was grabbing this object and sod forth. 
    /// </summary>
    /// <param name="grabber"></param>
    protected virtual void EndGrab(Grabber grabber)
    {
        held = false;
        myGrabber = null;
        grabber.HeldObject = null;
        if (GetComponent<BaseScalable>())
        {
            GrabEnded(grabber.gameObject);
        }
        if (GetComponent<BaseThrowable>())
        {
            GetComponent<BaseThrowable>().Throw(grabber.gameObject);
        }
        Debug.Log("End Grab -- from grabbable");
    }

    /// <summary>
    /// The combination of trigger enter and a "GrabActive" bool (attached to the grabber) provide all the necessary information
    /// about what is grabbing and what is being grabbed.
    /// Event fires.
    /// </summary>
    /// <param name="other"></param>

    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger Enter");
        if (other.GetComponent<Grabber>())
        {
            Debug.Log("Our other is a grabber");
            Grabber grbr = other.GetComponent<Grabber>();
            if (grbr.GrabActive)
                StartGrab(grbr);
        }
    }

    /// <summary>
    /// The trigger exit function on this base script looks like it is empty but it is overriden by scripts that inherit from this
    /// </summary>
    /// <param name="other"></param>
    protected virtual void OnTriggerExit(Collider other)
    {

    }
}

