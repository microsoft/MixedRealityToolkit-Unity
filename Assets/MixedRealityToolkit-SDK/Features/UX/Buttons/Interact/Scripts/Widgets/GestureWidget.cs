// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Blend;
using Interact.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Widgets
{
    /// <summary>
    /// the base class for all gesture widgets
    /// </summary>
    public abstract class GestureWidget : MonoBehaviour
    {
        public GestureValueControl Control;
        public AnimationCurve Curve;

        protected float curvedValue;

        protected virtual void Awake()
        {
            if (Curve == null)
            {
                SetEaseCurve(BasicEaseCurves.Linear);
            }
            else if (Curve.keys.Length < 1)
            {
                SetEaseCurve(BasicEaseCurves.Linear);
            }
        }

        protected virtual void Start()
        {
            if (Control == null)
            {
                Control = GetComponentInParent<GestureValueControl>();
            }
        }

        /// <summary>
        /// Handles applying gesture values to the object
        /// </summary>
        protected abstract void UpdateValues(float percent);

        /// <summary>
        /// Update visuals based on gaze and power the ticker
        /// </summary>
        protected void Update()
        {
            if (Control != null)
            {
                curvedValue = IsLinear() ? Control.GestureValue : Curve.Evaluate(Control.GestureValue);
                UpdateValues(curvedValue);
            }
        }

        public void SetEaseCurve(BasicEaseCurves curve)
        {
            switch (curve)
            {
                case BasicEaseCurves.Linear:
                    Curve = AnimationCurve.Linear(0, 1, 1, 1);
                    break;
                case BasicEaseCurves.EaseIn:
                    Curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1, 2.5f, 0));
                    break;
                case BasicEaseCurves.EaseOut:
                    Curve = new AnimationCurve(new Keyframe(0, 0, 0, 2.5f), new Keyframe(1, 1));
                    break;
                case BasicEaseCurves.EaseInOut:
                    Curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                    break;
                default:
                    break;
            }
        }
        
        protected bool IsLinear()
        {
            if (Curve.keys.Length > 1)
            {
                return (Curve.keys[0].value == 1 && Curve.keys[1].value == 1);
            }

            return false;
        }
    }
}
