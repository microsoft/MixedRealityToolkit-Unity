// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
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
        public TrackingSpaceType TrackingSpaceType { get; set; } = TrackingSpaceType.RoomScale;

        public float BoundaryHeight { get; set; } = 10.0f;

        public bool EnablePlatformBoundaryRendering { get; set; } = true;

        /// <summary>
        /// A three dimensional volume as described by the playspace boundary and
        /// the configured height.
        /// </summary>
        public Bounds BoundaryVolume { get; private set; } = new Bounds();

        /// <summary>
        /// A three dimensional volume as described by the inscribed rectangle and
        /// the configured height.
        /// </summary>
        public Bounds InscribedVolume { get; private set; } = new Bounds();

        /// <summary>
        /// MixedRealityBoundaryManager constructor
        /// </summary>
        public MixedRealityBoundaryManager()
        { }

        /// <summary>
        /// The initialize function is used to setup the manager once created.
        /// This method is called once all managers have been registered in the Mixed Reality Manager.
        /// </summary>
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
            XRDevice.SetTrackingSpaceType(TrackingSpaceType);

            CalculateBoundaryBounds();
            SetPlatformBoundaryVisibility();
        }

        /// <summary>
        /// Optional ProfileUpdate function to allow reconfiguration when the active configuration profile of the Mixed Reality Manager is replaced
        /// </summary>
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
            if (!Boundary.configured)
            {
                // The user has not configured a playspace boundary on this device.
                return;
            }

            if (XRDevice.GetTrackingSpaceType() != TrackingSpaceType.RoomScale)
            {
                // Boundaries are supported for Room Scale experiences only.
                return;
            }

            BoundaryVolume = new Bounds();

            // Get the boundary geometry.
            List<Vector3> boundaryGeometry = new List<Vector3>(0);
            if (Boundary.TryGetGeometry(boundaryGeometry))
            {
                for (int i = 0; i < boundaryGeometry.Count; i++)
                {
                    BoundaryVolume.Encapsulate(boundaryGeometry[i]);
                }

                // todo: CreateInscribedBounds()

                // Set the "ceiling" of the space using the configured height.
                BoundaryVolume.Encapsulate(new Vector3(0f, BoundaryHeight, 0f));
            }
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
