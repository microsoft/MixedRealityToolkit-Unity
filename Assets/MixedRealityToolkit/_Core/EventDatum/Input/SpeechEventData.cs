// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using System;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using UnityEngine.EventSystems;

#if UNITY_WSA || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using UnityEngine.Windows.Speech;
#endif

namespace Microsoft.MixedReality.Toolkit.Core.EventDatum.Input
{
    /// <summary>
    /// Describes an input event that involves keyword recognition.
    /// </summary>
    public class SpeechEventData : BaseInputEventData
    {
        /// <summary>
        /// The time it took for the phrase to be uttered.
        /// </summary>
        public TimeSpan PhraseDuration { get; private set; }

        /// <summary>
        /// The moment in time when uttering of the phrase began.
        /// </summary>
        public DateTime PhraseStartTime { get; private set; }

        /// <summary>
        /// The text that was recognized.
        /// </summary>
        public string RecognizedText { get; private set; }

        /// <inheritdoc />
        public SpeechEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// A measure of correct recognition certainty.
        /// </summary>
        public RecognitionConfidenceLevel Confidence { get; private set; }

        
#if UNITY_WSA  || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        /// <summary>
        /// A semantic meaning of recognized phrase.
        /// </summary>
        public SemanticMeaning[] SemanticMeanings { get; private set; }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="inputAction"></param>
        /// <param name="confidence"></param>
        /// <param name="phraseDuration"></param>
        /// <param name="phraseStartTime"></param>
        /// <param name="semanticMeanings"></param>
        /// <param name="recognizedText"></param>
        public void Initialize(IMixedRealityInputSource inputSource, MixedRealityInputAction inputAction, RecognitionConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, SemanticMeaning[] semanticMeanings, string recognizedText)
        {
            BaseInitialize(inputSource, inputAction);
            Confidence = confidence;
            PhraseDuration = phraseDuration;
            PhraseStartTime = phraseStartTime;
            SemanticMeanings = semanticMeanings;
            RecognizedText = recognizedText;
        }
#endif
    }
}