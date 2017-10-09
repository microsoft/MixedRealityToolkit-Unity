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

            Ray pointingRay = pointer.PointingSource.Ray;
            float extent = GetPointingExtent(pointer);
            var prioritizedLayerMasks = (pointer.PointingSource.PrioritizedLayerMasksOverride ?? pointingRaycastLayerMasks);

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

            if (EventSystem.current == null)
            {
                uiHit = null;
            }
            else
            {
                GetPointerEventData();

                Camera mainCamera = CameraCache.Main;

                // 2D cursor position
                Vector2 cursorScreenPos = mainCamera.WorldToScreenPoint(physicsHit.Value.point);
                UnityUIPointerEvent.delta = cursorScreenPos - UnityUIPointerEvent.position;
                UnityUIPointerEvent.position = cursorScreenPos;

                uiRaycastResults.Clear();
                EventSystem.current.RaycastAll(UnityUIPointerEvent, uiRaycastResults);
                uiHit = TryGetPreferredHit(uiRaycastResults, prioritizedLayerMasks);

                if (uiHit != null)
                {
                    RaycastResult patchedUiHit = uiHit.Value;

                    float totalDistance = (patchedUiHit.distance + CameraCache.Main.nearClipPlane);

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

        private int GetLayerMaskIndex(GameObject objectToCheck, IList<LayerMask> prioritizedLayerMasks)
        {
            Debug.Assert(prioritizedLayerMasks != null);

            for (int iMask = 0; iMask < prioritizedLayerMasks.Count; iMask++)
            {
                if (objectToCheck.IsInLayerMask(prioritizedLayerMasks[iMask]))
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

        #endregion
    }
}
