// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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