// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Blend
{
    /// <summary>
    /// animates the transform of an object with eases
    /// </summary>
    public class BlendTransform : AbstractBlend
    {
        [HideInInspector]
        public List<BlendTransformData> BlendData = new List<BlendTransformData>();
        public bool IsPlaying = false;

        [Tooltip("Fires when all the animations are complete")]
        public UnityEvent OnComplete;

        protected bool completed = true;

        protected BlendTransformCollection blendCollection;

        /// <summary>
        /// Get the OnComplete event
        /// </summary>
        /// <returns></returns>
        public override UnityEvent GetOnCompleteEvent()
        {
            return OnComplete;
        }

        // for the interface to enforce a status value
        public override bool GetIsPlaying()
        {
            return IsPlaying;
        }

        private void Awake()
        {
            blendCollection = new BlendTransformCollection(this);
            
            blendCollection.UpdateData(BlendData);

            if (IsPlaying)
            {
                Play();
            }
        }

        /// <summary>
        /// clean up
        /// </summary>
        private void OnDestroy()
        {
            if(blendCollection != null)
            {
                blendCollection.Destroy();
            }
        }

        /// <summary>
        /// Start the animation
        /// </summary>
        public override void Play()
        {
            blendCollection.Play();
        }

        /// <summary>
        /// Set the trandform to the cached starting value
        /// </summary>
        public override void ResetTransform()
        {
            blendCollection.ResetTransform();
        }

        public virtual void ResetTransitionValues()
        {
            blendCollection.ResetTransitionValues();
        }

        /// <summary>
        /// reverse the transition - go back
        /// </summary>
        public override void Reverse(bool relativeStart = false)
        {
            blendCollection.Reverse(relativeStart);
        }

        /// <summary>
        /// Stop the animation
        /// </summary>
        public override void Stop()
        {
            blendCollection.Stop();
        }

        /// <summary>
        /// manually set the lerpped value
        /// </summary>
        /// <param name="percent"></param>
        public override void Lerp(float percent)
        {
            blendCollection.Lerp(percent);
        }

    }
}
