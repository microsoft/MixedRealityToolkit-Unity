// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Extensions.SceneTransitions
{
    public enum CameraFaderState
    {
        Clear,      // No fade active
        FadingIn,
        Opaque,     // Fade has completed, color covers entire screen
        FadingOut,
    }
}