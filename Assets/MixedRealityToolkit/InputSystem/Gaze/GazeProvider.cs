// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.Cursors;
using Microsoft.MixedReality.Toolkit.InputSystem.Focus;
using Microsoft.MixedReality.Toolkit.InputSystem.Pointers;
using Microsoft.MixedReality.Toolkit.InputSystem.Sources;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Gaze
{
    /// <summary>
    /// The gaze manager manages everything related to a gaze ray that can interact with other objects.
    /// </summary>
    [DisallowMultipleComponent]
    public class GazeProvider : BaseInputSource, IMixedRealityGazeProvider
    {
        [SerializeField]
        [Tooltip("Optional Cursor Prefab to use if you don't wish to reference a cursor in the scene.")]
        private GameObject cursorPrefab = null;

        /// <summary>
        /// Maximum distance at which the gaze can collide with an object.
        /// </summary>
        [SerializeField]
        [Tooltip("Maximum distance at which the gaze can collide with an object.")]
        private float maxGazeCollisionDistance = 10.0f;

        /// <summary>
        /// The LayerMasks, in prioritized order, that are used to determine the GazeTarget when raycasting.
        ///
        /// Example Usage:
        ///
        /// // Allow the cursor to hit SR, but first prioritize any DefaultRaycastLayers (potentially behind SR)
        ///
        /// int sr = LayerMask.GetMask("SR");
        /// int nonSR = Physics.DefaultRaycastLayers & ~sr;
        /// GazeProvider.Instance.RaycastLayerMasks = new LayerMask[] { nonSR, sr };
        /// </summary>
        [SerializeField]
        [Tooltip("The LayerMasks, in prioritized order, that are used to determine the GazeTarget when raycasting.\n\nExample Usage:\n\n// Allow the cursor to hit SR, but first prioritize any DefaultRaycastLayers (potentially behind SR)\n\nint sr = LayerMask.GetMask(\"SR\");\nint nonSR = Physics.DefaultRaycastLayers & ~sr;\nGazeManager.Instance.RaycastLayerMasks = new LayerMask[] { nonSR, sr };")]
        private LayerMask[] raycastLayerMasks = { Physics.DefaultRaycastLayers };

        /// <summary>
        /// Current stabilization method, used to smooth out the gaze ray data.
        /// If left null, no stabilization will be performed.
        /// </summary>
        [SerializeField]
        [Tooltip("Stabilizer, if any, used to smooth out the gaze ray data.")]
        private BaseRayStabilizer stabilizer = null;

        /// <summary>
        /// Transform that should be used as the source of the gaze position and rotation.
        /// Defaults to the main camera.
        /// </summary>
        [SerializeField]
        [Tooltip("Transform that should be used to represent the gaze position and rotation. Defaults to CameraCache.Main")]
        private Transform gazeTransform = null;

        [SerializeField]
        [Tooltip("True to draw a debug view of the ray.")]
        private bool debugDrawRay = false;

        /// <summary>
        /// The game object that is currently being gazed at, if any.
        /// </summary>
        public static GameObject GazeTarget { get; private set; }

        /// <summary>
        /// HitInfo property gives access to information at the object being gazed at, if any.
        /// </summary>
        public static RaycastHit HitInfo { get; private set; }

        /// <summary>
        /// Position at which the gaze manager hit an object.
        /// If no object is currently being hit, this will use the last hit distance.
        /// </summary>
        public static Vector3 HitPosition { get; private set; }

        /// <summary>
        /// Normal of the point at which the gaze manager hit an object.
        /// If no object is currently being hit, this will return the previous normal.
        /// </summary>
        public static Vector3 HitNormal { get; private set; }

        /// <summary>
        /// Origin of the gaze.
        /// </summary>
        public static Vector3 GazeOrigin => GazePointer.Rays[0].Origin;

        /// <summary>
        /// Normal of the gaze.
        /// </summary>
        public static Vector3 GazeDirection => GazePointer.Rays[0].Direction;

        private static float lastHitDistance = 2.0f;

        private bool delayInitialization = true;

        #region IInputSource Implementation

        public override string SourceName => "Gaze";

        public static IPointer GazePointer { get; private set; } = null;
        private IPointer[] pointers = null;
        public override IPointer[] Pointers
        {
            get
            {
                if (GazePointer == null)
                {
                    GazePointer = new InternalGazePointer("Gaze Pointer", this, raycastLayerMasks, maxGazeCollisionDistance, gazeTransform, stabilizer);
                    pointers = new[] { GazePointer };
                }

                return pointers;
            }
        }

        #endregion IInputSource Implementation

        #region IPointer Implementation

        private class InternalGazePointer : GenericPointer
        {
            private readonly Transform gazeTransform;
            private readonly BaseRayStabilizer stabilizer;

            public InternalGazePointer(string pointerName, IInputSource inputSourceParent, LayerMask[] raycastLayerMasks, float pointerExtent, Transform gazeTransform, BaseRayStabilizer stabilizer)
                    : base(pointerName, inputSourceParent)
            {
                PrioritizedLayerMasksOverride = raycastLayerMasks;
                PointerExtent = pointerExtent;
                this.gazeTransform = gazeTransform;
                this.stabilizer = stabilizer;
                InteractionEnabled = true;
            }

            public override void OnPreRaycast()
            {
                if (gazeTransform == null)
                {
                    Rays[0] = default(RayStep);
                }
                else
                {
                    Vector3 newGazeOrigin = gazeTransform.position;
                    Vector3 newGazeNormal = gazeTransform.forward;

                    // Update gaze info from stabilizer
                    if (stabilizer != null)
                    {
                        stabilizer.UpdateStability(newGazeOrigin, gazeTransform.rotation);
                        newGazeOrigin = stabilizer.StablePosition;
                        newGazeNormal = stabilizer.StableRay.direction;
                    }

                    Rays[0].UpdateRayStep(newGazeOrigin, newGazeOrigin + (newGazeNormal * (PointerExtent ?? InputSystem.FocusProvider.GlobalPointingExtent)));
                }

                HitPosition = Rays[0].Origin + (lastHitDistance * Rays[0].Direction);
            }

            public override void OnPostRaycast()
            {
                HitInfo = Result.End.LastRaycastHit;
                GazeTarget = Result.End.Object;

                if (Result.End.Object != null)
                {
                    lastHitDistance = (Result.End.Point - Rays[0].Origin).magnitude;
                    HitPosition = Rays[0].Origin + (lastHitDistance * Rays[0].Direction);
                    HitNormal = Result.End.Normal;
                }
            }

            public override bool TryGetPointerPosition(out Vector3 position)
            {
                position = gazeTransform.position;
                return true;
            }

            public override bool TryGetPointingRay(out Ray pointingRay)
            {
                pointingRay = new Ray(GazeOrigin, GazeDirection);
                return true;
            }

            public override bool TryGetPointerRotation(out Quaternion rotation)
            {
                rotation = Quaternion.identity;
                return false;
            }
        }

        #endregion IPointer Implementation

        #region Monobehaiour Implementation

        protected virtual void OnEnable()
        {
            if (!delayInitialization)
            {
                // The first time we call OnEnable we skip this.
                RegisterSource();
            }
        }

        private void Start()
        {

            if (cursorPrefab != null)
            {
                var cursorObj = Instantiate(cursorPrefab, transform);
                Pointers[0].BaseCursor = cursorObj.GetComponent<BaseCursor>();
                 Debug.Assert(Pointers[0].BaseCursor != null, "Failed to load cursor");

                Pointers[0].BaseCursor.Pointer = Pointers[0];
            }

            // Add default RaycastLayers as first layerPriority
            if (raycastLayerMasks == null || raycastLayerMasks.Length == 0)
            {
                raycastLayerMasks = new LayerMask[] { Physics.DefaultRaycastLayers };
            }

            FindGazeTransform();

            if (delayInitialization)
            {
                delayInitialization = false;
                RegisterSource();
            }
        }

        private void Update()
        {
            if (!FindGazeTransform())
            {
                return;
            }

            if (debugDrawRay)
            {
                Debug.DrawRay(GazeOrigin, (HitPosition - GazeOrigin), Color.white);
            }
        }

        private void OnDisable()
        {
            InputSystem.RaiseSourceLost(this);

            if (Pointers[0].BaseCursor != null)
            {
                Pointers[0].BaseCursor.enabled = false;
            }
        }

        private void OnDestroy()
        {
            if (Pointers[0].BaseCursor != null)
            {
                Destroy(Pointers[0].BaseCursor.gameObject);
            }
        }

        #endregion Monobehaiour Implementation

        #region Utilities

        private void RegisterSource()
        {
            if (Pointers[0].BaseCursor != null)
            {
                Pointers[0].BaseCursor.enabled = true;
            }

            InputSystem.RaiseSourceDetected(this);
        }

        private bool FindGazeTransform()
        {
            if (gazeTransform != null) { return true; }

            if (CameraCache.Main != null)
            {
                gazeTransform = CameraCache.Main.transform;
                return true;
            }

            Debug.LogError("Gaze Manager was not given a GazeTransform and no main camera exists to default to!");
            return false;
        }

        #endregion Utilities
    }
}
