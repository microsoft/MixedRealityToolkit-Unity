using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackboxMaintainScale : MonoBehaviour
{
    [SerializeField]
    private Transform rootTransform;
    public Transform RootTransform
    {
        get => rootTransform;
        set => rootTransform = value;
    }

    private Vector3 initialScale;

    // Start is called before the first frame update
    void Awake()
    {
        initialScale = rootTransform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(Mathf.Clamp(initialScale.x / rootTransform.localScale.x, 0.0f, 3.0f), Mathf.Clamp(initialScale.y / rootTransform.localScale.y, 0.0f, 3.0f), Mathf.Clamp(initialScale.z / rootTransform.localScale.z, 0.0f, 3.0f));
    }
}
