// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The event configuration for the SpeechKeyword InteractionState.
    /// </summary>
    public class SpeechKeywordEvents : BaseInteractionEventConfiguration
    {
        [SerializeField]
        [Tooltip("Whether or not to register the IMixedRealitySpeechHandler for global input. If Global is true, then" +
        " events in the SpeechKeyword state will be fired without requiring an object to be in focus. ")]
        private bool global = false;

        /// <summary>
        /// Whether or not to register the IMixedRealitySpeechHandler for global input. If Global is true, then
        /// events in the SpeechKeyword state will be fired without requiring an object to be in focus. 
        /// </summary>
        public bool Global
        {
            get => global;
            set
            {
                global = value;
                OnGlobalChanged.Invoke();
            }
        }

        /// <summary>
        /// A Unity event used to track whether or not the Global property has changed.
        /// </summary>
        [HideInInspector]
        public UnityEvent OnGlobalChanged = new UnityEvent();

        [SerializeField]
        [Tooltip("A Unity event with SpeechEventData. This event will be fired when any of the keywords registered" +
            "in the Speech input Configuration Profile are recognized.")]
        private SpeechInteractionEvent onAnySpeechKeywordRecognized = new SpeechInteractionEvent();

        /// <summary>
        /// A Unity event with SpeechEventData. This event will be fired when any of the keywords registered
        /// in the Speech input Configuration Profile are recognized.
        /// </summary>
        public SpeechInteractionEvent OnAnySpeechKeywordRecognized
        {
            get => onAnySpeechKeywordRecognized;
            private set => onAnySpeechKeywordRecognized = value;
        }

        [SerializeField]
        [Tooltip("List of keywords and Unity Events for the Speech input handler to listen for.")]
        private List<KeywordEvent> keywords = new List<KeywordEvent>();

        /// <summary>
        /// List of keywords and Unity Events for the Speech input handler to listen for.
        /// </summary>
        public List<KeywordEvent> Keywords
        {
            get => keywords;
            set => keywords = value;
        }
    }
}
