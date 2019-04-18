// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SceneTransitions
{
    /// <summary>
    /// This class manages the 'rotating circle of dots' effect
    /// that is used as a Progress Indicator effect.
    /// </summary>
    public class ProgressIndicatorOrbsRotator : MonoBehaviour, IProgressIndicator
    {
        public Transform MainTransform { get { return transform; } }
        public ProgressIndicatorState State { get { return state; } }
        public float Progress { set { progress = value; } }
        public string Message { set { messageText.text = value; } }

        [SerializeField]
        private GameObject[] orbs = null;
        
        [SerializeField]
        private TextMeshPro messageText = null;

        [SerializeField]
        private ProgressIndicatorState state = ProgressIndicatorState.Closed;
        
        public float RotationSpeedRawDegrees;
        public float SpacingDegrees;
        public float Acceleration;
        public int Revolutions;
        public bool TestStop = false;
        public bool HasAnimationFinished = false;

        private float progress;
        private float timeElapsed;
        private int deployedCount;
        private bool timeUpdated;
        private float[] angles;
        private float timeSlice;
        private float deg2rad = Mathf.PI / 180.0f;
        private Renderer[] dots = null;
        private bool stopRequested;
        private float rotationWhenStopped;
        private MaterialPropertyBlock[] propertyBlocks = null;

        public async Task OpenAsync()
        {
            gameObject.SetActive(true);

            state = ProgressIndicatorState.Opening;

            StartOrbs();

            await Task.Yield();

            state = ProgressIndicatorState.Open;
        }

        public async Task CloseAsync()
        {
            state = ProgressIndicatorState.Closing;

            StopOrbs();

            while (!HasAnimationFinished)
            {
                await Task.Yield();
            }

            state = ProgressIndicatorState.Closed;

            gameObject.SetActive(false);
        }

        private void StartOrbs()
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

            dots = new Renderer[5];
            propertyBlocks = new MaterialPropertyBlock[dots.Length];
            
            for (int i = 0; i < orbs.Length; ++i)
            {
                propertyBlocks[i] = new MaterialPropertyBlock();
                propertyBlocks[i].SetColor("_Color", new Color(1, 1, 1, 1));
                dots[i] = orbs[i].transform.GetChild(0).gameObject.GetComponent<Renderer>();
                dots[i].SetPropertyBlock(propertyBlocks[i]);
            }
        }

        public void StopOrbs()
        {
            stopRequested = true;
            rotationWhenStopped = angles[0];
        }

        private void Update()
        {
            if (state == ProgressIndicatorState.Closed)
                return;

            if (HasAnimationFinished == false)
            {
                UpdateTime();
                ControlDotStarts();
                IncrementOrbs();
                HandleTestStop();
                HandleStopping();
            }
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
            Color adjustedColor = propertyBlocks[index].GetColor("_Color");

            //fade in
            if (stopRequested == false && adjustedColor.a < 1.0f)
            {
                adjustedColor.a += (1.0f * timeSlice);
                adjustedColor.a = Mathf.Min(1.0f, adjustedColor.a);
                propertyBlocks[index].SetColor("_Color", adjustedColor);
                dots[index].SetPropertyBlock(propertyBlocks[index]);
            }
            //fade out
            else if (stopRequested && angles[index] > rotationWhenStopped)
            {
                adjustedColor.a -= (1.0f * timeSlice);
                adjustedColor.a = Mathf.Max(0.0f, adjustedColor.a);
                propertyBlocks[index].SetColor("_Color", adjustedColor);
                dots[index].SetPropertyBlock(propertyBlocks[index]);
            }
        }

        private void HandleTestStop()
        {
            if (TestStop == true && stopRequested == false)
            {
                StopOrbs();
            }
        }

        private void HandleStopping()
        {
            Color adjustedColor = propertyBlocks[orbs.Length - 1].GetColor("_Color");
            if (stopRequested == true && adjustedColor.a <= 0.01f)
            {
                HasAnimationFinished = true;
            }
        }
    }
}
