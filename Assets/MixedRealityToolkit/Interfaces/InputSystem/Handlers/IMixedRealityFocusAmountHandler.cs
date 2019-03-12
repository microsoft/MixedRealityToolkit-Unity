// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

public interface IMixedRealityFocusAmountHandler : IEventSystemHandler
{
    /// <summary>
    /// Determines if all OnFocusEnter and OnFocusExit events will be received
    /// </summary>
    bool ReceiveAllFocusEvents { get; }
}
