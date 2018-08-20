// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace Interact.Controls
{
    /// <summary>
    /// A sample GestureInteractiveControl that moves and element in space using raw gesture data
    /// </summary>
    public class GestureControlMoveObject : GestureInteractiveControl
    {
        public GameObject MoveObject;
        public Interactive Button;
        public float FeebackVisualDistance = 0.95f;
        public bool SnapBack = false;

        private float tickerTime = 0.5f;
        private float tickerCount = 0;
        private Vector3 startPosition;

        private void Start()
        {
            startPosition = MoveObject.transform.localPosition;
            tickerCount = tickerTime;
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

            if (FeebackVisualDistance > 0)
            {
                MoveObject.transform.localPosition = startPosition + mDirection * FeebackVisualDistance * CurrentPercentage;
            }
            else
            {
                MoveObject.transform.localPosition = startPosition + mDirection * CurrentPercentage;
            }
            
            if ((gestureState == GestureInteractive.GestureManipulationState.Lost || gestureState == GestureInteractive.GestureManipulationState.None))
            {
                if (SnapBack)
                {
                    tickerCount = 0;
                }
                else
                {
                    startPosition = MoveObject.transform.localPosition;
                }
            }
        }

        /// <summary>
        /// Animate the dot snapping back to the center point on release
        /// </summary>
        /// <param name="percent"></param>
        private void TickerUpdate(float percent)
        {
            MoveObject.transform.localPosition = Vector3.Lerp(MoveObject.transform.localPosition, startPosition, percent);
        }

        /// <summary>
        /// Update visuals based on gaze and power the ticker
        /// </summary>
        protected override void Update()
        {
            if (tickerCount < tickerTime)
            {
                tickerCount += Time.deltaTime;
                if (tickerCount > tickerTime)
                {
                    tickerCount = tickerTime;
                }

                TickerUpdate(tickerCount / tickerTime);
            }
        }
    }
}
