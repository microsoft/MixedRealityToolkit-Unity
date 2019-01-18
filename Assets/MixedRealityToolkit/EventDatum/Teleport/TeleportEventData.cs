// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.TeleportSystem;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.EventDatum.Teleport
{
    /// <summary>
    /// Describes a Teleportation Event.
    /// </summary>
    public class TeleportEventData : GenericBaseEventData
    {
        /// <summary>
        /// The pointer that raised the event.
        /// </summary>
        public IMixedRealityPointer Pointer { get; private set; }

        /// <summary>
        /// The teleport hot spot.
        /// </summary>
        public IMixedRealityTeleportHotSpot HotSpot { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem">Typically will be <see cref="EventSystem.current"/></param>
        public TeleportEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="target"></param>
        public void Initialize(IMixedRealityPointer pointer, IMixedRealityTeleportHotSpot target)
        {
            BaseInitialize(pointer.InputSourceParent);
            Pointer = pointer;
            HotSpot = target;
        }
    }
}
