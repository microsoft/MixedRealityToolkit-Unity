using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDisplayScript : MonoBehaviour
{
    [Header("How fast does object rotate?")]
    public float RotationIncrement;

    [Header("Start scale of the object?")]
    public float MinScale;

    [Header("Final scale of the object?")]
    public float MaxScale;

    [Header("How fast does object grow?")]
    public float ScaleSpeed;

    [Header("Should object rotate after growing?")]
    public bool RotationActive;

    [Header("Should object grow before rotating?")]
    public bool GrowingActive;

    [Header("Rotation occurs about which axes?")]
    public bool XAxisRotation = false;
    public bool YAxisRotation = true;
    public bool ZAxisRotation = false;

    private float currentScale;
    private float elapsedTime;

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        elapsedTime = 0.0f;
        currentScale = MinScale;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.unscaledDeltaTime;

        if (GrowingActive && currentScale < MaxScale)
        {
            currentScale = MinScale + (ScaleSpeed * (MaxScale * Mathf.Pow(elapsedTime, 2.0f)));
        }

        transform.localScale = new Vector3(currentScale, currentScale, currentScale);

        if (RotationActive)
        {
            float increment = Time.deltaTime * RotationIncrement;
            float xRotation = XAxisRotation ? increment : 0;
            float yRotation = YAxisRotation ? increment : 0;
            float zRotation = ZAxisRotation ? increment : 0;
            transform.Rotate(xRotation, yRotation, zRotation);
        }
    }
}
