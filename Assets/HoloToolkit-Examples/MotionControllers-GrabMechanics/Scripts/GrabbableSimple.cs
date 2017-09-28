using UnityEngine;

/// <summary>
/// This type of grab makes the grabbed object follow the position and rotation of the grabber, but does not create a parent child relationship
/// </summary>

public class GrabbableSimple : BaseGrabbable
{
    public Color TouchColor;

    protected override void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
        base.Start();
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Specify the target and turn off gravity. Otherwise gravity will interfere with desired grab effect
    /// </summary>
    protected override void StartGrab(Grabber grabber)
    {
        base.StartGrab(grabber);
        if (rb)
            rb.useGravity = false;
    }

    /// <summary>
    /// On release turn garvity back on the so the object falls and set the target back to null
    /// </summary>
    protected override void EndGrab(Grabber grabber)
    {
        base.EndGrab(grabber);
        if (rb)
        {
            rb.useGravity = true;
        }
    }

    private void Update()
    {
        if (myGrabber)
        {
            transform.position = myGrabber.GrabHandle.position;
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
    private Rigidbody rb;


}
