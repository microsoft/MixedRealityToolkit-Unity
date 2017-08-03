// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine.EventSystems;

#if UNITY_WSA || UNITY_STANDALONE_WIN
using UnityEngine.Windows.Speech;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that involves keyword recognition.
    /// </summary>
    public class SpeechKeywordRecognizedEventData : InputEventData
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

        public SpeechKeywordRecognizedEventData(EventSystem eventSystem) : base(eventSystem) { }

#if UNITY_WSA || UNITY_STANDALONE_WIN
        /// <summary>
        /// A measure of correct recognition certainty.
        /// </summary>
        public ConfidenceLevel Confidence { get; private set; }

        /// <summary>
        /// A semantic meaning of recognized phrase.
        /// </summary>
        public SemanticMeaning[] SemanticMeanings { get; private set; }

        public void Initialize(IInputSource inputSource, uint sourceId, ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, SemanticMeaning[] semanticMeanings, string recognizedText)
        {
            BaseInitialize(inputSource, sourceId);
            Confidence = confidence;
            PhraseDuration = phraseDuration;
            PhraseStartTime = phraseStartTime;
            SemanticMeanings = semanticMeanings;
            RecognizedText = recognizedText;
        }
#endif
    }
}