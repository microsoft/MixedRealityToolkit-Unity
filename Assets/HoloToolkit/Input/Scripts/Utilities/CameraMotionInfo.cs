using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

namespace HoloToolkit.Unity.InputModule
{
    public class CameraMotionInfo : Singleton<CameraMotionInfo>
    {
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
        /// Sends raycast from Main camera and returns RaycastHit containing focusable object
        /// </summary>
        public RaycastHit RayCastFromHead()
        {
            return GazeManager.Instance.HitInfo;
        }
        
        /// <summary>
        /// Sends spherecast from Main camera and returns  RaycastHit containing focusable object
        /// </summary>
        public RaycastHit SphereCastFromHead(float radius)
        {
            RaycastHit hitInfo = default(RaycastHit);

            for (int i = 0; i < GazeManager.Instance.Rays.Length; i++)
            {
                if ( Physics.SphereCast(GazeManager.Instance.Rays[i].origin, radius, GazeManager.Instance.Rays[i].direction, out hitInfo) )
                {
                    return hitInfo;
                }
            }
            return hitInfo;
        }

        private void FixedUpdate()
        {
            // Update headVelocity
            Vector3 newHeadPos = CameraCache.Main.transform.position;
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

            lastHeadPos = CameraCache.Main.transform.position;

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
}