// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;


namespace MixedRealityToolkit.UX.Progress
{
    public class ProgressIndicatorOrbsRotator : MonoBehaviour
    {
        [SerializeField]
        public GameObject[] orbs;

        [SerializeField]
        private Material orbMaterial;

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
        private Material[] materials;

        private void Start()
        {
            rotationWhenStopped = 0.0f;
            stopRequested = false;
            timeSlice = 0;
            timeUpdated = false;
            deployedCount = 1;
            timeElapsed = 0.0f;
            angles = new float[orbs.Length];
            for (int i = 0; i < angles.Length; ++i)
            {
                angles[i] = 0;
            }

            dots = new GameObject[5];
            materials = new Material[dots.Length];
            
            for (int i = 0; i < orbs.Length; ++i)
            {
                materials[i] = (Material)Instantiate(orbMaterial);
                materials[i].color = new Color(1, 1, 1, 1);
                dots[i] = orbs[i].transform.GetChild(0).gameObject;
                materials[i] = dots[i].GetComponent<Renderer>().sharedMaterial = materials[i];
            }
        }

        private void Update()
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

        private void UpdateTime()
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

        private void ControlDotStarts()
        {
            if (deployedCount < orbs.Length)
            {
                if (angles[deployedCount - 1] >= SpacingDegrees)
                {
                    deployedCount++;
                }
            }
        }

        private void IncrementOrbs()
        {
            for (int i = 0; i < deployedCount; ++i)
            {
                IncrementOrb(i);
            }
        }

        private void IncrementOrb(int index)
        {
            float acceleratedDegrees = (RotationSpeedRawDegrees * (Acceleration + -Mathf.Cos(deg2rad * angles[index]))) * timeSlice;
            orbs[index].gameObject.transform.Rotate(0, 0, acceleratedDegrees);
            angles[index] += Mathf.Abs(acceleratedDegrees);

            HandleFade(index);
        }

        private void HandleFade(int index)
        {
            Color adjustedColor = materials[index].color;

            //fade in
            if (stopRequested == false && adjustedColor.a < 1.0f)
            {
                adjustedColor.a += (1.0f * timeSlice);
                adjustedColor.a = Mathf.Min(1.0f, adjustedColor.a);
                materials[index].color = adjustedColor;
            }
            //fade out
            else if (stopRequested && angles[index] > rotationWhenStopped)
            {
                adjustedColor.a -= (1.0f * timeSlice);
                adjustedColor.a = Mathf.Max(0.0f, adjustedColor.a);
                materials[index].color = adjustedColor;
            }
        }

        private void HandleTestStop()
        {
            if (TestStop == true && stopRequested == false)
            {
                Stop();
            }
        }

        private void HandleStopping()
        {
            if (stopRequested == true && materials[orbs.Length - 1].color.a <= 0.01f)
            {
                HasAnimationFinished = true;
            }
        }
    }
}
