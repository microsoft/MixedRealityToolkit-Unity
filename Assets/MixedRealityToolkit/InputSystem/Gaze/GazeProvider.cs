// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.Cursors;
using Microsoft.MixedReality.Toolkit.InputSystem.Pointers;
using Microsoft.MixedReality.Toolkit.InputSystem.Sources;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Gaze
{
    /// <summary>
    /// This class provides Gaze as an Input Source so users can interact with objects using their head.
    /// </summary>
    [DisallowMultipleComponent]
    public class GazeProvider : MonoBehaviour, IMixedRealityGazeProvider
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
        /// <example>
        /// <para>Allow the cursor to hit SR, but first prioritize any DefaultRaycastLayers (potentially behind SR)</para>
        /// <code language="csharp"><![CDATA[
        /// int sr = LayerMask.GetMask("SR");
        /// int nonSR = Physics.DefaultRaycastLayers &amp; ~sr;
        /// GazeProvider.Instance.RaycastLayerMasks = new LayerMask[] { nonSR, sr };
        /// ]]></code>
        /// </example>
        /// </summary>
        [SerializeField]
        [Tooltip("The LayerMasks, in prioritized order, that are used to determine the GazeTarget when raycasting.")]
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

        /// <inheritdoc />
        public IMixedRealityInputSource GazeInputSource
        {
            get
            {
                if (gazeInputSource == null)
                {
                    InitializeInputSource();
                }
                return gazeInputSource;
            }
        }
        private BaseGenericInputSource gazeInputSource;

        /// <inheritdoc />
        public IMixedRealityPointer GazePointer => gazePointer ?? InitializeGazePointer();
        private IMixedRealityPointer gazePointer = null;

        /// <inheritdoc />
        public GameObject GazeTarget { get; private set; }

        /// <inheritdoc />
        public RaycastHit HitInfo { get; private set; }

        /// <inheritdoc />
        public Vector3 HitPosition { get; private set; }

        /// <inheritdoc />
        public Vector3 HitNormal { get; private set; }

        /// <inheritdoc />
        public Vector3 GazeOrigin => GazePointer.Rays[0].Origin;

        /// <inheritdoc />
        public Vector3 GazeDirection => GazePointer.Rays[0].Direction;

        private float lastHitDistance = 2.0f;

        private bool delayInitialization = true;

        private IMixedRealityInputSystem inputSystem = null;
        private IMixedRealityInputSystem InputSystem => inputSystem ?? (inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>());

        #region IMixedRealityPointer Implementation

        private class InternalGazePointer : GenericPointer
        {
            private readonly Transform gazeTransform;
            private readonly BaseRayStabilizer stabilizer;
            private readonly GazeProvider gazeProvider;

            public InternalGazePointer(GazeProvider gazeProvider, string pointerName, IMixedRealityInputSource inputSourceParent, LayerMask[] raycastLayerMasks, float pointerExtent, Transform gazeTransform, BaseRayStabilizer stabilizer)
                    : base(pointerName, inputSourceParent)
            {
                this.gazeProvider = gazeProvider;
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

                gazeProvider.HitPosition = Rays[0].Origin + (gazeProvider.lastHitDistance * Rays[0].Direction);
            }

            public override void OnPostRaycast()
            {
                gazeProvider.HitInfo = Result.Details.LastRaycastHit;
                gazeProvider.GazeTarget = Result.Details.Object;

                if (Result.Details.Object != null)
                {
                    gazeProvider.lastHitDistance = (Result.Details.Point - Rays[0].Origin).magnitude;
                    gazeProvider.HitPosition = Rays[0].Origin + (gazeProvider.lastHitDistance * Rays[0].Direction);
                    gazeProvider.HitNormal = Result.Details.Normal;
                }
            }

            public override bool TryGetPointerPosition(out Vector3 position)
            {
                position = gazeTransform.position;
                return true;
            }

            public override bool TryGetPointingRay(out Ray pointingRay)
            {
                pointingRay = new Ray(gazeProvider.GazeOrigin, gazeProvider.GazeDirection);
                return true;
            }

            public override bool TryGetPointerRotation(out Quaternion rotation)
            {
                rotation = Quaternion.identity;
                return false;
            }
        }

        #endregion IMixedRealityPointer Implementation

        #region Monobehaiour Implementation

        protected virtual void OnEnable()
        {
            if (!delayInitialization)
            {
                // The first time we call OnEnable we skip this.
                RaiseSourceDetected();
            }
        }

        private void Start()
        {
            if (cursorPrefab != null)
            {
                var cursorObj = Instantiate(cursorPrefab, transform);
                GazePointer.BaseCursor = cursorObj.GetComponent<BaseCursor>();
                Debug.Assert(GazePointer.BaseCursor != null, "Failed to load cursor");
                GazePointer.BaseCursor.Pointer = GazePointer;
            }

            FindGazeTransform();

            if (delayInitialization)
            {
                delayInitialization = false;
                RaiseSourceDetected();
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
            InputSystem.RaiseSourceLost(GazeInputSource);
            GazePointer.BaseCursor?.SetVisibility(false);
        }

        private void OnDestroy()
        {
            if (GazePointer.BaseCursor != null)
            {
                Destroy(GazePointer.BaseCursor.GetGameObjectReference());
            }
        }

        #endregion Monobehaiour Implementation

        #region Utilities

        private void InitializeInputSource()
        {
            gazeInputSource = new BaseGenericInputSource("Gaze", new[] { new InteractionDefinition(1, AxisType.None, Internal.Definitions.Devices.DeviceInputType.Gaze) }); // TODO - Needs reviewing for Input Action
        }

        private IMixedRealityPointer InitializeGazePointer()
        {
            return gazePointer = new InternalGazePointer(this, "Gaze Pointer", gazeInputSource, raycastLayerMasks, maxGazeCollisionDistance, gazeTransform, stabilizer);
        }

        private void RaiseSourceDetected()
        {
            GazePointer.BaseCursor?.SetVisibility(true);
            InputSystem.RaiseSourceDetected(GazeInputSource);
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
