// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using MixedRealityToolkit.Common.Extensions;
using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.Gaze;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.InputModule.Focus
{
    /// <summary>
    /// Focus manager is the bridge that handles different types of pointing sources like gaze cursor
    /// or pointing ray enabled motion controllers.
    /// If you don't have pointing ray enabled controllers, it defaults to GazeManager.
    /// </summary>
    public class FocusManager : Singleton<FocusManager>
    {
        #region MonoBehaviour Implementation

        protected override void InitializeInternal()
        {
            if (registeredPointers != null)
            {
                for (int iPointer = 0; iPointer < registeredPointers.Length; iPointer++)
                {
                    GameObject owner = registeredPointers[iPointer];

                    if (owner == null)
                    {
                        Debug.LogError("AutoRegisteredPointers contains a null (\"None\") object.");
                        break;
                    }

                    IPointingSource pointingSource = owner.GetComponent<IPointingSource>();

                    if (pointingSource == null)
                    {
                        Debug.LogErrorFormat("AutoRegisteredPointers contains object \"{0}\" which is missing its {1} component.",
                            owner.name,
                            typeof(IPointingSource).Name
                        );
                        break;
                    }

                    RegisterPointer(pointingSource);
                }
            }

            if (pointers.Count == 0 && autoRegisterGazePointerIfNoPointersRegistered && GazeManager.ConfirmInitialized())
            {
                RegisterPointer(GazeManager.Instance);
            }
        }

        private void Update()
        {
            UpdatePointers();
            UpdateFocusedObjects();
        }

        #endregion

        #region Settings
        /// <summary>
        /// Maximum distance at which the pointer can collide with an object.
        /// </summary>
        [SerializeField]
        private float pointingExtent = 10f;

        /// <summary>
        /// The LayerMasks, in prioritized order, that are used to determine the HitObject when raycasting.
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
        [Tooltip("The LayerMasks, in prioritized order, that are used to determine the HitObject when raycasting.")]
        private LayerMask[] pointingRaycastLayerMasks = { Physics.DefaultRaycastLayers };

        [SerializeField]
        private GameObject[] registeredPointers = null;

        [SerializeField]
        private bool autoRegisterGazePointerIfNoPointersRegistered = true;

        [SerializeField]
        private bool debugDrawPointingRays = false;

        [SerializeField]
        private Color[] debugDrawPointingRayColors = null;

        #endregion

        #region Data
        private class PointerData : PointerResult
        {
            public readonly IPointingSource PointingSource;

            private PointerInputEventData pointerData;
            public PointerInputEventData UnityUIPointerData
            {
                get
                {
                    if (pointerData == null)
                    {
                        pointerData = new PointerInputEventData(EventSystem.current);
                    }

                    return pointerData;
                }
            }

            public PointerData(IPointingSource pointingSource)
            {
                PointingSource = pointingSource;
            }

            [Obsolete("Use UpdateHit(RaycastHit hit, RayStep sourceRay, int rayStepIndex) or UpdateHit (float extent)")]
            public void UpdateHit(RaycastHit hit)
            {
                throw new NotImplementedException();
            }

            public void UpdateHit(RaycastHit hit, RayStep sourceRay, int rayStepIndex)
            {
                LastRaycastHit = hit;
                PreviousEndObject = End.Object;
                RayStepIndex = rayStepIndex;

                StartPoint = sourceRay.Origin;
                End = new FocusDetails
                {
                    Point = hit.point,
                    Normal = hit.normal,
                    Object = hit.transform.gameObject
                };
            }

            public void UpdateHit(RaycastResult result, RaycastHit hit, RayStep sourceRay, int rayStepIndex)
            {
                // We do not update the PreviousEndObject here because
                // it's already been updated in the first physics raycast.

                RayStepIndex = rayStepIndex;
                StartPoint = sourceRay.Origin;
                End = new FocusDetails
                {
                    Point = hit.point,
                    Normal = hit.normal,
                    Object = result.gameObject
                };
            }

            public void UpdateHit(float extent)
            {
                PreviousEndObject = End.Object;

                RayStep firstStep = PointingSource.Rays[0];
                RayStep finalStep = PointingSource.Rays[PointingSource.Rays.Length - 1];
                RayStepIndex = 0;

                StartPoint = firstStep.Origin;
                End = new FocusDetails
                {
                    Point = finalStep.Terminus,
                    Normal = (-finalStep.Direction),
                    Object = null
                };
            }

            public void ResetFocusedObjects(bool clearPreviousObject = true)
            {
                if (clearPreviousObject)
                {
                    PreviousEndObject = null;
                }

                End = new FocusDetails
                {
                    Point = End.Point,
                    Normal = End.Normal,
                    Object = null
                };
            }
        }

        private readonly List<PointerData> pointers = new List<PointerData>(0);

        /// <summary>
        /// GazeManager is a little special, so we keep track of it even if it's not a registered pointer. For the sake
        /// of StabilizationPlaneModifier and potentially other components that care where the user's looking, we need
        /// to do a gaze raycast even if gaze isn't used for focus.
        /// </summary>
        private PointerData gazeManagerPointingData;

        [Obsolete("Use GetGazePointerEventData or GetSpecificPointerEventData")]
        public PointerInputEventData UnityUIPointerEvent { get; private set; }

        private readonly HashSet<GameObject> pendingOverallFocusEnterSet = new HashSet<GameObject>();
        private readonly HashSet<GameObject> pendingOverallFocusExitSet = new HashSet<GameObject>();
        private readonly List<PointerData> pendingPointerSpecificFocusChange = new List<PointerData>();

        /// <summary>
        /// Cached vector 3 reference to the new raycast position.
        /// <remarks>Only used to update UI raycast results.</remarks>
        /// </summary>
        private Vector3 newUiRaycastPosition = Vector3.zero;

        /// <summary>
        /// Private uiRaycastCamera used primarily for UI pointer data.
        /// </summary>
        [SerializeField]
        private Camera uiRaycastCamera;

        /// <summary>
        /// The Camera the Event System uses to raycast against.
        /// <remarks>Every uGUI canvas in your scene should use this camera as its event camera.</remarks>
        /// </summary>
        public Camera UIRaycastCamera
        {
            get
            {
                if (uiRaycastCamera == null)
                {
                    Debug.LogWarning("No UIRaycastCamera assigned! Falling back to the RaycastCamera.\n" +
                                     "It's highly recommended to use the RaycastCamera found on the EventSystem of this InputManager.");
                    uiRaycastCamera = GetComponentInChildren<Camera>();
                }

                return uiRaycastCamera;
            }
        }

        #endregion

        #region Accessors

        public void RegisterPointer(IPointingSource pointingSource)
        {
            Debug.Assert(pointingSource != null, "Can't register a pointer if you give us one.");

            int pointerIndex;
            PointerData pointer;

            if (TryGetPointerIndex(pointingSource, out pointerIndex))
            {
                // This pointing source is already registered and active.
                return;
            }

            if (pointingSource is GazeManager)
            {
                if (gazeManagerPointingData == null)
                {
                    if (GazeManager.IsInitialized)
                    {
                        gazeManagerPointingData = new PointerData(GazeManager.Instance);
                    }
                }
                else
                {
                    Debug.Assert(ReferenceEquals(gazeManagerPointingData.PointingSource, GazeManager.Instance));
                    gazeManagerPointingData.ResetFocusedObjects();
                }

                Debug.Assert(gazeManagerPointingData != null);
                pointer = gazeManagerPointingData;
            }
            else
            {
                pointer = new PointerData(pointingSource);
            }

            pointers.Add(pointer);
        }

        public void UnregisterPointer(IPointingSource pointingSource)
        {
            Debug.Assert(pointingSource != null, "Can't unregister a pointer if you give us one.");

            int pointerIndex;
            TryGetPointerIndex(pointingSource, out pointerIndex);
            Debug.Assert(pointerIndex >= 0, "Invalid pointer index!");

            PointerData pointer;
            GetPointerData(pointingSource, out pointer);
            Debug.Assert(pointer != null, "Attempting to unregister a pointer that was never registered!");

            // Should we be protecting against unregistering the GazeManager?

            pointers.RemoveAt(pointerIndex);

            // Raise focus events if needed:

            if (pointer.End.Object != null)
            {
                GameObject unfocusedObject = pointer.End.Object;

                bool objectIsStillFocusedByOtherPointer = false;

                for (int iOther = 0; iOther < pointers.Count; iOther++)
                {
                    if (pointers[iOther].End.Object == unfocusedObject)
                    {
                        objectIsStillFocusedByOtherPointer = true;
                        break;
                    }
                }

                if (!objectIsStillFocusedByOtherPointer)
                {
                    RaiseFocusExitedEvents(unfocusedObject);
                }

                RaisePointerSpecificFocusChangedEvents(pointer.PointingSource, unfocusedObject, null);
            }
        }

        public FocusDetails? TryGetFocusDetails(BaseEventData eventData)
        {
            for (int i = 0; i < pointers.Count; i++)
            {
                if (pointers[i].PointingSource.OwnsInput(eventData))
                {
                    return pointers[i].End;
                }
            }

            return null;
        }

        public GameObject TryGetFocusedObject(BaseEventData eventData)
        {
            FocusDetails? details = TryGetFocusDetails(eventData);

            if (details == null)
            {
                return null;
            }

            IPointingSource pointingSource;
            TryGetPointingSource(eventData, out pointingSource);
            PointerInputEventData pointerInputEventData = GetSpecificPointerEventData(pointingSource);

            Debug.Assert(pointerInputEventData != null);
            pointerInputEventData.selectedObject = details.Value.Object;

            return details.Value.Object;
        }

        public bool TryGetPointingSource(BaseEventData eventData, out IPointingSource pointingSource)
        {
            for (int i = 0; i < pointers.Count; i++)
            {
                if (pointers[i].PointingSource.OwnsInput(eventData))
                {
                    pointingSource = pointers[i].PointingSource;
                    return true;
                }
            }

            pointingSource = null;
            return false;
        }

        public FocusDetails GetFocusDetails(IPointingSource pointingSource)
        {
            PointerData pointerData;
            FocusDetails details = default(FocusDetails);

            if (GetPointerData(pointingSource, out pointerData))
            {
                details = pointerData.End;
            }

            return details;
        }

        public GameObject GetFocusedObject(IPointingSource pointingSource)
        {
            PointerData pointerData;
            GameObject focusedObject = null;

            if (GetPointerData(pointingSource, out pointerData))
            {
                focusedObject = pointerData.End.Object;
            }

            return focusedObject;
        }

        /// <summary>
        /// Checks if exactly one pointer is registered and returns it if so.
        /// </summary>
        /// <returns>The registered pointer if exactly one is registered, null otherwise.</returns>
        public bool TryGetSinglePointer(out IPointingSource pointingSource)
        {
            if (pointers.Count == 1)
            {
                pointingSource = pointers[0].PointingSource;
                return true;
            }

            pointingSource = null;
            return false;
        }

        public delegate void FocusEnteredMethod(GameObject focusedObject);
        public event FocusEnteredMethod FocusEntered;

        public delegate void FocusExitedMethod(GameObject unfocusedObject);
        public event FocusExitedMethod FocusExited;

        public delegate void PointerSpecificFocusChangedMethod(IPointingSource pointer, GameObject oldFocusedObject, GameObject newFocusedObject);
        public event PointerSpecificFocusChangedMethod PointerSpecificFocusChanged;

        [Obsolete("Use either GetGazePointerEventData or GetSpecificPointerEventData")]
        public PointerInputEventData GetPointerEventData()
        {
            return GetGazePointerEventData();
        }

        public PointerInputEventData GetGazePointerEventData()
        {
            return gazeManagerPointingData.UnityUIPointerData;
        }

        public PointerInputEventData GetSpecificPointerEventData(IPointingSource pointer)
        {
            PointerData pointerEventData;
            return GetPointerData(pointer, out pointerEventData) ? pointerEventData.UnityUIPointerData : null;
        }

        public float GetPointingExtent(IPointingSource pointingSource)
        {
            return pointingSource.ExtentOverride ?? pointingExtent;
        }

        #endregion

        #region Utilities

        private void UpdatePointers()
        {
            bool gazeManagerIsRegistered = false;

            for (int iPointer = 0; iPointer < pointers.Count; iPointer++)
            {
                PointerData pointer = pointers[iPointer];

                if (pointer == gazeManagerPointingData)
                {
                    gazeManagerIsRegistered = true;
                }

                UpdatePointer(pointer);

                if (debugDrawPointingRays)
                {
                    Color rayColor;

                    if ((debugDrawPointingRayColors != null) && (debugDrawPointingRayColors.Length > 0))
                    {
                        rayColor = debugDrawPointingRayColors[iPointer % debugDrawPointingRayColors.Length];
                    }
                    else
                    {
                        rayColor = Color.green;
                    }

                    Debug.DrawRay(pointer.StartPoint, (pointer.End.Point - pointer.StartPoint), rayColor);
                }
            }

            if (gazeManagerPointingData != null)
            {
                Debug.Assert(ReferenceEquals(gazeManagerPointingData.PointingSource, GazeManager.Instance));

                if (!gazeManagerIsRegistered)
                {
                    UpdatePointer(gazeManagerPointingData);
                }

                GazeManager.Instance.UpdateHitDetails(gazeManagerPointingData.End, gazeManagerPointingData.LastRaycastHit, gazeManagerIsRegistered);
            }
        }

        private void UpdatePointer(PointerData pointer)
        {
            // Call the pointer's OnPreRaycast function
            // This will give it a chance to prepare itself for raycasts
            // eg, by building its Rays array
            pointer.PointingSource.OnPreRaycast();

            // If pointer interaction isn't enabled, clear its result object and return
            if (!pointer.PointingSource.InteractionEnabled)
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
                if (!pointer.PointingSource.FocusLocked)
                {
                    // Otherwise, continue
                    var prioritizedLayerMasks = (pointer.PointingSource.PrioritizedLayerMasksOverride ?? pointingRaycastLayerMasks);

                    // Perform raycast to determine focused object
                    RaycastPhysics(pointer, prioritizedLayerMasks);

                    // If we have a unity event system, perform graphics raycasts as well to support Unity UI interactions
                    if (EventSystem.current != null)
                    {
                        // NOTE: We need to do this AFTER RaycastPhysics so we use the current hit point to perform the correct 2D UI Raycast.
                        RaycastUnityUI(pointer, prioritizedLayerMasks);
                    }

                    // Set the pointer's result last
                    pointer.PointingSource.Result = pointer;
                }
            }

            // Call the pointer's OnPostRaycast function
            // This will give it a chance to respond to raycast results
            // eg by updating its appearance
            pointer.PointingSource.OnPostRaycast();
        }

        /// <summary>
        /// Perform a Unity physics Raycast to determine which scene objects with a collider is currently being gazed at, if any.
        /// </summary>
        private void RaycastPhysics(PointerData pointer, LayerMask[] prioritizedLayerMasks)
        {
            bool isHit = false;
            int rayStepIndex = 0;
            RayStep rayStep = default(RayStep);
            RaycastHit physicsHit = default(RaycastHit);

            // Comment back in GetType() only when debugging for a specific pointer.
            Debug.Assert(pointer.PointingSource.Rays != null, "No valid rays for pointer "/* + pointer.GetType()*/);
            Debug.Assert(pointer.PointingSource.Rays.Length > 0, "No valid rays for pointer "/* + pointer.GetType()*/);

            // Check raycast for each step in the pointing source
            for (int i = 0; i < pointer.PointingSource.Rays.Length; i++)
            {
                if (RaycastPhysicsStep(pointer.PointingSource.Rays[i], prioritizedLayerMasks, out physicsHit))
                {
                    // Set the pointer source's origin ray to this step
                    isHit = true;
                    rayStep = pointer.PointingSource.Rays[i];
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
                pointer.UpdateHit(GetPointingExtent(pointer.PointingSource));
            }
        }

        private bool RaycastPhysicsStep(RayStep step, LayerMask[] prioritizedLayerMasks, out RaycastHit physicsHit)
        {
            bool isHit = false;
            physicsHit = default(RaycastHit);

            // If there is only one priority, don't prioritize
            if (prioritizedLayerMasks.Length == 1)
            {
                isHit = Physics.Raycast(step.Origin, step.Direction, out physicsHit, step.Length, prioritizedLayerMasks[0]);
            }
            else
            {
                // Raycast across all layers and prioritize
                RaycastHit? hit = PrioritizeHits(Physics.RaycastAll(step.Origin, step.Direction, step.Length, Physics.AllLayers), prioritizedLayerMasks);
                isHit = hit.HasValue;

                if (isHit)
                {
                    physicsHit = hit.Value;
                }
            }

            return isHit;
        }

        private void RaycastUnityUI(PointerData pointer, LayerMask[] prioritizedLayerMasks)
        {
            Debug.Assert(pointer.End.Point != Vector3.zero, "No pointer source end point found to raycast against!");
            Debug.Assert(UIRaycastCamera != null, "You must assign a UIRaycastCamera on the FocusManager before you can process uGUI raycasting.");

            RaycastResult uiRaycastResult = default(RaycastResult);
            bool overridePhysicsRaycast = false;
            RayStep rayStep = default(RayStep);
            int rayStepIndex = 0;

            // Comment back in GetType() only when debugging for a specific pointer.
            Debug.Assert(pointer.PointingSource.Rays != null, "No valid rays for pointer "/* + pointer.GetType()*/);
            Debug.Assert(pointer.PointingSource.Rays.Length > 0, "No valid rays for pointer "/* + pointer.GetType()*/);

            // Cast rays for every step until we score a hit
            for (int i = 0; i < pointer.PointingSource.Rays.Length; i++)
            {
                if (RaycastUnityUIStep(pointer, pointer.PointingSource.Rays[i], prioritizedLayerMasks, out overridePhysicsRaycast, out uiRaycastResult))
                {
                    rayStepIndex = i;
                    rayStep = pointer.PointingSource.Rays[i];
                    break;
                }
            }

            // Check if we need to overwrite the physics raycast info
            if ((pointer.End.Object == null || overridePhysicsRaycast) && uiRaycastResult.isValid &&
                 uiRaycastResult.module != null && uiRaycastResult.module.eventCamera == UIRaycastCamera)
            {
                newUiRaycastPosition.x = uiRaycastResult.screenPosition.x;
                newUiRaycastPosition.y = uiRaycastResult.screenPosition.y;
                newUiRaycastPosition.z = uiRaycastResult.distance;

                Vector3 worldPos = UIRaycastCamera.ScreenToWorldPoint(newUiRaycastPosition);

                var hitInfo = new RaycastHit
                {
                    point = worldPos,
                    normal = -uiRaycastResult.gameObject.transform.forward
                };

                pointer.UpdateHit(uiRaycastResult, hitInfo, rayStep, rayStepIndex);
            }
        }

        private bool RaycastUnityUIStep(PointerData pointer, RayStep step, LayerMask[] prioritizedLayerMasks, out bool overridePhysicsRaycast, out RaycastResult uiRaycastResult)
        {
            // Move the uiRaycast camera to the current pointer's position.
            UIRaycastCamera.transform.position = step.Origin;
            UIRaycastCamera.transform.forward = step.Direction;

            // We always raycast from the center of the camera.
            pointer.UnityUIPointerData.position = new Vector2(UIRaycastCamera.pixelWidth * 0.5f, UIRaycastCamera.pixelHeight * 0.5f);

            // Graphics raycast
            uiRaycastResult = EventSystem.current.Raycast(pointer.UnityUIPointerData, prioritizedLayerMasks);
            pointer.UnityUIPointerData.pointerCurrentRaycast = uiRaycastResult;

            overridePhysicsRaycast = false;

            // If we have a raycast result, check if we need to overwrite the physics raycast info
            if (uiRaycastResult.gameObject != null)
            {
                if (pointer.End.Object != null)
                {
                    // Check layer prioritization
                    if (prioritizedLayerMasks.Length > 1)
                    {
                        // Get the index in the prioritized layer masks
                        int uiLayerIndex = uiRaycastResult.gameObject.layer.FindLayerListIndex(prioritizedLayerMasks);
                        int threeDLayerIndex = pointer.LastRaycastHit.collider.gameObject.layer.FindLayerListIndex(prioritizedLayerMasks);

                        if (threeDLayerIndex > uiLayerIndex)
                        {
                            overridePhysicsRaycast = true;
                        }
                        else if (threeDLayerIndex == uiLayerIndex)
                        {
                            if (pointer.LastRaycastHit.distance > uiRaycastResult.distance)
                            {
                                overridePhysicsRaycast = true;
                            }
                        }
                    }
                    else
                    {
                        if (pointer.LastRaycastHit.distance > uiRaycastResult.distance)
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

        private void UpdateFocusedObjects()
        {
            Debug.Assert(pendingPointerSpecificFocusChange.Count == 0);
            Debug.Assert(pendingOverallFocusExitSet.Count == 0);
            Debug.Assert(pendingOverallFocusEnterSet.Count == 0);

            // NOTE: We compute the set of events to send before sending the first event
            //       just in case someone responds to the event by adding/removing a
            //       pointer which would change the structures we're iterating over.

            for (int iPointer = 0; iPointer < pointers.Count; iPointer++)
            {
                PointerData pointer = pointers[iPointer];

                if (pointer.PreviousEndObject != pointer.End.Object)
                {
                    pendingPointerSpecificFocusChange.Add(pointer);

                    // Initially, we assume all pointer-specific focus changes will result
                    // also result in an overall focus change...

                    if (pointer.PreviousEndObject != null)
                    {
                        pendingOverallFocusExitSet.Add(pointer.PreviousEndObject);
                    }

                    if (pointer.End.Object != null)
                    {
                        pendingOverallFocusEnterSet.Add(pointer.End.Object);
                    }
                }
            }

            // ... but now we trim out objects whose overall focus was maintained the same by a different pointer:

            for (int iPointer = 0; iPointer < pointers.Count; iPointer++)
            {
                PointerData pointer = pointers[iPointer];

                pendingOverallFocusExitSet.Remove(pointer.End.Object);

                pendingOverallFocusEnterSet.Remove(pointer.PreviousEndObject);
            }

            // Now we raise the events:

            foreach (GameObject exit in pendingOverallFocusExitSet)
            {
                RaiseFocusExitedEvents(exit);
            }

            foreach (GameObject enter in pendingOverallFocusEnterSet)
            {
                RaiseFocusEnteredEvents(enter);
            }

            for (int iChange = 0; iChange < pendingPointerSpecificFocusChange.Count; iChange++)
            {
                PointerData change = pendingPointerSpecificFocusChange[iChange];

                RaisePointerSpecificFocusChangedEvents(change.PointingSource, change.PreviousEndObject, change.End.Object);
            }

            pendingOverallFocusEnterSet.Clear();
            pendingOverallFocusExitSet.Clear();
            pendingPointerSpecificFocusChange.Clear();
        }

        private void RaiseFocusExitedEvents(GameObject unfocusedObject)
        {
            InputManager.Instance.RaiseFocusExit(unfocusedObject);
            //Debug.Log("Focus Exit: " + unfocusedObject.name);
            if (FocusExited != null)
            {
                FocusExited(unfocusedObject);
            }
        }

        private void RaiseFocusEnteredEvents(GameObject focusedObject)
        {
            InputManager.Instance.RaiseFocusEnter(focusedObject);
            //Debug.Log("Focus Enter: " + focusedObject.name);
            if (FocusEntered != null)
            {
                FocusEntered(focusedObject);
            }
        }

        private void RaisePointerSpecificFocusChangedEvents(IPointingSource pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            InputManager.Instance.RaisePointerSpecificFocusChangedEvents(pointer, oldFocusedObject, newFocusedObject);

            if (PointerSpecificFocusChanged != null)
            {
                PointerSpecificFocusChanged(pointer, oldFocusedObject, newFocusedObject);
            }
        }

        private bool GetPointerData(IPointingSource pointingSource, out PointerData pointerData)
        {
            int pointerIndex;

            if (TryGetPointerIndex(pointingSource, out pointerIndex))
            {
                pointerData = pointers[pointerIndex];
                return true;
            }

            pointerData = null;
            return false;
        }

        private bool TryGetPointerIndex(IPointingSource pointingSource, out int pointerIndex)
        {
            for (int i = 0; i < pointers.Count; i++)
            {
                if (pointingSource == pointers[i].PointingSource)
                {
                    pointerIndex = i;
                    return true;
                }
            }

            pointerIndex = -1;
            return false;
        }

        private RaycastHit? PrioritizeHits(RaycastHit[] hits, LayerMask[] layerMasks)
        {
            if (hits.Length == 0)
            {
                return null;
            }

            // Return the minimum distance hit within the first layer that has hits.
            // In other words, sort all hit objects first by layerMask, then by distance.
            for (int layerMaskIdx = 0; layerMaskIdx < layerMasks.Length; layerMaskIdx++)
            {
                RaycastHit? minHit = null;

                for (int hitIdx = 0; hitIdx < hits.Length; hitIdx++)
                {
                    RaycastHit hit = hits[hitIdx];
                    if (hit.transform.gameObject.layer.IsInLayerMask(layerMasks[layerMaskIdx]) &&
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

        /// <summary>
        /// Helper for assigning world space canvases event cameras.
        /// <remarks>Warning! Very expensive. Use sparingly at runtime.</remarks>
        /// </summary>
        public void UpdateCanvasEventSystems()
        {
            Debug.Assert(UIRaycastCamera != null, "You must assign a UIRaycastCamera on the FocusManager before updating your canvases.");

            // This will also find disabled GameObjects in the scene.
            // Warning! this look up is very expensive!
            var sceneCanvases = Resources.FindObjectsOfTypeAll<Canvas>();

            for (var i = 0; i < sceneCanvases.Length; i++)
            {
                if (sceneCanvases[i].isRootCanvas && sceneCanvases[i].renderMode == RenderMode.WorldSpace)
                {
                    sceneCanvases[i].worldCamera = UIRaycastCamera;
                }
            }
        }

        #endregion
    }
}
