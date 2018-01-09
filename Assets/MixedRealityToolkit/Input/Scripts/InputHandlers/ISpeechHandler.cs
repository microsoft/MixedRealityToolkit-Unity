// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Input.EventData;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.Input.InputHandlers
{
    /// <summary>
    /// Interface to implement to react to speech recognition.
    /// </summary>
    public interface ISpeechHandler : IEventSystemHandler
    {
        void OnSpeechKeywordRecognized(SpeechEventData eventData);
    }
}