// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Focus manager is the bridge that handles different types of pointing sources like gaze cursor
    /// or pointing ray enabled motion controllers.
    /// If you don't have pointing ray enabled controllers, it defaults to GazeManager.
    /// </summary>
    public class FocusManager : Singleton<FocusManager>
    {
        #region MonoBehaviour Implementation

        protected override void Awake()
        {
            base.Awake();

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
        }

        private void Start()
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
            }

            if ((pointers.Count == 0) && autoRegisterGazePointerIfNoPointersRegistered && GazeManager.IsInitialized)
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

        private class PointerData
        {
            public readonly IPointingSource PointingSource;

            public Vector3 StartPoint { get; private set; }

            public FocusDetails End { get; private set; }

            public GameObject PreviousEndObject { get; private set; }

            public RaycastHit LastRaycastHit { get; private set; }

            public PointerData(IPointingSource pointingSource)
            {
                PointingSource = pointingSource;
            }

            public void UpdateHit(RaycastHit hit)
            {
                LastRaycastHit = hit;
                PreviousEndObject = End.Object;

                StartPoint = PointingSource.Ray.origin;
                End = new FocusDetails
                {
                    Point = hit.point,
                    Normal = hit.normal,
                    Object = hit.transform.gameObject,
                };
            }

            public void UpdateHit(RaycastResult result)
            {
                PreviousEndObject = End.Object;

                StartPoint = PointingSource.Ray.origin;
                End = new FocusDetails
                {
                    Point = result.worldPosition,
                    Normal = result.worldNormal,
                    Object = result.gameObject,
                };
            }

            public void UpdateHit(float extent)
            {
                PreviousEndObject = End.Object;

                StartPoint = PointingSource.Ray.origin;
                End = new FocusDetails
                {
                    Point = (StartPoint + (extent * PointingSource.Ray.direction)),
                    Normal = (-PointingSource.Ray.direction),
                    Object = null,
                };
            }

            public void ResetFocusedObjects()
            {
                PreviousEndObject = null;
                End = new FocusDetails
                {
                    Point = End.Point,
                    Normal = End.Normal,
                    Object = null,
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

        public PointerInputEventData UnityUIPointerEvent { get; private set; }
        private List<RaycastResult> uiRaycastResults = new List<RaycastResult>(0);

        private readonly HashSet<GameObject> pendingOverallFocusEnterSet = new HashSet<GameObject>();
        private readonly HashSet<GameObject> pendingOverallFocusExitSet = new HashSet<GameObject>();
        private readonly List<PointerData> pendingPointerSpecificFocusChange = new List<PointerData>();

        #endregion

        #region Accessors

        public void RegisterPointer(IPointingSource pointingSource)
        {
            Debug.Assert(pointingSource != null);

            if (TryGetPointerIndex(pointingSource) != null)
            {
                // This pointing source is already registered and active.
                return;
            }

            PointerData pointer;

            if (pointingSource is GazeManager)
            {
                if (gazeManagerPointingData == null)
                {
                    gazeManagerPointingData = new PointerData(pointingSource);
                }
                else
                {
                    gazeManagerPointingData.ResetFocusedObjects();
                }

                pointer = gazeManagerPointingData;
            }
            else
            {
                pointer = new PointerData(pointingSource);
            }

            pointers.Add(pointer);
        }

        public void UnRegisterPointer(IPointingSource pointingSource)
        {
            Debug.Assert(pointingSource != null);

            int? iPointer = TryGetPointerIndex(pointingSource);
            Debug.Assert(iPointer != null);

            PointerData pointer = pointers[iPointer.Value];

            pointers.RemoveAt(iPointer.Value);

            // Raise focus events if needed:

            if (pointer.End.Object != null)
            {
                GameObject deFocusedObject = pointer.End.Object;

                bool objectIsStillFocusedByOtherPointer = false;

                for (int iOther = 0; iOther < pointers.Count; iOther++)
                {
                    if (pointers[iOther].End.Object == deFocusedObject)
                    {
                        objectIsStillFocusedByOtherPointer = true;
                        break;
                    }
                }

                if (!objectIsStillFocusedByOtherPointer)
                {
                    RaiseFocusExitedEvents(deFocusedObject);
                }

                RaisePointerSpecificFocusChangedEvents(pointer.PointingSource, deFocusedObject, null);
            }
        }

        public FocusDetails? TryGetFocusDetails(BaseEventData eventData)
        {
            for (int iPointer = 0; iPointer < pointers.Count; iPointer++)
            {
                PointerData pointer = pointers[iPointer];

                if (pointer.PointingSource.OwnsInput(eventData))
                {
                    return pointer.End;
                }
            }

            return null;
        }

        public GameObject TryGetFocusedObject(BaseEventData eventData)
        {
            FocusDetails? details = TryGetFocusDetails(eventData);

            return (details == null) ? null : details.Value.Object;
        }

        public bool TryGetPointingSource(BaseEventData eventData, out IPointingSource pointingSource)
        {
            for (int iPointer = 0; iPointer < pointers.Count; iPointer++)
            {
                PointerData pointer = pointers[iPointer];

                if (pointer.PointingSource.OwnsInput(eventData))
                {
                    pointingSource = pointer.PointingSource;
                    return true;
                }
            }

            pointingSource = null;
            return false;
        }

        public FocusDetails GetFocusDetails(IPointingSource pointingSource)
        {
            return GetPointer(pointingSource).End;
        }

        public GameObject GetFocusedObject(IPointingSource pointingSource)
        {
            return GetPointer(pointingSource).End.Object;
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

        public delegate void FocusExitedMethod(GameObject deFocusedObject);
        public event FocusExitedMethod FocusExited;

        public delegate void PointerSpecificFocusChangedMethod(IPointingSource pointer, GameObject oldFocusedObject, GameObject newFocusedObject);
        public event PointerSpecificFocusChangedMethod PointerSpecificFocusChanged;

        public PointerInputEventData GetPointerEventData()
        {
            if (UnityUIPointerEvent == null)
            {
                UnityUIPointerEvent = new PointerInputEventData(EventSystem.current);
            }
            else
            {
                UnityUIPointerEvent.Clear();
            }

            return UnityUIPointerEvent;
        }

        public float GetPointingExtent(IPointingSource pointingSource)
        {
            return GetPointingExtent(GetPointer(pointingSource));
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

                GazeManager.Instance.UpdateHitDetails(gazeManagerPointingData.End, gazeManagerIsRegistered);
            }
        }

        private void UpdatePointer(PointerData pointer)
        {
            pointer.PointingSource.UpdatePointer();
            var prioritizedLayerMasks = (pointer.PointingSource.PrioritizedLayerMasksOverride ?? pointingRaycastLayerMasks);

            // Perform raycast to determine focused object
            RaycastPhysics(pointer, prioritizedLayerMasks);

            // If we have a unity event system, perform graphics raycasts as well to support Unity UI interactions
            if (EventSystem.current != null)
            {
                // NOTE: We need to do this AFTER RaycastPhysics so we use the current hit point to perform the correct 2D UI Raycast.
                RaycastUnityUI(pointer, prioritizedLayerMasks);
            }
        }

        /// <summary>
        /// Perform a Unity physics Raycast to determine which scene objects with a collider is currently being gazed at, if any.
        /// </summary>
        private void RaycastPhysics(PointerData pointer, LayerMask[] prioritizedLayerMasks)
        {
            bool isHit;
            RaycastHit physicsHit = default(RaycastHit);
            float extent = GetPointingExtent(pointer);

            // If there is only one priority, don't prioritize
            if (prioritizedLayerMasks.Length == 1)
            {
                isHit = Physics.Raycast(pointer.PointingSource.Ray, out physicsHit, extent, prioritizedLayerMasks[0]);
            }
            else
            {
                // Raycast across all layers and prioritize
                RaycastHit? hit = PrioritizeHits(Physics.RaycastAll(pointer.PointingSource.Ray, extent, /*All layers*/ -1), prioritizedLayerMasks);
                isHit = hit.HasValue;

                if (isHit)
                {
                    physicsHit = hit.Value;
                }
            }

            if (isHit)
            {
                pointer.UpdateHit(physicsHit);
            }
            else
            {
                pointer.UpdateHit(GetPointingExtent(pointer));
            }
        }

        private void RaycastUnityUI(PointerData pointer, LayerMask[] prioritizedLayerMasks)
        {
            GetPointerEventData();

            Camera mainCamera = CameraCache.Main;

            // 2D cursor position
            Vector2 cursorScreenPos = mainCamera.WorldToScreenPoint(pointer.End.Point);
            UnityUIPointerEvent.delta = cursorScreenPos - UnityUIPointerEvent.position;
            UnityUIPointerEvent.position = cursorScreenPos;

            // Graphics raycast
            uiRaycastResults.Clear();
            EventSystem.current.RaycastAll(UnityUIPointerEvent, uiRaycastResults);
            RaycastResult uiRaycastResult = FindClosestRaycastHitInLayerMasks(uiRaycastResults, prioritizedLayerMasks);
            UnityUIPointerEvent.pointerCurrentRaycast = uiRaycastResult;

            // If we have a raycast result, check if we need to overwrite the 3D raycast info
            if (uiRaycastResult.gameObject != null)
            {
                bool superseded3DObject = false;
                if (pointer.End.Object != null)
                {
                    // Check layer prioritization
                    if (prioritizedLayerMasks.Length > 1)
                    {
                        // Get the index in the prioritized layer masks
                        int uiLayerIndex = FindLayerListIndex(uiRaycastResult.gameObject.layer, prioritizedLayerMasks);
                        int threeDLayerIndex = FindLayerListIndex(pointer.LastRaycastHit.collider.gameObject.layer, prioritizedLayerMasks);

                        if (threeDLayerIndex > uiLayerIndex)
                        {
                            superseded3DObject = true;
                        }
                        else if (threeDLayerIndex == uiLayerIndex)
                        {
                            if (pointer.LastRaycastHit.distance > uiRaycastResult.distance)
                            {
                                superseded3DObject = true;
                            }
                        }
                    }
                    else
                    {
                        if (pointer.LastRaycastHit.distance > uiRaycastResult.distance)
                        {
                            superseded3DObject = true;
                        }
                    }
                }

                // Check if we need to overwrite the 3D raycast info
                if (pointer.End.Object == null || superseded3DObject)
                {
                    pointer.UpdateHit(uiRaycastResult);
                }
            }
        }
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

            // ... but now we trim out objects whose overall focus was maintained the same by
            // a different pointer:

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

        private void RaiseFocusExitedEvents(GameObject deFocusedObject)
        {
            InputManager.Instance.RaiseFocusExit(deFocusedObject);

            if (FocusExited != null)
            {
                FocusExited(deFocusedObject);
            }
        }

        private void RaiseFocusEnteredEvents(GameObject focusedObject)
        {
            InputManager.Instance.RaiseFocusEnter(focusedObject);

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

        private PointerData GetPointer(IPointingSource pointingSource)
        {
            int? iPointer = TryGetPointerIndex(pointingSource);
            Debug.Assert(iPointer != null);
            return pointers[iPointer.Value];
        }

        private int? TryGetPointerIndex(IPointingSource pointingSource)
        {
            int? found = null;

            for (int i = 0; i < pointers.Count; i++)
            {
                if (pointers[i].PointingSource == pointingSource)
                {
                    found = i;
                    break;
                }
            }

            return found;
        }

        private float GetPointingExtent(PointerData pointer)
        {
            return (pointer.PointingSource.ExtentOverride ?? pointingExtent);
        }

        private RaycastHit? TryGetPreferredHit(IList<RaycastHit> orderedHits, IList<LayerMask> prioritizedLayerMasks)
        {
            Debug.Assert(orderedHits != null);
            Debug.Assert(prioritizedLayerMasks != null);

            // These hits are preferred based on:
            //   1. order in priority layer masks list.
            //   2. order in hits list, which should correspond to smallest distance.

            for (int iMask = 0; iMask < prioritizedLayerMasks.Count; iMask++)
            {
                LayerMask mask = prioritizedLayerMasks[iMask];

                for (int iHit = 0; iHit < orderedHits.Count; iHit++)
                {
                    RaycastHit hit = orderedHits[iHit];

                    if (hit.transform.gameObject.IsInLayerMask(mask))
                    {
                        return hit;
                    }
                }
            }

            return null;
        }

        private bool IsLayerInLayerMask(int layer, int layerMask)
        {
            return ((1 << layer) & layerMask) != 0;
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
                    if (IsLayerInLayerMask(hit.transform.gameObject.layer, layerMasks[layerMaskIdx]) &&
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

        #endregion
    }
}
