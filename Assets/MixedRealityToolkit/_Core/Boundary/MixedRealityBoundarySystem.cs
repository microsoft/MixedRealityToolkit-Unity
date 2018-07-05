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
        /// <summary>
        /// The size of the physical space available for Mixed Reality.
        /// </summary>
        internal TrackingSpaceType trackingSpaceType = TrackingSpaceType.RoomScale;

        /// <summary>
        /// The height of the playspace, in meters.
        /// </summary>
        /// <remarks>
        /// This is used to create a three dimensional boundary volume.
        /// </remarks>
        internal float boundaryHeight = 8.0f;

        /// <summary>
        /// Enable / disable the platform's playspace boundary rendering.
        /// </summary>
        /// <remarks>
        /// Not all platforms support specifying whether or not to render the playspace boundary.
        /// For platforms without boundary rendering control, the default behavior will be unchanged 
        /// regardless of the value provided.
        /// </remarks>
        internal bool enablePlatformBoundaryRendering = true;

        /// <summary>
        /// 
        /// </summary>
        internal bool createInscribedRectangle = false;

        /// <summary>
        /// 
        /// </summary>
        public Bounds BoundaryVolume { get; private set; } = new Bounds();

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

        private void InitializeInternal()
        {
            XRDevice.SetTrackingSpaceType(trackingSpaceType);

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
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool WithinBoundary(Vector3 position)
        {
            return BoundaryVolume.Contains(position);
        }

        /// <summary>
        /// 
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
                Debug.Log("Boundaries are supported for Room Scale experiences only.");
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

                // todo: if (createInscribedRectangle)

                // Set the "ceiling" of the space using the configured height.
                BoundaryVolume.Encapsulate(new Vector3(0f, boundaryHeight, 0f));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetPlatformBoundaryVisibility()
        {
            if (Boundary.configured)
            {
#if !UNITY_WSA
                // This value cannot be configured on Windows Mixed Reality. Automatic boundary rendering is performed.
                Boundary.visible = enablePlatformBoundaryRendering;
#endif
            }
        }
    }
}
