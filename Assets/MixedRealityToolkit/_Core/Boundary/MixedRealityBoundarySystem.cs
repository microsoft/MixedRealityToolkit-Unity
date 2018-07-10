// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Internal.Managers
{
    /// <summary>
    /// The Boundary system controls the presentation and display of the users boundary in a scene
    /// </summary>
    public class MixedRealityBoundaryManager : BaseManager, IMixedRealityBoundarySystem
    {
        /// <inheritdoc/>
        public ExperienceScale Scale { get; set; } = ExperienceScale.Room;

        /// <inheritdoc/>
        public float BoundaryHeight { get; set; } = 3.0f;

        /// <inheritdoc/>
        public bool EnablePlatformBoundaryRendering { get; set; } = true;

        /// <inheritdoc/>
        public Bounds OutscribedVolume { get; private set; } = new Bounds();

        /// <inheritdoc/>
        public Bounds InscribedVolume { get; private set; } = new Bounds();

        /// <summary>
        /// MixedRealityBoundaryManager constructor
        /// </summary>
        public MixedRealityBoundaryManager()
        {
            Scale = MixedRealityManager.Instance.ActiveProfile.TargetExperienceScale;
            BoundaryHeight = MixedRealityManager.Instance.ActiveProfile.BoundaryHeight;
            EnablePlatformBoundaryRendering = MixedRealityManager.Instance.ActiveProfile.EnablePlatformBoundaryRendering;
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            base.Initialize();
            InitializeInternal();
        }

        /// <summary>
        /// Performs initialization tasks for the BoundaryManager.
        /// </summary>
        private void InitializeInternal()
        {
            SetTrackingSpace();
            CalculateBoundaryBounds();
            SetPlatformBoundaryVisibility();
        }

        /// <inheritdoc/>
        public override void Reset()
        {
            base.Reset();
            InitializeInternal();
        }

        /// <summary>
        /// Retrieves the boundary geometry and creates the boundary and inscribed playspace volumes.
        /// </summary>
        private void CalculateBoundaryBounds()
        {
            if (XRDevice.GetTrackingSpaceType() != TrackingSpaceType.RoomScale)
            {
                // Boundaries are supported for Room Scale experiences only.
                return;
            }

            OutscribedVolume = new Bounds();

            // Get the boundary geometry.
            List<Vector3> boundaryGeometry = new List<Vector3>(0);
            if (Boundary.TryGetGeometry(boundaryGeometry))
            {
                for (int i = 0; i < boundaryGeometry.Count; i++)
                {
                    OutscribedVolume.Encapsulate(boundaryGeometry[i]);
                }

                // todo: CreateInscribedBounds()

                // Set the "ceiling" of the space using the configured height.
                OutscribedVolume.Encapsulate(new Vector3(0f, BoundaryHeight, 0f));
            }
        }

        /// <summary>
        /// Updates the <see cref="TrackingSpaceType"/> on the XR device.
        /// </summary>
        private void SetTrackingSpace()
        {
            TrackingSpaceType trackingSpace;

            // In current versions of Unity, there are two types of tracking spaces. For boundaries, if the scale
            // is not Room or Standing, it currently maps to TrackingSpaceType.Stationary.
            switch (Scale)
            {
                case ExperienceScale.Standing:
                case ExperienceScale.Room:
                    trackingSpace = TrackingSpaceType.RoomScale;
                    break;

                case ExperienceScale.OrientationOnly:
                case ExperienceScale.Seated:
                case ExperienceScale.World:
                    trackingSpace = TrackingSpaceType.Stationary;
                    break;

                default:
                    trackingSpace = TrackingSpaceType.Stationary;
                    Debug.LogWarning("Unknown / unsupported ExperienceScale. Defaulting to Stationary tracking space.");
                    break;
            }

            XRDevice.SetTrackingSpaceType(trackingSpace);
        }

        /// <summary>
        /// Sets the property indicating if the boundary should be rendered by the platform.
        /// </summary>
        /// <remarks>
        /// Not all platforms support specifying whether or not to render the playspace boundary.
        /// For platforms without boundary rendering control, the default behavior will be unchanged 
        /// regardless of the value provided.
        /// </remarks>
        private void SetPlatformBoundaryVisibility()
        {
            if (Boundary.configured)
            {
                // This value cannot be configured on Windows Mixed Reality. Automatic boundary rendering is performed.
                Boundary.visible = EnablePlatformBoundaryRendering;
            }
        }
    }
}
