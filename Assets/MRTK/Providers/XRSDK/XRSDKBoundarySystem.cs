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
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Boundary/BoundarySystemGettingStarted.html")]
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

        /// <inheritdoc/>
        protected override bool IsXRDevicePresent
        {
            get
            {
                List<InputDevice> devices = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, devices);
                return devices.Count > 0;
            }
        }

        #region IMixedRealityService Implementation

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "XR SDK Boundary System";

        #endregion IMixedRealityService Implementation

        /// <inheritdoc/>
        protected override List<Vector3> GetBoundaryGeometry()
        {
            // Boundaries are supported for Room Scale experiences only.
            if (XRSubsystemHelpers.InputSubsystem.GetTrackingOriginMode() != TrackingOriginModeFlags.Floor)
            {
                return null;
            }

            // Get the boundary geometry.
            var boundaryGeometry = new List<Vector3>(0);

            if (!XRSubsystemHelpers.InputSubsystem.TryGetBoundaryPoints(boundaryGeometry) || boundaryGeometry.Count == 0)
            {
                return null;
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

            if (!XRSubsystemHelpers.InputSubsystem.TrySetTrackingOriginMode(trackingOriginMode))
            {
                Debug.LogWarning("Tracking origin unable to be set.");
            }
        }
    }
}
