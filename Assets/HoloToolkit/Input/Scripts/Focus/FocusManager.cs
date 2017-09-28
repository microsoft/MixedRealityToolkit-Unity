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
    /// If you dont have pointing ray enabled controllers, it defaults to GazeManager.    
    /// </summary>
    public class FocusManager : Singleton<FocusManager>
    {
        #region MonoBehaviour Implementation

        protected override void Awake()
        {
            base.Awake();

            AwakePointing();
        }

        private void Start()
        {
            StartPointing();
        }

        private void Update()
        {
            UpdatePointing();
            UpdateFocus();
        }

        #endregion

        #region Settings

        [SerializeField]
        private float pointingExtent = 10f;

        [SerializeField]
        private LayerMask[] pointingPrioritizedLayerMasks = CreateDefaultPointingPrioritizedLayerMasks();

        [SerializeField]
        private GameObject[] autoRegisteredPointers = null;

        [SerializeField]
        private bool autoRegisterGazePointerIfNoPointersRegistered = true;

        [SerializeField]
        private Canvas[] autoRegisteredPointableCanvases = null;

        [SerializeField]
        private bool DebugDrawPointingRays = false;

        [SerializeField]
        private Color[] DebugDrawPointingRayColors = null;

        #endregion

        #region Data

        private class PointerData
        {
            public readonly IPointingSource PointingSource;

            public Vector3 StartPoint { get; private set; }
            public FocusDetails End { get; private set; }
            public GameObject PreviousEndObject { get; private set; }

            public PointerData(IPointingSource pointingSource)
            {
                PointingSource = pointingSource;
            }

            public void UpdateHit(RaycastHit physicsHit)
            {
                PreviousEndObject = End.Object;

                StartPoint = PointingSource.Ray.origin;
                End = new FocusDetails
                {
                    Point = physicsHit.point,
                    Normal = physicsHit.normal,
                    Object = physicsHit.transform.gameObject,
                };
            }

            public void UpdateHit(RaycastResult uiHit)
            {
                PreviousEndObject = End.Object;

                StartPoint = PointingSource.Ray.origin;
                End = new FocusDetails
                {
                    Point = uiHit.worldPosition,
                    Normal = uiHit.worldNormal,
                    Object = uiHit.gameObject,
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

        private readonly List<PointerData> pointers = new List<PointerData>();

        // GazeManager is a little special, so we keep track of it even if it's not a registered pointer. For the sake
        // of StabilizationPlaneModifier and potentially other components that care where the user's looking, we need
        // to do a gaze raycast even if gaze isn't used for focus.
        private PointerData gazeManagerPointingData;

        private readonly List<Canvas> pointableCanvases = new List<Canvas>();

        private Camera uiRaycastCamera;
        private PointerInputEventData uiRaycastPointerInputData;
        private List<RaycastResult> uiRaycastResults;

        private readonly HashSet<GameObject> pendingOverallFocusEnterSet = new HashSet<GameObject>();
        private readonly HashSet<GameObject> pendingOverallFocusExitSet = new HashSet<GameObject>();
        private readonly List<PointerData> pendingPointerSpecificFocusChange = new List<PointerData>();

        #endregion

        #region Accessors

        public bool IsUnityUiFocusable
        {
            get { return (pointableCanvases.Count > 0); }

        }

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

        public void UnregisterPointer(IPointingSource pointingSource)
        {
            Debug.Assert(pointingSource != null);

            int? iPointer = TryGetPointerIndex(pointingSource);
            Debug.Assert(iPointer != null);

            PointerData pointer = pointers[iPointer.Value];

            pointers.RemoveAt(iPointer.Value);

            // Raise focus events if needed:

            if (pointer.End.Object != null)
            {
                GameObject defocusedObject = pointer.End.Object;

                bool objectIsStillFocusedByOtherPointer = false;

                for (int iOther = 0; iOther < pointers.Count; iOther++)
                {
                    if (pointers[iOther].End.Object == defocusedObject)
                    {
                        objectIsStillFocusedByOtherPointer = true;
                        break;
                    }
                }

                if (!objectIsStillFocusedByOtherPointer)
                {
                    RaiseFocusExitedEvents(defocusedObject);
                }

                RaisePointerSpecificFocusChangedEvents(pointer.PointingSource, defocusedObject, null);
            }
        }

        public void RegisterPointableCanvas(Canvas canvas)
        {
            Debug.Assert(canvas != null);
            Debug.Assert(TryGetPointableCanvasIndex(canvas) == null);

            canvas.worldCamera = uiRaycastCamera;
            pointableCanvases.Add(canvas);
        }

        public void UnregisterPointableCanvas(Canvas canvas)
        {
            Debug.Assert(canvas != null);

            int? iCanvas = TryGetPointableCanvasIndex(canvas);
            Debug.Assert(iCanvas != null);

            pointableCanvases.RemoveAt(iCanvas.Value);
            canvas.worldCamera = null;
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
            else
            {
                pointingSource = null;
                return false;
            }
        }

        public delegate void FocusEnteredMethod(GameObject focusedObject);
        public event FocusEnteredMethod FocusEntered;

        public delegate void FocusExitedMethod(GameObject defocusedObject);
        public event FocusExitedMethod FocusExited;

        public delegate void PointerSpecificFocusChangedMethod(IPointingSource pointer, GameObject oldFocusedObject, GameObject newFocusedObject);
        public event PointerSpecificFocusChangedMethod PointerSpecificFocusChanged;

        public PointerInputEventData BorrowPointerEventData()
        {
            if (uiRaycastPointerInputData == null)
            {
                uiRaycastPointerInputData = new PointerInputEventData(EventSystem.current);
            }
            else
            {
                Clear(uiRaycastPointerInputData);
            }

            return uiRaycastPointerInputData;
        }

        public float GetPointingExtent(IPointingSource pointingSource)
        {
            return GetPointingExtent(GetPointer(pointingSource));
        }

        #endregion

        #region Utilities

        private void AwakePointing()
        {
            if ((pointingPrioritizedLayerMasks == null) || (pointingPrioritizedLayerMasks.Length == 0))
            {
                pointingPrioritizedLayerMasks = CreateDefaultPointingPrioritizedLayerMasks();
            }

            AutoRegisterPointers();
            AutoRegisterPointableCanvases();
        }

        private void StartPointing()
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
                Debug.Assert(object.ReferenceEquals(gazeManagerPointingData.PointingSource, GazeManager.Instance));
            }

            if ((pointers.Count == 0) && autoRegisterGazePointerIfNoPointersRegistered && GazeManager.IsInitialized)
            {
                RegisterPointer(GazeManager.Instance);
            }
        }

        private static LayerMask[] CreateDefaultPointingPrioritizedLayerMasks()
        {
            return new LayerMask[] { Physics.DefaultRaycastLayers };
        }

        private void AutoRegisterPointers()
        {
            if (autoRegisteredPointers != null)
            {
                for (int iPointer = 0; iPointer < autoRegisteredPointers.Length; iPointer++)
                {
                    GameObject owner = autoRegisteredPointers[iPointer];

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

        private void AutoRegisterPointableCanvases()
        {
            if (autoRegisteredPointableCanvases != null)
            {
                for (int iCanvas = 0; iCanvas < autoRegisteredPointableCanvases.Length; iCanvas++)
                {
                    Canvas canvas = autoRegisteredPointableCanvases[iCanvas];

                    if (canvas == null)
                    {
                        Debug.LogError("AutoRegisteredPointableCanvases contains a null (\"None\") component.");
                        break;
                    }

                    RegisterPointableCanvas(canvas);
                }
            }
        }

        private void UpdatePointing()
        {
            bool gazeManagerIsRegistered = false;

            for (int iPointer = 0; iPointer < pointers.Count; iPointer++)
            {
                PointerData pointer = pointers[iPointer];

                if (pointer == gazeManagerPointingData)
                {
                    gazeManagerIsRegistered = true;
                }

                UpdatePointing(pointer);

                if (DebugDrawPointingRays)
                {
                    Color rayColor;

                    if ((DebugDrawPointingRayColors != null) && (DebugDrawPointingRayColors.Length > 0))
                    {
                        rayColor = DebugDrawPointingRayColors[iPointer % DebugDrawPointingRayColors.Length];
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
                    UpdatePointing(gazeManagerPointingData);
                }

                GazeManager.Instance.UpdateHitDetails(gazeManagerPointingData.End, gazeManagerIsRegistered);
            }
        }

        private void UpdatePointing(PointerData pointer)
        {
            pointer.PointingSource.UpdatePointer();

            Ray pointingRay = pointer.PointingSource.Ray;
            float extent = GetPointingExtent(pointer);
            IList<LayerMask> prioritizedLayerMasks = (pointer.PointingSource.PrioritizedLayerMasksOverride ?? pointingPrioritizedLayerMasks);

            LayerMask combinedLayerMasks = GetCombinedLayerMask(prioritizedLayerMasks);

            RaycastHit? physicsHit;
            RaycastResult? uiHit;

            if (prioritizedLayerMasks.Count > 1)
            {
                RaycastHit[] hits = Physics.RaycastAll(pointingRay, extent, combinedLayerMasks);

                physicsHit = TryGetPreferredHit(hits, prioritizedLayerMasks);
            }
            else
            {
                RaycastHit hit;

                physicsHit = Physics.Raycast(pointingRay, out hit, extent, combinedLayerMasks)
                    ? (RaycastHit?)hit
                    : null;
            }

            if ((pointableCanvases.Count == 0) || (EventSystem.current == null))
            {
                uiHit = null;
            }
            else
            {
                if (uiRaycastCamera == null)
                {
                    uiRaycastCamera = new GameObject("UI Raycast Camera").AddComponent<Camera>();
                    uiRaycastCamera.transform.parent = transform;

                    // Make sure the raycast starts as close the the camera as possible. Ideally, this would
                    // be 0, but as of Unity 5.5, setting it to 0 causes the raycast to complain that the
                    // screen point isn't in the camera's view frustum.
                    uiRaycastCamera.nearClipPlane = 0.01f;

                    // Make sure the camera's pixel rect is large enough for raycasting to work as desired. As
                    // of Unity 5.5, setting it much smaller than this makes the raycast miss things.
                    uiRaycastCamera.pixelRect = new Rect(0, 0, 100, 100);

                    // Don't waste performance rendering anything.
                    uiRaycastCamera.enabled = false;


                    foreach (Canvas canvas in pointableCanvases)
                    {
                        canvas.worldCamera = uiRaycastCamera;
                    }

                    if (uiRaycastPointerInputData == null)
                    {
                        uiRaycastPointerInputData = new PointerInputEventData(EventSystem.current);
                    }

                    Debug.Assert(uiRaycastResults == null);
                    uiRaycastResults = new List<RaycastResult>();
                }

                Debug.Assert(uiRaycastCamera != null);
                Debug.Assert(uiRaycastPointerInputData != null);
                Debug.Assert(uiRaycastResults != null);

                foreach (Canvas canvas in pointableCanvases)
                {
                    Debug.Assert(canvas.worldCamera == uiRaycastCamera);
                }

                uiRaycastCamera.transform.position = pointingRay.origin;
                uiRaycastCamera.transform.forward = pointingRay.direction;

                Clear(uiRaycastPointerInputData);
                uiRaycastPointerInputData.position = new Vector2((uiRaycastCamera.pixelWidth / 2), (uiRaycastCamera.pixelHeight / 2));

                uiRaycastResults.Clear();

                EventSystem.current.RaycastAll(uiRaycastPointerInputData, uiRaycastResults);
                uiHit = TryGetPreferredHit(uiRaycastResults, prioritizedLayerMasks);

                if (uiHit != null)
                {
                    RaycastResult patchedUiHit = uiHit.Value;

                    float totalDistance = (patchedUiHit.distance + uiRaycastCamera.nearClipPlane);

                    patchedUiHit.distance = totalDistance;

                    Debug.Assert((patchedUiHit.worldPosition == Vector3.zero), "As of Unity 5.5, UI Raycasts always"
                        + " return worldPosition (0,0,0), so we'll fill it in here with the correct value. If this"
                        + " assertion fires, see what data is available, and consider using it instead of our fill in."
                        );

                    patchedUiHit.worldPosition = (pointingRay.origin + (totalDistance * pointingRay.direction));

                    Debug.Assert((patchedUiHit.worldNormal == Vector3.zero), "As of Unity 5.5, UI Raycasts always"
                        + " return worldNormal (0,0,0), so we'll fill it in here with something incorrect, but"
                        + " reasonable. If this assertion fires, see what data is available, and consider using it"
                        + " instead of our fill in."
                        );

                    patchedUiHit.worldNormal = (-patchedUiHit.gameObject.transform.forward);

                    uiHit = patchedUiHit;
                }
            }


            if ((physicsHit != null) && (uiHit != null))
            {
                for (int iMask = 0; iMask < prioritizedLayerMasks.Count; iMask++)
                {
                    LayerMask mask = prioritizedLayerMasks[iMask];

                    bool physicsIsInMask = physicsHit.Value.transform.gameObject.IsInLayerMask(mask);
                    bool uiIsInMask = uiHit.Value.gameObject.IsInLayerMask(mask);

                    if (physicsIsInMask && uiIsInMask)
                    {
                        // In the case of tie in priority and distance, we give preference to the UI,
                        // assuming that if people stick UI on top of 3D objects, they probably want
                        // the UI to take the pointer.

                        if (uiHit.Value.distance <= physicsHit.Value.distance)
                        {
                            pointer.UpdateHit(uiHit.Value);
                            break;
                        }
                        else
                        {
                            pointer.UpdateHit(physicsHit.Value);
                            break;
                        }
                    }
                    else if (physicsIsInMask)
                    {
                        pointer.UpdateHit(physicsHit.Value);
                        break;
                    }
                    else if (uiIsInMask)
                    {
                        pointer.UpdateHit(uiHit.Value);
                        break;
                    }
                    else
                    {
                        // Nothing... keep searching for a mask that contains at least one of the hits.
                    }
                }

                Debug.Assert(pointer.End.Object != null);
            }
            else if (physicsHit != null)
            {
                pointer.UpdateHit(physicsHit.Value);
            }
            else if (uiHit != null)
            {
                pointer.UpdateHit(uiHit.Value);
            }
            else
            {
                pointer.UpdateHit(extent);
            }
        }

        private void UpdateFocus()
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

        private void RaiseFocusExitedEvents(GameObject defocusedObject)
        {
            InputManager.Instance.RaiseFocusExit(defocusedObject);

            if (FocusExited != null)
            {
                FocusExited(defocusedObject);
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

        private int? TryGetPointableCanvasIndex(Canvas canvas)
        {
            int? found = null;

            for (int i = 0; i < pointableCanvases.Count; i++)
            {
                if (pointableCanvases[i] == canvas)
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

        private RaycastResult? TryGetPreferredHit(IList<RaycastResult> semiOrderedHits, IList<LayerMask> prioritizedLayerMasks)
        {
            Debug.Assert(semiOrderedHits != null);
            Debug.Assert(prioritizedLayerMasks != null);

            RaycastResult? preferred = null;
            int preferredLayerMaskIndex = int.MaxValue;

            for (int iHit = 0; iHit < semiOrderedHits.Count; iHit++)
            {
                RaycastResult hit = semiOrderedHits[iHit];
                int hitLayerMaskIndex = GetLayerMaskIndex(hit.gameObject, prioritizedLayerMasks);


                // First, prefer by order in priority layer masks list:

                if ((preferred == null) || (hitLayerMaskIndex < preferredLayerMaskIndex))
                {
                    preferred = hit;
                    preferredLayerMaskIndex = hitLayerMaskIndex;
                    continue;
                }
                else if (hitLayerMaskIndex > preferredLayerMaskIndex)
                {
                    continue;
                }
                Debug.Assert(hitLayerMaskIndex == preferredLayerMaskIndex);


                // Then by biggest sorting layer:

                if (hit.sortingLayer > preferred.Value.sortingLayer)
                {
                    preferred = hit;
                    preferredLayerMaskIndex = hitLayerMaskIndex;
                    continue;
                }
                else if (hit.sortingLayer < preferred.Value.sortingLayer)
                {
                    continue;
                }
                Debug.Assert(hit.sortingLayer == preferred.Value.sortingLayer);


                // Then by biggest order in layer:

                if (hit.sortingOrder > preferred.Value.sortingOrder)
                {
                    preferred = hit;
                    preferredLayerMaskIndex = hitLayerMaskIndex;
                    continue;
                }
                else if (hit.sortingOrder < preferred.Value.sortingOrder)
                {
                    continue;
                }
                Debug.Assert(hit.sortingOrder == preferred.Value.sortingOrder);


                // Then by smallest distance:

                if (hit.distance < preferred.Value.distance)
                {
                    preferred = hit;
                    preferredLayerMaskIndex = hitLayerMaskIndex;
                    continue;
                }
                else if (hit.distance > preferred.Value.distance)
                {
                    continue;
                }
                Debug.Assert(hit.distance == preferred.Value.distance);


                // Then by order in hits list, which seems to break the tie correctly for UI layered flat on
                // the same canvas. By virtue of letting the loop continue here without updating preferred,
                // we break the tie with the order in hits list.
            }

            return preferred;
        }

        private int GetLayerMaskIndex(GameObject gameObject, IList<LayerMask> prioritizedLayerMasks)
        {
            Debug.Assert(prioritizedLayerMasks != null);

            for (int iMask = 0; iMask < prioritizedLayerMasks.Count; iMask++)
            {
                if (gameObject.IsInLayerMask(prioritizedLayerMasks[iMask]))
                {
                    return iMask;
                }
            }

            Debug.Assert(false);
            return int.MaxValue;
        }

        private LayerMask GetCombinedLayerMask(IList<LayerMask> toCombine)
        {
            int combined = 0;

            for (int i = 0; i < toCombine.Count; i++)
            {
                combined |= toCombine[i];
            }

            return combined;
        }

        private void Clear(PointerEventData pointerEventData)
        {
            uiRaycastPointerInputData.Reset();

            uiRaycastPointerInputData.button = PointerEventData.InputButton.Left;
            uiRaycastPointerInputData.clickCount = 0;
            uiRaycastPointerInputData.clickTime = 0;
            uiRaycastPointerInputData.delta = Vector2.zero;
            uiRaycastPointerInputData.dragging = false;
            uiRaycastPointerInputData.eligibleForClick = false;
            uiRaycastPointerInputData.pointerCurrentRaycast = default(RaycastResult);
            uiRaycastPointerInputData.pointerDrag = null;
            uiRaycastPointerInputData.pointerEnter = null;
            uiRaycastPointerInputData.pointerId = 0;
            uiRaycastPointerInputData.pointerPress = null;
            uiRaycastPointerInputData.pointerPressRaycast = default(RaycastResult);
            uiRaycastPointerInputData.position = Vector2.zero;
            uiRaycastPointerInputData.pressPosition = Vector2.zero;
            uiRaycastPointerInputData.rawPointerPress = null;
            uiRaycastPointerInputData.scrollDelta = Vector2.zero;
            uiRaycastPointerInputData.selectedObject = null;
            uiRaycastPointerInputData.useDragThreshold = false;
            uiRaycastPointerInputData.InputSource = null;
            uiRaycastPointerInputData.SourceId = 0;
        }

        #endregion
    }
}
