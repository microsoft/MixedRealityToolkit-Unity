// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace HoloToolkit.Examples.Prototyping
{
    public class OrbitPoint : MonoBehaviour
    {
        public Vector3 CenterPoint = new Vector3();
        public Vector3 Axis = Vector3.forward;
        public float Radius = 0.2f;
        public float RevolutionSpeed = 2.0f;
        public float StartAngle = 0;
        public float Revolutions = 0;
        public bool Reversed = false;
        public bool AutoPlay = true;
        public bool IsPaused = false;
        public int RevolutionCount { get; private set; }
        public bool SmoothEaseInOut = false;
        public float SmoothRatio = 1;

        private float mAngle = 0;
        private float mTime = 0;
        private Vector3 mPositionVector;
        private Vector3 mRotatedPositionVector;

        // Use this for initialization
        private void Start()
        {
            RevolutionCount = 0;
            mPositionVector = transform.up;

            if (!Mathf.Approximately(Vector3.Angle(Axis, mPositionVector), 90))
            {
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

            transform.Rotate(Axis, mAngle - StartAngle);
            mRotatedPositionVector = Quaternion.AngleAxis(mAngle - StartAngle, Axis) * mPositionVector * Radius;
            transform.localPosition = CenterPoint + mRotatedPositionVector;

            IsPaused = !AutoPlay;
        }
        
        public void StartOrbit()
        {
            IsPaused = false;
            RevolutionCount = 0;
        }

        public void ResetOrbit()
        {
            mAngle = 0;
        }

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

            float percentage = mTime / RevolutionSpeed;
            if (SmoothEaseInOut)
            {
                float linearSmoothing = 1 * (percentage * (1 - SmoothRatio));
                percentage = QuartEaseInOut(0, 1, percentage) * SmoothRatio + linearSmoothing;
            }
            mAngle = 0 - (percentage) * 360;

            if (Reversed)
            {
                mAngle = -mAngle;
            }

            transform.Rotate(Axis, mAngle - StartAngle);
            mRotatedPositionVector = Quaternion.AngleAxis(mAngle - StartAngle, Axis) * mPositionVector * Radius;
            transform.localPosition = CenterPoint + mRotatedPositionVector;
            
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
