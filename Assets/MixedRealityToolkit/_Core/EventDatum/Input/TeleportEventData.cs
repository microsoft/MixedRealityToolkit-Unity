// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input
{
    /// <summary>
    /// Describes an input event that involves Teleportation from one location to another.
    /// </summary>
    public class TeleportEventData : InputEventData
    {
        /// <inheritdoc />
        public TeleportEventData(UnityEngine.EventSystems.EventSystem eventSystem) : base(eventSystem) { }
    }
}