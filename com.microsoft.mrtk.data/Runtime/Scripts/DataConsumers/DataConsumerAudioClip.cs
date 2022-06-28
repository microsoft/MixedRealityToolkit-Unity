// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Given a value from a data source, use that value to look up the correct AudioClip
    /// specified in the Unity inspector list. That AudioClip is then associated
    /// with any AudioClipRenderer being managed by this object.
    /// </summary>
    ///
    /// <remarks>
    /// TODO: Allow for a default AudioClip if no look up can be found.
    /// </remarks>
    [Serializable]
    [AddComponentMenu("MRTK/Data Binding/Consumers/Data Consumer Audio Clip", -10)]
    public class DataConsumerAudioClip : DataConsumerThemableBase<AudioClip>
    {
        /// </inheritdoc/>
        protected override Type[] GetComponentTypes()
        {
            Type[] types = { typeof(AudioSource) };
            return types;
        }

        /// </inheritdoc/>
        protected override void SetObject(Component component, object inValue, AudioClip audioClip)
        {
            AudioSource audioSource = component as AudioSource;

            audioSource.clip = audioClip;
        }
    }
}
