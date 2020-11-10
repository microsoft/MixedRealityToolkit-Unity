// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if UNITY_2019_3_OR_NEWER
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
#else
#if UNITY_2019_2_OR_NEWER
using UnityEngine.XR;
#endif
using UnityEngine.Experimental.XR;
#endif

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// A helper class to provide easier access to active Unity XR SDK subsystems.
    /// </summary>
    /// <remarks>These properties are only valid for the XR SDK pipeline.</remarks>
    public static class XRSubsystemHelpers
    {
        private static XRInputSubsystem inputSubsystem = null;
#if UNITY_2019_3_OR_NEWER
        private static readonly List<XRInputSubsystem> XRInputSubsystems = new List<XRInputSubsystem>();
#endif // UNITY_2019_3_OR_NEWER

        /// <summary>
        /// The XR SDK input subsystem for the currently loaded XR plug-in.
        /// </summary>
        public static XRInputSubsystem InputSubsystem
        {
            get
            {
#if UNITY_2019_3_OR_NEWER
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
#endif // UNITY_2019_3_OR_NEWER
                return inputSubsystem;
            }
        }

        private static XRMeshSubsystem meshSubsystem = null;
#if UNITY_2019_3_OR_NEWER
        private static readonly List<XRMeshSubsystem> XRMeshSubsystems = new List<XRMeshSubsystem>();
#endif // UNITY_2019_3_OR_NEWER

        /// <summary>
        /// The XR SDK mesh subsystem for the currently loaded XR plug-in.
        /// </summary>
        public static XRMeshSubsystem MeshSubsystem
        {
            get
            {
#if UNITY_2019_3_OR_NEWER
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
#endif // UNITY_2019_3_OR_NEWER
                return meshSubsystem;
            }
        }

        private static XRDisplaySubsystem displaySubsystem = null;
#if UNITY_2019_3_OR_NEWER
        private static readonly List<XRDisplaySubsystem> XRDisplaySubsystems = new List<XRDisplaySubsystem>();
#endif // UNITY_2019_3_OR_NEWER

        /// <summary>
        /// The XR SDK display subsystem for the currently loaded XR plug-in.
        /// </summary>
        public static XRDisplaySubsystem DisplaySubsystem
        {
            get
            {
#if UNITY_2019_3_OR_NEWER
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
#endif // UNITY_2019_3_OR_NEWER
                return displaySubsystem;
            }
        }
    }
}
