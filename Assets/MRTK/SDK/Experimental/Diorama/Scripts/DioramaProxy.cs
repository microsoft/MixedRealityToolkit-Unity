using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DioramaProxy : MonoBehaviour
{
    [SerializeField]
    [Tooltip("'Real' object associated with this proxy.")]
    private Transform targetTransform = null;

    /// <summary>
    /// 'Real' object associated with this proxy.
    /// </summary>
    public Transform TargetTransform
    {
        get => targetTransform;
        set => targetTransform = value;
    }

    private Vector3 localOriginOffset;
    private Quaternion localRotationOffset;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        localOriginOffset = TargetTransform.position - transform.localPosition;
        localRotationOffset = TargetTransform.rotation * Quaternion.Inverse(transform.localRotation);
    }

    // Update is called once per frame
    void Update()
    {
        TargetTransform.position = transform.localPosition;
        TargetTransform.rotation = transform.localRotation;
    }
}
