// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface to implement to react to speech recognition.
    /// </summary>
    public interface IMixedRealitySpeechHandler : IMixedRealityBaseInputHandler
    {
        void OnSpeechKeywordRecognized(SpeechEventData eventData);
    }
}