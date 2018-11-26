// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Examples.Prototyping;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// An animation system for rotating a group of objects around a point.
    /// Use a transparent material and the objects will fade at the end of each cycle.
    /// </summary>
    public class LoadingAnimation : MonoBehaviour
    {
        [Tooltip("The dark theme color")]
        public Color DarkColor;
        [Tooltip("The light theme color")]
        public Color LightColor;
        [Tooltip("which theme should be used")]
        public bool UseLightTheme = false;
        [Tooltip("should the loading animation auto start?")]
        public bool AutoStart = true;

        [Tooltip("An array of gameObjects to animate")]
        public GameObject[] Orbs;
        
        [Tooltip("The position to animate around")]
        public Vector3 CenterPoint = new Vector3();
        [Tooltip("The axis for the orbit")]
        public Vector3 Axis = Vector3.forward;
        [Tooltip("Radius of the orbit")]
        public float Radius = 0.075f;
        [Tooltip("Speed of the orbit")]
        public float RevolutionSpeed = 1.9f;
        [Tooltip("How many revolutions per cycle")]
        public int Revolutions = 3;
        [Tooltip("The space or angle between each element")]
        public float AngleSpace = 12;

        [Tooltip("Are we paused?")]
        public bool IsPaused = false;
        [Tooltip("smooth easing or linear revolutions")]
        public bool SmoothEaseInOut = false;
        [Tooltip("If smooth easing, how smooth?")]
        public float SmoothRatio = 0.65f;

        /// <summary>
        /// Internal functional values
        /// </summary>
        // current angle
        private float mAngle = 0;
        //current time
        private float mTime = 0;
        // current revolution count
        private int mRevolutionsCount = 0;
        // is it time to pause or setup the next cycle?
        private bool mLoopPause = false;
        // the currently fading element
        private int mFadeIndex = 0;
        // check the loopPause next Update
        private bool mCheckLoopPause = false;
        // The center position
        private Vector3 mPositionVector;
        // the rotation vector during the animation
        private Vector3 mRotatedPositionVector;
        
        // the loader is starting
        private bool mStartingLoader = false;
        // the index of the Orbs to start with
        private int mStartingIndex;

        /// <summary>
        /// setup all the orbs
        /// </summary>
        private void Awake()
        {
            for (int i = 0; i < Orbs.Length; ++i)
            {
                Orbs[i].GetComponent<Renderer>().material.color = UseLightTheme ? LightColor : DarkColor;
                Orbs[i].SetActive(false);
                FadeColors fade = Orbs[i].GetComponent<FadeColors>();
                fade.ResetFade(0);
            }
        }

        /// <summary>
        /// setup the position of the animation and elements
        /// </summary>
        void Start()
        {
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
                        mPositionVector = Vector3.Cross(Axis * Radius, transform.forward);
                    }

                    if (z > y && z > x)
                    {
                        // forward or backward - cross with the x axis
                        mPositionVector = Vector3.Cross(Axis * Radius, transform.right);
                    }

                    if (y > z && y > x)
                    {
                        // up or down - cross with the x axis
                        mPositionVector = Vector3.Cross(Axis * Radius, transform.right);
                    }
                }
            }
            
            if (AutoStart)
            {
                StartLoader();
            }
        }

        /// <summary>
        /// Starting the loading animation
        /// </summary>
        public void StartLoader()
        {
            mStartingLoader = true;
            mStartingIndex = 0;
            mRevolutionsCount = 0;
            IsPaused = false;
        }

        /// <summary>
        /// stopping the loading animation
        /// </summary>
        public void StopLoader()
        {
            for (int i = 0; i < Orbs.Length; ++i)
            {
                Orbs[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// expose a method to resume
        /// </summary>
        public void ResumeOrbit()
        {
            IsPaused = false;
        }

        /// <summary>
        /// reset the angle of the animation without restarting
        /// </summary>
        public void ResetOrbit()
        {
            mAngle = 0;
        }

        /// <summary>
        /// easing function
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public float QuartEaseInOut(float s, float e, float v)
        {
            //e -= s;
            if ((v /= 0.5f) < 1)
                return e / 2 * v * v * v * v + s;

            return -e / 2 * ((v -= 2) * v * v * v - 2) + s;
        }

        /// <summary>
        /// Animate the loader
        /// </summary>
        void Update()
        {
            if (IsPaused) return;
            
            float percentage = mTime / RevolutionSpeed;

            for (int i = 0; i < Orbs.Length; ++i)
            {
                GameObject orb = Orbs[i];

                // get the revolution completion percentage
                float orbPercentage = percentage - AngleSpace / 360 * i;
                if (orbPercentage < 0)
                {
                    orbPercentage = 1 + orbPercentage;
                }

                if (SmoothEaseInOut)
                {
                    float linearSmoothing = 1 * (orbPercentage * (1 - SmoothRatio));
                    orbPercentage = QuartEaseInOut(0, 1, orbPercentage) * SmoothRatio + linearSmoothing;
                }

                // set the angle
                mAngle = 0 - (orbPercentage) * 360;

                if (mStartingLoader)
                {
                    if (orbPercentage >= 0 && orbPercentage < 0.5f)
                    {
                        if (i == mStartingIndex)
                        {
                            orb.SetActive(true);
                            if (i >= Orbs.Length - 1)
                            {
                                mStartingLoader = false;
                            }
                            mStartingIndex += 1;
                        }
                    }
                }

                // apply the values
                orb.transform.Rotate(Axis, mAngle);
                mRotatedPositionVector = Quaternion.AngleAxis(mAngle, Axis) * mPositionVector * Radius;
                orb.transform.localPosition = CenterPoint + mRotatedPositionVector;

                // check for looping and handle loop counts
                if (mCheckLoopPause != mLoopPause)
                {
                    if (mLoopPause && orbPercentage > 0.25f)
                    {
                        if (i == mFadeIndex)
                        {
                            FadeColors fade = orb.GetComponent<FadeColors>();
                            fade.FadeOut(false);
                            if (i >= Orbs.Length - 1)
                            {
                                mCheckLoopPause = mLoopPause;
                            }
                            mFadeIndex += 1;
                        }
                        
                    }

                    if (!mLoopPause && orbPercentage > 0.5f)
                    {
                        if (i == mFadeIndex)
                        {
                            FadeColors fade = orb.GetComponent<FadeColors>();
                            fade.FadeIn(false);
                            if (i >= Orbs.Length - 1)
                            {
                                mCheckLoopPause = mLoopPause;
                            }
                            mFadeIndex += 1;
                        }
                    }
                    
                }
            }

            mTime += Time.deltaTime;
            if (!mLoopPause)
            {
                if (mTime >= RevolutionSpeed)
                {
                    mTime = mTime - RevolutionSpeed;

                    mRevolutionsCount += 1;

                    if (mRevolutionsCount >= Revolutions && Revolutions > 0)
                    {
                        mLoopPause = true;
                        mFadeIndex = 0;
                        mRevolutionsCount = 0;
                    }
                }
            }
            else
            {
                if (mTime >= RevolutionSpeed)
                {
                    mTime = 0;
                    mRevolutionsCount += 1;
                    if (mRevolutionsCount >= Revolutions * 0.25f)
                    {
                        mFadeIndex = 0;
                        mLoopPause = false;
                        mRevolutionsCount = 0;
                    }
                }
            }
        }
    }
}
