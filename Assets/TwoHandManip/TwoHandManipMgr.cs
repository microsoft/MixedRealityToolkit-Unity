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
    private double lastPressedA;
    private double lastPressedB;

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
    private readonly Dictionary<uint, Vector3> m_handPositions = new Dictionary<uint, Vector3>();
    private readonly Dictionary<uint, IInputSource> m_handSources = new Dictionary<uint, IInputSource>();
    private readonly Dictionary<uint, double> m_handPressedValues = new Dictionary<uint, double>();

    private GameObject coverCube;
    private GameObject upLeftFront;
    private GameObject upRightFront;
    private GameObject upLeftBack;
    private GameObject upRightBack;
    private GameObject downLeftFront;
    private GameObject downRightFront;
    private GameObject downLeftBack;
    private GameObject downRightBack;

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

    private double GetPressedInfo(InputEventData eventData)
    {
        bool isPressed = false;
        double pressedValue = 0.0;

        eventData.InputSource.TryGetSelect(eventData.SourceId, out isPressed, out pressedValue);
        return pressedValue;
    }

    public void OnInputDown(InputEventData eventData)
    {
        m_handSources[eventData.SourceId] = eventData.InputSource;
        m_handPositions[eventData.SourceId] = GetInputPosition(eventData);
        m_handPressedValues[eventData.SourceId] = GetPressedInfo(eventData);

        // Add to hand map
        if (srcA == 0)
        {
            srcA = eventData.SourceId;
        }
        else if (srcB == 0 && eventData.SourceId != srcA)
        {
            srcB = eventData.SourceId;
            CreateBoundingBox();
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
        // LeftText.text = lastPositionA.ToString();
        //RightText.text = lastPositionB.ToString();

        LeftText.text = ((int)(lastPressedA * 100)).ToString();
        RightText.text = ((int)(lastPressedB * 100)).ToString();
    }

    private void UpdateData()
    {
        foreach (var key in m_handSources.Keys)
        {
            var inputSource = m_handSources[key];
            Vector3 inputPosition = Vector3.zero;
            bool isPressed = false;
            double pressedValue = 0;

            if (inputSource.TryGetGripPosition(key, out inputPosition))
            {
                m_handPositions[key] = inputPosition;
            }

            if (inputSource.TryGetSelect(key, out isPressed, out pressedValue))
            {
                m_handPressedValues[key] = pressedValue;
            }

            if (key == srcA)
            {
                lastPositionA = m_handPositions[key];
                lastPressedA = m_handPressedValues[key];
            }
            else if (key == srcB)
            {
                lastPositionB = m_handPositions[key];
                lastPressedB = m_handPressedValues[key];
            }
        }
    }

    private void UpdateTransform()
    {
        if (lastPressedA > 0.5f && lastPressedB > 0.5f)
        {
            mode = TwoHandManipMode.scale;
        }
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

    private void CreateBoundingBox()
    {
        Shader shader;

        coverCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shader = Shader.Find("Transparent/Diffuse");
        coverCube.GetComponent<Renderer>().material.shader = shader;
        coverCube.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.1f);
        coverCube.transform.localPosition = this.gameObject.transform.localPosition;
        coverCube.transform.localRotation = this.gameObject.transform.localRotation;
        coverCube.transform.localScale = this.gameObject.transform.localScale * 1.05f;
        coverCube.transform.parent = this.gameObject.transform;
      

        upLeftFront = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shader = Shader.Find("Transparent/Diffuse");
        upLeftFront.GetComponent<Renderer>().material.shader = shader;
        upLeftFront.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 1.0f);
        upLeftFront.transform.localPosition = this.gameObject.transform.localPosition + new Vector3(0.5f, 0.5f, -0.5f);
        upLeftFront.transform.localRotation = this.gameObject.transform.localRotation;
        upLeftFront.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); ;
        upLeftFront.transform.parent = this.gameObject.transform;

        upRightFront = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shader = Shader.Find("Transparent/Diffuse");
        upRightFront.GetComponent<Renderer>().material.shader = shader;
        upRightFront.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 1.0f);
        upRightFront.transform.localPosition = this.gameObject.transform.localPosition + new Vector3(-0.5f, 0.5f, -0.5f);
        upRightFront.transform.localRotation = this.gameObject.transform.localRotation;
        upRightFront.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); ;
        upRightFront.transform.parent = this.gameObject.transform;

        upLeftBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shader = Shader.Find("Transparent/Diffuse");
        upLeftBack.GetComponent<Renderer>().material.shader = shader;
        upLeftBack.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 1.0f);
        upLeftBack.transform.localPosition = this.gameObject.transform.localPosition + new Vector3(0.5f, 0.5f, 0.5f);
        upLeftBack.transform.localRotation = this.gameObject.transform.localRotation;
        upLeftBack.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); ;
        upLeftBack.transform.parent = this.gameObject.transform;

        upRightBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shader = Shader.Find("Transparent/Diffuse");
        upRightBack.GetComponent<Renderer>().material.shader = shader;
        upRightBack.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 1.0f);
        upRightBack.transform.localPosition = this.gameObject.transform.localPosition + new Vector3(-0.5f, 0.5f, 0.5f);
        upRightBack.transform.localRotation = this.gameObject.transform.localRotation;
        upRightBack.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); ;
        upRightBack.transform.parent = this.gameObject.transform;

        downLeftFront = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shader = Shader.Find("Transparent/Diffuse");
        downLeftFront.GetComponent<Renderer>().material.shader = shader;
        downLeftFront.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 1.0f);
        downLeftFront.transform.localPosition = this.gameObject.transform.localPosition + new Vector3(0.5f, -0.5f, -0.5f);
        downLeftFront.transform.localRotation = this.gameObject.transform.localRotation;
        downLeftFront.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); ;
        downLeftFront.transform.parent = this.gameObject.transform;

        downRightFront = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shader = Shader.Find("Transparent/Diffuse");
        downRightFront.GetComponent<Renderer>().material.shader = shader;
        downRightFront.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 1.0f);
        downRightFront.transform.localPosition = this.gameObject.transform.localPosition + new Vector3(-0.5f, -0.5f, -0.5f);
        downRightFront.transform.localRotation = this.gameObject.transform.localRotation;
        downRightFront.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); ;
        downRightFront.transform.parent = this.gameObject.transform;

        downLeftBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shader = Shader.Find("Transparent/Diffuse");
        downLeftBack.GetComponent<Renderer>().material.shader = shader;
        downLeftBack.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 1.0f);
        downLeftBack.transform.localPosition = this.gameObject.transform.localPosition + new Vector3(0.5f, -0.5f, 0.5f);
        downLeftBack.transform.localRotation = this.gameObject.transform.localRotation;
        downLeftBack.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); ;
        downLeftBack.transform.parent = this.gameObject.transform;

        downRightBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shader = Shader.Find("Transparent/Diffuse");
        downRightBack.GetComponent<Renderer>().material.shader = shader;
        downRightBack.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 1.0f);
        downRightBack.transform.localPosition = this.gameObject.transform.localPosition + new Vector3(-0.5f, -0.5f, 0.5f);
        downRightBack.transform.localRotation = this.gameObject.transform.localRotation;
        downRightBack.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); ;
        downRightBack.transform.parent = this.gameObject.transform;
    }
}

