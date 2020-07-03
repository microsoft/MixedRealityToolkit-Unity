// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Interface to implement generic events.
    /// </summary>
    public interface IMixedRealityEventHandler : IEventSystemHandler
    {
        void OnEventRaised(GenericBaseEventData eventData);
    }
}