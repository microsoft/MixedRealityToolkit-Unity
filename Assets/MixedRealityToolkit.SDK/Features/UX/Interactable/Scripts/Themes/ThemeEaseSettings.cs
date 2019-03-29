// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Ease settings and cunctionality for themes
    /// </summary>
    
    [System.Serializable]
    public class ThemeEaseSettings
    {
        public enum BasicEaseCurves { Linear, EaseIn, EaseOut, EaseInOut }
        public bool EaseValues = false;
        public AnimationCurve Curve = AnimationCurve.Linear(0, 1, 1, 1);
        public float LerpTime = 0.5f;
        private float timer = 0.5f;

        public ThemeEaseSettings()
        {
            Stop();
        }

        public void OnUpdate()
        {
            if (timer < LerpTime)
            {
                timer = Mathf.Min(timer + Time.deltaTime, LerpTime);
            }
        }

        public void Start()
        {
            timer = 0;
            if (!EaseValues)
            {
                timer = LerpTime;
            }
        }

        public bool IsPlaying()
        {
            return timer < LerpTime;
        }

        public void Stop()
        {
            timer = LerpTime;
        }

        public float GetLinear()
        {
            return timer / LerpTime;
        }

        public float GetCurved()
        {
            return IsLinear() ? GetLinear() : Curve.Evaluate(GetLinear());
        }

        protected bool IsLinear()
        {
            if (Curve.keys.Length > 1)
            {
                return (Curve.keys[0].value == 1 && Curve.keys[1].value == 1);
            }

            return false;
        }

        public void SetCurve(BasicEaseCurves curve)
        {
            AnimationCurve animation = AnimationCurve.Linear(0, 1, 1, 1);
            switch (curve)
            {
                case BasicEaseCurves.EaseIn:
                    animation = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1, 2.5f, 0));
                    break;
                case BasicEaseCurves.EaseOut:
                    animation = new AnimationCurve(new Keyframe(0, 0, 0, 2.5f), new Keyframe(1, 1));
                    break;
                case BasicEaseCurves.EaseInOut:
                    animation = AnimationCurve.EaseInOut(0, 0, 1, 1);
                    break;
                default:
                    break;
            }

            Curve = animation;
        }
    }
}
