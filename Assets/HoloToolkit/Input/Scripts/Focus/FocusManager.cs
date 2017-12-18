// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
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

                    RegisterFocuser(pointingSource);
                }
            }
        }

        private void Start()
        {
            /*if (pointers.Count == 0 && autoRegisterGazePointerIfNoPointersRegistered && Gaze.IsInitialized)
            {
                RegisterFocuser(GazePointer.Instance);
            }*/
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

        //private readonly List<IPointingSource> pointers = new List<IPointingSource>();
        private readonly List<IFocuser> focusers = new List<IFocuser>();
        private readonly List<IFocuser> activeFocusers = new List<IFocuser>();
        private readonly List<IFocusTarget> currentFocusTargets = new List<IFocusTarget>();

        /// These sets are used to track changes in a focus target's FocusEnabled state
        private HashSet<IFocusTarget> prevActiveFocusTargets = new HashSet<IFocusTarget>();
        private HashSet<IFocusTarget> activeFocusTargets = new HashSet<IFocusTarget>();

        /// <summary>
        /// GazeManager is a little special, so we keep track of it even if it's not a registered pointer. For the sake
        /// of StabilizationPlaneModifier and potentially other components that care where the user's looking, we need
        /// to do a gaze raycast even if gaze isn't used for focus.
        /// </summary>
        //private PointerData gazeManagerPointingData;

        [Obsolete("Use GetGazePointerEventData or GetSpecificPointerEventData")]
        public PointerInputEventData UnityUIPointerEvent { get; private set; }
        
        private readonly HashSet<IFocusTarget> pendingGainFocusSet = new HashSet<IFocusTarget>();
        private readonly HashSet<IFocusTarget> pendingLoseFocusSet = new HashSet<IFocusTarget>();
        private readonly HashSet<InputManager.FocusEvent> pendingFocusEnterSet = new HashSet<InputManager.FocusEvent>();
        private readonly HashSet<InputManager.FocusEvent> pendingFocusExitSet = new HashSet<InputManager.FocusEvent>();

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

        public List<IFocuser> ActiveFocusers { get { return activeFocusers; } }

        public List<IFocusTarget> CurrentFocusTargets { get { return currentFocusTargets; } }

        public void RegisterFocuser(IFocuser focuser)
        {
            Debug.Assert(focuser != null, "Can't register a focuser if you give us one.");

            int focuserIndex;
            //PointerData pointer;

            if (TryGetFocuserIndex(focuser, out focuserIndex))
            {
                // This pointing source is already registered and active.
                return;
            }

            /*if (pointingSource is Gaze)
            {
                if (gazeManagerPointingData == null)
                {
                    if (Gaze.IsInitialized)
                    {
                        gazeManagerPointingData = new PointerData(GazePointer.Instance);
                    }
                }
                else
                {
                    Debug.Assert(ReferenceEquals(gazeManagerPointingData.PointingSource, GazePointer.Instance));
                    gazeManagerPointingData.ResetFocusedObjects();
                }

                Debug.Assert(gazeManagerPointingData != null);
                pointer = gazeManagerPointingData;
            }
            else
            {
                pointer = new PointerData(pointingSource);
            }
            
            pointers.Add(pointer);*/

            focusers.Add(focuser);
        }

        public void UnregisterFocuser(IFocuser focuser)
        {
            Debug.Assert(focuser != null, "Can't unregister a focuser if you give us one.");

            int focuserIndex;
            TryGetFocuserIndex(focuser, out focuserIndex);
            Debug.Assert(focuserIndex >= 0, "Invalid focuser index!");
            
            // Should we be protecting against unregistering the GazeManager?

            focusers.RemoveAt(focuserIndex);

            // Raise focus events if needed:

            if (focuser.Result.Target != null)
            {
                RaiseFocusExitedEvents(new InputManager.FocusEvent(focuser, focuser.Result.Target));

                RaiseFocusChangedEvents(focuser, focuser.Result.Target, null);
            }
        }

        public bool TryGetFocusResult(BaseInputEventData eventData, out FocusResult result)
        {
            result = null;

            for (int i = 0; i < focusers.Count; i++)
            {
                if (focusers[i].OwnsInput(eventData))
                {
                    result = focusers[i].Result;
                    break;
                }
            }

            return result != null;
        }

        public bool TryGetFocuser(BaseInputEventData eventData, out IFocuser focuser)
        {
            for (int i = 0; i < focusers.Count; i++)
            {
                if (focusers[i].OwnsInput(eventData))
                {
                    focuser = focusers[i];
                    return true;
                }
            }

            focuser = null;
            return false;
        }

        public delegate void FocusMethodInfo(InputManager.FocusEvent focusedObject);
        public event FocusMethodInfo FocusEnteredInfo;
        public event FocusMethodInfo FocusExitedInfo;

        /// <summary>
        /// Called when a focus target without focus gains focus from one or more focusers
        /// </summary>
        public delegate void FocusGainedMethod(IFocusTarget focusedObject);
        public event FocusGainedMethod FocusGained;

        /// <summary>
        /// Called when a focus target with focus loses focus from ALL focusers
        /// </summary>
        public delegate void FocusLostMethod(IFocusTarget unfocusedObject);
        public event FocusLostMethod FocusLost;

        public delegate void FocusChangedMethod(IFocuser pointer, GameObject oldFocusedObject, GameObject newFocusedObject);
        public event FocusChangedMethod FocusChanged;

        public PointerInputEventData GetGazePointerEventData()
        {
            /*if (gazeManagerPointingData != null)
            {
                return gazeManagerPointingData.UnityUIPointerData;
            }*/
            return null;
        }

        /*public PointerInputEventData GetSpecificFocuserEventData(IFocuser focuser)
        {
            return focuser.Result.UnityUIPointerData;
            PointerData pointerEventData;
            return GetPointerData(pointer, out pointerEventData) ? pointerEventData.UnityUIPointerData : null;
        }*/

        public float GetPointingExtent(IPointingSource pointingSource)
        {
            return pointingSource.ExtentOverride ?? pointingExtent;
        }

        #endregion

        #region Utilities

        private void UpdatePointers()
        {
            activeFocusers.Clear();

            //bool gazeManagerIsRegistered = false;

            for (int iFocuser = 0; iFocuser < focusers.Count; iFocuser++)
            {
                if (focusers[iFocuser].InteractionEnabled)
                {
                    activeFocusers.Add(focusers[iFocuser]);
                }

                IPointingSource pointer = focusers[iFocuser] as IPointingSource;

                if (pointer == null)
                {
                    continue;
                }

                /*if (pointer == gazeManagerPointingData)
                {
                    gazeManagerIsRegistered = true;
                }*/

                UpdatePointer(pointer);

                if (debugDrawPointingRays)
                {
                    Color rayColor;

                    if ((debugDrawPointingRayColors != null) && (debugDrawPointingRayColors.Length > 0))
                    {
                        rayColor = debugDrawPointingRayColors[iFocuser % debugDrawPointingRayColors.Length];
                    }
                    else
                    {
                        rayColor = Color.green;
                    }

                    Debug.DrawRay(pointer.Result.StartPoint, (pointer.Result.Point - pointer.Result.StartPoint), rayColor);
                }
            }

            /*if (gazeManagerPointingData != null)
            {
                Debug.Assert(ReferenceEquals(gazeManagerPointingData.PointingSource, GazeManager.Instance));

                if (!gazeManagerIsRegistered)
                {
                    UpdatePointer(gazeManagerPointingData);
                }

                GazeManager.Instance.UpdateHitDetails(gazeManagerPointingData.End, gazeManagerPointingData.LastRaycastHit, gazeManagerIsRegistered);
            }*/
        }

        private void UpdatePointer(IPointingSource pointer)
        {
            // Call the pointer's OnPreRaycast function
            // This will give it a chance to prepare itself for raycasts
            // eg, by building its Rays array
            pointer.OnPreRaycast();

            // If pointer interaction isn't enabled, clear its result object and return
            if (!pointer.InteractionEnabled)
            {
                // Don't clear the previous focused object since we still want to trigger FocusExit events
                pointer.Result.ResetFocusedObjects(false);
            }
            else
            {
                // If the pointer is locked
                // Keep the focus objects the same
                // This will ensure that we execute events on those objects
                // even if the pointer isn't pointing at them
                if (!pointer.FocusLocked)
                {
                    // Otherwise, continue
                    var prioritizedLayerMasks = (pointer.PrioritizedLayerMasksOverride ?? pointingRaycastLayerMasks);

                    // Perform raycast to determine focused object
                    RaycastPhysics(pointer, prioritizedLayerMasks);

                    // If we have a unity event system, perform graphics raycasts as well to support Unity UI interactions
                    if (EventSystem.current != null)
                    {
                        // NOTE: We need to do this AFTER RaycastPhysics so we use the current hit point to perform the correct 2D UI Raycast.
                        RaycastUnityUI(pointer, prioritizedLayerMasks);
                    }

                    // Set the pointer's result last
                    //pointer.Result = pointer;
                }
            }

            // Call the pointer's OnPostRaycast function
            // This will give it a chance to respond to raycast results
            // eg by updating its appearance
            pointer.OnPostRaycast();
        }

        /// <summary>
        /// Perform a Unity physics Raycast to determine which scene objects with a collider is currently being gazed at, if any.
        /// </summary>
        private void RaycastPhysics(IPointingSource pointer, LayerMask[] prioritizedLayerMasks)
        {
            bool isHit = false;
            int rayStepIndex = 0;
            RayStep rayStep = default(RayStep);
            RaycastHit physicsHit = default(RaycastHit);

            Debug.Assert(pointer.Rays != null, "No valid rays for " + pointer.GetType());
            Debug.Assert(pointer.Rays.Length > 0, "No valid rays for " + pointer.GetType());

            // Check raycast for each step in the pointing source
            for (int i = 0; i < pointer.Rays.Length; i++)
            {
                if (RaycastPhysicsStep(pointer.Rays[i], prioritizedLayerMasks, out physicsHit))
                {
                    // Set the pointer source's origin ray to this step
                    isHit = true;
                    rayStep = pointer.Rays[i];
                    rayStepIndex = i;
                    // No need to continue once we've hit something
                    break;
                }
            }

            if (isHit)
            {
                pointer.Result.UpdateHit(physicsHit, rayStep, rayStepIndex);
            }
            else
            {
                pointer.Result.UpdateHit(GetPointingExtent(pointer));
            }
        }

        private bool RaycastPhysicsStep(RayStep step, LayerMask[] prioritizedLayerMasks, out RaycastHit physicsHit)
        {
            bool isHit = false;
            physicsHit = default(RaycastHit);

            // If there is only one priority, don't prioritize
            if (prioritizedLayerMasks.Length == 1)
            {
                isHit = Physics.Raycast(step.origin, step.direction, out physicsHit, step.length, prioritizedLayerMasks[0]);
            }
            else
            {
                // Raycast across all layers and prioritize
                RaycastHit? hit = PrioritizeHits(Physics.RaycastAll(step.origin, step.direction, step.length, Physics.AllLayers), prioritizedLayerMasks);
                isHit = hit.HasValue;

                if (isHit)
                {
                    physicsHit = hit.Value;
                }
            }

            return isHit;
        }

        private void RaycastUnityUI(IPointingSource pointer, LayerMask[] prioritizedLayerMasks)
        {
            Debug.Assert(pointer.Result.Point != Vector3.zero, string.Format("No pointer {0} end point found to raycast against!", pointer.GetType()));
            Debug.Assert(UIRaycastCamera != null, "You must assign a UIRaycastCamera on the FocusManager before you can process uGUI raycasting.");

            RaycastResult uiRaycastResult = default(RaycastResult);
            bool overridePhysicsRaycast = false;
            RayStep rayStep = default(RayStep);
            int rayStepIndex = 0;

            Debug.Assert(pointer.Rays != null, "No valid rays for " + pointer.GetType());
            Debug.Assert(pointer.Rays.Length > 0, "No valid rays for " + pointer.GetType());

            // Cast rays for every step until we score a hit
            for (int i = 0; i < pointer.Rays.Length; i++)
            {
                if (RaycastUnityUIStep(pointer, pointer.Rays[i], prioritizedLayerMasks, out overridePhysicsRaycast, out uiRaycastResult))
                {
                    rayStepIndex = i;
                    rayStep = pointer.Rays[i];
                    break;
                }
            }

            // Check if we need to overwrite the physics raycast info
            if ((pointer.Result.Target == null || overridePhysicsRaycast) && uiRaycastResult.isValid && uiRaycastResult.module.eventCamera == UIRaycastCamera)
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

                pointer.Result.UpdateHit(uiRaycastResult, hitInfo, rayStep, rayStepIndex);
            }
        }

        private bool RaycastUnityUIStep(IPointingSource pointer, RayStep step, LayerMask[] prioritizedLayerMasks, out bool overridePhysicsRaycast, out RaycastResult uiRaycastResult)
        {
            // Move the uiRaycast camera to the the current pointer's position.
            UIRaycastCamera.transform.position = step.origin;
            UIRaycastCamera.transform.forward = step.direction;

            // We always raycast from the center of the camera.
            pointer.Result.UnityUIPointerData.position = new Vector2(UIRaycastCamera.pixelWidth * 0.5f, UIRaycastCamera.pixelHeight * 0.5f);

            // Graphics raycast
            uiRaycastResult = EventSystem.current.Raycast(pointer.Result.UnityUIPointerData, prioritizedLayerMasks);
            pointer.Result.UnityUIPointerData.pointerCurrentRaycast = uiRaycastResult;

            overridePhysicsRaycast = false;

            // If we have a raycast result, check if we need to overwrite the physics raycast info
            if (uiRaycastResult.gameObject != null)
            {
                if (pointer.Result.Target != null)
                {
                    // Check layer prioritization
                    if (prioritizedLayerMasks.Length > 1)
                    {
                        // Get the index in the prioritized layer masks
                        int uiLayerIndex = uiRaycastResult.gameObject.layer.FindLayerListIndex(prioritizedLayerMasks);
                        int threeDLayerIndex = pointer.Result.LastRaycastHit.collider.gameObject.layer.FindLayerListIndex(prioritizedLayerMasks);

                        if (threeDLayerIndex > uiLayerIndex)
                        {
                            overridePhysicsRaycast = true;
                        }
                        else if (threeDLayerIndex == uiLayerIndex)
                        {
                            if (pointer.Result.LastRaycastHit.distance > uiRaycastResult.distance)
                            {
                                overridePhysicsRaycast = true;
                            }
                        }
                    }
                    else
                    {
                        if (pointer.Result.LastRaycastHit.distance > uiRaycastResult.distance)
                        {
                            overridePhysicsRaycast = true;
                        }
                    }
                }
                // If we've hit somthing, no need to go further
                return true;
            }
            // If we haven't hit something, keep going
            return false;
        }

        private void UpdateFocusedObjects()
        {
            Debug.Assert(pendingFocusExitSet.Count == 0);
            Debug.Assert(pendingFocusEnterSet.Count == 0);

            Debug.Assert(pendingGainFocusSet.Count == 0);
            Debug.Assert(pendingLoseFocusSet.Count == 0);

            // NOTE: We compute the set of events to send before sending the first event
            //       just in case someone responds to the event by adding/removing a
            //       pointer which would change the structures we're iterating over.

            // Clear our current focus objects
            currentFocusTargets.Clear();
            // Copy our active set to our previous active set, then clear the active set for use
            prevActiveFocusTargets.Clear();
            prevActiveFocusTargets.UnionWith(activeFocusTargets);
            activeFocusTargets.Clear();

            for (int iFocuser = 0; iFocuser < focusers.Count; iFocuser++)
            {
                IFocuser focuser = focusers[iFocuser];

                // Check for an active focus target
                if (focuser.Result.Target != null)
                {
                    IFocusTarget focusTarget = null;
                    if (GetFocusTargetFromGameObject(focuser.Result.Target, out focusTarget))
                    {
                        // Add this to our current focus targets regardless of whether focus is enabled
                        currentFocusTargets.Add(focusTarget);
                        // Add this to our active focus targets if focus is enabled
                        if (focusTarget.FocusEnabled == true)
                        {
                            activeFocusTargets.Add(focusTarget);
                        }
                    }
                }

                if (focuser.Result.PreviousTarget != focuser.Result.Target)
                {                    
                    // Initially, we assume all pointer-specific focus changes will result
                    // also result in an overall focus change...

                    if (focuser.Result.PreviousTarget != null)
                    {
                        pendingFocusExitSet.Add(new InputManager.FocusEvent(focuser, focuser.Result.PreviousTarget));
                    }

                    if (focuser.Result.Target != null)
                    {
                        pendingFocusEnterSet.Add(new InputManager.FocusEvent(focuser, focuser.Result.Target));

                    }
                }
            }

            // First go through each of the previous active focus targets and see if they're in the current
            // If they're not, focus has been lost (or has been disabled)
            foreach (IFocusTarget prevActiveFocusTarget in prevActiveFocusTargets)
            {
                if (!activeFocusTargets.Contains(prevActiveFocusTarget))
                {
                    pendingLoseFocusSet.Add(prevActiveFocusTarget);
                }
            }

            // Then go through each of the active focus targets and see if they're in the previous
            // If they're not, focus has been gained (or has been enabled)
            foreach (IFocusTarget activeFocusTarget in activeFocusTargets)
            {
                if (!prevActiveFocusTargets.Contains(activeFocusTarget))
                {
                    pendingGainFocusSet.Add(activeFocusTarget);
                }
            }

            // Now we raise the events:

            foreach (InputManager.FocusEvent exit in pendingFocusExitSet)
            {
                RaiseFocusExitedEvents(exit);
            }

            foreach (InputManager.FocusEvent enter in pendingFocusEnterSet)
            {
                RaiseFocusEnteredEventsInfo(enter);
            }

            // Handle our global focus gained / lost events
            if (FocusGained != null)
            {
                foreach (IFocusTarget focusedObject in pendingGainFocusSet)
                {
                    FocusGained(focusedObject);
                }
            }

            if (FocusLost != null)
            {
                foreach (IFocusTarget unfocusedObject in pendingLoseFocusSet)
                {
                    FocusLost(unfocusedObject);
                }
            }
            
            pendingFocusEnterSet.Clear();
            pendingFocusExitSet.Clear();

            pendingGainFocusSet.Clear();
            pendingLoseFocusSet.Clear();
        }

        private void RaiseFocusEnteredEventsInfo(InputManager.FocusEvent focusedEvent)
        {
            InputManager.Instance.RaiseFocusEnter(focusedEvent);
            //Debug.Log("Focus Enter: " + focusedObject.name);
            if (FocusEnteredInfo != null)
            {
                FocusEnteredInfo(focusedEvent);
            }
        }

        private void RaiseFocusExitedEvents(InputManager.FocusEvent unfocusedEvent)
        {
            InputManager.Instance.RaiseFocusExit(unfocusedEvent);
            //Debug.Log("Focus Exit: " + unfocusedObject.name);
            if (FocusExitedInfo != null)
            {
                FocusExitedInfo(unfocusedEvent);
            }
        }

        private void RaiseFocusChangedEvents(IFocuser focuser, GameObject oldFocusedObject, GameObject newFocusedObject)
        {
            InputManager.Instance.RaiseFocusChangedEvents(focuser, oldFocusedObject, newFocusedObject);

            if (FocusChanged != null)
            {
                FocusChanged(focuser, oldFocusedObject, newFocusedObject);
            }
        }

        private bool TryGetFocuserIndex(IFocuser focuser, out int focuserIndex)
        {
            for (int i = 0; i < focusers.Count; i++)
            {
                if (focuser == focusers[i])
                {
                    focuserIndex = i;
                    return true;
                }
            }

            focuserIndex = -1;
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

        public static bool GetFocusTargetFromGameObject (GameObject gameObject, out IFocusTarget target)
        {
            // TODO: Discuss restrictions on IFocusTarget objects to avoid this costly search
            // Perhaps requiring a collider on the same object as the IFocusTarget script?

            target = (IFocusTarget)gameObject.GetComponent(typeof(IFocusTarget));

            if (target == null)
            {
                // See if any objects on the parent are a focus target
                while (target == null && gameObject.transform.parent != null)
                {
                    gameObject = gameObject.transform.parent.gameObject;
                    target = (IFocusTarget)gameObject.GetComponent(typeof(IFocusTarget));
                }
            }

            return target != null;
        }

        #endregion
    }
}
