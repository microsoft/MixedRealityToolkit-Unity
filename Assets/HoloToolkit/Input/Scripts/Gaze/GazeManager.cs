// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// The gaze manager manages everything related to a gaze ray that can interact with other objects.
    /// </summary>
    public class GazeManager : Singleton<GazeManager>, IPointingSource
    {
        [SerializeField]
        [Tooltip("Optional Cursor Prefab to use if you don't wish to reference a cursor in the scene.")]
        private GameObject cursorPrefab;

        [SerializeField]
        [Tooltip("Scene Cursor to use if you don't wish to instantiate the cursor during runtime.")]
        private Cursor sceneCursor;

        /// <summary>
        /// Maximum distance at which the gaze can collide with an object.
        /// </summary>
        [Tooltip("Maximum distance at which the gaze can collide with an object.")]
        public float MaxGazeCollisionDistance = 10.0f;

        /// <summary>
        /// The LayerMasks, in prioritized order, that are used to determine the HitObject when raycasting.
        ///
        /// Example Usage:
        ///
        /// // Allow the cursor to hit SR, but first prioritize any DefaultRaycastLayers (potentially behind SR)
        ///
        /// int sr = LayerMask.GetMask("SR");
        /// int nonSR = Physics.DefaultRaycastLayers & ~sr;
        /// GazeManager.Instance.RaycastLayerMasks = new LayerMask[] { nonSR, sr };
        /// </summary>
        [Tooltip("The LayerMasks, in prioritized order, that are used to determine the HitObject when raycasting.\n\nExample Usage:\n\n// Allow the cursor to hit SR, but first prioritize any DefaultRaycastLayers (potentially behind SR)\n\nint sr = LayerMask.GetMask(\"SR\");\nint nonSR = Physics.DefaultRaycastLayers & ~sr;\nGazeManager.Instance.RaycastLayerMasks = new LayerMask[] { nonSR, sr };")]
        public LayerMask[] RaycastLayerMasks = { Physics.DefaultRaycastLayers };

        /// <summary>
        /// Current stabilization method, used to smooth out the gaze ray data.
        /// If left null, no stabilization will be performed.
        /// </summary>
        [Tooltip("Stabilizer, if any, used to smooth out the gaze ray data.")]
        public BaseRayStabilizer Stabilizer;

        /// <summary>
        /// Transform that should be used as the source of the gaze position and rotation.
        /// Defaults to the main camera.
        /// </summary>
        [Tooltip("Transform that should be used to represent the gaze position and rotation. Defaults to CameraCache.Main")]
        public Transform GazeTransform;

        [Tooltip("True to draw a debug view of the ray.")]
        public bool DebugDrawRay;

        /// <summary>
        /// HitInfo property gives access to information at the object being gazed at, if any.
        /// </summary>
        public RaycastHit HitInfo { get; private set; }

        /// <summary>
        /// The game object that is currently being gazed at, if any.
        /// </summary>
        public GameObject HitObject { get; private set; }

        /// <summary>
        /// Position at which the gaze manager hit an object.
        /// If no object is currently being hit, this will use the last hit distance.
        /// </summary>
        public Vector3 HitPosition { get; private set; }

        /// <summary>
        /// Normal of the point at which the gaze manager hit an object.
        /// If no object is currently being hit, this will return the previous normal.
        /// </summary>
        public Vector3 HitNormal { get; private set; }

        /// <summary>
        /// Origin of the gaze.
        /// </summary>
        public Vector3 GazeOrigin { get { return Rays[0].Origin; } }

        /// <summary>
        /// Normal of the gaze.
        /// </summary>
        public Vector3 GazeNormal { get { return Rays[0].Direction; } }

        private float lastHitDistance = 2.0f;

        #region IInputSource Implementation

        public uint SourceId { get; protected set; }

        public string Name { get { return "Gaze"; } }

        public SupportedInputInfo GetSupportedInputInfo()
        {
            return SupportedInputInfo.Pointing;
        }

        public bool SupportsInputInfo(SupportedInputInfo inputInfo)
        {
            return (GetSupportedInputInfo() & inputInfo) == inputInfo;
        }

        #endregion IInputSource Implementation

        #region IPointingSource Implementation

        public Cursor Cursor { get; set; }

        public bool InteractionEnabled { get { return true; } }

        public bool FocusLocked { get; set; }

        public float? ExtentOverride
        {
            get { return MaxGazeCollisionDistance; }
            set
            {
                if (value.HasValue)
                {
                    MaxGazeCollisionDistance = value.Value;
                }
            }
        }

        public RayStep[] Rays { get { return rays; } }
        private readonly RayStep[] rays = { new RayStep(Vector3.zero, Vector3.zero) };

        public LayerMask[] PrioritizedLayerMasksOverride
        {
            get
            {
                return RaycastLayerMasks;
            }

            set
            {
                RaycastLayerMasks = value;
            }
        }

        public PointerResult Result { get; set; }

        public BaseRayStabilizer RayStabilizer { get; set; }

        public bool OwnsInput(InputEventData eventData)
        {
            return Equals(eventData.InputSource);
        }

        public virtual void OnPreRaycast()
        {
            UpdateGazeInfo();
        }

        public virtual void OnPostRaycast() { }

        public bool TryGetPointerPosition(out Vector3 position)
        {
            position = GazeTransform.position;
            return true;
        }

        public bool TryGetPointingRay(out Ray pointingRay)
        {
            pointingRay = new Ray(GazeOrigin, GazeNormal);
            return true;
        }

        public bool TryGetPointerRotation(out Quaternion rotation)
        {
            rotation = Quaternion.identity;
            return false;
        }

        #endregion IPointingSource Implementation

        #region IEquality Implementation

        private bool Equals(IInputSource other)
        {
            return base.Equals(other) && SourceId == other.SourceId && string.Equals(Name, other.Name);
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
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation

        protected override void Awake()
        {
            base.Awake();

            SourceId = InputManager.GenerateNewSourceId();

            if (sceneCursor != null)
            {
                Cursor = sceneCursor;
                Debug.Assert(Cursor != null, "Failed to load cursor");
            }

            if (Cursor == null && cursorPrefab != null)
            {
                var cursorObj = Instantiate(cursorPrefab, transform);
                Cursor = cursorObj.GetComponent<Cursor>();
                Debug.Assert(Cursor != null, "Failed to load cursor");
            }

            // Add default RaycastLayers as first layerPriority
            if (RaycastLayerMasks == null || RaycastLayerMasks.Length == 0)
            {
                RaycastLayerMasks = new LayerMask[] { Physics.DefaultRaycastLayers };
            }

            FindGazeTransform();
        }

        private void Update()
        {
            if (!FindGazeTransform())
            {
                return;
            }

            if (DebugDrawRay)
            {
                Debug.DrawRay(GazeOrigin, (HitPosition - GazeOrigin), Color.white);
            }
        }

        private bool FindGazeTransform()
        {
            if (GazeTransform != null) { return true; }

            if (CameraCache.Main != null)
            {
                GazeTransform = CameraCache.Main.transform;
                return true;
            }

            Debug.LogError("Gaze Manager was not given a GazeTransform and no main camera exists to default to.");
            return false;
        }

        /// <summary>
        /// Updates the current gaze information, so that the gaze origin and normal are accurate.
        /// </summary>
        private void UpdateGazeInfo()
        {
            if (GazeTransform == null)
            {
                Rays[0] = default(RayStep);
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

                Rays[0].UpdateRayStep(newGazeOrigin, newGazeOrigin + (newGazeNormal * FocusManager.Instance.GetPointingExtent(this)));
            }

            UpdateHitPosition();
        }

        /// <summary>
        /// Notifies this gaze manager of its new hit details.
        /// </summary>
        /// <param name="focusDetails">Details of the current focus.</param>
        /// <param name="hitInfo">Details of the focus raycast hit.</param>
        /// <param name="isRegisteredForFocus">Whether or not this gaze manager is registered as a focus pointer.</param>
        public void UpdateHitDetails(FocusDetails focusDetails, RaycastHit hitInfo, bool isRegisteredForFocus)
        {
            HitInfo = hitInfo;
            HitObject = isRegisteredForFocus
                ? focusDetails.Object
                : null; // If we're not actually registered for focus, we keep HitObject as null so we don't mislead anyone.

            if (focusDetails.Object != null)
            {
                lastHitDistance = (focusDetails.Point - Rays[0].Origin).magnitude;
                UpdateHitPosition();
                HitNormal = focusDetails.Normal;
            }
        }

        private void UpdateHitPosition()
        {
            HitPosition = (Rays[0].Origin + (lastHitDistance * Rays[0].Direction));
        }
    }
}
