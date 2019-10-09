// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityPhysics = UnityEngine.Physics;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// The focus provider handles the focused objects per input source.
    /// </summary>
    /// <remarks>There are convenience properties for getting only Gaze Pointer if needed.</remarks>
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Input/Overview.html")]
    public class FocusProvider : BaseCoreSystem, 
        IMixedRealityFocusProvider, 
        IPointerPreferences
    {
        public FocusProvider(
            IMixedRealityServiceRegistrar registrar,
            MixedRealityInputSystemProfile profile) : base(registrar, profile)
        {
            maxQuerySceneResults = profile.FocusQueryBufferSize;
            focusIndividualCompoundCollider = profile.FocusIndividualCompoundCollider;
        }

        private readonly HashSet<PointerData> pointers = new HashSet<PointerData>();
        private readonly HashSet<GameObject> pendingOverallFocusEnterSet = new HashSet<GameObject>();
        private readonly Dictionary<GameObject, int> pendingOverallFocusExitSet = new Dictionary<GameObject, int>();
        private readonly List<PointerData> pendingPointerSpecificFocusChange = new List<PointerData>();
        private readonly Dictionary<uint, IMixedRealityPointerMediator> pointerMediators = new Dictionary<uint, IMixedRealityPointerMediator>();
        private readonly PointerHitResult hitResult3d = new PointerHitResult();
        private readonly PointerHitResult hitResultUi = new PointerHitResult();

        private readonly int maxQuerySceneResults = 128;
        private bool focusIndividualCompoundCollider = false;

        public IReadOnlyDictionary<uint, IMixedRealityPointerMediator> PointerMediators => pointerMediators;

        /// <summary>
        /// Number of IMixedRealityNearPointers that are active (IsInteractionEnabled == true).
        /// </summary>
        public int NumNearPointersActive { get; private set; }

        /// <summary>
        /// The number of pointers that support far interaction (like motion controller rays, hand rays) that 
        /// are active (IsInteractionEnabled == true), excluding the gaze cursor
        /// </summary>
        public int NumFarPointersActive { get; private set; }

        private IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        private IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                }
                return inputSystem;
            }
        }

        private IMixedRealityPointer primaryPointer;

        public IMixedRealityPointer PrimaryPointer
        {
            get { return primaryPointer; }
            private set
            {
                if (value != PrimaryPointer)
                {
                    IMixedRealityPointer oldPointer = primaryPointer;
                    primaryPointer = value;
                    PrimaryPointerChanged?.Invoke(oldPointer, primaryPointer);
                }
            }
        }

        #region IFocusProvider Properties

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "Focus Provider";

        /// <inheritdoc />
        public override uint Priority => 2;

        /// <inheritdoc />
        float IMixedRealityFocusProvider.GlobalPointingExtent
        {
            get
            {
                MixedRealityInputSystemProfile profile = ConfigurationProfile as MixedRealityInputSystemProfile;

                if (profile != null && profile.PointerProfile != null)
                {
                    return profile.PointerProfile.PointingExtent;
                }

                return 10f;
            }
        }

        private LayerMask[] focusLayerMasks = null;

        /// <inheritdoc />
        public LayerMask[] FocusLayerMasks
        {
            get
            {
                if (focusLayerMasks == null)
                {
                    MixedRealityInputSystemProfile profile = ConfigurationProfile as MixedRealityInputSystemProfile;

                    if (profile != null && profile.PointerProfile != null)
                    {
                        return focusLayerMasks = profile.PointerProfile.PointingRaycastLayerMasks;
                    }

                    return focusLayerMasks = new LayerMask[] { UnityPhysics.DefaultRaycastLayers };
                }

                return focusLayerMasks;
            }
        }

        private RenderTexture uiRaycastCameraTargetTexture = null;
        private Camera uiRaycastCamera = null;

        /// <inheritdoc />
        public Camera UIRaycastCamera => uiRaycastCamera;

        #endregion IFocusProvider Properties

        /// <summary>
        /// Checks if the <see cref="MixedRealityToolkit"/> is setup correctly to start this service.
        /// </summary>
        private bool IsSetupValid
        {
            get
            {
                if (InputSystem == null)
                {
                    Debug.LogError($"Unable to start {Name}. An Input System is required for this feature.");
                    return false;
                }

                MixedRealityInputSystemProfile profile = ConfigurationProfile as MixedRealityInputSystemProfile;

                if (profile == null)
                {
                    Debug.LogError($"Unable to start {Name}. An Input System Profile is required for this feature.");
                    return false;
                }

                if (profile.PointerProfile == null)
                {
                    Debug.LogError($"Unable to start {Name}. An Pointer Profile is required for this feature.");
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// GazeProvider is a little special, so we keep track of it even if it's not a registered pointer. For the sake
        /// of StabilizationPlaneModifier and potentially other components that care where the user's looking, we need
        /// to do a gaze raycast even if gaze isn't used for focus.
        /// </summary>
        private PointerData gazeProviderPointingData;
        private PointerHitResult gazeHitResult;

        /// <summary>
        /// Cached <see href="https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> reference to the new raycast position.
        /// </summary>
        /// <remarks>Only used to update UI raycast results.</remarks>
        private Vector3 newUiRaycastPosition = Vector3.zero;

        /// <summary>
        /// Helper class for storing intermediate hit results. Should be applied to the PointerData once all
        /// possible hits of a pointer have been processed.
        /// </summary>
        private class PointerHitResult
        {
            public MixedRealityRaycastHit raycastHit;
            public RaycastResult graphicsRaycastResult;

            public GameObject hitObject;
            public Vector3 hitPointOnObject = Vector3.zero;
            public Vector3 hitNormalOnObject = Vector3.zero;

            public RayStep ray;
            public int rayStepIndex = -1;
            public float rayDistance;

            public void Clear()
            {
                raycastHit = default(MixedRealityRaycastHit);
                graphicsRaycastResult = default(RaycastResult);

                hitObject = null;
                hitPointOnObject = Vector3.zero;
                hitNormalOnObject = Vector3.zero;

                ray = default(RayStep);
                rayStepIndex = -1;
                rayDistance = 0.0f;
            }

            /// <summary>
            /// Set hit focus information from a closest-colliders-to pointer check.
            /// </summary>
            public void Set(GameObject hitObject, Vector3 hitPointOnObject, Vector4 hitNormalOnObject, RayStep ray, int rayStepIndex, float rayDistance)
            {
                raycastHit = default(MixedRealityRaycastHit);
                graphicsRaycastResult = default(RaycastResult);

                this.hitObject = hitObject;
                this.hitPointOnObject = hitPointOnObject;
                this.hitNormalOnObject = hitNormalOnObject;

                this.ray = ray;
                this.rayStepIndex = rayStepIndex;
                this.rayDistance = rayDistance;
            }

            /// <summary>
            /// Set hit focus information from a physics raycast.
            /// </summary>
            public void Set(MixedRealityRaycastHit hit, RayStep ray, int rayStepIndex, float rayDistance, bool focusIndividualCompoundCollider)
            {
                raycastHit = hit;
                graphicsRaycastResult = default(RaycastResult);

                hitObject = focusIndividualCompoundCollider? hit.collider.gameObject : hit.transform.gameObject;
                hitPointOnObject = hit.point;
                hitNormalOnObject = hit.normal;

                this.ray = ray;
                this.rayStepIndex = rayStepIndex;
                this.rayDistance = rayDistance;
            }

            /// <summary>
            /// Set hit information from a canvas raycast.
            /// </summary>
            public void Set(RaycastResult result, Vector3 hitPointOnObject, Vector4 hitNormalOnObject, RayStep ray, int rayStepIndex, float rayDistance)
            {
                raycastHit = default(MixedRealityRaycastHit);
                graphicsRaycastResult = result;

                this.hitObject = result.gameObject;
                this.hitPointOnObject = hitPointOnObject;
                this.hitNormalOnObject = hitNormalOnObject;

                this.ray = ray;
                this.rayStepIndex = rayStepIndex;
                this.rayDistance = rayDistance;
            }
        }

        [Serializable]
        private class PointerData : IPointerResult, IEquatable<PointerData>
        {
            public readonly IMixedRealityPointer Pointer;

            /// <inheritdoc />
            public Vector3 StartPoint { get; private set; }

            /// <inheritdoc />
            public FocusDetails Details
            {
                get
                {
                    return focusDetails;
                }
                set
                {
                    focusDetails = value;
                }
            }

            /// <inheritdoc />
            public GameObject CurrentPointerTarget => focusDetails.Object;

            /// <inheritdoc />
            public GameObject PreviousPointerTarget { get; private set; }

            /// <inheritdoc />
            public int RayStepIndex { get; private set; }

            /// <summary>
            /// The graphic input event data used for raycasting uGUI elements.
            /// </summary>
            public PointerEventData GraphicEventData
            {
                get
                {
                    if (graphicData == null)
                    {
                        graphicData = new PointerEventData(EventSystem.current);
                    }

                    Debug.Assert(graphicData != null);

                    return graphicData;
                }
            }
            private PointerEventData graphicData;

            /// <summary>
            /// Returns true if the current pointer target has been disabled or destroyed
            /// </summary>
            public bool IsCurrentPointerTargetInvalid => ((CurrentPointerTarget != null && !CurrentPointerTarget.activeInHierarchy)) ||
                (CurrentPointerTarget == null && !ReferenceEquals(CurrentPointerTarget, null));

            private FocusDetails focusDetails = new FocusDetails();

            public PointerData(IMixedRealityPointer pointer)
            {
                Pointer = pointer;
            }

            public void UpdateHit(PointerHitResult hitResult)
            {
                if (hitResult.hitObject != CurrentPointerTarget)
                {
                    Pointer.OnPreCurrentPointerTargetChange();

                    // Set to default:
                    Pointer.IsTargetPositionLockedOnFocusLock = true;
                }

                PreviousPointerTarget = CurrentPointerTarget;

                focusDetails.Object = hitResult.hitObject;
                focusDetails.LastRaycastHit = hitResult.raycastHit;
                focusDetails.LastGraphicsRaycastResult = hitResult.graphicsRaycastResult;

                if (hitResult.rayStepIndex >= 0)
                {
                    RayStepIndex = hitResult.rayStepIndex;
                    StartPoint = hitResult.ray.Origin;

                    focusDetails.RayDistance = hitResult.rayDistance;
                    focusDetails.Point = hitResult.hitPointOnObject;
                    focusDetails.Normal = hitResult.hitNormalOnObject;
                }
                else
                {
                    // If we don't have a valid ray cast, use the whole pointer ray.
                    RayStep firstStep = Pointer.Rays[0];
                    RayStep finalStep = Pointer.Rays[Pointer.Rays.Length - 1];
                    RayStepIndex = 0;

                    StartPoint = firstStep.Origin;

                    float rayDist = 0.0f;
                    for (int i = 0; i < Pointer.Rays.Length; i++)
                    {
                        rayDist += Pointer.Rays[i].Length;
                    }

                    focusDetails.RayDistance = rayDist;
                    focusDetails.Point = finalStep.Terminus;
                    focusDetails.Normal = -finalStep.Direction;
                }

                if (hitResult.hitObject != null)
                {
                    focusDetails.PointLocalSpace = hitResult.hitObject.transform.InverseTransformPoint(focusDetails.Point);
                    focusDetails.NormalLocalSpace = hitResult.hitObject.transform.InverseTransformDirection(focusDetails.Normal);
                }
                else
                {
                    focusDetails.PointLocalSpace = Vector3.zero;
                    focusDetails.NormalLocalSpace = Vector3.zero;
                }
            }

            /// <summary>
            /// Update focus information while focus is locked. If the object is moving,
            /// this updates the hit point to its new world transform.
            /// </summary>
            public void UpdateFocusLockedHit()
            {
                PreviousPointerTarget = focusDetails.Object;

                if (focusDetails.Object != null && focusDetails.Object.transform != null)
                {
                    // In case the focused object is moving, we need to update the focus point based on the object's new transform.
                    focusDetails.Point = focusDetails.Object.transform.TransformPoint(focusDetails.PointLocalSpace);
                    focusDetails.Normal = focusDetails.Object.transform.TransformDirection(focusDetails.NormalLocalSpace);
                    focusDetails.PointLocalSpace = focusDetails.Object.transform.InverseTransformPoint(focusDetails.Point);
                    focusDetails.NormalLocalSpace = focusDetails.Object.transform.InverseTransformDirection(focusDetails.Normal);
                }

                StartPoint = Pointer.Rays[0].Origin;

                for (int i = 0; i < Pointer.Rays.Length; i++)
                {
                    // TODO: figure out how reliable this is. Should focusDetails.RayDistance be updated?
                    if (Pointer.Rays[i].Contains(focusDetails.Point))
                    {
                        RayStepIndex = i;
                        break;
                    }
                }
            }

            public void ResetFocusedObjects(bool clearPreviousObject = true)
            {
                if (CurrentPointerTarget != null)
                {
                    Pointer.OnPreCurrentPointerTargetChange();
                }

                PreviousPointerTarget = clearPreviousObject ? null : CurrentPointerTarget;

                focusDetails.Point = Details.Point;
                focusDetails.Normal = Details.Normal;
                focusDetails.NormalLocalSpace = Details.NormalLocalSpace;
                focusDetails.PointLocalSpace = Details.PointLocalSpace;
                focusDetails.Object = null;
            }

            /// <inheritdoc />
            public bool Equals(PointerData other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return Pointer.PointerId == other.Pointer.PointerId;
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != GetType())
                {
                    return false;
                }

                return Equals((PointerData)obj);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return Pointer != null ? Pointer.GetHashCode() : 0;
            }
        }

        private readonly GazePointerVisibilityStateMachine gazePointerStateMachine = new GazePointerVisibilityStateMachine();

        /// <summary>
        /// Interface used for selecting the primary pointer.
        /// </summary>
        private IMixedRealityPrimaryPointerSelector primaryPointerSelector;

        /// <summary>
        /// Event raised on primary pointer changes.
        /// </summary>
        private event PrimaryPointerChangedHandler PrimaryPointerChanged;

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            if (!IsSetupValid) { return; }

            if (Application.isPlaying)
            {
                Debug.Assert(uiRaycastCamera == null);
                FindOrCreateUiRaycastCamera();
            }

            var primaryPointerSelectorType = InputSystem?.InputSystemProfile.PointerProfile.PrimaryPointerSelector.Type;
            if (primaryPointerSelectorType != null)
            {
                primaryPointerSelector = Activator.CreateInstance(primaryPointerSelectorType) as IMixedRealityPrimaryPointerSelector;
                primaryPointerSelector.Initialize();
            }

            foreach (var inputSource in InputSystem.DetectedInputSources)
            {
                RegisterPointers(inputSource);
            }
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            if (primaryPointerSelector != null)
            {
                primaryPointerSelector.Destroy();
            }

            CleanUpUiRaycastCamera();
            base.Destroy();
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (!IsSetupValid) { return; }

            UpdatePointers();

            if (gazeProviderPointingData?.Pointer != null)
            {
                UpdateGazeProvider();
            }

            UpdateFocusedObjects();

            PrimaryPointer = primaryPointerSelector?.Update();
        }

        /// <summary>
        /// Updates the gaze raycast provider even in scenarios where gaze isn't used for focus
        /// </summary>
        private void UpdateGazeProvider()
        {
            // The gaze hit result may be populated from previous raycasts this frame, only recompute
            // another raycast if it's not populated
            if (gazeHitResult == null)
            {
                hitResult3d.Clear();
                var raycastProvider = InputSystem.RaycastProvider;
                LayerMask[] prioritizedLayerMasks = (gazeProviderPointingData.Pointer.PrioritizedLayerMasksOverride ?? FocusLayerMasks);
                QueryScene(gazeProviderPointingData.Pointer, raycastProvider, prioritizedLayerMasks,
                    hitResult3d, maxQuerySceneResults, focusIndividualCompoundCollider);
                gazeHitResult = hitResult3d;
            }

            InputSystem.GazeProvider.UpdateGazeInfoFromHit(gazeHitResult.raycastHit);

            // Zero out value after every use to ensure the hit result is updated every frame.
            gazeHitResult = null;
        }

        #endregion IMixedRealityService Implementation

        #region Focus Details by IMixedRealityPointer

        /// <inheritdoc />
        public GameObject GetFocusedObject(IMixedRealityPointer pointingSource)
        {
            if (pointingSource == null)
            {
                Debug.LogError("No Pointer passed to get focused object");
                return null;
            }

            FocusDetails focusDetails;
            if (!TryGetFocusDetails(pointingSource, out focusDetails)) { return null; }

            return focusDetails.Object;
        }

        /// <inheritdoc />
        public bool TryGetFocusDetails(IMixedRealityPointer pointer, out FocusDetails focusDetails)
        {
            PointerData pointerData;
            if (TryGetPointerData(pointer, out pointerData))
            {
                focusDetails = pointerData.Details;
                return true;
            }

            focusDetails = default(FocusDetails);
            return false;
        }

        /// <inheritdoc />
        public bool TryOverrideFocusDetails(IMixedRealityPointer pointer, FocusDetails focusDetails)
        {
            if (TryGetPointerData(pointer, out PointerData pointerData))
            {
                pointerData.Details = focusDetails;
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion Focus Details by IMixedRealityPointer

        #region Utilities

        /// <inheritdoc />
        public uint GenerateNewPointerId()
        {
            var newId = (uint)UnityEngine.Random.Range(1, int.MaxValue);

            foreach (var pointerData in pointers)
            {
                if (pointerData.Pointer.PointerId == newId)
                {
                    return GenerateNewPointerId();
                }
            }

            return newId;
        }

        /// <summary>
        /// Utility for creating the UIRaycastCamera.
        /// </summary>
        /// <returns>The UIRaycastCamera</returns>
        private void FindOrCreateUiRaycastCamera()
        {
            GameObject cameraObject = null;

            var existingUiRaycastCameraObject = GameObject.Find("UIRaycastCamera");
            if (existingUiRaycastCameraObject != null)
            {
                cameraObject = existingUiRaycastCameraObject;
            }
            else
            {
                cameraObject = new GameObject { name = "UIRaycastCamera" };
            }

            uiRaycastCamera = cameraObject.EnsureComponent<Camera>();
            uiRaycastCamera.enabled = false;
            uiRaycastCamera.clearFlags = CameraClearFlags.Color;
            uiRaycastCamera.backgroundColor = new Color(0, 0, 0, 1);
            uiRaycastCamera.cullingMask = CameraCache.Main.cullingMask;
            uiRaycastCamera.orthographic = true;
            uiRaycastCamera.orthographicSize = 0.5f;
            uiRaycastCamera.nearClipPlane = 0.0f;
            uiRaycastCamera.farClipPlane = 1000f;
            uiRaycastCamera.rect = new Rect(0, 0, 1, 1);
            uiRaycastCamera.depth = 0;
            uiRaycastCamera.renderingPath = RenderingPath.UsePlayerSettings;
            uiRaycastCamera.useOcclusionCulling = false;
            uiRaycastCamera.allowHDR = false;
            uiRaycastCamera.allowMSAA = false;
            uiRaycastCamera.allowDynamicResolution = false;
            uiRaycastCamera.targetDisplay = 0;
            uiRaycastCamera.stereoTargetEye = StereoTargetEyeMask.Both;

            if (uiRaycastCameraTargetTexture == null)
            {
                // Set target texture to specific pixel size so that drag thresholds are treated the same regardless of underlying
                // device display resolution.
                uiRaycastCameraTargetTexture = new RenderTexture(128, 128, 0);
            }

            uiRaycastCamera.targetTexture = uiRaycastCameraTargetTexture;
        }

        private void CleanUpUiRaycastCamera()
        {
            if (uiRaycastCameraTargetTexture != null)
            {
                UnityEngine.Object.Destroy(uiRaycastCameraTargetTexture);
            }
            uiRaycastCameraTargetTexture = null;

            if (uiRaycastCamera != null)
            {
                UnityEngine.Object.Destroy(uiRaycastCamera.gameObject);
            }
            uiRaycastCamera = null;
        }

        /// <inheritdoc />
        public bool IsPointerRegistered(IMixedRealityPointer pointer)
        {
            Debug.Assert(pointer.PointerId != 0, $"{pointer} does not have a valid pointer id!");
            PointerData pointerData;
            return TryGetPointerData(pointer, out pointerData);
        }

        /// <inheritdoc />
        public bool RegisterPointer(IMixedRealityPointer pointer)
        {
            Debug.Assert(pointer.PointerId != 0, $"{pointer} does not have a valid pointer id!");

            if (IsPointerRegistered(pointer)) { return false; }

            pointers.Add(new PointerData(pointer));

            if (primaryPointerSelector != null)
            {
                primaryPointerSelector.RegisterPointer(pointer);
            }

            return true;
        }

        private void RegisterPointers(IMixedRealityInputSource inputSource)
        {
            // If our input source does not have any pointers, then skip.
            if (inputSource.Pointers == null) { return; }

            IMixedRealityPointerMediator mediator = null;

            var mediatorType = InputSystem?.InputSystemProfile.PointerProfile.PointerMediator.Type;
            if (mediatorType != null)
            {
                try
                {
                    // First, try to use constructor used by DefaultPointerMediator (it takes a IPointePreferences)
                    mediator = Activator.CreateInstance(
                    InputSystem.InputSystemProfile.PointerProfile.PointerMediator.Type,
                    this) as IMixedRealityPointerMediator;
                }
                catch (MissingMethodException)
                {
                    // We are using custom mediator not provided by MRTK, instantiate with empty constructor
                    mediator = Activator.CreateInstance(
                        InputSystem.InputSystemProfile.PointerProfile.PointerMediator.Type) as IMixedRealityPointerMediator;
                }
            }

            if (mediator != null)
            {
                mediator.RegisterPointers(inputSource.Pointers);

                if (!pointerMediators.ContainsKey(inputSource.SourceId))
                {
                    pointerMediators.Add(inputSource.SourceId, mediator);
                }
            }

            for (int i = 0; i < inputSource.Pointers.Length; i++)
            {
                RegisterPointer(inputSource.Pointers[i]);

                // Special Registration for Gaze
                if (inputSource.SourceId == InputSystem.GazeProvider.GazeInputSource.SourceId && gazeProviderPointingData == null)
                {
                    gazeProviderPointingData = new PointerData(inputSource.Pointers[i]);
                }
            }
        }

        /// <inheritdoc />
        public bool UnregisterPointer(IMixedRealityPointer pointer)
        {
            Debug.Assert(pointer.PointerId != 0, $"{pointer} does not have a valid pointer id!");

            PointerData pointerData;
            if (!TryGetPointerData(pointer, out pointerData)) { return false; }

            // Raise focus events if needed.
            if (pointerData.CurrentPointerTarget != null)
            {
                GameObject unfocusedObject = pointerData.CurrentPointerTarget;
                bool objectIsStillFocusedByOtherPointer = false;

                foreach (var otherPointer in pointers)
                {
                    if (otherPointer.Pointer != pointer && otherPointer.CurrentPointerTarget == unfocusedObject)
                    {
                        objectIsStillFocusedByOtherPointer = true;
                        break;
                    }
                }

                InputSystem?.RaisePreFocusChanged(pointer, unfocusedObject, null);

                if (!objectIsStillFocusedByOtherPointer)
                {
                    // Policy: only raise focus exit if no other pointers are still focusing the object
                    InputSystem?.RaiseFocusExit(pointer, unfocusedObject);
                }

                InputSystem?.RaiseFocusChanged(pointer, unfocusedObject, null);
            }

            pointers.Remove(pointerData);

            if (primaryPointerSelector != null)
            {
                primaryPointerSelector.UnregisterPointer(pointer);
                PrimaryPointer = primaryPointerSelector.Update();
            }

            return true;
        }

        /// <inheritdoc />
        public IEnumerable<T> GetPointers<T>() where T : class, IMixedRealityPointer
        {
            List<T> typePointers = new List<T>();
            foreach (var pointer in pointers)
            {
                T typePointer = pointer.Pointer as T;
                if (typePointer != null)
                {
                    typePointers.Add(typePointer);
                }
            }

            return typePointers;
        }

        public void SubscribeToPrimaryPointerChanged(PrimaryPointerChangedHandler handler, bool invokeHandlerWithCurrentPointer)
        {
            if (invokeHandlerWithCurrentPointer)
            {
                handler(null, PrimaryPointer);
            }

            PrimaryPointerChanged += handler;
        }

        public void UnsubscribeFromPrimaryPointerChanged(PrimaryPointerChangedHandler handler)
        {
            PrimaryPointerChanged -= handler;
        }

        /// <summary>
        /// Returns the registered PointerData for the provided pointing input source.
        /// </summary>
        /// <param name="pointer">the pointer who's data we're looking for</param>
        /// <param name="data">The data associated to the pointer</param>
        /// <returns>Pointer Data if the pointing source is registered.</returns>
        private bool TryGetPointerData(IMixedRealityPointer pointer, out PointerData data)
        {
            foreach (var pointerData in pointers)
            {
                if (pointerData.Pointer.PointerId == pointer.PointerId)
                {
                    data = pointerData;
                    return true;
                }
            }

            data = null;
            return false;
        }

        private void UpdatePointers()
        {
            MixedRealityInputSystemProfile profile = ConfigurationProfile as MixedRealityInputSystemProfile;
            if (profile == null) { return; }

            ReconcilePointers();

            foreach (var pointerMediator in pointerMediators)
            {
                pointerMediator.Value.UpdatePointers();
            }

#if UNITY_EDITOR
            int pointerCount = 0;
#endif
            foreach (var pointerData in pointers)
            {
                UpdatePointer(pointerData);

#if UNITY_EDITOR
                var pointerProfile = profile.PointerProfile;
                if (pointerProfile != null && pointerProfile.DebugDrawPointingRays)
                {
                    MixedRealityRaycaster.DebugEnabled = pointerProfile.DebugDrawPointingRays;

                    Color rayColor;
                    if ((pointerProfile.DebugDrawPointingRayColors != null) && (pointerProfile.DebugDrawPointingRayColors.Length > 0))
                    {
                        rayColor = pointerProfile.DebugDrawPointingRayColors[pointerCount++ % pointerProfile.DebugDrawPointingRayColors.Length];
                    }
                    else
                    {
                        rayColor = Color.green;
                    }

                    if (!pointerData.Pointer.IsActive)
                    {
                        // Only draw pointers that are currently active, but make sure to 
                        // increment color even if pointer is disabled so that the color for e.g. the 
                        // sphere pointer or the poke pointer remains consistent.
                        continue;
                    }

                    Debug.DrawRay(pointerData.StartPoint, (pointerData.Details.Point - pointerData.StartPoint), rayColor);
                }
#endif
            }
        }

        private void UpdatePointer(PointerData pointerData)
        {
            // Call the pointer's OnPreSceneQuery function
            // This will give it a chance to prepare itself for raycasts
            // e.g., by building its Rays array
            pointerData.Pointer.OnPreSceneQuery();

            // If pointer interaction isn't enabled, clear its result object and return
            if (!pointerData.Pointer.IsInteractionEnabled)
            {
                // Don't clear the previous focused object since we still want to trigger FocusExit events
                pointerData.ResetFocusedObjects(false);
            }
            else
            {
                LayerMask[] prioritizedLayerMasks = (pointerData.Pointer.PrioritizedLayerMasksOverride ?? FocusLayerMasks);

                if (pointerData.IsCurrentPointerTargetInvalid)
                {
                    pointerData.Pointer.IsFocusLocked = false;
                }

                // If the pointer is locked, keep the focused object the same.
                // This will ensure that we execute events on those objects
                // even if the pointer isn't pointing at them.
                if (pointerData.Pointer.IsFocusLocked && pointerData.Pointer.IsTargetPositionLockedOnFocusLock)
                {
                    pointerData.UpdateFocusLockedHit();

                    // If we have a unity event system, perform graphics raycasts as well to support Unity UI interactions
                    if (EventSystem.current != null)
                    {
                        // NOTE: We need to do this AFTER RaycastPhysics so we use the current hit point to perform the correct 2D UI Raycast.
                        hitResultUi.Clear();
                        RaycastGraphics(pointerData.Pointer, pointerData.GraphicEventData, prioritizedLayerMasks, hitResultUi);
                    }
                }
                else
                {
                    // Perform raycast to determine focused object
                    var raycastProvider = InputSystem.RaycastProvider;
                    hitResult3d.Clear();
                    QueryScene(pointerData.Pointer, raycastProvider, prioritizedLayerMasks, hitResult3d, maxQuerySceneResults, focusIndividualCompoundCollider);

                    if (pointerData.Pointer.PointerId == gazeProviderPointingData.Pointer.PointerId)
                    {
                        gazeHitResult = hitResult3d;
                    }

                    int hitResult3dLayer = hitResult3d.hitObject?.layer ?? -1;
                    if (hitResult3dLayer == 0)
                    {
                        // If we have a hit in the highest priority layer, we can go ahead and truncate the pointer before doing the UI raycast
                        // (if it's not highest priority it's possible the UI raycast could produce a higher-priority hit that is further than the physics hit,
                        // and we'd lose that hit if the pointer were truncated)
                        TruncatePointerRayToHit(pointerData.Pointer, hitResult3d);
                    }

                    PointerHitResult hit = hitResult3d;
                    // If we have a unity event system, perform graphics raycasts as well to support Unity UI interactions
                    if (EventSystem.current != null)
                    {
                        // NOTE: We need to do this AFTER RaycastPhysics so we use the current hit point to perform the correct 2D UI Raycast.
                        hitResultUi.Clear();
                        RaycastGraphics(pointerData.Pointer, pointerData.GraphicEventData, prioritizedLayerMasks, hitResultUi);

                        hit = GetPrioritizedHitResult(hit, hitResultUi, prioritizedLayerMasks);
                    }

                    if (hit != hitResult3d || hitResult3dLayer > 0)
                    {
                        // Truncate if we didn't already for this hit
                        TruncatePointerRayToHit(pointerData.Pointer, hitResult3d);
                    }

                    // Make sure to keep focus on the previous object if focus is locked (no target position lock here).
                    if (pointerData.Pointer.IsFocusLocked && pointerData.Pointer.Result?.CurrentPointerTarget != null)
                    {
                        hit.hitObject = pointerData.Pointer.Result.CurrentPointerTarget;
                    }

                    // Apply the hit result only now so changes in the current target are detected only once per frame.
                    pointerData.UpdateHit(hit);

                    // Set the pointer's result last
                    pointerData.Pointer.Result = pointerData;
                }
            }

            // Call the pointer's OnPostSceneQuery function.
            // This will give it a chance to respond to raycast results
            // e.g., by updating its appearance.
            pointerData.Pointer.OnPostSceneQuery();
        }

        private void TruncatePointerRayToHit(IMixedRealityPointer pointer, PointerHitResult hit)
        {
            if (hit.rayStepIndex >= 0)
            {
                RayStep rayStep = pointer.Rays[hit.rayStepIndex];
                Vector3 origin = rayStep.Origin;
                Vector3 terminus = hit.raycastHit.point;
                rayStep.UpdateRayStep(ref origin, ref terminus);
            }
        }

        PointerHitResult GetPrioritizedHitResult(PointerHitResult hit1, PointerHitResult hit2, LayerMask[] prioritizedLayerMasks)
        {
            if (hit1.hitObject != null && hit2.hitObject != null)
            {
                // Check layer prioritization.
                if (prioritizedLayerMasks.Length > 1)
                {
                    // Get the index in the prioritized layer masks
                    int layerIndex1 = hit1.hitObject.layer.FindLayerListIndex(prioritizedLayerMasks);
                    int layerIndex2 = hit2.hitObject.layer.FindLayerListIndex(prioritizedLayerMasks);

                    if (layerIndex1 != layerIndex2)
                    {
                        return (layerIndex1 < layerIndex2) ? hit1 : hit2;
                    }
                }

                // Check which hit is closer.
                return (hit1.rayDistance < hit2.rayDistance) ? hit1 : hit2;
            }

            return (hit1.hitObject != null) ? hit1 : hit2;
        }

        /// <summary>
        /// Disable inactive pointers to unclutter the way for active ones.
        /// </summary>
        private void ReconcilePointers()
        {
            var gazePointer = gazeProviderPointingData?.Pointer as GenericPointer;
            NumFarPointersActive = 0;
            NumNearPointersActive = 0;
            int numFarPointersWithoutCursorActive = 0;

            foreach (var pointerData in pointers)
            {
                if (pointerData.Pointer is IMixedRealityNearPointer)
                {
                    if (pointerData.Pointer.IsInteractionEnabled)
                    {
                        NumNearPointersActive++;
                    }
                }
                else if (
                    // pointerData.Pointer.BaseCursor == null means this is a GGV Pointer
                    pointerData.Pointer.BaseCursor != null
                    && !(pointerData.Pointer == gazePointer)
                    && pointerData.Pointer.IsInteractionEnabled)
                {
                    // We ignore the currentGazePointer here because for cases like HoloLens 1
                    // hand input or the gamepad, we want to show the cursor still.
                    NumFarPointersActive++;
                }
                else if (pointerData.Pointer.BaseCursor == null
                    && pointerData.Pointer.IsInteractionEnabled)
                {
                    numFarPointersWithoutCursorActive++;
                }
            }
            if (gazePointer != null)
            {
                gazePointerStateMachine.UpdateState(
                    NumNearPointersActive,
                    NumFarPointersActive,
                    numFarPointersWithoutCursorActive,
                    InputSystem.EyeGazeProvider.IsEyeGazeValid);

                // The gaze cursor's visibility is controlled by IsInteractionEnabled
                gazePointer.IsInteractionEnabled = gazePointerStateMachine.IsGazePointerActive;
            }
        }

        #region Physics Raycasting

        // Colliders used to store sphere overlap results
        private static Collider[] colliders = null;

        /// <summary>
        /// Perform a scene query to determine which scene objects with a collider is currently being gazed at, if any.
        /// </summary>
        private static void QueryScene(IMixedRealityPointer pointer, IMixedRealityRaycastProvider raycastProvider, LayerMask[] prioritizedLayerMasks, PointerHitResult hit, int maxQuerySceneResults, bool focusIndividualCompoundCollider)
        {
            float rayStartDistance = 0;
            MixedRealityRaycastHit hitInfo;
            RayStep[] pointerRays = pointer.Rays;

            if (pointerRays == null)
            {
                Debug.LogError($"No valid rays for {pointer.PointerName} pointer.");
                return;
            }

            if (pointerRays.Length <= 0)
            {
                Debug.LogError($"No valid rays for {pointer.PointerName} pointer");
                return;
            }

            // Perform query for each step in the pointing source
            for (int i = 0; i < pointerRays.Length; i++)
            {
                switch (pointer.SceneQueryType)
                {
                    case SceneQueryType.SimpleRaycast:
                        if (raycastProvider.Raycast(pointerRays[i], prioritizedLayerMasks, focusIndividualCompoundCollider, out hitInfo))
                        {
                            hit.Set(hitInfo, pointerRays[i], i, rayStartDistance + hitInfo.distance, focusIndividualCompoundCollider);
                            return;
                        }
                        break;
                    case SceneQueryType.BoxRaycast:
                        Debug.LogWarning("Box Raycasting Mode not supported for pointers.");
                        break;
                    case SceneQueryType.SphereCast:
                        if (raycastProvider.SphereCast(pointerRays[i], pointer.SphereCastRadius, prioritizedLayerMasks, focusIndividualCompoundCollider, out hitInfo))
                        {
                            hit.Set(hitInfo, pointerRays[i], i, rayStartDistance + hitInfo.distance, focusIndividualCompoundCollider);
                            return;
                        }
                        break;
                    case SceneQueryType.SphereOverlap:
                        // Set up our results array
                        if (colliders == null)
                        {
                            colliders = new Collider[maxQuerySceneResults];
                        }
                        else if (colliders.Length != maxQuerySceneResults)
                        {
                            Array.Resize<Collider>(ref colliders, maxQuerySceneResults);
                        }

                        int numColliders = UnityEngine.Physics.OverlapSphereNonAlloc(pointer.Rays[i].Origin, pointer.SphereCastRadius, colliders, ~UnityEngine.Physics.IgnoreRaycastLayer);

                        if (numColliders > 0)
                        {
                            if (numColliders >= maxQuerySceneResults)
                            {
                                Debug.LogWarning($"Maximum number of {numColliders} colliders found in FocusProvider overlap query. Consider increasing the focus query buffer size in the input profile.");
                            }

                            Vector3 testPoint = pointer.Rays[i].Origin;
                            GameObject closest = null;
                            float closestDistance = Mathf.Infinity;
                            Vector3 objectHitPoint = testPoint;

                            for (int colliderIndex = 0; colliderIndex < numColliders; colliderIndex++)
                            {
                                Collider collider = colliders[colliderIndex];
                                // Policy: in order for an collider to be near interactable it must have
                                // a NearInteractionGrabbable component on it.
                                // FIXME: This is assuming only the grab pointer is using SceneQueryType.SphereOverlap,
                                //        but there may be other pointers using the same query type which have different semantics.
                                if (collider.GetComponent<NearInteractionGrabbable>() == null)
                                {
                                    continue;
                                }
                                // From https://docs.unity3d.com/ScriptReference/Collider.ClosestPoint.html
                                // If location is in the collider the closestPoint will be inside.
                                Vector3 closestPointToCollider = collider.ClosestPoint(testPoint);
                                float distance = (testPoint - closestPointToCollider).sqrMagnitude;
                                if (distance < closestDistance)
                                {
                                    closestDistance = distance;
                                    closest = collider.gameObject;
                                    objectHitPoint = closestPointToCollider;
                                }
                            }
                            if (closest != null)
                            {
                                hit.Set(closest, objectHitPoint, Vector3.zero, pointer.Rays[i], 0, closestDistance);
                                return;
                            }
                        }
                        break;
                    default:
                        Debug.LogError($"Invalid raycast mode {pointer.SceneQueryType} for {pointer.PointerName} pointer.");
                        break;
                }

                rayStartDistance += pointer.Rays[i].Length;
            }
        }

        #endregion Physics Raycasting

        #region uGUI Graphics Raycasting

        /// <summary>
        /// Perform a Unity Graphics Raycast to determine which uGUI element is currently being pointed at, if any.
        /// </summary>
        private void RaycastGraphics(IMixedRealityPointer pointer, PointerEventData graphicEventData, LayerMask[] prioritizedLayerMasks, PointerHitResult hit)
        {
            Debug.Assert(UIRaycastCamera != null, "Missing UIRaycastCamera!");
            Debug.Assert(UIRaycastCamera.nearClipPlane == 0, "Near plane must be zero for raycast distances to be correct");

            RaycastResult raycastResult = default(RaycastResult);

            if (pointer.Rays == null)
            {
                Debug.LogError($"No valid rays for {pointer.PointerName} pointer.");
                return;
            }

            if (pointer.Rays.Length <= 0)
            {
                Debug.LogError($"No valid rays for {pointer.PointerName} pointer");
                return;
            }

            // Cast rays for every step until we score a hit
            float totalDistance = 0.0f;
            for (int i = 0; i < pointer.Rays.Length; i++)
            {
                if (RaycastGraphicsStep(graphicEventData, pointer.Rays[i], prioritizedLayerMasks, out raycastResult))
                {
                    if (raycastResult.isValid &&
                        raycastResult.distance < pointer.Rays[i].Length &&
                        raycastResult.module != null &&
                        raycastResult.module.eventCamera == UIRaycastCamera)
                    {
                        totalDistance += raycastResult.distance;

                        newUiRaycastPosition.x = raycastResult.screenPosition.x;
                        newUiRaycastPosition.y = raycastResult.screenPosition.y;
                        newUiRaycastPosition.z = raycastResult.distance;

                        Vector3 worldPos = UIRaycastCamera.ScreenToWorldPoint(newUiRaycastPosition);
                        Vector3 normal = -raycastResult.gameObject.transform.forward;

                        hit.Set(raycastResult, worldPos, normal, pointer.Rays[i], i, totalDistance);
                        return;
                    }
                }

                totalDistance += pointer.Rays[i].Length;
            }
        }

        /// <summary>
        /// Raycasts a single graphic <see cref="Microsoft.MixedReality.Toolkit.Physics.RayStep"/>
        /// </summary>
        private bool RaycastGraphicsStep(PointerEventData graphicEventData, RayStep step, LayerMask[] prioritizedLayerMasks, out RaycastResult uiRaycastResult)
        {
            Debug.Assert(step.Direction != Vector3.zero, "RayStep Direction is Invalid.");

            // Move the uiRaycast camera to the current pointer's position.
            UIRaycastCamera.transform.position = step.Origin;
            UIRaycastCamera.transform.rotation = Quaternion.LookRotation(step.Direction, Vector3.up);

            // We always raycast from the center of the camera.
            graphicEventData.position = new Vector2(UIRaycastCamera.pixelWidth * 0.5f, UIRaycastCamera.pixelHeight * 0.5f);

            // Graphics raycast
            uiRaycastResult = InputSystem.RaycastProvider.GraphicsRaycast(EventSystem.current, graphicEventData, prioritizedLayerMasks);
            graphicEventData.pointerCurrentRaycast = uiRaycastResult;

            return (uiRaycastCamera.gameObject != null);
        }

        #endregion uGUI Graphics Raycasting

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
                        int numExits;
                        if (pendingOverallFocusExitSet.TryGetValue(pointer.PreviousPointerTarget, out numExits))
                        {
                            pendingOverallFocusExitSet[pointer.PreviousPointerTarget] = numExits + 1;
                        }
                        else
                        {
                            pendingOverallFocusExitSet.Add(pointer.PreviousPointerTarget, 1);
                        }
                    }

                    if (pointer.CurrentPointerTarget != null)
                    {
                        pendingOverallFocusEnterSet.Add(pointer.CurrentPointerTarget);
                    }
                }
            }

            // Early out if there have been no focus changes
            if (pendingPointerSpecificFocusChange.Count == 0)
            {
                return;
            }

            // ... but now we trim out objects whose overall focus was maintained the same by a different pointer:

            foreach (var pointer in pointers)
            {
                if (pointer.CurrentPointerTarget != null)
                {
                    pendingOverallFocusExitSet.Remove(pointer.CurrentPointerTarget);
                }
                pendingOverallFocusEnterSet.Remove(pointer.PreviousPointerTarget);
            }

            // Now we raise the events:
            for (int iChange = 0; iChange < pendingPointerSpecificFocusChange.Count; iChange++)
            {
                PointerData change = pendingPointerSpecificFocusChange[iChange];
                GameObject pendingUnfocusObject = change.PreviousPointerTarget;
                GameObject pendingFocusObject = change.CurrentPointerTarget;

                InputSystem.RaisePreFocusChanged(change.Pointer, pendingUnfocusObject, pendingFocusObject);

                int numExits;
                if (pendingUnfocusObject != null && pendingOverallFocusExitSet.TryGetValue(pendingUnfocusObject, out numExits))
                {
                    if (numExits > 1)
                    {
                        pendingOverallFocusExitSet[pendingUnfocusObject] = numExits - 1;
                    }
                    else
                    {
                        InputSystem.RaiseFocusExit(change.Pointer, pendingUnfocusObject);
                        pendingOverallFocusExitSet.Remove(pendingUnfocusObject);
                    }
                }

                if (pendingOverallFocusEnterSet.Contains(pendingFocusObject))
                {
                    InputSystem.RaiseFocusEnter(change.Pointer, pendingFocusObject);
                    pendingOverallFocusEnterSet.Remove(pendingFocusObject);
                }

                InputSystem.RaiseFocusChanged(change.Pointer, pendingUnfocusObject, pendingFocusObject);
            }

            Debug.Assert(pendingOverallFocusExitSet.Count == 0);
            Debug.Assert(pendingOverallFocusEnterSet.Count == 0);
            pendingPointerSpecificFocusChange.Clear();
        }

        #endregion Utilities

        #region ISourceState Implementation

        /// <inheritdoc />
        public void OnSourceDetected(SourceStateEventData eventData)
        {
            RegisterPointers(eventData.InputSource);
        }

        /// <inheritdoc />
        public void OnSourceLost(SourceStateEventData eventData)
        {
            // If the input source does not have pointers, then skip.
            if (eventData.InputSource.Pointers == null) { return; }

            // Let the pointer behavior know that the pointer has been lost
            IMixedRealityPointerMediator mediator;
            if (pointerMediators.TryGetValue(eventData.SourceId, out mediator))
            {
                mediator.UnregisterPointers(eventData.InputSource.Pointers);
            }

            pointerMediators.Remove(eventData.SourceId);

            for (var i = 0; i < eventData.InputSource.Pointers.Length; i++)
            {
                // Special unregistration for Gaze
                if (gazeProviderPointingData != null && eventData.InputSource.Pointers[i].PointerId == gazeProviderPointingData.Pointer.PointerId)
                {
                    // If the source lost is the gaze input source, then reset it.
                    if (eventData.InputSource.SourceId == InputSystem.GazeProvider?.GazeInputSource.SourceId)
                    {
                        gazeProviderPointingData.ResetFocusedObjects();
                        gazeProviderPointingData = null;
                    }
                    // Otherwise, don't unregister the gaze pointer, since the gaze input source is still active.
                    else
                    {
                        continue;
                    }
                }

                UnregisterPointer(eventData.InputSource.Pointers[i]);
            }
        }


        #endregion ISourceState Implementation

        #region IMixedRealitySpeechHandler Implementation
        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            gazePointerStateMachine.OnSpeechKeywordRecognized(eventData);
        }
        #endregion


        #region IPointerPreferences Implementation
        private List<PointerPreferences> customPointerBehaviors = new List<PointerPreferences>();

        /// <inheritdoc />
        public PointerBehavior GetPointerBehavior(IMixedRealityPointer pointer)
        {
            // Assumption: all pointers have controllers, input sources, except the gaze pointers
            // if the controller, input source is null, return the gaze pointer behavior here.
            if (pointer.Controller == null || pointer.InputSourceParent == null)
            {
                // gazepointer means input source is null
                return GazePointerBehavior;
            }

            return GetPointerBehavior(
                pointer.GetType(), 
                pointer.Controller.ControllerHandedness,
                pointer.InputSourceParent.SourceType);
        }

        /// <summary>
        /// Gets the behavior for the given pointer type.
        /// </summary>
        /// <param name="pointerType">Pointer type to query</param>
        /// <param name="handedness">Handedness to query</param>
        /// <returns><seealso cref="Microsoft.MixedReality.Toolkit.Input.PointerBehavior"/> for the given pointer type and handedness. If right hand is enabled, left
        /// hand is not enabled, and Handedness.Any is passed, returns value for the right hand.</returns>
        public PointerBehavior GetPointerBehavior<T>(
            Handedness handedness,
            InputSourceType sourceType) where T : class, IMixedRealityPointer
        {
            return GetPointerBehavior(typeof(T), handedness, sourceType);
        }

        private PointerBehavior GetPointerBehavior(Type type, Handedness handedness, InputSourceType sourceType)
        {
            for (int i = 0; i < customPointerBehaviors.Count; i++)
            {
                if (customPointerBehaviors[i].Matches(type, sourceType))
                {
                    return customPointerBehaviors[i].GetBehaviorForHandedness(handedness);
                }
            }
            return PointerBehavior.Default;
        }

        /// <inheritdoc />
        public PointerBehavior GazePointerBehavior { get; set; } = PointerBehavior.Default;

        /// <inheritdoc />
        public void SetPointerBehavior<T>(Handedness handedness, InputSourceType inputType, PointerBehavior pointerBehavior) where T : class, IMixedRealityPointer
        {
            PointerPreferences preference = null;
            for (int i = 0; i < customPointerBehaviors.Count; i++)
            {
                if (customPointerBehaviors[i].Matches(typeof(T), inputType))
                {
                    preference = customPointerBehaviors[i];
                }
            }
            if (preference == null)
            {
                preference = new PointerPreferences(typeof(T), inputType);
                customPointerBehaviors.Add(preference);
            }
            preference.SetBehaviorForHandedness(handedness, pointerBehavior);
        }

        private class PointerPreferences
        {
            public InputSourceType InputSourceType;
            public Type PointerType;

            public bool Matches(Type queryType, InputSourceType queryInputType)
            {
                return Matches(queryType) && queryInputType == InputSourceType;
            }

            public bool Matches(Type queryType)
            {
                return queryType.IsAssignableFrom(PointerType);
            }

            public PointerBehavior Left;
            public PointerBehavior Right;
            public PointerBehavior Other;
            public PointerBehavior GetBehaviorForHandedness(Handedness h)
            {
                if ((h & Handedness.Right) != 0)
                {
                    return Right;
                }
                if ((h & Handedness.Left) != 0)
                {
                    return Left;
                }
                if ((h & Handedness.Other) != 0)
                {
                    return Other;
                }
                return PointerBehavior.Default;
            }
            public void SetBehaviorForHandedness(
                Handedness h, 
                PointerBehavior b)
            {
                if ((h & Handedness.Right) != 0)
                {
                    Right = b;
                }
                if ((h & Handedness.Left) != 0)
                {
                    Left = b;
                }
                if ((h & Handedness.Other) != 0)
                {
                    Other = b;
                }
            }
            public PointerPreferences(Type pointerType, InputSourceType inputType)
            {
                Left = PointerBehavior.Default;
                Right = PointerBehavior.Default;
                Other = PointerBehavior.Default;
                InputSourceType = inputType;
                PointerType = pointerType;
            }
        }
        #endregion

    }
}
