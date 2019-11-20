// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// Class specifying targeting event arguments.
    /// </summary>
    public class TargetEventArgs : System.EventArgs
    {
        public EyeTrackingTarget HitTarget { get; private set; }

        public TargetEventArgs(EyeTrackingTarget hitTarget)
        {
            HitTarget = hitTarget;
        }
    }
}
