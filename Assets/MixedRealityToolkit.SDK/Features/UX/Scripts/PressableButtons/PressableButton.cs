// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    ///<summary>
    /// A button that can be pushed via direct touch.
    /// You can use <see cref="Microsoft.MixedReality.Toolkit.PhysicalPressEventRouter"/> to route these events to <see cref="Microsoft.MixedReality.Toolkit.UI.Interactable"/>.
    ///</summary>
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_Button.html")]
    [AddComponentMenu("Scripts/MRTK/SDK/PressableButton")]
    public class PressableButton : MonoBehaviour, IMixedRealityTouchHandler
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
        [Tooltip("The offset at which pushing starts. Offset is relative to the pivot of either the moving visuals if there's any or the button itself.  For UnityUI based PressableButtons, this cannot be a negative value.")]
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

        [Header("Events")]
        public UnityEvent TouchBegin = new UnityEvent();
        public UnityEvent TouchEnd = new UnityEvent();
        public UnityEvent ButtonPressed = new UnityEvent();
        public UnityEvent ButtonReleased = new UnityEvent();

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

        ///<summary>
        /// Represents the state of whether or not a finger is currently touching this button.
        ///</summary>
        public bool IsTouching
        {
            get
            {
                return isTouching;
            }

            private set
            {
                if (value != isTouching)
                {
                    isTouching = value;

                    if (isTouching)
                    {
                        TouchBegin.Invoke();
                    }
                    else
                    {
                        // Abort press.
                        if (!releaseOnTouchEnd)
                        {
                            IsPressing = false;
                        }
                        TouchEnd.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// Represents the state of whether the button is currently being pressed.
        /// </summary>
        public bool IsPressing { get; private set; }

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

        private Transform PushSpaceSourceTransform
        {
            get { return movingButtonVisuals != null ? movingButtonVisuals.transform : transform; }
        }

        /// <summary>
        /// Transform for world to local space in the world direction of press
        /// Multiply world scale positions by this value to convert to local space
        /// </summary>
        private float WorldToLocalScale
        {
            get
            {
                return transform.InverseTransformVector(WorldSpacePressDirection).magnitude;
            }
        }

        /// <summary>
        /// Transform for local to world space in the world direction of a press
        /// Multiply local scale positions by this value to convert to world space
        /// </summary>
        private float LocalToWorldScale
        {
            get
            {
                return 1.0f / WorldToLocalScale;
            }
        }
        
        /// <summary>
        /// Initial offset from moving visuals to button
        /// </summary>
        private Vector3 movingVisualsInitialLocalPosition = Vector3.zero;

        /// <summary>
        /// The position from where the button starts to move.  Projected into world space based on the button's current world space position.
        /// </summary>
        private Vector3 InitialPosition
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

        #endregion

        private void OnEnable()
        {
            currentPushDistance = startPushDistance;
        }

        private Vector3 PushSpaceSourceParentPosition => (PushSpaceSourceTransform.parent != null) ? PushSpaceSourceTransform.parent.position : Vector3.zero;

        protected virtual void Start()
        {
            hasStarted = true;

            if (gameObject.layer == 2)
            {
                Debug.LogWarning("PressableButton will not work if game object layer is set to 'Ignore Raycast'.");
            }

            movingVisualsInitialLocalPosition = PushSpaceSourceTransform.localPosition;

            // Ensure everything is set to initial positions correctly.
            UpdateMovingVisualsPosition();
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
                currentPushDistance = GetFarthestDistanceAlongPressDirection();

                UpdateMovingVisualsPosition();

                // Hand Press is only allowed to happen while touching.
                UpdatePressedState(currentPushDistance);
            }
            else if (currentPushDistance > startPushDistance)
            {
                // Retract the button.
                float retractDistance = currentPushDistance - startPushDistance;
                retractDistance = retractDistance - retractDistance * returnSpeed * Time.deltaTime;

                // Apply inverse scale of local z-axis. This constant should always have the same value in world units.
                float localMaxRetractDistanceBeforeReset =
                    MaxRetractDistanceBeforeReset * WorldSpacePressDirection.magnitude;
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
        }

        #region IMixedRealityTouchHandler implementation

        private void PulseProximityLight()
        {
            // Pulse each proximity light on pointer cursors' interacting with this button.
            if (currentInputSources.Count != 0)
            {
                foreach (var pointer in currentInputSources[currentInputSources.Count - 1].Pointers)
                {
                    ProximityLight[] proximityLights = pointer.BaseCursor?.GameObjectReference?.GetComponentsInChildren<ProximityLight>();

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
                PokePointer poke = pointer as PokePointer;
                if (poke)
                {
                    // Extrapolate to get previous position.
                    float previousDistance = GetDistanceAlongPushDirection(poke.PreviousPosition);
                    return previousDistance <= StartPushDistance;
                }
            }

            return false;
        }

        void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
        {
            if (touchPoints.ContainsKey(eventData.Controller))
            {
                return;
            }

            // Back-Press Detection:
            // Accept touch only if controller pushed from the front.
            if (enforceFrontPush && !HasPassedThroughStartPlane(eventData))
            {
                return;
            }

            touchPoints.Add(eventData.Controller, eventData.InputData);

            // Make sure only one instance of this input source exists and is at the "top of the stack."
            currentInputSources.Remove(eventData.InputSource);
            currentInputSources.Add(eventData.InputSource);

            IsTouching = true;

            eventData.Use();
        }

        void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
        {
            if (touchPoints.ContainsKey(eventData.Controller))
            {
                touchPoints[eventData.Controller] = eventData.InputData;
                eventData.Use();
            }
        }

        void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            if (touchPoints.ContainsKey(eventData.Controller))
            {
                touchPoints.Remove(eventData.Controller);
                currentInputSources.Remove(eventData.InputSource);

                IsTouching = (touchPoints.Count > 0);
                eventData.Use();
            }
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
            return InitialPosition + WorldSpacePressDirection.normalized * distance;
        }


        /// <summary>
        /// Returns the local distance along the push direction for the passed in world position
        /// </summary>
        public float GetDistanceAlongPushDirection(Vector3 positionWorldSpace)
        {
            Vector3 localPosition = positionWorldSpace - InitialPosition;
            float distance = Vector3.Dot(localPosition, WorldSpacePressDirection.normalized);
            return (distanceSpaceMode == SpaceMode.Local) ? distance / LocalToWorldScale : distance;
        }

        #endregion

        #region private Methods

        protected virtual void UpdateMovingVisualsPosition()
        {
            if (movingButtonVisuals != null)
            {
                // Always move relative to startPushDistance
                movingButtonVisuals.transform.position = GetWorldPositionAlongPushDirection(currentPushDistance - startPushDistance);
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
                    IsPressing = true;
                    ButtonPressed.Invoke();
                    PulseProximityLight();
                }
            }
            // If we're in a press, check if the press is released now.
            else
            {
                float releaseDistance = pressDistance - releaseDistanceDelta;
                if (pushDistance <= releaseDistance)
                {
                    IsPressing = false;
                    ButtonReleased.Invoke();
                }
            }
        }

        #endregion
    }
}