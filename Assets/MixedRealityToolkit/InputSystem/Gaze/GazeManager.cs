// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using Microsoft.MixedReality.Toolkit.InputSystem.Cursors;
using Microsoft.MixedReality.Toolkit.InputSystem.Focus;
using Microsoft.MixedReality.Toolkit.InputSystem.InputSources;
using Microsoft.MixedReality.Toolkit.InputSystem.Pointers;
using Microsoft.MixedReality.Toolkit.InputSystem.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Gaze
{
    /// <summary>
    /// The gaze manager manages everything related to a gaze ray that can interact with other objects.
    /// </summary>
    public class GazeManager : MonoBehaviour, IInputSource
    {
        [SerializeField]
        [Tooltip("Optional Cursor Prefab to use if you don't wish to reference a cursor in the scene.")]
        private GameObject cursorPrefab;

        /// <summary>
        /// Maximum distance at which the gaze can collide with an object.
        /// </summary>
        [SerializeField]
        [Tooltip("Maximum distance at which the gaze can collide with an object.")]
        private float maxGazeCollisionDistance = 10.0f;

        public static float MaxGazeCollisionDistance { get; private set; }

        /// <summary>
        /// The LayerMasks, in prioritized order, that are used to determine the GazeTarget when raycasting.
        ///
        /// Example Usage:
        ///
        /// // Allow the cursor to hit SR, but first prioritize any DefaultRaycastLayers (potentially behind SR)
        ///
        /// int sr = LayerMask.GetMask("SR");
        /// int nonSR = Physics.DefaultRaycastLayers & ~sr;
        /// GazeManager.Instance.RaycastLayerMasks = new LayerMask[] { nonSR, sr };
        /// </summary>
        [SerializeField]
        [Tooltip("The LayerMasks, in prioritized order, that are used to determine the GazeTarget when raycasting.\n\nExample Usage:\n\n// Allow the cursor to hit SR, but first prioritize any DefaultRaycastLayers (potentially behind SR)\n\nint sr = LayerMask.GetMask(\"SR\");\nint nonSR = Physics.DefaultRaycastLayers & ~sr;\nGazeManager.Instance.RaycastLayerMasks = new LayerMask[] { nonSR, sr };")]
        private LayerMask[] raycastLayerMasks = { Physics.DefaultRaycastLayers };

        public static LayerMask[] RaycastLayerMasks { get; private set; }

        /// <summary>
        /// Current stabilization method, used to smooth out the gaze ray data.
        /// If left null, no stabilization will be performed.
        /// </summary>
        [SerializeField]
        [Tooltip("Stabilizer, if any, used to smooth out the gaze ray data.")]
        private BaseRayStabilizer stabilizer;

        public static BaseRayStabilizer Stabilizer { get; private set; }

        /// <summary>
        /// Transform that should be used as the source of the gaze position and rotation.
        /// Defaults to the main camera.
        /// </summary>
        [SerializeField]
        [Tooltip("Transform that should be used to represent the gaze position and rotation. Defaults to CameraCache.Main")]
        private Transform gazeTransform;

        public static Transform GazeTransform { get; private set; }

        [SerializeField]
        [Tooltip("True to draw a debug view of the ray.")]
        private bool debugDrawRay;

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
        public static Vector3 GazeOrigin => Pointers[0].Rays[0].Origin;

        /// <summary>
        /// Normal of the gaze.
        /// </summary>
        public static Vector3 GazeDirection => Pointers[0].Rays[0].Direction;

        private static float lastHitDistance = 2.0f;

        private bool delayInitialization = true;

        #region IInputSource Implementation

        uint IInputSource.SourceId => SourceId;

        public static uint SourceId { get; protected set; }

        public string SourceName => "Gaze";

        IPointer[] IInputSource.Pointers => Pointers;

        public static IPointer[] Pointers { get; } = new IPointer[1];

        public SupportedInputInfo GetSupportedInputInfo()
        {
            return SupportedInputInfo.Pointing;
        }

        public bool SupportsInputInfo(SupportedInputInfo inputInfo)
        {
            return (GetSupportedInputInfo() & inputInfo) == inputInfo;
        }

        #endregion IInputSource Implementation

        #region IPointer Implementation

        private class GazePointer : GenericPointer
        {
            public GazePointer(string pointerName, IInputSource inputSourceParent) : base(pointerName, inputSourceParent)
            {
                PrioritizedLayerMasksOverride = RaycastLayerMasks;
                PointerExtent = MaxGazeCollisionDistance;
                InteractionEnabled = true;
            }

            public override void OnPreRaycast()
            {
                if (GazeTransform == null)
                {
                    Pointers[0].Rays[0] = default(RayStep);
                }
                else
                {
                    Vector3 newGazeOrigin = GazeTransform.position;
                    Vector3 newGazeNormal = GazeTransform.forward;

                    // Update gaze info from stabilizer
                    if (Stabilizer != null)
                    {
                        Stabilizer.UpdateStability(newGazeOrigin, GazeTransform.rotation);
                        newGazeOrigin = Stabilizer.StablePosition;
                        newGazeNormal = Stabilizer.StableRay.direction;
                    }

                    Pointers[0].Rays[0].UpdateRayStep(newGazeOrigin, newGazeOrigin + (newGazeNormal * (Pointers[0].PointerExtent ?? FocusManager.GlobalPointingExtent)));
                }

                HitPosition = Pointers[0].Rays[0].Origin + (lastHitDistance * Pointers[0].Rays[0].Direction);
            }

            public override void OnPostRaycast()
            {
                HitInfo = Pointers[0].Result.End.LastRaycastHit;
                GazeTarget = Pointers[0].Result.End.Object;

                if (Pointers[0].Result.End.Object != null)
                {
                    lastHitDistance = (Pointers[0].Result.End.Point - Pointers[0].Rays[0].Origin).magnitude;
                    HitPosition = Pointers[0].Rays[0].Origin + (lastHitDistance * Pointers[0].Rays[0].Direction);
                    HitNormal = Pointers[0].Result.End.Normal;
                }
            }

            public override bool TryGetPointerPosition(out Vector3 position)
            {
                position = GazeTransform.position;
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

        #region IEquality Implementation

        private bool Equals(IInputSource other)
        {
            return base.Equals(other) && SourceId == other.SourceId && string.Equals(SourceName, other.SourceName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IInputSource)obj);
        }

        public static bool Equals(IInputSource left, IInputSource right)
        {
            return left.SourceId == right.SourceId;
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            var left = (IInputSource)x;
            var right = (IInputSource)y;
            if (left != null && right != null)
            {
                return Equals(left, right);
            }

            return false;
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)SourceId;
                hashCode = (hashCode * 397) ^ (SourceName != null ? SourceName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation

        #region Monobehaiour Implementation

        private void Awake()
        {
            Stabilizer = stabilizer;
            GazeTransform = gazeTransform;
            RaycastLayerMasks = raycastLayerMasks;
            MaxGazeCollisionDistance = maxGazeCollisionDistance;
        }

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
            SourceId = MixedRealityInputManager.GenerateNewSourceId();

            Pointers[0] = new GazePointer("Gaze Pointer", this);

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
            MixedRealityInputManager.RaiseSourceLost(this);

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

            MixedRealityInputManager.RaiseSourceDetected(this);
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
