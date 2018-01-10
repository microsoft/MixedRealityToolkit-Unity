// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.InputModule.InputHandlers
{
    /// <summary>
    /// Interface to implement to react to source rotation changes.
    /// </summary>
    public interface ISourceRotationHandler : IEventSystemHandler
    {
        void OnRotationChanged(SourceRotationEventData eventData);
    }
}