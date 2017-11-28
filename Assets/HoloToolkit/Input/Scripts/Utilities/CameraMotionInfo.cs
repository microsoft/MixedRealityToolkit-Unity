using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;

public class CameraMotionInfo : Singleton<CameraMotionInfo> {

    #region Public accessors
    public Vector3 HeadVelocity { get { return headVelocity; } }
    public Vector3 MoveDirection { get { return headMoveDirection; } }

    public float HeadZoneSizeIdle = 0.2f;
    public float HeadZoneSizeMin = 0.01f;
    #endregion

    #region Private members
    private Vector3 headVelocity;
    private Vector3 lastHeadPos;
    private Vector3 lastHeadZone;
    private Vector3 newHeadMoveDirection;
    private float headZoneSize = 1f;
    private Vector3 headMoveDirection = Vector3.one;
    #endregion

    /// <summary>
    /// Sends raycast from Main camera and returns focusable object
    /// </summary>
    public RaycastHit RayCastFromHead()
    {
        Transform head = Camera.main.transform;

        RaycastHit hitInfo;

        Ray gazeRay = new Ray(head.position, head.forward);

        if (Physics.Raycast(gazeRay, out hitInfo))
        {
            Transform hitTransform = hitInfo.collider.transform;

            // Traverse up parent path until interactible object found
            while (hitTransform != null)
            {
                IFocusable focusable = hitTransform.GetComponent(typeof(IFocusable)) as IFocusable;

                if (focusable != null)
                {
                    return hitInfo;
                }

                hitTransform = hitTransform.parent;
            }
        }

        return hitInfo;
    }

    /// <summary>
    /// Sends raycast from Main camera and returns focusable objects
    /// </summary>
    public RaycastHit[] RayCastAllFromHead()
    {
        Transform head = Camera.main.transform;
        RaycastHit[] hitInfo;

        Ray gazeRay = new Ray(head.position, head.forward);
        hitInfo = Physics.RaycastAll(gazeRay);

        return hitInfo;
    }

    /// <summary>
    /// Sends spherecast from Main camera and returns focusable object
    /// </summary>
    public RaycastHit SphereCastFromHead(float radius)
    {
        Transform head = Camera.main.transform;
        RaycastHit hitInfo;
        Ray gazeRay = new Ray(head.position, head.forward);

        if (Physics.SphereCast(gazeRay, radius, out hitInfo))
        {
            Transform hitTransform = hitInfo.collider.transform;

            // Traverse up parent path until interactible object found
            while (hitTransform != null)
            {
                IFocusable focusable = hitTransform.GetComponent(typeof(IFocusable)) as IFocusable;

                if (focusable != null)
                {
                    return hitInfo;
                }

                hitTransform = hitTransform.parent;
            }
        }

        return hitInfo;
    }

    private void FixedUpdate()
    {
        // Update headVelocity
        Vector3 newHeadPos = Camera.main.transform.position;
        Vector3 headDelta = newHeadPos - lastHeadPos;

        float moveThreshold = 0.01f;
        if (headDelta.sqrMagnitude < moveThreshold * moveThreshold)
        {
            headDelta = Vector3.zero;
        }

        if (Time.fixedDeltaTime > 0)
        {
            float adjustRate = 3f * Time.fixedDeltaTime;
            headVelocity = headVelocity * (1f - adjustRate) + headDelta * adjustRate / Time.fixedDeltaTime;

            float velThreshold = .1f;
            if (headVelocity.sqrMagnitude < velThreshold * velThreshold)
            {
                headVelocity = Vector3.zero;
            }
        }

        lastHeadPos = Camera.main.transform.position;

        // Update headDirection
        float headVelIdleThresh = 0.5f;
        float headVelMoveThresh = 2f;

        float velP = Mathf.Clamp01(Mathf.InverseLerp(headVelIdleThresh, headVelMoveThresh, headVelocity.magnitude));
        float newHeadZoneSize = Mathf.Lerp(HeadZoneSizeIdle, HeadZoneSizeMin, velP);
        headZoneSize = Mathf.Lerp(headZoneSize, newHeadZoneSize, Time.fixedDeltaTime);

        Vector3 headZoneDelta = newHeadPos - lastHeadZone;
        if (headZoneDelta.sqrMagnitude >= headZoneSize * headZoneSize)
        {
            newHeadMoveDirection = Vector3.Lerp(newHeadPos - lastHeadZone, headVelocity, velP).normalized;
            lastHeadZone = newHeadPos;
        }

        {
            float adjustRate = Mathf.Clamp01(5f * Time.fixedDeltaTime);
            headMoveDirection = Vector3.Slerp(headMoveDirection, newHeadMoveDirection, adjustRate);
        }

        Debug.DrawLine(lastHeadPos, lastHeadPos + headMoveDirection * 10f, Color.Lerp(Color.red, Color.green, velP));
        Debug.DrawLine(lastHeadPos, lastHeadPos + headVelocity, Color.yellow);
    }
}
