// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.Prototyping
{
    /// <summary>
    /// places the object in orbit around a point
    /// </summary>
    public class OrbitPoint : MonoBehaviour
    {
        [Tooltip("position to orbit around")]
        public Vector3 CenterPoint = new Vector3();

        [Tooltip("Axis to orbit around")]
        public Vector3 Axis = Vector3.forward;

        [Tooltip("size of the orbit")]
        public float Radius = 0.2f;

        [Tooltip("orbit speed")]
        public float RevolutionSpeed = 2.0f;

        [Tooltip("starting position based on 360 degrees")]
        public float StartAngle = 0;

        [Tooltip("Limit the amount or revolutions, set to zero for infinite")]
        public float Revolutions = 0;

        [Tooltip("Orbit the other way")]
        public bool Reversed = false;

        [Tooltip("Start automatically")]
        public bool AutoPlay = true;

        [Tooltip("pause the orbit or status")]
        public bool IsPaused = false;

        /// <summary>
        /// The current revolution count
        /// </summary>
        public int RevolutionCount { get; private set; }

        [Tooltip("Smooth in and out of the orbit")]
        public bool SmoothEaseInOut = false;

        [Tooltip("Smoothness factor")]
        public float SmoothRatio = 1;

        // starting angle
        private float mAngle = 0;

        // current time
        private float mTime = 0;

        // position
        private Vector3 mPositionVector;

        // rotation
        private Vector3 mRotatedPositionVector;

        /// <summary>
        /// set up the object's initial orbit position
        /// </summary>
        private void Start()
        {
            RevolutionCount = 0;
            mPositionVector = transform.up;

            // are we rotating around the y
            if (!Mathf.Approximately(Vector3.Angle(Axis, mPositionVector), 90))
            {
                // are we rotating around the z
                mPositionVector = transform.forward;
                if (!Mathf.Approximately(Vector3.Angle(Axis, mPositionVector), 90))
                {
                    float x = Mathf.Abs(Axis.x);
                    float y = Mathf.Abs(Axis.y);
                    float z = Mathf.Abs(Axis.z);

                    if (x > y && x > z)
                    {
                        // left or right - cross with the z axis
                        mPositionVector = Vector3.Cross(Axis * Radius, Vector3.forward);
                    }

                    if (z > y && z > x)
                    {
                        // forward or backward - cross with the x axis
                        mPositionVector = Vector3.Cross(Axis * Radius, Vector3.right);
                    }

                    if (y > z && y > x)
                    {
                        // up or down - cross with the x axis
                        mPositionVector = Vector3.Cross(Axis * Radius, Vector3.right);
                    }
                }
            }

            // set the position
            transform.Rotate(Axis, mAngle - StartAngle);
            mRotatedPositionVector = Quaternion.AngleAxis(mAngle - StartAngle, Axis) * mPositionVector * Radius;
            transform.localPosition = CenterPoint + mRotatedPositionVector;

            IsPaused = !AutoPlay;
        }
        
        /// <summary>
        /// Start the orbit animation
        /// </summary>
        public void StartOrbit()
        {
            IsPaused = false;
            RevolutionCount = 0;
        }

        /// <summary>
        /// reset the orbit position (does not stop)
        /// </summary>
        public void ResetOrbit()
        {
            mAngle = 0;
        }

        // easing function
        public  float QuartEaseInOut(float s, float e, float v)
        {
            //e -= s;
            if ((v /= 0.5f) < 1)
                return e / 2 * v * v * v * v + s;

            return -e / 2 * ((v -= 2) * v * v * v - 2) + s;
        }

        // Update is called once per frame
        private void Update()
        {
            if (IsPaused) return;

            // get the percent of time passed
            float percentage = mTime / RevolutionSpeed;

            // are we smooth?
            if (SmoothEaseInOut)
            {
                // apply the smooth factor
                float linearSmoothing = 1 * (percentage * (1 - SmoothRatio));
                percentage = QuartEaseInOut(0, 1, percentage) * SmoothRatio + linearSmoothing;
            }

            // update the angle
            mAngle = 0 - (percentage) * 360;

            // rotate the right direction
            if (Reversed)
            {
                mAngle = -mAngle;
            }

            // set the position
            transform.Rotate(Axis, mAngle - StartAngle);
            mRotatedPositionVector = Quaternion.AngleAxis(mAngle - StartAngle, Axis) * mPositionVector * Radius;
            transform.localPosition = CenterPoint + mRotatedPositionVector;
            
            // update the time
            mTime += Time.deltaTime;
            if (mTime >= RevolutionSpeed)
            {
                mTime = RevolutionSpeed - mTime;
                RevolutionCount += 1;

                if (RevolutionCount >= Revolutions && Revolutions > 0)
                {
                    IsPaused = true;
                    mTime = 0;
                }
            }
        }
    }
}
