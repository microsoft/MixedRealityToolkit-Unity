// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// #defines for features that are not yet implemented
#define TODO_ROTATE_FACE_USER

using MixedRealityToolkit.Common;
using MixedRealityToolkit.Common.Extensions;
using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.Focus;
using MixedRealityToolkit.InputModule.InputHandlers;
using MixedRealityToolkit.InputModule.InputSources;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;


public enum TwoHandManipMode
{
    rotation,
    translation,
    scale
};

public class TwoHandManipMgr : MonoBehaviour, IInputHandler, ISourceStateHandler
{
    private Vector3 lastPositionA;
    private Vector3 lastPositionB;

    public TextMesh LeftText;
    public TextMesh RightText;

    private uint srcA = 0;
    private uint srcB = 0;

    private float scaleCalibration = 1.0f;
    private Vector3 orientationNormal;
    private Quaternion orientationCalibration;
    private bool calibrationSet = false;

    private TwoHandManipMode mode = TwoHandManipMode.rotation;

    // Maps input id -> position of hand
    private readonly Dictionary<uint, Vector3> m_handsPressedLocationsMap = new Dictionary<uint, Vector3>();
    // Maps input id -> input source. Then obtain position of input source using currentInputSource.TryGetGripPosition(currentInputSourceId, out inputPosition);
    private readonly Dictionary<uint, IInputSource> m_handsPressedInputSourceMap = new Dictionary<uint, IInputSource>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        UpdateData();

        HandleCalibration();

        UpdateTransform();

        UpdateTextPorts();
    }

    private Vector3 GetInputPosition(InputEventData eventData)
    {
        Vector3 result = Vector3.zero;
        eventData.InputSource.TryGetGripPosition(eventData.SourceId, out result);
        return result;
    }


    public void OnInputDown(InputEventData eventData)
    {
        Vector3 pos = GetInputPosition(eventData);
        IInputSource source = eventData.InputSource;

        m_handsPressedLocationsMap[eventData.SourceId] = pos;
        m_handsPressedInputSourceMap[eventData.SourceId] = source;

        // Add to hand map
        if (srcA == 0)
        {
            srcA = eventData.SourceId;
        }
        else if (srcB == 0 && eventData.SourceId != srcA)
        {
            srcB = eventData.SourceId;
        }

        eventData.Use();
    }

    public void OnInputUp(InputEventData eventData)
    {
        eventData.Use();
    }

    public void OnSourceDetected(SourceStateEventData eventData)
    {
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        eventData.Use();
    }

    private void HandleCalibration()
    {
        if (calibrationSet == false)
        {
            if (srcA != 0 && srcB != 0)
            {
                scaleCalibration = (lastPositionB - lastPositionA).magnitude;

                orientationNormal = lastPositionB - lastPositionA;
                orientationNormal.Normalize();

                Vector3 axis = (lastPositionB - lastPositionA);
                axis.Normalize();
                orientationCalibration = Quaternion.AngleAxis(0, axis);

                calibrationSet = true;
            }
        }
    }

    private void UpdateTextPorts()
    {
        LeftText.text = lastPositionA.ToString();
        RightText.text = lastPositionB.ToString();
    }

    private void UpdateData()
    {
        foreach (var key in m_handsPressedInputSourceMap.Keys)
        {
            var inputSource = m_handsPressedInputSourceMap[key];
            Vector3 inputPosition = Vector3.zero;

            if (inputSource.TryGetGripPosition(key, out inputPosition))
            {
                m_handsPressedLocationsMap[key] = inputPosition;
            }

            if (key == srcA)
            {
                lastPositionA = m_handsPressedLocationsMap[key];
            }
            else if (key == srcB)
            {
                lastPositionB = m_handsPressedLocationsMap[key];
            }
        }
    }

    private void UpdateTransform()
    {
        if (calibrationSet == true && scaleCalibration != 0.0f)
        {
            Vector3 deltaPosition = lastPositionB - lastPositionA;

            if (mode == TwoHandManipMode.scale)
            {
                Vector3 currentScale = this.gameObject.transform.localScale;
                float newScalar = deltaPosition.magnitude / scaleCalibration;

                Vector3 finalScale = new Vector3(newScalar, newScalar, newScalar);

                if (Mathf.Abs(deltaPosition.x) < 0.2f && Math.Abs(deltaPosition.y) > 0.3)
                {
                    finalScale = new Vector3(currentScale.x, newScalar, currentScale.z);
                }
                else if (Mathf.Abs(deltaPosition.x) > 0.2f && Math.Abs(deltaPosition.y) < 0.3f)
                {
                    finalScale = new Vector3(newScalar, currentScale.y, currentScale.z);
                }
                else
                {
                    finalScale = currentScale;
                }

                this.gameObject.transform.localScale = Smooth(currentScale, finalScale);
            }
            else if (mode == TwoHandManipMode.rotation)
            {
                deltaPosition.Normalize();
                Vector3 axis = Vector3.Cross(deltaPosition, orientationNormal);
                axis.Normalize();
                float angle = Mathf.Acos(Vector3.Dot(deltaPosition, orientationNormal)) * (180.0f / Mathf.PI);
                Quaternion rotation = Quaternion.AngleAxis(-angle, axis);
                
                this.gameObject.transform.rotation = rotation;
            }
        }
    }

    private Vector3 Smooth(Vector3 a, Vector3 b)
    {
        Vector3 result = new Vector3(0, 0, 0);

        result.x = a.x + (0.1f * (b.x - a.x));
        result.y = a.y + (0.1f * (b.y - a.y));
        result.z = a.z + (0.1f * (b.z - a.z));

        return result;
    }
}

