// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Blend
{
    [System.Serializable]
    public enum LerpTypes { Timed, Free }

    /// <summary>
    /// ease types
    ///     Linear: steady progress
    ///     EaseIn: ramp up in speed
    ///     EaseOut: ramp down in speed
    ///     EaseInOut: ramp up then down in speed
    ///     Free: super ease - just updates as the TargetValue changes
    /// </summary>
    public enum BasicEaseCurves { Linear, EaseIn, EaseOut, EaseInOut }

    [System.Serializable]
    public enum LoopTypes { None, Repeat, PingPong }

    [System.Serializable]
    public struct BlendStatus
    {
        public AbstractBlend Blender;
        public bool Disabled;
    }

    [System.Serializable]
    public struct ShaderProperties
    {
        public string Name;
        public ShaderPropertyType Type;
        public Vector2 Range;
    }

    public enum ShaderPropertyType {Color, Float, Range, TexEnv, Vector, None }

    /// <summary>
    /// animates the rotation of an object with eases
    /// </summary>
    [ExecuteInEditMode]
    public abstract class Blend<T> : AbstractBlend
    {

        [Tooltip("The object to animate")]
        public GameObject TargetObject;

        [Tooltip("The rotation value to animate to")]
        public T TargetValue;

        [Tooltip("Timed requires Run() to blend, Free automatically updates when TargetValue changes, ignoring ease and looping.")]
        public LerpTypes LerpType;

        [Tooltip("The ease curve to use while animating")]
        public AnimationCurve EaseCurve;

        [Tooltip("Duration of the animation in seconds")]
        public float LerpTime = 1f;

        [Tooltip("Auto start? or status")]
        public bool IsPlaying = false;

        [Tooltip("If and how the blend should loop")]
        public LoopTypes LoopType;

        [Tooltip("animation complete!")]
        public UnityEvent OnComplete;

        public override UnityEvent GetOnCompleteEvent()
        {
            return OnComplete;
        }

        // for the interface to enforce a status value
        public override bool GetIsPlaying()
        {
            return IsPlaying;
        }
        
        public bool IsReversed { get { return CompareValues(TargetValue, GetCachedStart()); } }

        // animation ticker
        protected float lerpTimeCounter = 0;

        // starting/current rotation
        protected T startValue;

        // handle on start
        protected bool inited = false;

        // Lerp time should make sense when in free mode
        // a speed range from the seed to the ratio, slowest to fastest, but reversed for LerpTime.
        protected float FreeTimeRatio = 10;
        protected float FreeTimeRatioSeed = 0.5f;

        protected T[] cachedValues = new T[2];

        protected virtual void Awake()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }

            // set a linear curve by default
            if (EaseCurve == null)
            {
                SetEaseCurve(BasicEaseCurves.Linear);
            }
            else if (EaseCurve.keys.Length < 1)
            {
                SetEaseCurve(BasicEaseCurves.Linear);
            }

            startValue = GetValue();
            CacheValues(startValue, TargetValue);

            inited = true;
        }

        /// <summary>
        /// Start the animation
        /// </summary>
        public override void Play()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }

            startValue = GetValue();

            lerpTimeCounter = 0;
            IsPlaying = true;
        }

        /// <summary>
        /// Set the trandform to the cached starting value
        /// </summary>
        public override void ResetTransform()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }

            if (!inited) return;

            SetValue(startValue);
            IsPlaying = false;
            lerpTimeCounter = 0;
        }

        public virtual void ResetTransitionValues()
        {
            startValue = GetCachedStart();
            TargetValue = GetCachedTarget();
        }

        /// <summary>
        /// reverse the transition - go back
        /// </summary>
        public override void Reverse(bool relitiveStart = false)
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }

            if (!inited) return;

            T lastTarget = TargetValue;

            TargetValue = startValue;

            if (relitiveStart)
            {
                startValue = GetValue();
            }

            startValue = lastTarget;
            lerpTimeCounter = 0;
            IsPlaying = true;
        }

        /// <summary>
        /// Stop the animation
        /// </summary>
        public override void Stop()
        {
            IsPlaying = false;
        }

        /// <summary>
        /// set the starting value, for control more over reversing
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetStartValue(T value)
        {
            startValue = value;
            CacheValues(startValue, TargetValue);
        }

        /// <summary>
        /// cache the current values, save this state.
        /// </summary>
        public void CacheCurrentValues()
        {
            CacheValues(startValue, TargetValue);
        }

        // get the current value
        public abstract T GetValue();

        // set the value
        public abstract void SetValue(T value);

        // compare values
        public abstract bool CompareValues(T value1, T value2);

        // lerp values
        public abstract T LerpValues(T startValue, T targetValue, float percent);
        
        /// <summary>
        /// Calculate the new transform based on time and ease settings
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        protected T GetNewValue(T currentValue, float percent)
        {
            T newValue = GetValue();
            float easedPercent = IsLinear() ? percent : EaseCurve.Evaluate(percent);
            
            switch (LerpType)
            {
                case LerpTypes.Timed:
                    newValue = LerpValues(startValue, TargetValue, easedPercent);
                    break;
                case LerpTypes.Free:
                    newValue = LerpValues(currentValue, TargetValue, easedPercent);
                    break;
                default:
                    break;
            }
            
            return newValue;
        }
        
        public void SetEaseCurve(BasicEaseCurves curve)
        {
            EaseCurve = GetEaseCurve(curve);
        }

        public override void Lerp(float percent)
        {
            SetValue(GetNewValue(GetValue(), percent));
        }

        /// <summary>
        /// Animate
        /// </summary>
        protected void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            // manual animation, only runs when auto started or StartRunning() is called
            if (IsPlaying && LerpType != LerpTypes.Free)
            {
                // get the time
                lerpTimeCounter += Time.deltaTime;

                if (lerpTimeCounter >= LerpTime)
                {
                    lerpTimeCounter = LerpTime;
                }

                float percent = lerpTimeCounter / LerpTime;
                
                // set the rotation
                SetValue(GetNewValue(GetValue(), percent));

                // fire the event if complete
                if (percent >= 1)
                {
                    IsPlaying = false;
                    OnComplete.Invoke();

                    switch (LoopType)
                    {
                        case LoopTypes.None:
                            break;
                        case LoopTypes.Repeat:
                            ResetTransform();
                            Play();
                            break;
                        case LoopTypes.PingPong:
                            Reverse();
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (LerpType == LerpTypes.Free) // is always running, just waiting for the TargetValue to change
            {
                bool wasRunning = IsPlaying;
                
                SetValue(GetNewValue(GetValue(), Mathf.Clamp(Mathf.Pow(FreeTimeRatioSeed, LerpTime) * FreeTimeRatio, FreeTimeRatioSeed, FreeTimeRatio) * Time.deltaTime));
                IsPlaying = CompareValues(GetValue(), TargetValue);

                // fire the event if complete
                if (IsPlaying != wasRunning && !IsPlaying)
                {
                    OnComplete.Invoke();
                }
            }
        }

        protected bool IsLinear()
        {
            if (EaseCurve.keys.Length > 1)
            {
                return (EaseCurve.keys[0].value == 1 && EaseCurve.keys[1].value == 1);
            }

            return false;
        }

        protected void CacheValues(T start, T target)
        {
            cachedValues[0] = start;
            cachedValues[1] = target;
        }

        protected T GetCachedStart()
        {
            return cachedValues[0];
        }

        protected T GetCachedTarget()
        {
            return cachedValues[1];
        }
    }
}
