// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace MixedRealityToolkit.InputModule.InputHandlers
{
    /// <summary>
    /// Interface to implement to react to focus enter/exit.
    /// </summary>
    public interface IFocusable : IEventSystemHandler
    {
        void OnFocusEnter();
        void OnFocusExit();
    }
}
