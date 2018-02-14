// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;


namespace MixedRealityToolkit.UX.Progress
{
    public class ProgressIndicatorOrbsRotator : MonoBehaviour
    {
        public GameObject[] Orbs;

        public float RotationSpeedRawDegrees;

        public float SpacingDegrees;

        public float Acceleration;

        public int Revolutions;

        public bool TestStop = false;

        public bool HasAnimationFinished = false;

        private float timeElapsed;
        private int deployedCount;
        private bool timeUpdated;
        private float[] angles;
        private float timeSlice;
        private float deg2rad = Mathf.PI / 180.0f;
        private GameObject[] dots;
        private bool stopRequested;
        private float rotationWhenStopped;

        // Use this for initialization
        void Start()
        {
            rotationWhenStopped = 0.0f;
            stopRequested = false;
            timeSlice = 0;
            timeUpdated = false;
            deployedCount = 1;
            timeElapsed = 0.0f;
            angles = new float[Orbs.Length];
            for (int i = 0; i < angles.Length; ++i)
            {
                angles[i] = 0;
            }

            dots = new GameObject[5];
            for (int i = 0; i < Orbs.Length; ++i)
            {
                dots[i] = Orbs[i].transform.GetChild(0).gameObject;
                dots[i].GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (HasAnimationFinished == false)
            {
                UpdateTime();
                ControlDotStarts();
                IncrementOrbs();
                HandleTestStop();
                HandleStopping();
            }
        }

        public void Stop()
        {
            stopRequested = true;
            rotationWhenStopped = angles[0];
        }

        void UpdateTime()
        {
            if (timeUpdated == false)
            {
                timeSlice = 0.0f;
                timeElapsed = 0.0f;
                timeUpdated = true;
            }
            else
            {
                timeSlice = Time.unscaledDeltaTime;
                timeElapsed += timeSlice;
            }
        }

        void ControlDotStarts()
        {
            if (deployedCount < Orbs.Length)
            {
                if (angles[deployedCount - 1] >= SpacingDegrees)
                {
                    deployedCount++;
                }
            }
        }

        void IncrementOrbs()
        {
            for (int i = 0; i < deployedCount; ++i)
            {
                IncrementOrb(i);
            }
        }

        void IncrementOrb(int index)
        {
            float acceleratedDegrees = (RotationSpeedRawDegrees * (Acceleration + -Mathf.Cos(deg2rad * angles[index]))) * timeSlice;
            Orbs[index].gameObject.transform.Rotate(0, 0, acceleratedDegrees);
            angles[index] += Mathf.Abs(acceleratedDegrees);

            HandleFade(index);
        }

        void HandleFade(int index)
        {
            Color adjustedColor = dots[index].GetComponent<Renderer>().material.color;

            //fade in
            if (stopRequested == false && adjustedColor.a < 1.0f)
            {
                adjustedColor.a += (1.0f * timeSlice);
                adjustedColor.a = Mathf.Min(1.0f, adjustedColor.a);
                dots[index].GetComponent<Renderer>().material.color = adjustedColor;
            }
            //fade out
            else if (stopRequested && angles[index] > rotationWhenStopped)
            {
                adjustedColor.a -= (1.0f * timeSlice);
                adjustedColor.a = Mathf.Max(0.0f, adjustedColor.a);
                dots[index].GetComponent<Renderer>().material.color = adjustedColor;
            }
        }

        void HandleTestStop()
        {
            if (TestStop == true && stopRequested == false)
            {
                Stop();
            }
        }

        void HandleStopping()
        {
            if (stopRequested == true && dots[Orbs.Length - 1].GetComponent<Renderer>().material.color.a <= 0.01f)
            {
                HasAnimationFinished = true;
            }
        }
    }
}
