// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    ///<summary>
    /// A button that can be pushed via direct touch.
    /// You can use <see cref="Microsoft.MixedReality.Toolkit.Examples.Demos.PhysicalPressEventRouter"/> to route these events to <see cref="Interactable"/>.
    ///</summary>
    [InitializeOnLoad]
    [RequireComponent(typeof(BoxCollider))]
    public class PressableButton : MonoBehaviour, IMixedRealityTouchHandler
    {
        const string InitialMarkerTransformName = "Initial Marker";

        #region Experimental Versionining 

        const int CurrentVersion = 1;
        const float NewVersionCheckValue = -1000.0f;

        [SerializeField]
        [HideInInspector]
        [ReadOnly]
        private int version = CurrentVersion;

        static PressableButton()
        {
            // Patch on reload.
            EditorApplication.update += PatchAllInstancesDeferred;

            // Patch on scene change.
            UnityEditor.SceneManagement.EditorSceneManager.activeSceneChangedInEditMode += 
                delegate (UnityEngine.SceneManagement.Scene from, UnityEngine.SceneManagement.Scene to) { PatchAllInstances(); };
        }

        private static void PatchAllInstancesDeferred()
        {
            EditorApplication.update -= PatchAllInstancesDeferred;
            PatchAllInstances();
        }

        private static void PatchAllInstances()
        {
            PressableButton[] buttons = FindObjectsOfType<PressableButton>();
            for (int i = 0; i < buttons.Length; i++)
            {
                Undo.RecordObject(buttons[i], "patch PressableButton");
                buttons[i].PerformVersionPatching();
            }
        }

        private void PerformVersionPatching()
        {
            // Old version migration.
            if (oldVersionCheck != NewVersionCheckValue)
            {
                version = 0;
                returnSpeed = oldVersionCheck;
                oldVersionCheck = NewVersionCheckValue;
            }

            if (version == 0)
            {
                // Distances and speeds along the press direction in this version were in world space units. 
                // Convert to local space units.
                Vector3 worldPressDir = WorldSpacePressDirection;
                float worldToLocalScale = transform.InverseTransformVector(worldPressDir).magnitude;

                maxPushDistance *= worldToLocalScale;
                pressDistance *= worldToLocalScale;
                releaseDistanceDelta *= worldToLocalScale;
                returnSpeed *= worldToLocalScale;
            }

            version = CurrentVersion;
        }

        [SerializeField]
        [HideInInspector]
        [ReadOnly]
        [FormerlySerializedAs("returnRate")]
        private float oldVersionCheck = NewVersionCheckValue; // Old variable to check for versions before there was a version field.

        #endregion

        // All the following distances are in local space units to facilitate scaling of the button:
        [SerializeField]
        [Tooltip("The object that is being pushed.")]
        private GameObject movingButtonVisuals = null;

        [SerializeField]
        [Header("Press Settings")]
        [Tooltip("The offset at which pushing starts.")]
        private float startPushDistance = 0.0f;

        [SerializeField]
        [Tooltip("Maximum push distance")]
        private float maxPushDistance = 0.2f;

        [SerializeField]
        [FormerlySerializedAs("minPressDepth")]
        [Tooltip("Distance the button must be pushed until it is considered pressed.")]
        private float pressDistance = 0.02f;

        [SerializeField]
        [FormerlySerializedAs("withdrawActivationAmount")]
        [Tooltip("Withdraw amount needed to transition from Pressed to Released.")]
        private float releaseDistanceDelta = 0.01f;

        [SerializeField]
        [Tooltip("Speed for retracting the moving button visuals on release.")]
        private float returnSpeed = 25.0f;

        [SerializeField]
        [Tooltip("Ensures that the button can only be pushed from the front. Touching the button from the back or side is prevented.")]
        private bool enforceFrontPush = true;

        /// <summary>
        /// The position from where the button starts to move.
        /// </summary>
        public Vector3 InitialPosition { get => PushSpaceSourceTransform.position; }

        [Header("Events")]
        public UnityEvent TouchBegin;
        public UnityEvent TouchEnd;
        public UnityEvent ButtonPressed;
        public UnityEvent ButtonReleased;

        #region Private Members

        // The maximum distance before the button is reset to its initial position when retracting.
        private const float MaxRetractDistanceBeforeReset = 0.0001f;

        private float currentPushDistance = 0.0f;

        private Transform pushSpaceTransform;

        private Dictionary<IMixedRealityController, Vector3> touchPoints = new Dictionary<IMixedRealityController, Vector3>();

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
                        IsPressing = false;

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
        /// The press direction of the button as defined by a NearInteractionTouchable.
        /// </summary>
        private Vector3 WorldSpacePressDirection
        {
            get
            {
                var nearInteractionTouchable = GetComponent<NearInteractionTouchable>();
                if (nearInteractionTouchable != null)
                {
                    return -1.0f * nearInteractionTouchable.Forward;
                }
                
                return transform.forward;
            }
        }

        private Transform PushSpaceSourceTransform
        {
            get { return movingButtonVisuals != null ? movingButtonVisuals.transform : transform; }
        }

        #endregion

        private void Awake()
        {
            currentPushDistance = startPushDistance;
        }

        private void Start()
        {
            if (gameObject.layer == 2)
            {
                Debug.LogWarning("PressableButton will not work if game object layer is set to 'Ignore Raycast'.");
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
                    MaxRetractDistanceBeforeReset * ((Vector3)pushSpaceTransform.worldToLocalMatrix.GetRow(2)).magnitude;
                if (retractDistance < localMaxRetractDistanceBeforeReset)
                {
                    currentPushDistance = startPushDistance;
                }
                else
                {
                    currentPushDistance = startPushDistance + retractDistance;
                }

                UpdateMovingVisualsPosition();
            }
        }

        #region IMixedRealityTouchHandler implementation

        void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
        {
            if (touchPoints.ContainsKey(eventData.Controller))
            {
                return;
            }

            EnsurePushSpaceMarkerCreated();

            if (enforceFrontPush)
            {
                // Back-Press Detection:
                // Accept touch only if controller pushed from the front.
                // Extrapolate to get previous position.
                Vector3 previousPosition = eventData.InputData - eventData.Controller.Velocity * Time.deltaTime;
                float previousDistance = GetDistanceAlongPushDirection(previousPosition);

                if (previousDistance > startPushDistance)
                {
                    return;
                }
            }

            touchPoints.Add(eventData.Controller, eventData.InputData);
            IsTouching = true;

            // Pulse each proximity light on pointer cursors' interacting with this button.
            foreach (var pointer in eventData.InputSource.Pointers)
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

                IsTouching = (touchPoints.Count > 0);

                eventData.Use();
            }
        }

        #endregion OnTouch

        #region private Methods

        private void EnsurePushSpaceMarkerCreated()
        {
            // If we don't find them, create them
            if (pushSpaceTransform == null)
            {
                Transform sourceTransform = PushSpaceSourceTransform;
                
                pushSpaceTransform = new GameObject(InitialMarkerTransformName).transform;
                pushSpaceTransform.parent = sourceTransform.parent;
                pushSpaceTransform.position = sourceTransform.position;
                // Z-axis in push space is the press direction.
                pushSpaceTransform.rotation = Quaternion.LookRotation(WorldSpacePressDirection);
                // Make sure to use the same scale as the parent transform.
                pushSpaceTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }

        private void UpdateMovingVisualsPosition()
        {
            if (movingButtonVisuals != null)
            {
                Debug.Assert(pushSpaceTransform != null);
                movingButtonVisuals.transform.position =
                    pushSpaceTransform.position + (Vector3)pushSpaceTransform.localToWorldMatrix.GetColumn(2) * 
                    // Always move relative to startPushDistance:
                    (currentPushDistance - startPushDistance);
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

        private float GetDistanceAlongPushDirection(Vector3 positionWorldSpace)
        {
            Debug.Assert(pushSpaceTransform != null);

            // In push space, the z-axis is the press direction.
            return Vector4.Dot(pushSpaceTransform.worldToLocalMatrix.GetRow(2), positionWorldSpace - pushSpaceTransform.position);
        }

        private void UpdatePressedState(float pushDistance)
        {
            if (!IsPressing)
            {
                if (pushDistance >= pressDistance)
                {
                    IsPressing = true;
                    ButtonPressed.Invoke();
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