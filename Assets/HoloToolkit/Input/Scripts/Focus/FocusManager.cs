// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Focus manager handles the focused objects per input source.
    /// <remarks>There are convenience properties for getting only Gaze Pointer if needed.</remarks>
    /// </summary>
    public class FocusManager : Singleton<FocusManager>, ISourceStateHandler
    {
        /// <summary>
        /// Maximum distance at which the pointer can collide with an object.
        /// </summary>
        [SerializeField]
        private static float pointingExtent = 10f;

        public static float GlobalPointingExtent { get { return pointingExtent; } }

        /// <summary>
        /// The LayerMasks, in prioritized order, that are used to determine the GazeTarget when raycasting.
        ///
        /// Example Usage:
        ///
        /// Allow the cursor to hit SR, but first prioritize any DefaultRaycastLayers (potentially behind SR)
        ///
        /// int sr = LayerMask.GetMask("SR");
        /// int nonSR = Physics.DefaultRaycastLayers &amp; ~sr;
        /// GazeManager.Instance.RaycastLayerMasks = new LayerMask[] { nonSR, sr };
        /// </summary>
        [SerializeField]
        [Tooltip("The LayerMasks, in prioritized order, that are used to determine the GazeTarget when raycasting.")]
        private LayerMask[] pointingRaycastLayerMasks = { Physics.DefaultRaycastLayers };

        [SerializeField]
        private bool debugDrawPointingRays = false;

        [SerializeField]
        private Color[] debugDrawPointingRayColors = null;

        /// <summary>
        /// GazeManager is a little special, so we keep track of it even if it's not a registered pointer. For the sake
        /// of StabilizationPlaneModifier and potentially other components that care where the user's looking, we need
        /// to do a gaze raycast even if gaze isn't used for focus.
        /// </summary>
        private PointerData gazeManagerPointingData;

        private readonly HashSet<PointerData> pointers = new HashSet<PointerData>();
        private readonly HashSet<GameObject> pendingOverallFocusEnterSet = new HashSet<GameObject>();
        private readonly HashSet<GameObject> pendingOverallFocusExitSet = new HashSet<GameObject>();
        private readonly List<PointerData> pendingPointerSpecificFocusChange = new List<PointerData>();

        /// <summary>
        /// Cached vector 3 reference to the new raycast position.
        /// <remarks>Only used to update UI raycast results.</remarks>
        /// </summary>
        private Vector3 newUiRaycastPosition = Vector3.zero;

        /// <summary>
        /// Camera to use for raycasting uGUI pointer events.
        /// </summary>
        [SerializeField]
        [Tooltip("Camera to use for raycasting uGUI pointer events.")]
        private Camera uiRaycastCamera;

        /// <summary>
        /// The Camera the Event System uses to raycast against.
        /// <para><remarks>Every uGUI canvas in your scene should use this camera as its event camera.</remarks></para>
        /// </summary>
        public Camera UIRaycastCamera
        {
            get
            {
                if (uiRaycastCamera == null)
                {
                    Debug.LogWarning("No UIRaycastCamera assigned! Falling back to the UIRaycastCamera.\n" +
                                     "It's highly recommended to use the UIRaycastCamera found on the EventSystem of this InputManager.");
                    uiRaycastCamera = GetComponentInChildren<Camera>();
                }

                return uiRaycastCamera;
            }
        }

        /// <summary>
        /// To tap on a hologram even when not focused on,
        /// set OverrideFocusedObject to desired game object.
        /// If it's null, then focused object will be used.
        /// </summary>
        public GameObject OverrideFocusedObject { get; set; }

        private class PointerData : PointerResult, IEquatable<PointerData>
        {
            public readonly IPointer Pointer;
            private FocusDetails focusDetails;

            private GraphicInputEventData graphicData;
            public GraphicInputEventData GraphicEventData
            {
                get
                {
                    if (graphicData == null)
                    {
                        graphicData = new GraphicInputEventData(EventSystem.current);
                    }

                    Debug.Assert(graphicData != null);

                    return graphicData;
                }
            }

            public PointerData(IPointer pointer)
            {
                Pointer = pointer;
            }

            public void UpdateHit(RaycastHit hit, RayStep sourceRay, int rayStepIndex)
            {
                PreviousPointerTarget = End.Object;
                RayStepIndex = rayStepIndex;
                StartPoint = sourceRay.Origin;

                focusDetails.LastRaycastHit = hit;
                focusDetails.Point = hit.point;
                focusDetails.Normal = hit.normal;
                focusDetails.Object = hit.transform.gameObject;
                End = focusDetails;
                CurrentPointerTarget = End.Object;
            }

            public void UpdateHit(RaycastResult result, RaycastHit hit, RayStep sourceRay, int rayStepIndex)
            {
                // We do not update the PreviousPointerTarget here because
                // it's already been updated in the first physics raycast.

                RayStepIndex = rayStepIndex;
                StartPoint = sourceRay.Origin;

                focusDetails.Point = hit.point;
                focusDetails.Normal = hit.normal;
                focusDetails.Object = result.gameObject;
                End = focusDetails;
            }

            public void UpdateHit()
            {
                PreviousPointerTarget = End.Object;

                RayStep firstStep = Pointer.Rays[0];
                RayStep finalStep = Pointer.Rays[Pointer.Rays.Length - 1];
                RayStepIndex = 0;

                StartPoint = firstStep.Origin;

                focusDetails.Point = finalStep.Terminus;
                focusDetails.Normal = -finalStep.Direction;
                focusDetails.Object = null;
                End = focusDetails;
                CurrentPointerTarget = End.Object;
            }

            public void ResetFocusedObjects(bool clearPreviousObject = true)
            {
                if (clearPreviousObject)
                {
                    PreviousPointerTarget = null;
                }

                focusDetails.Point = End.Point;
                focusDetails.Normal = End.Normal;
                focusDetails.Object = null;
                End = focusDetails;
                CurrentPointerTarget = null;
            }

            public bool Equals(PointerData other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Pointer.PointerId == other.Pointer.PointerId;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((PointerData)obj);
            }

            public override int GetHashCode()
            {
                return Pointer != null ? Pointer.GetHashCode() : 0;
            }
        }

        #region MonoBehaviour Implementation

        private void Start()
        {
            // Register the cursor as a global listener to get source events.
            InputManager.Instance.AddGlobalListener(gameObject);
        }

        private void Update()
        {
            UpdatePointers();
            UpdateFocusedObjects();
        }

        #endregion MonoBehaviour Implementation

        #region Focus Details by EventData

        /// <summary>
        /// Gets the currently focused object based on specified the event data.
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns>Currently focused <see cref="GameObject"/> for the events input source.</returns>
        public GameObject GetFocusedObject(BaseInputEventData eventData)
        {
            if (OverrideFocusedObject != null) { return OverrideFocusedObject; }

            FocusDetails focusDetails;
            if (!TryGetFocusDetails(eventData, out focusDetails)) { return null; }

            IPointer pointer;
            if (TryGetPointingSource(eventData, out pointer))
            {
                GraphicInputEventData graphicInputEventData = GetSpecificPointerGraphicEventData(pointer);
                Debug.Assert(graphicInputEventData != null);
                graphicInputEventData.selectedObject = focusDetails.Object;
            }

            return focusDetails.Object;
        }

        /// <summary>
        /// Try to get the focus details based on the specified event data.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="focusDetails"></param>
        /// <returns>True, if event data pointer input source is registered.</returns>
        public bool TryGetFocusDetails(BaseInputEventData eventData, out FocusDetails focusDetails)
        {
            foreach (var pointerData in pointers)
            {
                if (pointerData.Pointer.InputSourceParent.SourceId == eventData.SourceId)
                {
                    focusDetails = pointerData.End;
                    return true;
                }
            }

            focusDetails = default(FocusDetails);
            return false;
        }

        /// <summary>
        /// Try to get the registered pointer source that raised the event.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="pointer"></param>
        /// <returns>True, if event datas pointer input source is registered.</returns>
        public bool TryGetPointingSource(BaseInputEventData eventData, out IPointer pointer)
        {
            foreach (var pointerData in pointers)
            {
                if (pointerData.Pointer.InputSourceParent.SourceId == eventData.SourceId)
                {
                    pointer = pointerData.Pointer;
                    return true;
                }
            }

            pointer = null;
            return false;
        }

        #endregion Focus Details by EventData

        #region Focus Details by IPointer

        /// <summary>
        /// Gets the currently focused object for the pointing source.
        /// <para><remarks>If the pointing source is not registered, then the Gaze's Focused <see cref="GameObject"/> is returned.</remarks></para>
        /// </summary>
        /// <param name="pointingSource"></param>
        /// <returns>Currently Focused Object.</returns>
        public GameObject GetFocusedObject(IPointer pointingSource)
        {
            if (OverrideFocusedObject != null) { return OverrideFocusedObject; }

            FocusDetails focusDetails;
            if (!TryGetFocusDetails(pointingSource, out focusDetails)) { return null; }

            GraphicInputEventData graphicInputEventData = GetSpecificPointerGraphicEventData(pointingSource);
            Debug.Assert(graphicInputEventData != null);
            graphicInputEventData.selectedObject = focusDetails.Object;

            return focusDetails.Object;
        }

        /// <summary>
        /// Gets the currently focused object for the pointing source.
        /// <para><remarks>If the pointing source is not registered, then the Gaze's <see cref="FocusDetails"/> is returned.</remarks></para>
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="focusDetails"></param>
        public bool TryGetFocusDetails(IPointer pointer, out FocusDetails focusDetails)
        {
            foreach (var pointerData in pointers)
            {
                if (pointerData.Pointer.PointerId == pointer.PointerId)
                {
                    focusDetails = pointerData.End;
                    return true;
                }
            }

            focusDetails = default(FocusDetails);
            return false;
        }

        /// <summary>
        /// Get the Graphic Event Data for the specified pointing source.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public GraphicInputEventData GetSpecificPointerGraphicEventData(IPointer pointer)
        {
            var pointerData = GetPointerData(pointer);
            return pointerData == null ? null : pointerData.GraphicEventData;
        }

        #endregion Focus Details by IPointer

        #region Utilities

        public static uint GenerateNewPointerId()
        {
            var newId = (uint)UnityEngine.Random.Range(1, int.MaxValue);

            foreach (var pointerData in Instance.pointers)
            {
                if (pointerData.Pointer.PointerId == newId)
                {
                    return GenerateNewPointerId();
                }
            }

            return newId;
        }

        /// <summary>
        /// Helper for assigning world space canvases event cameras.
        /// <remarks>Can be used at runtime.</remarks>
        /// </summary>
        public void UpdateCanvasEventSystems()
        {
            Debug.Assert(UIRaycastCamera != null, "You must assign a UIRaycastCamera on the FocusManager before updating your canvases.");

            // This will also find disabled GameObjects in the scene.
            var sceneCanvases = Resources.FindObjectsOfTypeAll<Canvas>();

            for (var i = 0; i < sceneCanvases.Length; i++)
            {
                if (sceneCanvases[i].isRootCanvas && sceneCanvases[i].renderMode == RenderMode.WorldSpace)
                {
                    sceneCanvases[i].worldCamera = UIRaycastCamera;
                }
            }
        }

        /// <summary>
        /// Returns the registered PointerData for the provided pointing input source.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns>Pointer Data if the pointing source is registered.</returns>
        private PointerData GetPointerData(IPointer pointer)
        {
            foreach (var pointerData in pointers)
            {
                if (pointerData.Pointer.PointerId == pointer.PointerId)
                {
                    return pointerData;
                }
            }

            return null;
        }

        private void UpdatePointers()
        {
            int pointerCount = 0;

            foreach (var pointer in pointers)
            {
                UpdatePointer(pointer);

                if (debugDrawPointingRays)
                {
                    Color rayColor;

                    if ((debugDrawPointingRayColors != null) && (debugDrawPointingRayColors.Length > 0))
                    {
                        rayColor = debugDrawPointingRayColors[pointerCount++ % debugDrawPointingRayColors.Length];
                    }
                    else
                    {
                        rayColor = Color.green;
                    }

                    Debug.DrawRay(pointer.StartPoint, (pointer.End.Point - pointer.StartPoint), rayColor);
                }
            }
        }

        private void UpdatePointer(PointerData pointer)
        {
            // Call the pointer's OnPreRaycast function
            // This will give it a chance to prepare itself for raycasts
            // eg, by building its Rays array
            pointer.Pointer.OnPreRaycast();

            // If pointer interaction isn't enabled, clear its result object and return
            if (!pointer.Pointer.InteractionEnabled)
            {
                // Don't clear the previous focused object since we still want to trigger FocusExit events
                pointer.ResetFocusedObjects(false);
            }
            else
            {
                // If the pointer is locked
                // Keep the focus objects the same
                // This will ensure that we execute events on those objects
                // even if the pointer isn't pointing at them
                if (!pointer.Pointer.FocusLocked)
                {
                    // Otherwise, continue
                    var prioritizedLayerMasks = (pointer.Pointer.PrioritizedLayerMasksOverride ?? pointingRaycastLayerMasks);

                    // Perform raycast to determine focused object
                    RaycastPhysics(pointer, prioritizedLayerMasks);

                    // If we have a unity event system, perform graphics raycasts as well to support Unity UI interactions
                    if (EventSystem.current != null)
                    {
                        // NOTE: We need to do this AFTER RaycastPhysics so we use the current hit point to perform the correct 2D UI Raycast.
                        RaycastGraphics(pointer, prioritizedLayerMasks);
                    }

                    // Set the pointer's result last
                    pointer.Pointer.Result = pointer;
                }
            }

            // Call the pointer's OnPostRaycast function
            // This will give it a chance to respond to raycast results
            // eg by updating its appearance
            pointer.Pointer.OnPostRaycast();
        }

        /// <summary>
        /// Perform a Unity physics Raycast to determine which scene objects with a collider is currently being gazed at, if any.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="prioritizedLayerMasks"></param>
        private static void RaycastPhysics(PointerData pointer, LayerMask[] prioritizedLayerMasks)
        {
            bool isHit = false;
            int rayStepIndex = 0;
            RayStep rayStep = default(RayStep);
            RaycastHit physicsHit = default(RaycastHit);

            Debug.Assert(pointer.Pointer.Rays != null, "No valid rays for pointer");
            Debug.Assert(pointer.Pointer.Rays.Length > 0, "No valid rays for pointer");

            // Check raycast for each step in the pointing source
            for (int i = 0; i < pointer.Pointer.Rays.Length; i++)
            {
                if (RaycastPhysicsStep(pointer.Pointer.Rays[i], prioritizedLayerMasks, out physicsHit))
                {
                    // Set the pointer source's origin ray to this step
                    isHit = true;
                    rayStep = pointer.Pointer.Rays[i];
                    rayStepIndex = i;
                    // No need to continue once we've hit something
                    break;
                }
            }

            if (isHit)
            {
                pointer.UpdateHit(physicsHit, rayStep, rayStepIndex);
            }
            else
            {
                pointer.UpdateHit();
            }
        }

        /// <summary>
        /// Raycasts each physics <see cref="RayStep"/>
        /// </summary>
        /// <param name="step"></param>
        /// <param name="prioritizedLayerMasks"></param>
        /// <param name="physicsHit"></param>
        /// <returns></returns>
        private static bool RaycastPhysicsStep(RayStep step, LayerMask[] prioritizedLayerMasks, out RaycastHit physicsHit)
        {
            return prioritizedLayerMasks.Length == 1
                // If there is only one priority, don't prioritize
                ? Physics.Raycast(step.Origin, step.Direction, out physicsHit, step.Length, prioritizedLayerMasks[0])
                // Raycast across all layers and prioritize
                : TryGetPrioritizedHit(Physics.RaycastAll(step.Origin, step.Direction, step.Length, Physics.AllLayers), prioritizedLayerMasks, out physicsHit);
        }

        /// <summary>
        /// Tries to ge the prioritized raycast hit based on the prioritized layer masks.
        /// <para><remarks>Sorts all hit objects first by layerMask, then by distance.</remarks></para>
        /// </summary>
        /// <param name="hits"></param>
        /// <param name="priorityLayers"></param>
        /// <param name="raycastHit"></param>
        /// <returns>The minimum distance hit within the first layer that has hits</returns>
        private static bool TryGetPrioritizedHit(RaycastHit[] hits, LayerMask[] priorityLayers, out RaycastHit raycastHit)
        {
            raycastHit = default(RaycastHit);

            if (hits.Length == 0)
            {
                return false;
            }

            for (int layerMaskIdx = 0; layerMaskIdx < priorityLayers.Length; layerMaskIdx++)
            {
                RaycastHit? minHit = null;

                for (int hitIdx = 0; hitIdx < hits.Length; hitIdx++)
                {
                    RaycastHit hit = hits[hitIdx];
                    if (hit.transform.gameObject.layer.IsInLayerMask(priorityLayers[layerMaskIdx]) &&
                        (minHit == null || hit.distance < minHit.Value.distance))
                    {
                        minHit = hit;
                    }
                }

                if (minHit != null)
                {
                    raycastHit = minHit.Value;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Perform a Unity Graphics Raycast to determine which uGUI element is currently being gazed at, if any.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="prioritizedLayerMasks"></param>
        private void RaycastGraphics(PointerData pointer, LayerMask[] prioritizedLayerMasks)
        {
            Debug.Assert(pointer.End.Point != Vector3.zero, "No pointer source end point found to raycast against!");
            Debug.Assert(UIRaycastCamera != null, "You must assign a UIRaycastCamera on the FocusManager before you can process uGUI raycasting.");

            RaycastResult raycastResult = default(RaycastResult);
            bool overridePhysicsRaycast = false;
            RayStep rayStep = default(RayStep);
            int rayStepIndex = 0;

            Debug.Assert(pointer.Pointer.Rays != null, "No valid rays for pointer");
            Debug.Assert(pointer.Pointer.Rays.Length > 0, "No valid rays for pointer");

            // Cast rays for every step until we score a hit
            for (int i = 0; i < pointer.Pointer.Rays.Length; i++)
            {
                if (RaycastGraphicsStep(pointer, pointer.Pointer.Rays[i], prioritizedLayerMasks, out overridePhysicsRaycast, out raycastResult))
                {
                    rayStepIndex = i;
                    rayStep = pointer.Pointer.Rays[i];
                    break;
                }
            }

            // Check if we need to overwrite the physics raycast info
            if ((pointer.CurrentPointerTarget == null || overridePhysicsRaycast) && raycastResult.isValid &&
                 raycastResult.module != null && raycastResult.module.eventCamera == UIRaycastCamera)
            {
                newUiRaycastPosition.x = raycastResult.screenPosition.x;
                newUiRaycastPosition.y = raycastResult.screenPosition.y;
                newUiRaycastPosition.z = raycastResult.distance;

                Vector3 worldPos = UIRaycastCamera.ScreenToWorldPoint(newUiRaycastPosition);

                var hitInfo = new RaycastHit
                {
                    point = worldPos,
                    normal = -raycastResult.gameObject.transform.forward
                };

                pointer.UpdateHit(raycastResult, hitInfo, rayStep, rayStepIndex);
            }
        }

        private bool RaycastGraphicsStep(PointerData pointer, RayStep step, LayerMask[] prioritizedLayerMasks, out bool overridePhysicsRaycast, out RaycastResult uiRaycastResult)
        {
            // Move the uiRaycast camera to the current pointer's position.
            UIRaycastCamera.transform.position = step.Origin;
            UIRaycastCamera.transform.forward = step.Direction;

            // We always raycast from the center of the camera.
            pointer.GraphicEventData.position = new Vector2(UIRaycastCamera.pixelWidth * 0.5f, UIRaycastCamera.pixelHeight * 0.5f);

            // Graphics raycast
            uiRaycastResult = EventSystem.current.Raycast(pointer.GraphicEventData, prioritizedLayerMasks);
            pointer.GraphicEventData.pointerCurrentRaycast = uiRaycastResult;

            overridePhysicsRaycast = false;

            // If we have a raycast result, check if we need to overwrite the physics raycast info
            if (uiRaycastResult.gameObject != null)
            {
                if (pointer.CurrentPointerTarget != null)
                {
                    // Check layer prioritization
                    if (prioritizedLayerMasks.Length > 1)
                    {
                        // Get the index in the prioritized layer masks
                        int uiLayerIndex = uiRaycastResult.gameObject.layer.FindLayerListIndex(prioritizedLayerMasks);
                        int threeDLayerIndex = pointer.End.LastRaycastHit.collider.gameObject.layer.FindLayerListIndex(prioritizedLayerMasks);

                        if (threeDLayerIndex > uiLayerIndex)
                        {
                            overridePhysicsRaycast = true;
                        }
                        else if (threeDLayerIndex == uiLayerIndex)
                        {
                            if (pointer.End.LastRaycastHit.distance > uiRaycastResult.distance)
                            {
                                overridePhysicsRaycast = true;
                            }
                        }
                    }
                    else
                    {
                        if (pointer.End.LastRaycastHit.distance > uiRaycastResult.distance)
                        {
                            overridePhysicsRaycast = true;
                        }
                    }
                }
                // If we've hit something, no need to go further
                return true;
            }
            // If we haven't hit something, keep going
            return false;
        }

        /// <summary>
        /// Raises the Focus Events to the Input Manger if needed.
        /// </summary>
        private void UpdateFocusedObjects()
        {
            Debug.Assert(pendingPointerSpecificFocusChange.Count == 0);
            Debug.Assert(pendingOverallFocusExitSet.Count == 0);
            Debug.Assert(pendingOverallFocusEnterSet.Count == 0);

            // NOTE: We compute the set of events to send before sending the first event
            //       just in case someone responds to the event by adding/removing a
            //       pointer which would change the structures we're iterating over.

            foreach (var pointer in pointers)
            {
                if (pointer.PreviousPointerTarget != pointer.CurrentPointerTarget)
                {
                    pendingPointerSpecificFocusChange.Add(pointer);

                    // Initially, we assume all pointer-specific focus changes will
                    // also result in an overall focus change...

                    if (pointer.PreviousPointerTarget != null)
                    {
                        pendingOverallFocusExitSet.Add(pointer.PreviousPointerTarget);
                    }

                    if (pointer.CurrentPointerTarget != null)
                    {
                        pendingOverallFocusEnterSet.Add(pointer.CurrentPointerTarget);
                    }
                }
            }

            // ... but now we trim out objects whose overall focus was maintained the same by a different pointer:

            foreach (var pointer in pointers)
            {
                pendingOverallFocusExitSet.Remove(pointer.CurrentPointerTarget);

                pendingOverallFocusEnterSet.Remove(pointer.PreviousPointerTarget);
            }

            // Now we raise the events:
            for (int iChange = 0; iChange < pendingPointerSpecificFocusChange.Count; iChange++)
            {
                PointerData change = pendingPointerSpecificFocusChange[iChange];
                GameObject pendingUnfocusObject = change.PreviousPointerTarget;
                GameObject pendingFocusObject = change.CurrentPointerTarget;

                InputManager.Instance.RaisePreFocusChangedEvent(change.Pointer, pendingUnfocusObject, pendingFocusObject);

                if (pendingOverallFocusExitSet.Contains(pendingUnfocusObject))
                {
                    InputManager.Instance.RaiseFocusExit(change.Pointer, pendingUnfocusObject);
                    pendingOverallFocusExitSet.Remove(pendingUnfocusObject);
                }

                if (pendingOverallFocusEnterSet.Contains(pendingFocusObject))
                {
                    InputManager.Instance.RaiseFocusEnter(change.Pointer, pendingFocusObject);
                    pendingOverallFocusEnterSet.Remove(pendingFocusObject);
                }

                InputManager.Instance.OnFocusChangedEvent(change.Pointer, pendingUnfocusObject, pendingFocusObject);
            }

            Debug.Assert(pendingOverallFocusExitSet.Count == 0);
            Debug.Assert(pendingOverallFocusEnterSet.Count == 0);
            pendingPointerSpecificFocusChange.Clear();
        }

        #endregion Accessors

        #region ISourceState Implementation

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            foreach (var sourcePointer in eventData.InputSource.Pointers)
            {
                PointerData pointerData = GetPointerData(sourcePointer);

                Debug.Assert(pointerData == null, "This pointing source is already registered!");

                // Special Registration for Gaze
                if (eventData.InputSource.SourceId == GazeManager.Instance.SourceId)
                {
                    if (gazeManagerPointingData == null)
                    {
                        if (GazeManager.IsInitialized)
                        {
                            gazeManagerPointingData = new PointerData(sourcePointer);
                        }
                    }
                    else
                    {
                        Debug.Assert(gazeManagerPointingData.Pointer.PointerId == GazeManager.Instance.Pointers[0].PointerId);
                        gazeManagerPointingData.ResetFocusedObjects();
                    }

                    Debug.Assert(gazeManagerPointingData != null);
                    pointerData = gazeManagerPointingData;
                }
                else
                {
                    pointerData = new PointerData(sourcePointer);
                }

                pointers.Add(pointerData);
            }
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            foreach (var sourcePointer in eventData.InputSource.Pointers)
            {
                PointerData pointerData = GetPointerData(sourcePointer);
                Debug.Assert(pointerData != null, "Pointing Source was never registered!");

                // Raise focus events if needed
                if (pointerData.CurrentPointerTarget != null)
                {
                    GameObject unfocusedObject = pointerData.CurrentPointerTarget;
                    bool objectIsStillFocusedByOtherPointer = false;

                    foreach (var otherPointer in pointers)
                    {
                        if (otherPointer.CurrentPointerTarget == unfocusedObject)
                        {
                            objectIsStillFocusedByOtherPointer = true;
                            break;
                        }
                    }

                    if (!objectIsStillFocusedByOtherPointer)
                    {
                        InputManager.Instance.RaiseFocusExit(sourcePointer, unfocusedObject);
                    }

                    InputManager.Instance.RaisePreFocusChangedEvent(sourcePointer, unfocusedObject, null);
                }

                pointers.Remove(pointerData);
            }
        }

        public void OnSourcePositionChanged(SourcePositionEventData eventData) { }

        public void OnSourceRotationChanged(SourceRotationEventData eventData) { }

        #endregion ISourceState Implementation
    }
}
