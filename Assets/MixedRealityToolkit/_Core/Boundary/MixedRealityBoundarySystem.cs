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
        /// 
        /// </summary>
        private TrackingSpaceType trackingSpaceType = TrackingSpaceType.RoomScale;

        /// <summary>
        /// 
        /// </summary>
        private float boundaryHeight = 10.0f;

        /// <summary>
        /// 
        /// </summary>
        private bool forcePlatformBoundaryRendering = false;

        /// <summary>
        /// 
        /// </summary>
        private bool useInscribedRectangle = false;

        /// <summary>
        /// 
        /// </summary>
        public Bounds BoundaryVolume { get; private set; }

        /// <summary>
        /// MixedRealityBoundaryManager constructor
        /// </summary>
        public MixedRealityBoundaryManager()
        {
            // TODO define any constructor requirements
        }

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
            // todo: what should we do for NON opaque devices?
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

        public bool NearBoundaryEdge(Vector3 position)
        {
            // todo
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CalculateBoundaryBounds()
        {
            if (XRDevice.GetTrackingSpaceType() != TrackingSpaceType.RoomScale)
            {
                Debug.Log("Boundaries are supported for Room Scale experiences only.");
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
            }
            else
            {
                Debug.LogWarning("Failed to acquire the geometry of the space boundary.");
            }

            // Set the "ceiling" of the space using the configured height.
            BoundaryVolume.Encapsulate(new Vector3(0f, boundaryHeight, 0f));
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetPlatformBoundaryVisibility()
        {
            if (UnityEngine.Experimental.XR.Boundary.configured)
            {
                UnityEngine.Experimental.XR.Boundary.visible = forcePlatformBoundaryRendering;
            }
        }
    }
}
