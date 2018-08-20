// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Interact.Controls
{
    /// <summary>
    /// A Gesture Interactive Controlller for use with GestureWidgets
    /// </summary>
    public class GestureValueControl : GestureInteractiveControl
    {
        [Tooltip("Should the gesture snap back when released?")]
        public bool SnapBack = false;

        [Tooltip("How fast should the gesture snap back?")]
        public float SnapBackTime = 0.25f;
        
        [Tooltip("The value of the slider")]
        public float GestureValue = 0;

        [Tooltip("The currentGesture vector")]
        public Vector3 GestureVector;

        [Tooltip("The Gesture Distance")]
        public float GestureDistance;

        [Tooltip("Should the value originate from a center point, ranges from -1 to +1")]
        public bool Centered = false;
        
        protected float snapBackCount = 0;
        protected float cachedValue;
        protected float deltaValue;
        protected Vector3 lastPosition;

        protected override void Awake()
        {
            base.Awake();

            deltaValue = GestureValue;

            cachedValue = deltaValue;

            snapBackCount = SnapBackTime;
        }

        public override void ManipulationUpdate(Vector3 startGesturePosition, Vector3 currentGesturePosition, Vector3 startHeadOrigin, Vector3 startHeadRay, GestureInteractive.GestureManipulationState gestureState)
        {

            base.ManipulationUpdate(startGesturePosition, currentGesturePosition, startHeadOrigin, startHeadRay, gestureState);

            CalculateGesture();

            if (gestureState == GestureInteractive.GestureManipulationState.None || gestureState == GestureInteractive.GestureManipulationState.Lost)
            {
                // gesture ended - cache the current delta
                cachedValue = deltaValue;

                if (SnapBack)
                {
                    lastPosition = CurrentGesturePosition;
                    snapBackCount = 0;
                }
            }
        }

        protected void CalculateGesture()
        {
            // get the current delta
            float delta = (CurrentDistance > 0) ? CurrentPercentage : -CurrentPercentage;

            // combine the delta with the current slider position so the slider does not start over every time
            if (ClampPercentage)
            {
                deltaValue = Mathf.Clamp01(delta + cachedValue);
            }
            else
            {
                deltaValue = delta * cachedValue;
            }

            GestureValue = deltaValue;
            GestureVector = DirectionVector;
            GestureDistance = CurrentDistance;
        }

        /// <summary>
        /// Animate the dot snapping back to the center point on release
        /// </summary>
        /// <param name="percent"></param>
        protected void TickerUpdate(float percent)
        {
            CurrentGesturePosition = Vector3.Lerp(lastPosition, StartGesturePosition, percent);

            UpdateGesture();
            cachedValue = 0;
            CalculateGesture();
            
        }

        /// <summary>
        /// Handle automation
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (snapBackCount < SnapBackTime)
            {
                snapBackCount += Time.deltaTime;
                if (snapBackCount > SnapBackTime)
                {
                    snapBackCount = SnapBackTime;
                }

                TickerUpdate(snapBackCount / SnapBackTime);
            }
        }
    }
}
