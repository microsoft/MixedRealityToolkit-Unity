// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that involves keyword recognition.
    /// </summary>
    [Obsolete("Please use SpeechEventData")]
    public class SpeechKeywordRecognizedEventData : SpeechEventData
    {
        public SpeechKeywordRecognizedEventData(EventSystem eventSystem) : base(eventSystem) { }
    }
}
