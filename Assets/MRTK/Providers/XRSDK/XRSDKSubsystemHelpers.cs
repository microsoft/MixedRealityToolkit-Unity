// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.XRSDK
{
    /// <summary>
    /// A helper class to provide easier access to active Unity XR SDK subsystems.
    /// </summary>
    public static class XRSDKSubsystemHelpers
    {
        private static XRInputSubsystem inputSubsystem = null;
        private static readonly List<XRInputSubsystem> XRInputSubsystems = new List<XRInputSubsystem>();

        /// <summary>
        /// The XR SDK input subsystem for the currently loaded XR plug-in.
        /// </summary>
        public static XRInputSubsystem InputSubsystem
        {
            get
            {
                if (inputSubsystem == null || !inputSubsystem.running)
                {
                    inputSubsystem = null;
                    SubsystemManager.GetInstances(XRInputSubsystems);
                    foreach (XRInputSubsystem xrInputSubsystem in XRInputSubsystems)
                    {
                        if (xrInputSubsystem.running)
                        {
                            inputSubsystem = xrInputSubsystem;
                            break;
                        }
                    }
                }

                return inputSubsystem;
            }
        }

        private static XRMeshSubsystem meshSubsystem = null;
        private static readonly List<XRMeshSubsystem> XRMeshSubsystems = new List<XRMeshSubsystem>();

        /// <summary>
        /// The XR SDK mesh subsystem for the currently loaded XR plug-in.
        /// </summary>
        public static XRMeshSubsystem MeshSubsystem
        {
            get
            {
                if (meshSubsystem == null || !meshSubsystem.running)
                {
                    meshSubsystem = null;
                    SubsystemManager.GetInstances(XRMeshSubsystems);
                    foreach (XRMeshSubsystem xrMeshSubsystem in XRMeshSubsystems)
                    {
                        if (xrMeshSubsystem.running)
                        {
                            meshSubsystem = xrMeshSubsystem;
                            break;
                        }
                    }
                }

                return meshSubsystem;
            }
        }

        private static XRDisplaySubsystem displaySubsystem = null;
        private static readonly List<XRDisplaySubsystem> XRDisplaySubsystems = new List<XRDisplaySubsystem>();

        /// <summary>
        /// The XR SDK display subsystem for the currently loaded XR plug-in.
        /// </summary>
        public static XRDisplaySubsystem DisplaySubsystem
        {
            get
            {
                if (displaySubsystem == null || !displaySubsystem.running)
                {
                    displaySubsystem = null;
                    SubsystemManager.GetInstances(XRDisplaySubsystems);
                    foreach (XRDisplaySubsystem xrDisplaySubsystem in XRDisplaySubsystems)
                    {
                        if (xrDisplaySubsystem.running)
                        {
                            displaySubsystem = xrDisplaySubsystem;
                            break;
                        }
                    }
                }

                return displaySubsystem;
            }
        }
    }
}
