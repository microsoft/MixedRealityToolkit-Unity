// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Ease settings and functionality for animation with curves
    /// </summary>

    [System.Serializable]
    public class Easing
    {
        /// <summary>
        /// basic ease curves for quick settings
        /// </summary>
        public enum BasicEaseCurves { Linear, EaseIn, EaseOut, EaseInOut }

        /// <summary>
        /// Is the ease enabled?
        /// </summary>
        public bool Enabled = false;

        /// <summary>
        /// The animation curve to use for the ease - default should be linear
        /// </summary>
        public AnimationCurve Curve = AnimationCurve.Linear(0, 1, 1, 1);


        /// <summary>
        /// The amount of time the ease should run in seconds
        /// </summary>
        public float LerpTime = 0.5f;

        private float timer = 0.5f;
        private Keyframe[] cachedKeys;

        public Easing()
        {
            Stop();
        }

        /// <summary>
        /// Create Easing object with copied internal properties
        /// </summary>
        /// <returns>Copy of current Easing instance</returns>
        public Easing Copy()
        {
            return new Easing()
            {
                Curve = this.Curve,
                Enabled = this.Enabled,
                LerpTime = this.LerpTime,
            };
        }

        /// <summary>
        /// Update the ease each frame or on Update
        /// </summary>
        public void OnUpdate()
        {
            if (timer < LerpTime)
            {
                timer = Mathf.Min(timer + Time.deltaTime, LerpTime);
            }
        }

        /// <summary>
        /// start the ease if enabled
        /// </summary>
        public void Start()
        {
            timer = 0;
            if (!Enabled)
            {
                timer = LerpTime;
            }
        }

        /// <summary>
        /// Is the ease currently running?
        /// </summary>
        public bool IsPlaying()
        {
            return timer < LerpTime;
        }

        /// <summary>
        /// stop the ease
        /// </summary>
        public void Stop()
        {
            timer = LerpTime;
        }

        /// <summary>
        /// get the linear ease value
        /// </summary>
        public float GetLinear()
        {
            return timer / LerpTime;
        }

        /// <summary>
        /// get the ease value based on the animation curve
        /// </summary>
        public float GetCurved()
        {
            return IsLinear() ? GetLinear() : Curve.Evaluate(GetLinear());
        }

        protected bool IsLinear()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // If we're in the editor and we're not playing
                // always update the cached keys to reflect recent changes
                cachedKeys = Curve.keys;
            }
#endif
            if (cachedKeys == null)
            {
                cachedKeys = Curve.keys;
            }

            if (cachedKeys.Length > 1)
            {
                return (cachedKeys[0].value == 1 && cachedKeys[1].value == 1);
            }

            return false;
        }

        /// <summary>
        /// set the animation curve using a preset
        /// </summary>
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

            // Update the cached keys
            cachedKeys = Curve.keys;
        }
    }
}
