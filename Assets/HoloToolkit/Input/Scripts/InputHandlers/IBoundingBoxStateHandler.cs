// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Interface to implement reacting to bounding box rig's activation or deactivation.
    /// </summary>
    public interface IBoundingBoxStateHandler : IEventSystemHandler
    {
        void OnBoundingBoxRigActivated(BoundingBoxEventData eventData);

        void OnBoundingBoxRigDeactivated(BoundingBoxEventData eventData);
    }
}