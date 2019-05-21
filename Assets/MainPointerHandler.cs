using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPointerHandler : MonoBehaviour
{
    public float scaleFactor = 1f;

    private Vector3 initialScale;

    private void OnEnable()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        var mainPointer = MixedRealityToolkit.InputSystem?.FocusProvider?.MainPointer;
        if (mainPointer != null && mainPointer.Result != null)
        {
            var result = mainPointer.Result;
            if (result.Details.Normal != Vector3.zero)
            {
                transform.SetPositionAndRotation(result.Details.Point, Quaternion.LookRotation(result.Details.Normal));
            }

            float distToCamera = (transform.position - CameraCache.Main.transform.position).magnitude;
            transform.localScale = (1 + distToCamera * scaleFactor) * initialScale;
        }
    }

    private void OnDisable()
    {
        transform.localScale = initialScale;
    }
}
