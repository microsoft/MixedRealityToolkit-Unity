// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.InputModule.InputHandlers
{
    /// <summary>
    /// Interface to implement dictation events.
    /// </summary>
    public interface IDictationHandler : IEventSystemHandler
    {
        void OnDictationHypothesis(DictationEventData eventData);

        void OnDictationResult(DictationEventData eventData);

        void OnDictationComplete(DictationEventData eventData);

        void OnDictationError(DictationEventData eventData);
    }
}
