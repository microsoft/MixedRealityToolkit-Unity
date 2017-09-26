using UnityEngine;

/// <summary>
/// This type of grab uses a parent child relationship and also immediately orients the child's forward to the parent's forward position
/// </summary>

public class GrabbableSnapToOrient : BaseGrabbable
{
    public Color TouchColor;

    protected override void StartGrab(Grabber grabber1)
    {
        base.StartGrab(grabber1);
        transform.SetParent(grabber1.transform);
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        transform.rotation = transform.parent.rotation;
    }

    protected override void EndGrab(Grabber grabber1)
    {
        base.EndGrab(grabber1);
        transform.SetParent(null);
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
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

    protected override void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
        base.Start();
    }

    private Color originalColor;


}
