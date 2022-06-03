// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Environment
{
    [Preserve]
    [MRTKSubsystem(
        Name = "com.microsoft.mixedreality.boundary",
        DisplayName = "Subsystem for XRSDK Boundary API",
        Author = "Microsoft",
        ProviderType = typeof(XRSDKProvider),
        SubsystemTypeOverride = typeof(XRSDKBoundarySystem),
        ConfigType = typeof(BaseSubsystemConfig))]
    public class XRSDKBoundarySystem : BoundarySubsystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            // Fetch subsystem metadata from the attribute.
            var cinfo = XRSubsystemHelpers.ConstructCinfo<XRSDKBoundarySystem, BoundarySubsystemCinfo>();

            if (!BoundarySubsystem.Register(cinfo))
            {
                Debug.LogError($"Failed to register the {cinfo.Name} subsystem.");
            }
        }

        class XRSDKProvider : Provider
        {
            /// <inheritdoc/>
            public override ExperienceScale Scale { get; set; }
            private static readonly List<XRInputSubsystem> XRInputSubsystems = new List<XRInputSubsystem>();

            #region IBoundarySystem implementation

            /// <inheritdoc/>
            public override List<Vector3> GetBoundaryGeometry()
            {

                Debug.Log("XRSDK Get Boundary");
                // Get the boundary geometry.
                var boundaryGeometry = new List<Vector3>(0);

                if (XRSubsystemHelpers.InputSubsystem?.GetTrackingOriginMode() != TrackingOriginModeFlags.Floor
                    || !XRSubsystemHelpers.InputSubsystem.TryGetBoundaryPoints(boundaryGeometry)
                    || boundaryGeometry.Count == 0)
                {
                    // If the "main" input subsystem doesn't have an available boundary, check the rest of them
                    SubsystemManager.GetInstances(XRInputSubsystems);
                    foreach (XRInputSubsystem xrInputSubsystem in XRInputSubsystems)
                    {
                        Debug.Log("Evaluating input system " + xrInputSubsystem.GetType().FullName);
                        Debug.Log("Running? " + xrInputSubsystem.running);
                        Debug.Log("TrackingOriginMode: " + xrInputSubsystem.GetTrackingOriginMode());
                        if (xrInputSubsystem.running
                            && xrInputSubsystem.GetTrackingOriginMode() == TrackingOriginModeFlags.Floor
                            && xrInputSubsystem.TryGetBoundaryPoints(boundaryGeometry)
                            && boundaryGeometry.Count > 0)
                        {
                            Debug.Log("Got boundary");
                            break;
                        }
                        Debug.Log("Didn't get boundary, continuing");
                    }
                }
                else
                {
                    Debug.Log("No boundary to report");
                }

                return boundaryGeometry;
            }

            /// <inheritdoc/>
            public override void SetTrackingSpace()
            {
                TrackingOriginModeFlags trackingOriginMode;

                // In current versions of Unity, there are two types of tracking spaces. For boundaries, if the scale
                // is not Room or Standing, it currently maps to TrackingSpaceType.Stationary.
                switch (Scale)
                {
                    case ExperienceScale.Standing:
                    case ExperienceScale.Room:
                        trackingOriginMode = TrackingOriginModeFlags.Floor;
                        break;

                    case ExperienceScale.OrientationOnly:
                    case ExperienceScale.Seated:
                    case ExperienceScale.World:
                        trackingOriginMode = TrackingOriginModeFlags.Device;
                        break;

                    default:
                        trackingOriginMode = TrackingOriginModeFlags.Device;
                        Debug.LogWarning("Unknown / unsupported ExperienceScale. Defaulting to Device tracking space.");
                        break;
                }

                if (XRSubsystemHelpers.InputSubsystem == null || !XRSubsystemHelpers.InputSubsystem.TrySetTrackingOriginMode(trackingOriginMode))
                {
                    // If the "main" input subsystem can't set the origin mode, check the rest of them
                    SubsystemManager.GetInstances(XRInputSubsystems);
                    foreach (XRInputSubsystem xrInputSubsystem in XRInputSubsystems)
                    {
                        if (xrInputSubsystem.running && xrInputSubsystem.TrySetTrackingOriginMode(trackingOriginMode))
                        {
                            return;
                        }
                    }
                    Debug.LogWarning("Tracking origin unable to be set.");
                }
            }

            #endregion IBoundarySystem implementation
        }
    }
}
