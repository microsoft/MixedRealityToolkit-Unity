// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The internal event receiver for the events defined in the SpeechKeyword Interaction Event Configuration.
    /// </summary>
    public class SpeechKeywordReceiver : BaseEventReceiver
    {
        public SpeechKeywordReceiver(BaseInteractionEventConfiguration eventConfiguration) : base(eventConfiguration) { }

        private SpeechKeywordEvents SpeechKeywordEventConfig => EventConfiguration as SpeechKeywordEvents;

        private SpeechInteractionEvent onSpeechKeywordRecognized => SpeechKeywordEventConfig.OnAnySpeechKeywordRecognized;

        private List<KeywordEvent> keywordsAndResponses => SpeechKeywordEventConfig.Keywords;

        /// <inheritdoc />
        public override void OnUpdate(StateManager stateManager, BaseEventData eventData)
        {
            bool keywordRecognized = stateManager.GetState(StateName).Value > 0;

            if (keywordRecognized)
            {
                SpeechEventData speechData = eventData as SpeechEventData;

                onSpeechKeywordRecognized.Invoke(speechData);

                bool speechKeywordRecognized = speechData.Command.Keyword != null;

                if (speechKeywordRecognized)
                {
                    // Get the keyword that was recognized
                    string speechEventKeyword = speechData.Command.Keyword;

                    // Find the corresponding event for the speech keyword that was recognized
                    KeywordEvent keywordResponseEvent = keywordsAndResponses.Find((keyEvent) => String.Equals(keyEvent.Keyword, speechEventKeyword, StringComparison.OrdinalIgnoreCase));

                    if (keywordResponseEvent != null)
                    {
                        // Fire the OnKeywordRecognized event that is associated with the recognized keyword
                        keywordResponseEvent.OnKeywordRecognized.Invoke();
                    }
                }

                // Set the state to off after the events have been fired
                stateManager.SetStateOff(StateName);
            }
        }
    }
}
