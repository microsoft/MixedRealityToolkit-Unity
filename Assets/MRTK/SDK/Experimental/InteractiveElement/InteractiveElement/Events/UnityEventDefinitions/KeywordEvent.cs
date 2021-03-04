// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// A container for a keyword and its associated Unity event. This container is utilized
    /// in SpeechKeywordEvents. 
    /// </summary>
    [Serializable]
    public class KeywordEvent
    {
        [SerializeField]
        [Tooltip("The Keyword for the Speech Handler to listen for if speech is enabled.  If this keyword is recognized, the OnKeywordRecognized" +
            "event will fire.  This keyword must also be registered in the Speech Input configuration profile. ")]
        private string keyword;

        /// <summary>
        /// The Keyword for the Speech Handler to listen for if speech is enabled.  If this keyword is recognized, the OnKeywordRecognized
        /// event will fire.  This keyword must also be registered in the Speech Input configuration profile. 
        /// </summary>
        public string Keyword
        {
            get => keyword;
            set => keyword = value;
        }

        /// <summary>
        /// Unity Event fired when a specific keyword is recognized. 
        /// </summary>
        public UnityEvent OnKeywordRecognized = new UnityEvent();

    }
}
