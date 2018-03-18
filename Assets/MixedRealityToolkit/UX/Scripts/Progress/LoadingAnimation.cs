// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using MixedRealityToolkit.Examples.InteractiveElements;

namespace MixedRealityToolkit.Examples.UX
{
    public class LoadingAnimation : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] orbs;
        private bool mStartingLoader = false;
        private int mStartingIndex;

        public Vector3 CenterPoint = new Vector3();
        public Vector3 Axis = Vector3.forward;
        public float Radius = 0.075f;
        public float RevolutionSpeed = 1.9f;
        public int Revolutions = 3;
        public float AngleSpace = 12;
        public bool IsPaused = false;
        public bool SmoothEaseInOut = false;
        public float SmoothRatio = 0.65f;

        private float mAngle = 0;
        private float mTime = 0;
        private int mRevolutionsCount = 0;
        private bool mLoopPause = false;
        private int mFadeIndex = 0;
        private bool mCheckLoopPause = false;
        private Vector3 mPositionVector;
        private Vector3 mRotatedPositionVector;
        private LoadingAnimation loadingAnimation;

        public GameObject[] Orbs
        {
            get
            {
                return orbs;
            }

            set
            {
                orbs = value;
            }
        }

        private void Start()
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
        }

        public void StartLoader()
        {
            loadingAnimation = gameObject.GetComponent<LoadingAnimation>();
            for (int i = 0; i < Orbs.Length; ++i)
            {
                Orbs[i].SetActive(false);
                FadeObject fade = Orbs[i].GetComponent<FadeObject>();
                fade.ResetFade(0);
            }

            mStartingLoader = true;
            mStartingIndex = 0;
            mRevolutionsCount = 0;
            IsPaused = false;
        }

        public void StopLoader()
        {
            for (int i = 0; i < Orbs.Length; ++i)
            {
                Orbs[i].gameObject.SetActive(false);
            }
        }

        public void StartOrbit()
        {
            IsPaused = false;
        }

        public void ResetOrbit()
        {
            mAngle = 0;
        }

        public float QuartEaseInOut(float s, float e, float v)
        {
            //e -= s;
            if ((v /= 0.5f) < 1)
                return e / 2 * v * v * v * v + s;

            return -e / 2 * ((v -= 2) * v * v * v - 2) + s;
        }

        private void Update()
        {
            if (IsPaused) return;

            float percentage = mTime / RevolutionSpeed;

            for (int i = 0; i < Orbs.Length; ++i)
            {
                GameObject orb = Orbs[i];
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

                orb.transform.Rotate(Axis, mAngle);
                mRotatedPositionVector = Quaternion.AngleAxis(mAngle, Axis) * mPositionVector * Radius;
                orb.transform.localPosition = CenterPoint + mRotatedPositionVector;

                if (mCheckLoopPause != mLoopPause)
                {
                    if (mLoopPause && orbPercentage > 0.25f)
                    {
                        if (i == mFadeIndex)
                        {
                            FadeObject fade = orb.GetComponent<FadeObject>();
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
                            FadeObject fade = orb.GetComponent<FadeObject>();
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
                        loadingAnimation.gameObject.SetActive(false);
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

