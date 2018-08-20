// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Blend
{
    [System.Serializable]
    public struct BlendInstanceProperties
    {
        public AnimationCurve EaseCurve;
        public float LerpTime;
        public LoopTypes LoopType;
    }

    public class BlendInstance
    {
        public bool IsReversed { get { return direction > 0; } }
        public bool IsCompleted { get { return completed; } }

        // animation ticker
        protected float lerpTimeCounter = 0;
        protected float direction = 1;
        protected float lerpTime;
        protected AnimationCurve easeCurve;

        protected bool isLinear;
        protected bool isPlaying;

        protected bool completed = false;

        // handle on start
        protected bool inited = false;

        public float GetLerpValue()
        {
            return GetEasedTime(lerpTimeCounter / lerpTime);
        }

        public bool IsPlaying { get { return isPlaying; } }

        public BlendInstanceProperties Init(BlendInstanceProperties properties)
        {
            if (properties.LerpTime == 0 || properties.EaseCurve == null || properties.EaseCurve.keys.Length < 1)
            {
                properties = SetupProperties(properties);
            }
            else
            {
                inited = true;
            }
            
            lerpTimeCounter = 0;

            return properties;
            
        }

        public BlendInstanceProperties SetupProperties(BlendInstanceProperties properties)
        {
            easeCurve = ValidateCurve(properties.EaseCurve);
            isLinear = IsLinear(easeCurve);

            if (properties.LerpTime == 0 )
            {
                properties.LerpTime = 1;
                lerpTime = 1;
            }

            inited = true;

            return properties;
        }

        protected AnimationCurve ValidateCurve(AnimationCurve curve)
        {
            // set a linear curve by default
            if (curve == null)
            {
                curve = AbstractBlend.GetEaseCurve(BasicEaseCurves.Linear);
            }
            else if (curve.keys.Length < 1)
            {
                curve = AbstractBlend.GetEaseCurve(BasicEaseCurves.Linear);
            }

            return curve;
        }
        
        /// <summary>
        /// Start the animation
        /// </summary>
        public void Play()
        {
            if (!inited)
            {
                return;
            }

            lerpTimeCounter = 0;
            isPlaying = true;
            direction = 1;
        }

        /// <summary>
        /// Stops the Blend and resets the counter
        /// </summary>
        public void ResetBlend()
        {
            if (!inited)
            {
                return;
            }

            isPlaying = false;
            lerpTimeCounter = 0;
            direction = 1;
        }

        /// <summary>
        /// reverse the transition - go back
        /// </summary>
        public void Reverse(bool relitiveStart = false)
        {
            if (!inited)
            {
                return;
            }

            direction = -direction;

            if (!relitiveStart)
            {
                if (direction > 0)
                {
                    lerpTimeCounter = 0;
                }
                else
                {
                    lerpTimeCounter = lerpTime;
                }
            }

            isPlaying = true;
        }

        /// <summary>
        /// Stop the animation
        /// </summary>
        public void Stop()
        {
            ResetBlend();
        }

        /// <summary>
        /// Calculate the new transform based on time and ease settings
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        protected float GetEasedTime(float percent)
        {
            if(easeCurve == null)
            {
                return 0;
            }

            float easedPercent = isLinear ? percent : easeCurve.Evaluate(percent);
            
            return easedPercent;
        }

        public float GetCurrentPercent()
        {
            return lerpTimeCounter / lerpTime;
        }

        public void Lerp(float percent, BlendInstanceProperties properties)
        {
            lerpTime = properties.LerpTime;
            easeCurve = ValidateCurve(properties.EaseCurve);
            isLinear = IsLinear(easeCurve);
            lerpTimeCounter = lerpTime * percent;
        }

        /// <summary>
        /// Animate
        /// </summary>
        public void Update(float delta, BlendInstanceProperties properties)
        {
            lerpTime = properties.LerpTime;
            easeCurve = ValidateCurve(properties.EaseCurve);
            isLinear = IsLinear(easeCurve);
            completed = false;

            // get the time
            lerpTimeCounter = lerpTimeCounter + delta * direction;

            float percent = Mathf.Clamp01(lerpTimeCounter / lerpTime);

            // fire the event if complete
            if ((percent >= 1 && direction > 0) || (percent <= 0 && direction < 0))
            {
                isPlaying = false;
                switch (properties.LoopType)
                {
                    case LoopTypes.None:
                        break;
                    case LoopTypes.Repeat:
                        Play();
                        break;
                    case LoopTypes.PingPong:
                        Reverse();
                        break;
                    default:
                        break;
                }
                completed = true;
            }
        }
        
        protected bool IsLinear(AnimationCurve curve)
        {
            if (curve.keys.Length > 1)
            {
                return (curve.keys[0].value == 1 && curve.keys[1].value == 1);
            }
            return false;
        }
    }
}
