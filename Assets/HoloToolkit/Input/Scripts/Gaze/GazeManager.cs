// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// The gaze manager manages everything related to a gaze ray that can interact with other objects.
    /// </summary>
    public class GazeManager : Singleton<GazeManager>, IPointingSource
    {
        [Obsolete]
        public delegate void FocusedChangedDelegate(GameObject previousObject, GameObject newObject);

        /// <summary>
        /// Indicates whether the user is currently gazing at an object.
        /// </summary>
        [Obsolete]
        public bool IsGazingAtObject { get; private set; }

        /// <summary>
        /// HitInfo property gives access to information at the object being gazed at, if any.
        /// </summary>
        [Obsolete]
        public RaycastHit HitInfo { get { return hitInfo; } }
        private RaycastHit hitInfo;

        /// <summary>
        /// Dispatched when focus shifts to a new object, or focus on current object
        /// is lost.
        /// </summary>
        [Obsolete]
        public event FocusedChangedDelegate FocusedObjectChanged;

        /// <summary>
        /// Unity UI pointer event.  This will be null if the EventSystem is not defined in the scene.
        /// </summary>
        [Obsolete]
        public PointerEventData UnityUIPointerEvent { get; private set; }

        /// <summary>
        /// Cached results of raycast results.
        /// </summary>
        [Obsolete]
        private List<RaycastResult> raycastResultList = new List<RaycastResult>();



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
        public LayerMask[] RaycastLayerMasks = new LayerMask[] { Physics.DefaultRaycastLayers };

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

        public IList<LayerMask> PrioritizedLayerMasksOverride
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

            UpdateGazeInfo();

            // Perform raycast to determine gazed object
            GameObject previousFocusObject = RaycastPhysics();

            // If we have a unity event system, perform graphics raycasts as well to support Unity UI interactions
            if (EventSystem.current != null)
            {
                // NOTE: We need to do this AFTER we set the HitPosition and HitObject since we need to use HitPosition to perform the correct 2D UI Raycast.
                RaycastUnityUI();
            }

            // Dispatch changed event if focus is different
            if (previousFocusObject != HitObject && FocusedObjectChanged != null)
            {
                FocusedObjectChanged(previousFocusObject, HitObject);
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
        /// <param name="focusDetails">Details of the current hit (focus).</param>
        /// <param name="isRegisteredForFocus">Whether or not this gaze manager is registered as a focus pointer.</param>
        public void UpdateHitDetails(FocusDetails focusDetails, bool isRegisteredForFocus)
        {
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

        /// <summary>
        /// Perform a Unity physics Raycast to determine which scene objects with a collider is currently being gazed at, if any.
        /// </summary>
        [Obsolete]
        private GameObject RaycastPhysics()
        {
            GameObject previousFocusObject = HitObject;

            // If there is only one priority, don't prioritize
            if (RaycastLayerMasks.Length == 1)
            {
                IsGazingAtObject = Physics.Raycast(GazeOrigin, GazeNormal, out hitInfo, MaxGazeCollisionDistance, RaycastLayerMasks[0]);
            }
            else
            {
                // Raycast across all layers and prioritize
                RaycastHit? hit = PrioritizeHits(Physics.RaycastAll(new Ray(GazeOrigin, GazeNormal), MaxGazeCollisionDistance, -1));

                IsGazingAtObject = hit.HasValue;
                if (IsGazingAtObject)
                {
                    hitInfo = hit.Value;
                }
            }

            if (IsGazingAtObject)
            {
                HitObject = HitInfo.collider.gameObject;
                HitPosition = HitInfo.point;
                lastHitDistance = HitInfo.distance;
            }
            else
            {
                HitObject = null;
                HitPosition = GazeOrigin + (GazeNormal * lastHitDistance);
            }
            return previousFocusObject;
        }

        /// <summary>
        /// Perform a Unity UI Raycast, compare with the latest 3D raycast, and overwrite the hit object info if the UI gets focus
        /// </summary>
        [Obsolete]
        private void RaycastUnityUI()
        {
            if (UnityUIPointerEvent == null)
            {
                UnityUIPointerEvent = new PointerEventData(EventSystem.current);
            }

            Camera mainCamera = CameraCache.Main;

            // 2D cursor position
            Vector2 cursorScreenPos = mainCamera.WorldToScreenPoint(HitPosition);
            UnityUIPointerEvent.delta = cursorScreenPos - UnityUIPointerEvent.position;
            UnityUIPointerEvent.position = cursorScreenPos;

            // Graphics raycast
            raycastResultList.Clear();
            EventSystem.current.RaycastAll(UnityUIPointerEvent, raycastResultList);
            RaycastResult uiRaycastResult = FindClosestRaycastHitInLayerMasks(raycastResultList, RaycastLayerMasks);
            UnityUIPointerEvent.pointerCurrentRaycast = uiRaycastResult;

            // If we have a raycast result, check if we need to overwrite the 3D raycast info
            if (uiRaycastResult.gameObject != null)
            {
                bool superseded3DObject = false;
                if (IsGazingAtObject)
                {
                    // Check layer prioritization
                    if (RaycastLayerMasks.Length > 1)
                    {
                        // Get the index in the prioritized layer masks
                        int uiLayerIndex = FindLayerListIndex(uiRaycastResult.gameObject.layer, RaycastLayerMasks);
                        int threeDLayerIndex = FindLayerListIndex(hitInfo.collider.gameObject.layer, RaycastLayerMasks);

                        if (threeDLayerIndex > uiLayerIndex)
                        {
                            superseded3DObject = true;
                        }
                        else if (threeDLayerIndex == uiLayerIndex)
                        {
                            if (hitInfo.distance > uiRaycastResult.distance)
                            {
                                superseded3DObject = true;
                            }
                        }
                    }
                    else
                    {
                        if (hitInfo.distance > uiRaycastResult.distance)
                        {
                            superseded3DObject = true;
                        }
                    }
                }

                // Check if we need to overwrite the 3D raycast info
                if (!IsGazingAtObject || superseded3DObject)
                {
                    IsGazingAtObject = true;
                    Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(uiRaycastResult.screenPosition.x, uiRaycastResult.screenPosition.y, uiRaycastResult.distance));
                    hitInfo = new RaycastHit
                    {
                        distance = uiRaycastResult.distance,
                        normal = -uiRaycastResult.gameObject.transform.forward,
                        point = worldPos
                    };

                    HitObject = uiRaycastResult.gameObject;
                    HitPosition = HitInfo.point;
                    lastHitDistance = HitInfo.distance;
                }
            }
        }

        #region Helpers

        /// <summary>
        /// Find the closest raycast hit in the list of RaycastResults that is also included in the LayerMask list.
        /// </summary>
        /// <param name="candidates">List of RaycastResults from a Unity UI raycast</param>
        /// <param name="layerMaskList">List of layers to support</param>
        /// <returns>RaycastResult if hit, or an empty RaycastResult if nothing was hit</returns>
        [Obsolete]
        private RaycastResult FindClosestRaycastHitInLayerMasks(List<RaycastResult> candidates, LayerMask[] layerMaskList)
        {
            int combinedLayerMask = 0;
            for (int i = 0; i < layerMaskList.Length; i++)
            {
                combinedLayerMask = combinedLayerMask | layerMaskList[i].value;
            }

            RaycastResult? minHit = null;
            for (var i = 0; i < candidates.Count; ++i)
            {
                if (candidates[i].gameObject == null || !IsLayerInLayerMask(candidates[i].gameObject.layer, combinedLayerMask))
                {
                    continue;
                }
                if (minHit == null || candidates[i].distance < minHit.Value.distance)
                {
                    minHit = candidates[i];
                }
            }

            return minHit ?? new RaycastResult();
        }

        /// <summary>
        /// Look through the layerMaskList and find the index in that list for which the supplied layer is part of
        /// </summary>
        /// <param name="layer">Layer to search for</param>
        /// <param name="layerMaskList">List of LayerMasks to search</param>
        /// <returns>LayerMaskList index, or -1 for not found</returns>
        [Obsolete]
        private int FindLayerListIndex(int layer, LayerMask[] layerMaskList)
        {
            for (int i = 0; i < layerMaskList.Length; i++)
            {
                if (IsLayerInLayerMask(layer, layerMaskList[i].value))
                {
                    return i;
                }
            }

            return -1;
        }

        [Obsolete]
        private bool IsLayerInLayerMask(int layer, int layerMask)
        {
            return ((1 << layer) & layerMask) != 0;
        }

        [Obsolete]
        private RaycastHit? PrioritizeHits(RaycastHit[] hits)
        {
            if (hits.Length == 0)
            {
                return null;
            }

            // Return the minimum distance hit within the first layer that has hits.
            // In other words, sort all hit objects first by layerMask, then by distance.
            for (int layerMaskIdx = 0; layerMaskIdx < RaycastLayerMasks.Length; layerMaskIdx++)
            {
                RaycastHit? minHit = null;

                for (int hitIdx = 0; hitIdx < hits.Length; hitIdx++)
                {
                    RaycastHit hit = hits[hitIdx];
                    if (IsLayerInLayerMask(hit.transform.gameObject.layer, RaycastLayerMasks[layerMaskIdx]) &&
                        (minHit == null || hit.distance < minHit.Value.distance))
                    {
                        minHit = hit;
                    }
                }

                if (minHit != null)
                {
                    return minHit;
                }
            }

            return null;
        }

        #endregion Helpers
    }
}