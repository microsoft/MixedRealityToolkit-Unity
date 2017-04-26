// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HoloToolkit.Examples.Prototyping
{
    public class CycleColors : CycleArray<Color>
    {
        public bool BlendColors;
        public float BlendTime = 0.75f;
        public bool SmoothBlend;

        private float mBlendCounter = 0;
        private Color mTargetColor;

        private Renderer mRenderer;

        public override void SetIndex(int index)
        {
            base.SetIndex(index);

            if (mRenderer == null)
            {
                mRenderer = TargetObject.GetComponent<Renderer>();
            }

            mTargetColor = Array[Index];

            if (BlendColors)
            {
                mBlendCounter = 0;
            }
            else
            {
                if (mRenderer != null)
                {
                    GetComponent<Renderer>().material.color = mTargetColor;
                }
                mBlendCounter = BlendTime;
            }
        }

        private void ApplyColorBlend(Color targetColor, float lerpPercentage)
        {
            Color newColor = targetColor;

            if (mRenderer == null)
            {
                mRenderer = TargetObject.GetComponent<Renderer>();
            }

            if (mRenderer != null)
            {
                if (lerpPercentage < 1)
                {
                    float smoothPercentage = lerpPercentage;
                    if (SmoothBlend)
                    {
                        smoothPercentage = -1 * 0.5f * (Mathf.Cos(Mathf.PI * lerpPercentage) - 1);
                    }

                    newColor = Color.LerpUnclamped(GetComponent<Renderer>().material.color, targetColor, smoothPercentage);
                }
                GetComponent<Renderer>().material.color = newColor;
            }
        }

        private void Update()
        {
            if (mBlendCounter < BlendTime)
            {
                mBlendCounter += Time.deltaTime;

                if (mBlendCounter >= BlendTime)
                {
                    mBlendCounter = BlendTime;
                }

                ApplyColorBlend(mTargetColor, mBlendCounter / BlendTime);
            }
        }
    }
}
