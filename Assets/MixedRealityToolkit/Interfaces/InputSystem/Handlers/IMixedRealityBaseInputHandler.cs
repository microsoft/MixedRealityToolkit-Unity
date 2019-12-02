// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Base interface for all input handlers. This allows us to use ExecuteEvents.ExecuteHierarchy&lt;IMixedRealityBaseInputHandler&gt;
    /// to send an event to all input handling interfaces.
    /// </summary>
    public interface IMixedRealityBaseInputHandler : IEventSystemHandler {}
}