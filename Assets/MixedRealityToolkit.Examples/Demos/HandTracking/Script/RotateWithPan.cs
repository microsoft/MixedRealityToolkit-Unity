// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class RotateWithPan : MonoBehaviour, IMixedRealityHandPanHandler
{
    private Renderer rd;

    private void Start ()
    {
        rd = GetComponent<Renderer>();
    }

    // Update is called once per frame
    private void Update ()
    {
	}

    public void OnPanEnded(HandPanEventData eventData)
    {
        if (rd != null)
        {
            rd.material.color = new Color(1.0f, 1.0f, 1.0f);
        }
    }

    public void OnPanning(HandPanEventData eventData)
    {
        Vector3 eulers = new Vector3(eventData.PanPosition.y * (2.0f * Mathf.PI), eventData.PanPosition.x * (2.0f * Mathf.PI), 0.0f);
        eulers *= Mathf.Rad2Deg;
        eulers *= 0.2f;
        transform.localEulerAngles += eulers;
    }

    public void OnPanStarted(HandPanEventData eventData)
    {
        if (rd != null)
        {
            rd.material.color = new Color(0.0f, 1.0f, 0.0f);
        }
    }
}
