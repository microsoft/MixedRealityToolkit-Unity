// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface to implement dictation events.
    /// </summary>
    public interface IMixedRealityDictationHandler : IEventSystemHandler
    {
        void OnDictationHypothesis(DictationEventData eventData);

        void OnDictationResult(DictationEventData eventData);

        void OnDictationComplete(DictationEventData eventData);

        void OnDictationError(DictationEventData eventData);
    }
}
