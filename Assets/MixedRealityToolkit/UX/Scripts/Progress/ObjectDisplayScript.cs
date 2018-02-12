using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDisplayScript : MonoBehaviour
{
    [Header("How fast does object rotate?")]
    public float rotationIncrement;

    [Header("Start scale of the object?")]
    public float minScale;

    [Header("Final scale of the object?")]
    public float maxScale;

    [Header("How fast does object grow?")]
    public float scaleSpeed;

    [Header("Should object rotate after growing?")]
    public bool rotationActive;

    [Header("Should object grow before rotating?")]
    public bool growingActive;

    [Header("Rotation occurs about which axes?")]
    public bool xAxisRotation = false;
    public bool yAxisRotation = true;
    public bool zAxisRotation = false;

    private float currentScale;
    private float elapsedTime;

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        elapsedTime = 0.0f;
        currentScale = minScale;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.unscaledDeltaTime;

        if (growingActive && currentScale < maxScale)
        {
            currentScale = minScale + (scaleSpeed * (maxScale * Mathf.Pow(elapsedTime, 2.0f)));
        }

        transform.localScale = new Vector3(currentScale, currentScale, currentScale);

        if (rotationActive)
        {
            float increment = Time.deltaTime * rotationIncrement;
            float xRotation = xAxisRotation ? increment : 0;
            float yRotation = yAxisRotation ? increment : 0;
            float zRotation = zAxisRotation ? increment : 0;
            transform.Rotate(xRotation, yRotation, zRotation);
        }
    }
}
