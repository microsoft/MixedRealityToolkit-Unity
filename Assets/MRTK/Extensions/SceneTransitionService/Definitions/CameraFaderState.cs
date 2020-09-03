// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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