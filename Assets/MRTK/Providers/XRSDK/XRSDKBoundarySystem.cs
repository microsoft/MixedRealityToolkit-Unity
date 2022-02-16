// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Boundary;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.XRSDK
{
    /// <summary>
    /// The Boundary system controls the presentation and display of the users boundary in a scene.
    /// </summary>
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/boundary/boundary-system-getting-started")]
    public class XRSDKBoundarySystem : BaseBoundarySystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile">The configuration profile for the service.</param>
        /// <param name="scale">The application's configured <see cref="Utilities.ExperienceScale"/>.</param>
        public XRSDKBoundarySystem(
            MixedRealityBoundaryVisualizationProfile profile,
            ExperienceScale scale) : base(profile, scale)
        {
        }

#if UNITY_2019_3_OR_NEWER
        private static readonly List<XRInputSubsystem> XRInputSubsystems = new List<XRInputSubsystem>();
#endif // UNITY_2019_3_OR_NEWER

        #region IMixedRealityService Implementation

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "XR SDK Boundary System";

        #endregion IMixedRealityService Implementation

        /// <inheritdoc/>
        protected override List<Vector3> GetBoundaryGeometry()
        {
            // Get the boundary geometry.
            var boundaryGeometry = new List<Vector3>(0);

            if (XRSubsystemHelpers.InputSubsystem?.GetTrackingOriginMode() != TrackingOriginModeFlags.Floor
                || !XRSubsystemHelpers.InputSubsystem.TryGetBoundaryPoints(boundaryGeometry)
                || boundaryGeometry.Count == 0)
            {
#if UNITY_2019_3_OR_NEWER
                // If the "main" input subsystem doesn't have an available boundary, check the rest of them
                SubsystemManager.GetInstances(XRInputSubsystems);
                foreach (XRInputSubsystem xrInputSubsystem in XRInputSubsystems)
                {
                    if (xrInputSubsystem.running
                        && xrInputSubsystem.GetTrackingOriginMode() == TrackingOriginModeFlags.Floor
                        && xrInputSubsystem.TryGetBoundaryPoints(boundaryGeometry)
                        && boundaryGeometry.Count > 0)
                    {
                        break;
                    }
                }
#endif // UNITY_2019_3_OR_NEWER
            }

            return boundaryGeometry;
        }

        /// <summary>
        /// Updates the <see href="https://docs.unity3d.com/2019.3/Documentation/ScriptReference/XR.TrackingOriginModeFlags.html">TrackingOriginModeFlags</see> on the XR device.
        /// </summary>
        protected override void SetTrackingSpace()
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
#if UNITY_2019_3_OR_NEWER
                // If the "main" input subsystem can't set the origin mode, check the rest of them
                SubsystemManager.GetInstances(XRInputSubsystems);
                foreach (XRInputSubsystem xrInputSubsystem in XRInputSubsystems)
                {
                    if (xrInputSubsystem.running && xrInputSubsystem.TrySetTrackingOriginMode(trackingOriginMode))
                    {
                        return;
                    }
                }
#endif // UNITY_2019_3_OR_NEWER
                Debug.LogWarning("Tracking origin unable to be set.");
            }
        }
    }
}
