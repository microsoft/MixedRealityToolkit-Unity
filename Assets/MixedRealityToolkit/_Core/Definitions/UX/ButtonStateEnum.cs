// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// A button typically has 8 potential states.
    /// We can update visual feedback based on state change, all the logic is already done, making InteractiveEffects behaviors less complex then comparing selected + Disabled.
    /// </summary>
    public enum ButtonStateEnum
    {
        Default,
        Focus,
        Press,
        Selected,
        FocusSelected,
        PressSelected,
        Disabled,
        DisabledSelected
    }
}
