// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Teleport
{
    /// <summary>
    /// Describes a Teleportation Event.
    /// </summary>
    public class TeleportEventData : BaseEventData
    {


        public TeleportEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IMixedRealityPointer pointer)
        {
        }
    }
}
