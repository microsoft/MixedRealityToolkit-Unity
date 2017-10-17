// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// The gaze manager manages everything related to a gaze ray that can interact with other objects.
    /// </summary>
    public class GazeManager : Singleton<GazeManager>, IPointingSource
    {
        [Obsolete("Use FocusManager.PointerSpecificFocusChangedMethod")]
        public delegate void FocusedChangedDelegate(GameObject previousObject, GameObject newObject);

        /// <summary>
        /// Indicates whether the user is currently gazing at an object.
        /// </summary>
        [Obsolete("Use FocusManager.TryGetFocusDetails")]
        public bool IsGazingAtObject { get; private set; }

        /// <summary>
        /// Dispatched when focus shifts to a new object, or focus on current object
        /// is lost.
        /// </summary>
        [Obsolete("Use FocusManager.PointerSpecificFocusChanged")]
#pragma warning disable 618
#pragma warning disable 67
        public event FocusedChangedDelegate FocusedObjectChanged;
#pragma warning restore 67
#pragma warning restore 618

        /// <summary>
        /// Unity UI pointer event.  This will be null if the EventSystem is not defined in the scene.
        /// </summary>
        [Obsolete("Use FocusManager.UnityUIPointerEvent")]
        public PointerEventData UnityUIPointerEvent { get; private set; }

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
        public Vector3 GazeOrigin
        {
            get { return Ray.origin; }
        }

        /// <summary>
        /// Normal of the gaze.
        /// </summary>
        public Vector3 GazeNormal
        {
            get { return Ray.direction; }
        }

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
        public BaseRayStabilizer Stabilizer = null;

        /// <summary>
        /// Transform that should be used as the source of the gaze position and rotation.
        /// Defaults to the main camera.
        /// </summary>
        [Tooltip("Transform that should be used to represent the gaze position and rotation. Defaults to CameraCache.Main")]
        public Transform GazeTransform;

        [Tooltip("True to draw a debug view of the ray.")]
        public bool DebugDrawRay;

        public Ray Ray { get; private set; }

        public float? ExtentOverride
        {
            get { return MaxGazeCollisionDistance; }
        }

        public LayerMask[] PrioritizedLayerMasksOverride
        {
            get { return RaycastLayerMasks; }
        }

        private float lastHitDistance = 2.0f;

        protected override void Awake()
        {
            base.Awake();

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
                Ray = default(Ray);
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

                Ray = new Ray(newGazeOrigin, newGazeNormal);
            }

            UpdateHitPosition();
        }

        public void UpdatePointer()
        {
            UpdateGazeInfo();
        }

        public bool OwnsInput(BaseEventData eventData)
        {
            // NOTE: This is a simple pointer and not meant to be used simultaneously with others.
            return true;
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
                lastHitDistance = (focusDetails.Point - Ray.origin).magnitude;
                UpdateHitPosition();
                HitNormal = focusDetails.Normal;
            }
        }

        private void UpdateHitPosition()
        {
            HitPosition = (Ray.origin + (lastHitDistance * Ray.direction));
        }
    }
}
