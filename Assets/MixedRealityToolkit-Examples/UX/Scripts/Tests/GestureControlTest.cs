// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.UX.Tests
{
    /// <summary>
    /// A sample GestureInteractiveControl that moves and element in space using raw gesture data
    /// </summary>
    public class GestureControlTest : GestureInteractiveControl
    {

        public GameObject EffectDot;
        public Color[] EffectColors;
        public Interactive Button;
        public float FeebackVisualDistance = 0.95f;
        
        private Renderer mEffectRenderer;
        private bool mHasGaze = false;

        private float mTickerTime = 0.5f;
        private float mTickerCount = 0;

        private void Start()
        {
            mEffectRenderer = EffectDot.GetComponent<Renderer>();
            mTickerCount = mTickerTime;
        }

        /// <summary>
        /// provide visual feedback based on state and update element position
        /// </summary>
        /// <param name="startVector"></param>
        /// <param name="currentVector"></param>
        /// <param name="startOrigin"></param>
        /// <param name="startRay"></param>
        /// <param name="gestureState"></param>
        public override void ManipulationUpdate(Vector3 startVector, Vector3 currentVector, Vector3 startOrigin, Vector3 startRay, GestureInteractive.GestureManipulationState gestureState)
        {
            base.ManipulationUpdate(startVector, currentVector, startOrigin, startRay, gestureState);

            Vector3 mDirection = DirectionVector.normalized;

            if (gestureState == GestureInteractive.GestureManipulationState.Start)
            {
                mTickerCount = mTickerTime;

                mEffectRenderer.material.color = EffectColors[1];
            }

            if (gestureState == GestureInteractive.GestureManipulationState.None)
            {
                mTickerCount = 0;

                mEffectRenderer.material.color = EffectColors[0];
            }

            EffectDot.transform.localPosition = mDirection * FeebackVisualDistance * CurrentPercentage;
        }

        /// <summary>
        /// Animate the dot snapping back to the center point on release
        /// </summary>
        /// <param name="percent"></param>
        private void TickerUpdate(float percent)
        {
            EffectDot.transform.localPosition = Vector3.Lerp(EffectDot.transform.localPosition, Vector3.zero, percent);
        }

        /// <summary>
        /// Update visuals based on gaze and power the ticker
        /// </summary>
        protected override void Update()
        {
            if (mHasGaze != Button.HasGaze)
            {
                EffectDot.SetActive(Button.HasGaze);
                mHasGaze = Button.HasGaze;
            }

            if (mTickerCount < mTickerTime)
            {
                mTickerCount += Time.deltaTime;
                if (mTickerCount > mTickerTime)
                {
                    mTickerCount = mTickerTime;
                }

                TickerUpdate(mTickerCount / mTickerTime);
            }
        }
    }
}
