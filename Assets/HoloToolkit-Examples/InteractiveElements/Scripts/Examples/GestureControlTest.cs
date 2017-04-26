// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using HoloToolkit.Examples.Prototyping;

namespace HoloToolkit.Examples.InteractiveElements
{

    public class GestureControlTest : GestureInteractiveControl
    {

        public GameObject EffectDot;
        public Color[] EffectColors;
        public Interactive Button;
        public float FeebackVisualDistance = 0.95f;

        private Ticker mSnapBackTicker;
        private Renderer mEffectRenderer;
        private bool mHasGaze = false;

        private void Start()
        {
            mEffectRenderer = EffectDot.GetComponent<Renderer>();
        }

        public override void ManipulationUpdate(Vector3 startVector, Vector3 currentVector, Vector3 startOrigin, Vector3 startRay, GestureInteractive.GestureManipulationState gestureState)
        {
            base.ManipulationUpdate(startVector, currentVector, startOrigin, startRay, gestureState);

            Vector3 mDirection = DirectionVector.normalized;

            if (gestureState == GestureInteractive.GestureManipulationState.Start)
            {
                if (mSnapBackTicker != null)
                {
                    mSnapBackTicker.Stop();
                }

                mEffectRenderer.material.color = EffectColors[1];
            }

            if (gestureState == GestureInteractive.GestureManipulationState.None)
            {
                if (mSnapBackTicker == null)
                {
                    mSnapBackTicker = new Ticker(this, 0.5f, null);
                    mSnapBackTicker.OnTick += TickerUpdate;
                }
                mSnapBackTicker.Start();

                mEffectRenderer.material.color = EffectColors[0];
            }

            EffectDot.transform.localPosition = mDirection * FeebackVisualDistance * CurrentPercentage;
        }

        private void TickerUpdate(Ticker ticker, Ticker.TickerEventType type)
        {
            EffectDot.transform.localPosition = Vector3.Lerp(EffectDot.transform.localPosition, new Vector3(), ticker.CurrentTime / ticker.Duration);
        }

        protected override void Update()
        {
            if (mHasGaze != Button.HasGaze)
            {
                EffectDot.SetActive(Button.HasGaze);
                mHasGaze = Button.HasGaze;
            }

        }

        private void OnDestory()
        {
            if (mSnapBackTicker != null)
            {
                mSnapBackTicker.OnTick -= TickerUpdate;
                mSnapBackTicker.Stop();

            }
        }

    }
}
