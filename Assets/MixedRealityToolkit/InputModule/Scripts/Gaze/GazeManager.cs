// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using MixedRealityToolkit.InputModule.Cursors;
using MixedRealityToolkit.InputModule.Focus;
using MixedRealityToolkit.InputModule.InputSources;
using MixedRealityToolkit.InputModule.Pointers;
using MixedRealityToolkit.InputModule.Utilities;
using System.Collections;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.Gaze
{
    /// <summary>
    /// The gaze manager manages everything related to a gaze ray that can interact with other objects.
    /// </summary>
    public class GazeManager : Singleton<GazeManager>, IInputSource
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

        /// <summary>
        /// Current stabilization method, used to smooth out the gaze ray data.
        /// If left null, no stabilization will be performed.
        /// </summary>
        [SerializeField]
        [Tooltip("Stabilizer, if any, used to smooth out the gaze ray data.")]
        private BaseRayStabilizer stabilizer;

        /// <summary>
        /// Transform that should be used as the source of the gaze position and rotation.
        /// Defaults to the main camera.
        /// </summary>
        [SerializeField]
        [Tooltip("Transform that should be used to represent the gaze position and rotation. Defaults to CameraCache.Main")]
        private Transform gazeTransform;

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
        public static Vector3 GazeOrigin { get { return pointers[0].Rays[0].Origin; } }

        /// <summary>
        /// Normal of the gaze.
        /// </summary>
        public static Vector3 GazeDirection { get { return pointers[0].Rays[0].Direction; } }

        private static float lastHitDistance = 2.0f;

        /// <summary>
        /// Always true initially so we only initialize our interaction sources 
        /// after all <see cref="Singleton{T}"/> Instances have been properly initialized.
        /// </summary>
        private bool delayInitialization = true;

        #region IInputSource Implementation

        public uint SourceId { get; protected set; }

        public string SourceName { get { return "Gaze"; } }

        public IPointer[] Pointers
        {
            get { return pointers; }
        }

        private static readonly IPointer[] pointers = new IPointer[1];

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
                PrioritizedLayerMasksOverride = Instance.raycastLayerMasks;
                PointerExtent = Instance.maxGazeCollisionDistance;
                InteractionEnabled = true;
            }

            public override void OnPreRaycast()
            {
                if (Instance.gazeTransform == null)
                {
                    pointers[0].Rays[0] = default(RayStep);
                }
                else
                {
                    Vector3 newGazeOrigin = Instance.gazeTransform.position;
                    Vector3 newGazeNormal = Instance.gazeTransform.forward;

                    // Update gaze info from stabilizer
                    if (Instance.stabilizer != null)
                    {
                        Instance.stabilizer.UpdateStability(newGazeOrigin, Instance.gazeTransform.rotation);
                        newGazeOrigin = Instance.stabilizer.StablePosition;
                        newGazeNormal = Instance.stabilizer.StableRay.direction;
                    }

                    pointers[0].Rays[0].UpdateRayStep(newGazeOrigin, newGazeOrigin + (newGazeNormal * (pointers[0].PointerExtent ?? FocusManager.GlobalPointingExtent)));
                }

                HitPosition = pointers[0].Rays[0].Origin + (lastHitDistance * pointers[0].Rays[0].Direction);
            }

            public override void OnPostRaycast()
            {
                HitInfo = pointers[0].Result.End.LastRaycastHit;
                GazeTarget = pointers[0].Result.End.Object;

                if (pointers[0].Result.End.Object != null)
                {
                    lastHitDistance = (pointers[0].Result.End.Point - Instance.Pointers[0].Rays[0].Origin).magnitude;
                    HitPosition = pointers[0].Rays[0].Origin + (lastHitDistance * pointers[0].Rays[0].Direction);
                    HitNormal = pointers[0].Result.End.Normal;
                }
            }

            public override bool TryGetPointerPosition(out Vector3 position)
            {
                position = Instance.gazeTransform.position;
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

        protected virtual void OnEnable()
        {
            if (!delayInitialization)
            {
                // The first time we call OnEnable we skip this.
                RegisterSource();
            }
        }

        protected virtual void Start()
        {
            FocusManager.AssertIsInitialized();
            InputManager.AssertIsInitialized();
            Debug.Assert(InputManager.GlobalListeners.Contains(FocusManager.Instance.gameObject));

            SourceId = InputManager.GenerateNewSourceId();

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
            InputManager.Instance.RaiseSourceLost(this);

            if (Pointers[0].BaseCursor != null)
            {
                Pointers[0].BaseCursor.enabled = false;
            }
        }

        protected override void OnDestroy()
        {
            if (Pointers[0].BaseCursor != null)
            {
                Destroy(Pointers[0].BaseCursor.gameObject);
            }

            base.OnDestroy();
        }

        #endregion Monobehaiour Implementation

        #region Utilities

        private void RegisterSource()
        {
            if (Pointers[0].BaseCursor != null)
            {
                Pointers[0].BaseCursor.enabled = true;
            }

            InputManager.Instance.RaiseSourceDetected(this);
        }

        private bool FindGazeTransform()
        {
            if (gazeTransform != null) { return true; }

            if (CameraCache.Main != null)
            {
                gazeTransform = CameraCache.Main.transform;
                return true;
            }

            Debug.LogError("Gaze Manager was not given a GazeTransform and no main camera exists to default to.");
            return false;
        }

        #endregion Utilities
    }
}
