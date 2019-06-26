// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// This class manages the 'rotating circle of dots' effect
    /// that is used as a Progress Indicator effect.
    /// </summary>
    public class ProgressIndicatorOrbsRotator : MonoBehaviour, IProgressIndicator
    {
        /// <inheritdoc/>
        public Transform MainTransform { get { return transform; } }

        /// <inheritdoc/>
        public ProgressIndicatorState State { get { return state; } }

        /// <inheritdoc/>
        public float Progress { set { progress = value; } }

        /// <inheritdoc/>
        public string Message { set { messageText.text = value; } }

        [SerializeField]
        private GameObject[] orbs = null;
        [SerializeField]
        private TextMeshPro messageText = null;
        [SerializeField]
        private ProgressIndicatorState state = ProgressIndicatorState.Closed;
        [SerializeField]
        public float rotationSpeedRawDegrees = -200f;
        [SerializeField]
        public float spacingDegrees = 22f;
        [SerializeField]
        public float acceleration = 1.4f;
        [SerializeField]
        public int revolutions = 3;
        [SerializeField]
        public bool testStop = false;
        [SerializeField]
        public bool hasAnimationFinished = false;

        private float progress;
        private float timeElapsed;
        private int deployedCount;
        private float[] angles;
        private float timeSlice;
        private Renderer[] dots = null;
        private bool stopRequested;
        private float rotationWhenStopped;
        private MaterialPropertyBlock[] propertyBlocks = null;

        /// <inheritdoc/>
        public async Task OpenAsync()
        {
            if (state != ProgressIndicatorState.Closed)
            {
                throw new System.Exception("Can't open in state " + state);
            }

            gameObject.SetActive(true);

            state = ProgressIndicatorState.Opening;

            StartOrbs();

            await Task.Yield();

            state = ProgressIndicatorState.Open;
        }

        /// <inheritdoc/>
        public async Task CloseAsync()
        {
            if (state != ProgressIndicatorState.Open)
            {
                throw new System.Exception("Can't close in state " + state);
            }

            state = ProgressIndicatorState.Closing;

            StopOrbs();

            while (!hasAnimationFinished)
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
            timeSlice = 0.0f;
            timeElapsed = 0.0f;
            deployedCount = 1;
            timeElapsed = 0.0f;

            if (angles == null || angles.Length != orbs.Length)
            {
                angles = new float[orbs.Length];
                for (int i = 0; i < angles.Length; ++i)
                {
                    angles[i] = 0;
                }
            }

            dots = new Renderer[orbs.Length];
            propertyBlocks = new MaterialPropertyBlock[orbs.Length];

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
            {
                return;
            }

            if (!hasAnimationFinished)
            {
                timeSlice = Time.unscaledDeltaTime;
                timeElapsed += timeSlice;

                if (deployedCount < orbs.Length)
                {
                    if (angles[deployedCount - 1] >= spacingDegrees)
                    {
                        deployedCount++;
                    }
                }

                for (int index = 0; index < deployedCount; ++index)
                {
                    float acceleratedDegrees = (rotationSpeedRawDegrees * (acceleration + -Mathf.Cos(Mathf.Deg2Rad * angles[index]))) * timeSlice;
                    orbs[index].gameObject.transform.Rotate(0, 0, acceleratedDegrees);
                    angles[index] += Mathf.Abs(acceleratedDegrees);

                    Color orbColor = propertyBlocks[index].GetColor("_Color");

                    //fade in
                    if (!stopRequested && orbColor.a < 1.0f)
                    {
                        orbColor.a += (1.0f * timeSlice);
                        orbColor.a = Mathf.Min(1.0f, orbColor.a);
                        propertyBlocks[index].SetColor("_Color", orbColor);
                        dots[index].SetPropertyBlock(propertyBlocks[index]);
                    }
                    //fade out
                    else if (stopRequested && angles[index] > rotationWhenStopped)
                    {
                        orbColor.a -= (1.0f * timeSlice);
                        orbColor.a = Mathf.Max(0.0f, orbColor.a);
                        propertyBlocks[index].SetColor("_Color", orbColor);
                        dots[index].SetPropertyBlock(propertyBlocks[index]);
                    }
                }

                Color adjustedColor = propertyBlocks[orbs.Length - 1].GetColor("_Color");
                if (stopRequested == true && adjustedColor.a <= 0.01f)
                {
                    hasAnimationFinished = true;
                }

#if UNITY_EDITOR
                if (testStop && !stopRequested)
                {
                    stopRequested = true;
                    rotationWhenStopped = angles[0];
                }
#endif
            }
        }
    }
}
