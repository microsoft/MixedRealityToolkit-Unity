// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Interface to implement to react to focus enter/exit.
    /// </summary>
    public interface IFocusHandler : IFocusChangedHandler
    {
        bool HasFocus { get; }

        bool FocusEnabled { get; set; }

        List<IPointingSource> Focusers { get; }

        void OnFocusEnter(FocusEventData eventData);

        void OnFocusExit(FocusEventData eventData);
    }
}
