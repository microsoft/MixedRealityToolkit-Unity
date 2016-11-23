// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Basic event args for an input source event.
    /// </summary>
    public class InputSourceEventArgs : EventArgs
    {
        /// <summary>
        /// Input source that triggered the event.
        /// </summary>
        public IInputSource InputSource { get; private set; }

        /// <summary>
        /// Source ID that triggered the event.
        /// This is used to differentiate between indivial inputs, when an input source support multiple inputs.
        /// </summary>
        public uint SourceId { get; private set; }

        public InputSourceEventArgs(IInputSource inputSource, uint sourceId)
        {
            InputSource = inputSource;
            SourceId = sourceId;
        }
    }

    /// <summary>
    /// Event args for a click event.
    /// </summary>
    public class SourceClickEventArgs : InputSourceEventArgs
    {
        /// <summary>
        /// Number of taps that triggered the event.
        /// </summary>
        public int TapCount { get; private set; }

        public SourceClickEventArgs(IInputSource inputSource, uint sourceId, int tapCount)
            : base(inputSource, sourceId)
        {
            TapCount = tapCount;
        }
    }

    /// <summary>
    /// Event args for a hold event.
    /// </summary>
    public class HoldEventArgs : InputSourceEventArgs
    {
        public HoldEventArgs(IInputSource inputSource, uint sourceId)
            : base(inputSource, sourceId)
        {
        }
    }

    /// <summary>
    /// Event args for a manipulation event.
    /// </summary>
    public class ManipulationEventArgs : InputSourceEventArgs
    {
        /// <summary>
        /// Total distance moved since the beginning of the manipulation gesture.
        /// </summary>
        public Vector3 CumulativeDelta { get; private set; }

        public ManipulationEventArgs(IInputSource inputSource, uint sourceId, Vector3 cumulativeDelta)
            : base(inputSource, sourceId)
        {
            CumulativeDelta = cumulativeDelta;
        }
    }

    /// <summary>
    /// Event args for a navigation event.
    /// </summary>
    public class NavigationEventArgs : InputSourceEventArgs
    {
        /// <summary>
        /// The normalized offset, since the navigation gesture began, of the input within 
        /// the unit cube for the navigation gesture.
        /// </summary>
        public Vector3 NormalizedOffset { get; private set; }

        public NavigationEventArgs(IInputSource inputSource, uint sourceId, Vector3 normalizedOffset)
            : base(inputSource, sourceId)
        {
            NormalizedOffset = normalizedOffset;
        }
    }

    /// <summary>
    /// Event args for a phrase recognized event.
    /// </summary>
    public class PhraseRecognizedEventArgs : InputSourceEventArgs
    {
        /// <summary>
        /// A measure of correct recognition certainty.
        /// </summary>
        public UnityEngine.Windows.Speech.ConfidenceLevel Confidence { get; private set; }

        /// <summary>
        /// The time it took for the phrase to be uttered.
        /// </summary>
        public TimeSpan PhraseDuration { get; private set; }

        /// <summary>
        /// The moment in time when uttering of the phrase began.
        /// </summary>
        public DateTime PhraseStartTime { get; private set; }

        /// <summary>
        /// A semantic meaning of recognized phrase.
        /// </summary>
        public UnityEngine.Windows.Speech.SemanticMeaning[] SemanticMeanings { get; private set; }

        /// <summary>
        /// The text that was recognized.
        /// </summary>
        public string Text { get; private set; }

        public PhraseRecognizedEventArgs(IInputSource inputSource, uint sourceId,
            UnityEngine.Windows.Speech.ConfidenceLevel confidence, 
            TimeSpan phraseDuration, DateTime phraseStartTime,
            UnityEngine.Windows.Speech.SemanticMeaning[] semanticMeanings,
            string text)
            : base(inputSource, sourceId)
        {
            Confidence = confidence;
            PhraseDuration = phraseDuration;
            PhraseStartTime = phraseStartTime;
            SemanticMeanings = semanticMeanings;
            Text = text;
        }
    }
}
