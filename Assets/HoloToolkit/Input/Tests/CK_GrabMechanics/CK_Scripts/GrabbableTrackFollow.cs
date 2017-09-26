using UnityEngine;


/// <summary>
/// This type of grab makes the grabbed object track the position of the grabber
/// The follow can be tight or loose depending on the lagAmount specified.
/// </summary>

public class GrabbableTrackFollow : BaseGrabbable
{
    public GameObject target;
    public float lagAmount = 5;
    public float rotationSpeed = 5;
    public Color TouchColor;
    private Rigidbody rb;

    protected override void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
        base.Start();
        rb = GetComponent<Rigidbody>();
    }

    protected override void StartGrab(Grabber grabber1)
    {
        base.StartGrab(grabber1);
        target = grabber1.gameObject;
        rb.useGravity = false;
    }

    protected override void EndGrab(Grabber grabber1)
    {
        base.EndGrab(grabber1);
        rb.useGravity = true;
        target = null;
    }

    void Update()
    {
        if (target)
            transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.time / (lagAmount * 1000));
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
