// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    public class DictationEventData : InputEventData
    {
        /// <summary>
        /// String result of the current dictation.
        /// </summary>
        public string DictationResult { get; private set; }
        /// <summary>
        /// Audio Clip of the last Dictation recording Session.
        /// </summary>
        public AudioClip DictationAudioClip { get; private set; }

        public DictationEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, uint sourceId, object tag, string dictationResult, AudioClip dictationAudioClip = null)
        {
            BaseInitialize(inputSource, sourceId, tag);
            DictationResult = dictationResult;
            DictationAudioClip = dictationAudioClip;
        }
    }
}
