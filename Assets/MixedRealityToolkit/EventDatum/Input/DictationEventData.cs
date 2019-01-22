// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.EventDatum.Input
{
    /// <summary>
    /// Describes an Input Event with voice dictation.
    /// </summary>
    public class DictationEventData : BaseInputEventData
    {
        /// <summary>
        /// String result of the current dictation.
        /// </summary>
        public string DictationResult { get; private set; }

        /// <summary>
        /// Audio Clip of the last Dictation recording Session.
        /// </summary>
        public AudioClip DictationAudioClip { get; private set; }

        /// <inheritdoc />
        public DictationEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="dictationResult"></param>
        /// <param name="dictationAudioClip"></param>
        public void Initialize(IMixedRealityInputSource inputSource, string dictationResult, AudioClip dictationAudioClip = null)
        {
            BaseInitialize(inputSource, MixedRealityInputAction.None);
            DictationResult = dictationResult;
            DictationAudioClip = dictationAudioClip;
        }
    }
}
