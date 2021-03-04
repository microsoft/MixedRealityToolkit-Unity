// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    ///<summary>
    /// A button that can be pushed via direct touch.
    ///</summary>
    [RequireComponent(typeof(NearInteractionTouchable))]
    public class CompressableButton : BaseInteractiveElement
    {
        const string InitialMarkerTransformName = "Initial Marker";

        bool hasStarted = false;

        /// <summary>
        /// The object that is being pushed.
        /// </summary>
        [SerializeField]
        [Tooltip("The object that is being pushed.")]
        protected GameObject movingButtonVisuals = null;

        /// <summary>
        /// Enum for defining space of plane distances.
        /// </summary>
        public enum SpaceMode
        {
            World,
            Local
        }

        [SerializeField]
        [Tooltip("Describes in which coordinate space the plane distances are stored and calculated")]
        private SpaceMode distanceSpaceMode = SpaceMode.Local;

        /// <summary>
        /// Describes in which coordinate space the plane distances are stored and calculated
        /// </summary>
        public SpaceMode DistanceSpaceMode
        {
            get => distanceSpaceMode;
            set
            {
                // Convert world to local distances and vice versa whenever we switch the mode
                if (value != distanceSpaceMode)
                {
                    distanceSpaceMode = value;
                    float scale = (distanceSpaceMode == SpaceMode.Local) ? WorldToLocalScale : LocalToWorldScale;

                    startPushDistance *= scale;
                    maxPushDistance *= scale;
                    pressDistance *= scale;
                    releaseDistanceDelta *= scale;
                }
            }
        }

        [SerializeField]
        [Tooltip("The offset at which pushing starts. Offset is relative to the pivot of either the moving visuals if there's any or the button itself.  For UnityUI based CompressableButtons, this cannot be a negative value.")]
        protected float startPushDistance = 0.0f;

        /// <summary>
        /// The offset at which pushing starts. Offset is relative to the pivot of either the moving visuals if there's any or the button itself.
        /// </summary>
        public float StartPushDistance { get => startPushDistance; set => startPushDistance = value; }

        [SerializeField]
        [Tooltip("Maximum push distance. Distance is relative to the pivot of either the moving visuals if there's any or the button itself.")]
        private float maxPushDistance = 0.2f;

        /// <summary>
        /// Maximum push distance. Distance is relative to the pivot of either the moving visuals if there's any or the button itself.
        /// </summary>
        public float MaxPushDistance { get => maxPushDistance; set => maxPushDistance = value; }

        [SerializeField]
        [FormerlySerializedAs("minPressDepth")]
        [Tooltip("Distance the button must be pushed until it is considered pressed. Distance is relative to the pivot of either the moving visuals if there's any or the button itself.")]
        private float pressDistance = 0.02f;

        /// <summary>
        /// Distance the button must be pushed until it is considered pressed. Distance is relative to the pivot of either the moving visuals if there's any or the button itself.
        /// </summary>
        public float PressDistance { get => pressDistance; set => pressDistance = value; }

        [SerializeField]
        [FormerlySerializedAs("withdrawActivationAmount")]
        [Tooltip("Withdraw amount needed to transition from Pressed to Released.")]
        private float releaseDistanceDelta = 0.01f;

        /// <summary>
        ///  Withdraw amount needed to transition from Pressed to Released.
        /// </summary>
        public float ReleaseDistanceDelta { get => releaseDistanceDelta; set => releaseDistanceDelta = value; }

        /// <summary>
        ///  Speed for retracting the moving button visuals on release.
        /// </summary>
        [SerializeField]
        [Tooltip("Speed for retracting the moving button visuals on release.")]
        [FormerlySerializedAs("returnRate")]
        private float returnSpeed = 25.0f;

        [SerializeField]
        [Tooltip("Button will send the release event on touch end after successful press even if release plane hasn't been passed.")]
        private bool releaseOnTouchEnd = true;

        /// <summary>
        ///  Button will send the release event on touch end after successful press even if release plane hasn't been passed.
        /// </summary>
        public bool ReleaseOnTouchEnd { get => releaseOnTouchEnd; set => releaseOnTouchEnd = value; }

        [SerializeField]
        [Tooltip("Ensures that the button can only be pushed from the front. Touching the button from the back or side is prevented.")]
        private bool enforceFrontPush = true;

        /// <summary>
        /// Ensures that the button can only be pushed from the front. Touching the button from the back or side is prevented.
        /// </summary>
        public bool EnforceFrontPush { get => enforceFrontPush; private set => enforceFrontPush = value; }

        #region Private Members

        // The maximum distance before the button is reset to its initial position when retracting.
        private const float MaxRetractDistanceBeforeReset = 0.0001f;

        private Dictionary<IMixedRealityController, Vector3> touchPoints = new Dictionary<IMixedRealityController, Vector3>();

        private List<IMixedRealityInputSource> currentInputSources = new List<IMixedRealityInputSource>();

        private float currentPushDistance = 0.0f;

        /// <summary>
        /// Current push distance relative to the start push plane. 
        /// </summary>
        public float CurrentPushDistance { get => currentPushDistance; protected set => currentPushDistance = value; }

        private bool isTouching = false;

        // The PressedNear state is not considered a CoreInteractionState because it is specific to the compressable button class
        protected string PressedNearStateName = "PressedNear";

        ///<summary>
        /// Represents the state of whether or not a finger is currently touching this button.
        ///</summary>
        public bool IsTouching
        {
            get => isTouching;
            private set
            {
                if (value != isTouching)
                {
                    isTouching = value;

                    if (!isTouching)
                    {
                        // Abort press.
                        if (!releaseOnTouchEnd)
                        {
                            IsPressing = false;
                            SetStateAndInvokeEvent(PressedNearStateName, 0);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Represents the state of whether the button is currently being pressed.
        /// </summary>
        public bool IsPressing { get; private set; }

        /// <summary>
        /// Transform for local to world space in the world direction of a press
        /// Multiply local scale positions by this value to convert to world space
        /// </summary>
        public float LocalToWorldScale => (WorldToLocalScale != 0) ? 1.0f / WorldToLocalScale : 0.0f;

        /// <summary>
        /// The press direction of the button as defined by a NearInteractionTouchableSurface.
        /// </summary>
        private Vector3 WorldSpacePressDirection
        {
            get
            {
                var nearInteractionTouchable = GetComponent<NearInteractionTouchableSurface>();
                if (nearInteractionTouchable != null)
                {
                    return nearInteractionTouchable.transform.TransformDirection(nearInteractionTouchable.LocalPressDirection);
                }

                return transform.forward;
            }
        }

        /// <summary>
        /// The press direction of the button as defined by a NearInteractionTouchableSurface, in local space,
        /// using Vector3.forward as an optional fallback when no NearInteractionTouchableSurface is defined.
        /// </summary>
        private Vector3 LocalSpacePressDirection
        {
            get
            {
                var nearInteractionTouchable = GetComponent<NearInteractionTouchableSurface>();
                if (nearInteractionTouchable != null)
                {
                    return nearInteractionTouchable.LocalPressDirection;
                }

                return Vector3.forward;
            }
        }

        private Transform PushSpaceSourceTransform
        {
            get => movingButtonVisuals != null ? movingButtonVisuals.transform : transform;
        }

        /// <summary>
        /// Transform for world to local space in the world direction of press
        /// Multiply world scale positions by this value to convert to local space
        /// </summary>
        private float WorldToLocalScale => transform.InverseTransformVector(WorldSpacePressDirection).magnitude;

        /// <summary>
        /// Initial offset from moving visuals to button
        /// </summary>
        private Vector3 movingVisualsInitialLocalPosition = Vector3.zero;

        /// <summary>
        /// The position from where the button starts to move.  Projected into world space based on the button's current world space position.
        /// </summary>
        private Vector3 InitialWorldPosition
        {
            get
            {
                if (Application.isPlaying && movingButtonVisuals) // we're using a cached position in play mode as the moving visuals will be moved during button interaction
                {
                    var parentTransform = PushSpaceSourceTransform.parent;
                    var localPosition = (parentTransform == null) ? movingVisualsInitialLocalPosition : parentTransform.TransformVector(movingVisualsInitialLocalPosition);
                    return PushSpaceSourceParentPosition + localPosition;
                }
                else
                {
                    return PushSpaceSourceTransform.position;
                }
            }
        }

        /// <summary>
        /// The position from where the button starts to move.  In local space, relative to button root.
        /// </summary>
        private Vector3 InitialLocalPosition
        {
            get
            {
                if (Application.isPlaying && movingButtonVisuals) // we're using a cached position in play mode as the moving visuals will be moved during button interaction
                {
                    return movingVisualsInitialLocalPosition;
                }
                else
                {
                    return PushSpaceSourceTransform.position;
                }
            }
        }

        #endregion

        #region  Properties from PressableButtonHoloLens2
        [Space()]
        [SerializeField]
        [Tooltip("The icon and text content moving inside the button.")]
        private GameObject movingButtonIconText = null;

        [SerializeField]
        [Tooltip("The visuals which become compressed (scaled) along the z-axis when pressed.")]
        private GameObject compressableButtonVisuals = null;

        /// <summary>
        /// The visuals which become compressed (scaled) along the z-axis when pressed.
        /// </summary>
        public GameObject CompressableButtonVisuals
        {
            get => compressableButtonVisuals;
            set
            {
                compressableButtonVisuals = value;

                if (compressableButtonVisuals != null)
                {
                    initialCompressableButtonVisualsLocalScale = compressableButtonVisuals.transform.localScale;
                }

            }
        }

        [SerializeField]
        [Range(0.0f, 1.0f)]
        [Tooltip("The minimum percentage of the original scale the compressableButtonVisuals can be compressed to.")]
        private float minCompressPercentage = 0.25f;

        /// <summary>
        /// The minimum percentage of the original scale the compressableButtonVisuals can be compressed to.
        /// </summary>
        public float MinCompressPercentage { get => minCompressPercentage; set => minCompressPercentage = value; }

        /// <summary>
        /// Public property to set the moving content part(icon and text) of the button. 
        /// This content part moves 1/2 distance of the front cage 
        /// </summary>
        public GameObject MovingButtonIconText
        {
            get
            {
                return movingButtonIconText;
            }
            set
            {
                if (movingButtonIconText != value)
                {
                    movingButtonIconText = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("The plate which represents the press-able surface of the button that highlights when focused.")]
        private Renderer highlightPlate = null;

        [SerializeField]
        [Tooltip("The duration of time it takes to animate in/out the highlight plate.")]
        private float highlightPlateAnimationTime = 0.25f;

        #region Private Members

        Vector3 initialCompressableButtonVisualsLocalScale = Vector3.one;
        private int fluentLightIntensityID = 0;
        private float targetFluentLightIntensity = 1.0f;
        private MaterialPropertyBlock properties = null;
        private Coroutine highlightPlateAnimationRoutine = null;

        #endregion
        #endregion

        private void OnEnable()
        {
            currentPushDistance = startPushDistance;
        }

        private Vector3 PushSpaceSourceParentPosition => (PushSpaceSourceTransform.parent != null) ? PushSpaceSourceTransform.parent.position : Vector3.zero;


        public override void Start()
        {
            base.Start();

            hasStarted = true;

            if (gameObject.layer == 2)
            {
                Debug.LogWarning("CompressableButton will not work if game object layer is set to 'Ignore Raycast'.");
            }

            movingVisualsInitialLocalPosition = PushSpaceSourceTransform.localPosition;

            // Ensure everything is set to initial positions correctly.
            UpdateMovingVisualsPosition();

            AddRequiredStates();

            if (IsStatePresent(TouchStateName))
            {
                var touchEvents = GetStateEvents<TouchEvents>("Touch");

                touchEvents.OnTouchStarted.AddListener((touchEventData) =>
                {
                    if (touchPoints.ContainsKey(touchEventData.Controller))
                    {
                        return;
                    }

                    // Back-Press Detection:
                    // Accept touch only if controller pushed from the front.
                    if (enforceFrontPush && !HasPassedThroughStartPlane(touchEventData))
                    {
                        return;
                    }

                    touchPoints.Add(touchEventData.Controller, touchEventData.InputData);

                    // Make sure only one instance of this input source exists and is at the "top of the stack."
                    currentInputSources.Remove(touchEventData.InputSource);
                    currentInputSources.Add(touchEventData.InputSource);

                    IsTouching = true;

                    touchEventData.Use();

                });

                touchEvents.OnTouchCompleted.AddListener((touchEventData) =>
                {
                    if (touchPoints.ContainsKey(touchEventData.Controller))
                    {
                        // When focus is lost, before removing controller, update the respective touch point to give a last chance for checking if pressed occurred 
                        touchPoints[touchEventData.Controller] = touchEventData.InputData;
                        UpdateTouch();

                        touchPoints.Remove(touchEventData.Controller);
                        currentInputSources.Remove(touchEventData.InputSource);

                        IsTouching = (touchPoints.Count > 0);
                        touchEventData.Use();
                    }
                });

                touchEvents.OnTouchUpdated.AddListener((touchEventData) =>
                {
                    if (touchPoints.ContainsKey(touchEventData.Controller))
                    {
                        touchPoints[touchEventData.Controller] = touchEventData.InputData;
                        touchEventData.Use();
                    }
                });
            }

            if (compressableButtonVisuals != null)
            {
                initialCompressableButtonVisualsLocalScale = compressableButtonVisuals.transform.localScale;
            }

            if (highlightPlate != null)
            {
                // Cache the initial highlight plate state.
                fluentLightIntensityID = Shader.PropertyToID("_FluentLightIntensity");
                properties = new MaterialPropertyBlock();
                targetFluentLightIntensity = highlightPlate.sharedMaterial.GetFloat(fluentLightIntensityID);

                // Hide the highlight plate initially.
                UpdateHightlightPlateVisuals(0.0f);
                highlightPlate.enabled = false;
            }
        }

        void OnDisable()
        {
            // clear touch points in case we get disabled and can't receive the touch end event anymore
            touchPoints.Clear();
            currentInputSources.Clear();

            if (hasStarted)
            {
                // make sure button doesn't stay in a pressed state in case we disable the button while pressing it
                currentPushDistance = startPushDistance;
                UpdateMovingVisualsPosition();
            }
        }

        private void Update()
        {
            if (IsTouching)
            {
                UpdateTouch();
            }
            else if (currentPushDistance > startPushDistance)
            {
                RetractButton();
            }

            if (IsPressing)
            {
                // If the button is currently being pressed, invoke the OnButtonPressHold event
                // contained in the PressedNear state
                EventReceiverManager.InvokeStateEvent(PressedNearStateName);
            }
        }

        private void UpdateTouch()
        {
            currentPushDistance = GetFarthestDistanceAlongPressDirection();

            UpdateMovingVisualsPosition();

            // Hand press is only allowed to happen while touching.
            UpdatePressedState(currentPushDistance);
        }

        private void RetractButton()
        {
            float retractDistance = currentPushDistance - startPushDistance;
            retractDistance -= retractDistance * returnSpeed * Time.deltaTime;

            // Apply inverse scale of local z-axis. This constant should always have the same value in world units.
            float localMaxRetractDistanceBeforeReset = MaxRetractDistanceBeforeReset * WorldSpacePressDirection.magnitude;
            if (retractDistance < localMaxRetractDistanceBeforeReset)
            {
                currentPushDistance = startPushDistance;
            }
            else
            {
                currentPushDistance = startPushDistance + retractDistance;
            }

            UpdateMovingVisualsPosition();

            if (releaseOnTouchEnd && IsPressing)
            {
                UpdatePressedState(currentPushDistance);
            }
        }

        #region IMixedRealityTouchHandler implementation

        private void PulseProximityLight()
        {
            // Pulse each proximity light on pointer cursors' interacting with this button.
            if (currentInputSources.Count != 0)
            {
                foreach (var pointer in currentInputSources[currentInputSources.Count - 1].Pointers)
                {
                    if (!pointer.BaseCursor.TryGetMonoBehaviour(out MonoBehaviour baseCursor))
                    {
                        return;
                    }

                    GameObject cursorGameObject = baseCursor.gameObject;
                    if (cursorGameObject == null)
                    {
                        return;
                    }

                    ProximityLight[] proximityLights = cursorGameObject.GetComponentsInChildren<ProximityLight>();

                    if (proximityLights != null)
                    {
                        foreach (var proximityLight in proximityLights)
                        {
                            proximityLight.Pulse();
                        }
                    }
                }
            }
        }

        private bool HasPassedThroughStartPlane(HandTrackingInputEventData eventData)
        {
            foreach (var pointer in eventData.InputSource.Pointers)
            {
                // In the case that the input source has multiple poke pointers, this code
                // will reason over the first such pointer that is actually interacting with
                // an object. For input sources that have a single poke pointer, this is one
                // and the same (i.e. this event will only fire for this object when the poke
                // pointer is touching this object).
                PokePointer poke = pointer as PokePointer;
                if (poke && poke.CurrentTouchableObjectDown)
                {
                    // Extrapolate to get previous position.
                    float previousDistance = GetDistanceAlongPushDirection(poke.PreviousPosition);
                    return previousDistance <= StartPushDistance;
                }
            }

            return false;
        }

        #endregion OnTouch

        #region public transform utils

        /// <summary>
        /// Returns world space position along the push direction for the given local distance
        /// </summary>
        /// 
        public Vector3 GetWorldPositionAlongPushDirection(float localDistance)
        {
            float distance = (distanceSpaceMode == SpaceMode.Local) ? localDistance * LocalToWorldScale : localDistance;
            return InitialWorldPosition + WorldSpacePressDirection.normalized * distance;
        }

        /// <summary>
        /// Returns local position along the push direction for the given local distance
        /// </summary>
        /// 
        public Vector3 GetLocalPositionAlongPushDirection(float localDistance)
        {
            return InitialLocalPosition + LocalSpacePressDirection.normalized * localDistance;
        }

        /// <summary>
        /// Returns the local distance along the push direction for the passed in world position
        /// </summary>
        public float GetDistanceAlongPushDirection(Vector3 positionWorldSpace)
        {
            Vector3 localPosition = positionWorldSpace - InitialWorldPosition;
            float distance = Vector3.Dot(localPosition, WorldSpacePressDirection.normalized);
            return (distanceSpaceMode == SpaceMode.Local) ? distance * WorldToLocalScale : distance;
        }

        #endregion

        #region private Methods

        protected virtual void UpdateMovingVisualsPosition()
        {
            if (movingButtonVisuals != null)
            {
                // Always move relative to startPushDistance
                movingButtonVisuals.transform.localPosition = GetLocalPositionAlongPushDirection(currentPushDistance - startPushDistance);
            }

            if (compressableButtonVisuals != null)
            {
                // Compress the button visuals by the push amount.
                Vector3 scale = compressableButtonVisuals.transform.localScale;
                float pressPercentage;

                // Prevent divide by zero when calculating pressPercentage.
                if (MaxPushDistance <= float.Epsilon)
                {
                    pressPercentage = 0.0f;
                }
                else
                {
                    pressPercentage = Mathf.Max(minCompressPercentage, (1.0f - (CurrentPushDistance - startPushDistance) / MaxPushDistance));
                }

                scale.z = initialCompressableButtonVisualsLocalScale.z * pressPercentage;
                compressableButtonVisuals.transform.localScale = scale;
            }

            if (movingButtonIconText != null)
            {
                // Always move relative to startPushDistance
                movingButtonIconText.transform.localPosition = GetLocalPositionAlongPushDirection((CurrentPushDistance - startPushDistance) / 2.0f);
            }

        }

        // This function projects the current touch positions onto the 1D press direction of the button.
        // It will output the farthest pushed distance from the button's initial position.
        private float GetFarthestDistanceAlongPressDirection()
        {
            float farthestDistance = startPushDistance;

            foreach (var touchEntry in touchPoints)
            {
                float testDistance = GetDistanceAlongPushDirection(touchEntry.Value);
                farthestDistance = Mathf.Max(testDistance, farthestDistance);
            }

            return Mathf.Clamp(farthestDistance, startPushDistance, maxPushDistance);
        }

        private void UpdatePressedState(float pushDistance)
        {
            // If we aren't in a press and can't start a simple one.
            if (!IsPressing)
            {
                // Compare to our previous push depth. Use previous push distance to handle back-presses.
                if (pushDistance >= pressDistance)
                {
                    SetStateAndInvokeEvent(PressedNearStateName, 1);
                    IsPressing = true;
                    TriggerClickedState();
                    PulseProximityLight();
                }
            }
            // If we're in a press, check if the press is released now.
            else
            {
                float releaseDistance = pressDistance - releaseDistanceDelta;
                if (pushDistance <= releaseDistance)
                {
                    SetStateAndInvokeEvent(PressedNearStateName, 0);
                    IsPressing = false;
                }
            }
        }

        #endregion

        // Add the required states for CompressableButton during edit mode
        internal void AddRequiredStatesEditMode()
        {
            if (!IsStatePresentEditMode(TouchStateName))
            {
                States.Add(new InteractionState(TouchStateName));
            }

            if (!IsStatePresentEditMode(PressedNearStateName))
            {
                States.Add(new InteractionState(PressedNearStateName));
            }

            if (!IsStatePresentEditMode(FocusStateName))
            {
                States.Add(new InteractionState(FocusStateName));
            }
        }

        // Add the required states for CompressableButton during runtime
        private void AddRequiredStates()
        {
            if (!IsStatePresent(TouchStateName))
            {
                AddNewState(TouchStateName);
            }

            if (!IsStatePresent(PressedNearStateName))
            {
                AddNewState(PressedNearStateName);
            }

            if (!IsStatePresent(FocusStateName))
            {
                AddNewState(FocusStateName);
            }
        }

        public override void OnValidate()
        {
            base.OnValidate();
            AddRequiredStatesEditMode();
        }

        /// <summary>
        /// Animates in the highlight plate.
        /// </summary>
        public void AnimateInHighlightPlate()
        {
            if (highlightPlate != null)
            {
                if (highlightPlateAnimationRoutine != null)
                {
                    StopCoroutine(highlightPlateAnimationRoutine);
                }

                highlightPlateAnimationRoutine = StartCoroutine(AnimateHighlightPlate(true, highlightPlateAnimationTime));
            }
        }

        /// <summary>
        /// Animates out the highlight plate and disables it when animated out.
        /// </summary>
        public void AnimateOutHighlightPlate()
        {
            if (highlightPlate != null)
            {
                if (highlightPlateAnimationRoutine != null)
                {
                    StopCoroutine(highlightPlateAnimationRoutine);
                }

                highlightPlateAnimationRoutine = StartCoroutine(AnimateHighlightPlate(false, highlightPlateAnimationTime));
            }
        }

        private IEnumerator AnimateHighlightPlate(bool fadeIn, float time)
        {
            highlightPlate.enabled = true;

            // Calculate how much time is left in the blend based on current intensity.
            var normalizedIntensity = (targetFluentLightIntensity != 0.0f) ? properties.GetFloat(fluentLightIntensityID) / targetFluentLightIntensity : 1.0f;
            var blendTime = fadeIn ? (1.0f - normalizedIntensity) * time : normalizedIntensity * time;

            while (blendTime > 0.0f)
            {
                float t = 1.0f - (blendTime / time);
                UpdateHightlightPlateVisuals(fadeIn ? t : 1.0f - t);
                blendTime -= Time.deltaTime;

                yield return null;
            }

            UpdateHightlightPlateVisuals(fadeIn ? targetFluentLightIntensity : 0.0f);

            // When completely faded out, hide the highlight plate.
            if (!fadeIn)
            {
                highlightPlate.enabled = false;
            }
        }

        private void UpdateHightlightPlateVisuals(float lightIntensity)
        {
            highlightPlate.GetPropertyBlock(properties);
            properties.SetFloat(fluentLightIntensityID, lightIntensity);
            highlightPlate.SetPropertyBlock(properties);
        }
    }
}