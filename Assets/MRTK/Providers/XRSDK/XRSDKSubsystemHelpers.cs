// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.XRSDK
{
    /// <summary>
    /// A helper class to provide easier access to active Unity XR SDK subsystems.
    /// </summary>
    [Obsolete("Use Microsoft.MixedReality.Toolkit.Utilities.XRSubsystemHelpers instead.")]
    public static class XRSDKSubsystemHelpers
    {
        /// <summary>
        /// The XR SDK input subsystem for the currently loaded XR plug-in.
        /// </summary>
        public static XRInputSubsystem InputSubsystem => XRSubsystemHelpers.InputSubsystem;

        /// <summary>
        /// The XR SDK mesh subsystem for the currently loaded XR plug-in.
        /// </summary>
        public static XRMeshSubsystem MeshSubsystem => XRSubsystemHelpers.MeshSubsystem;

        /// <summary>
        /// The XR SDK display subsystem for the currently loaded XR plug-in.
        /// </summary>
        public static XRDisplaySubsystem DisplaySubsystem => XRSubsystemHelpers.DisplaySubsystem;
    }
}
