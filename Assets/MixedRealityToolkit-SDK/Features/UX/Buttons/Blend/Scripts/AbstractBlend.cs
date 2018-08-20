// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Blend
{
    public abstract class AbstractBlend : MonoBehaviour
    {
        public abstract UnityEvent GetOnCompleteEvent();

        public abstract bool GetIsPlaying();
        
        public abstract void Play();

        public abstract void ResetTransform();

        public abstract void Reverse(bool relativeStart = false);

        public abstract void Stop();

        public abstract void Lerp(float percent);

        public static AnimationCurve GetEaseCurve(BasicEaseCurves curve)
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

            return animation;
        }

        public static BlendStatus[] BlendDataList(AbstractBlend[] blends)
        {
            List<BlendStatus> list = new List<BlendStatus>();

            for (int i = 0; i < blends.Length; i++)
            {
                BlendStatus data = new BlendStatus();
                data.Blender = blends[i];
                list.Add(data);
                data.Disabled = false;
            }

            return list.ToArray();
        }
    }
}
