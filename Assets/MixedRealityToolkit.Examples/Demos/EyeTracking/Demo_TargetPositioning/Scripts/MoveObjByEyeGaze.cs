// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    public enum PlacementSurfaces
    {
        Horizontal,
        Vertical
    }

    public class MoveObjByEyeGaze : MonoBehaviour,
        IMixedRealitySpeechHandler,
        IMixedRealitySourceStateHandler,
        IMixedRealityPointerHandler,
        IMixedRealityHandJointHandler
    {
        private IMixedRealityEyeGazeProvider EyeTrackingProvider => eyeTrackingProvider ?? (eyeTrackingProvider = MixedRealityToolkit.InputSystem?.EyeGazeProvider);
        private IMixedRealityEyeGazeProvider eyeTrackingProvider = null;

        #region Serialized variables
        [Header("Eyes")]
        [SerializeField]
        private bool useEyeSupportedTargetPlacement = false;

        [Tooltip("The user has to look away at least this far to enable the eye-supported target placement. " +
            "This is to allow for local manual positioning using hand input.")]
        [SerializeField]
        [Range(1, 10)]
        private float minLookAwayDistToEnableEyeWarp = 5f;

        [Header("Hands")]
        [Tooltip("Use this to enforce only voice commands to move targets.")]
        [SerializeField]
        private bool handInputEnabled = true;

        [Tooltip("To control whether the hand motion is used 1:1 to move a target or to use different gains to allow for smaller hand motions.")]
        [SerializeField]
        private float handmapping = 1;

        [Tooltip("Minimal amount of hand movement to trigger target repositioning.")]
        [SerializeField]
        private float deltaHandMovemThresh = 0.05f;

        [Header("Transitioning")]
        [Tooltip("Transparency of the target itself while dragging is active.")]
        [SerializeField]
        [Range(0, 1)]
        private float transparency_inTransition = 130 / 255f;

        [Tooltip("Transparency of the target preview while dragging it around.")]
        [SerializeField]
        [Range(0, 1)]
        private float transparency_preview = 50 / 255f;

        [Tooltip("Minimal distance between the old and new preview. This is to prevent the preview to always follow the eye gaze immediately. " +
            "The value should depend on the size of the target.")]
        [SerializeField]
        [Range(0, 1)]
        private float previewPlacemDistThresh = 0.05f;

        [Header("Constrained Movement")]
        [SerializeField]
        private bool freezeX = false;

        [SerializeField]
        private bool freezeY = false;

        [SerializeField]
        private bool freezeZ = false;

        public Vector2 LocalMinMax_X = new Vector2(float.NegativeInfinity, float.PositiveInfinity);
        public Vector2 LocalMinMax_Y = new Vector2(float.NegativeInfinity, float.PositiveInfinity);
        public Vector2 LocalMinMax_Z = new Vector2(float.NegativeInfinity, float.PositiveInfinity);

        public PlacementSurfaces PlacementSurface = PlacementSurfaces.Horizontal;

        [Header("Audio Feedback")]
        [SerializeField]
        private AudioClip audio_OnDragStart = null;

        [SerializeField]
        private AudioClip audio_OnDragStop = null;

        [Header("Event Handlers")]
        [SerializeField]
        private MixedRealityInputAction voiceAction_PutThis = MixedRealityInputAction.None;

        [SerializeField]
        private MixedRealityInputAction voiceAction_OverHere = MixedRealityInputAction.None;

        [SerializeField]
        private UnityEvent OnDragStart = null;

        [SerializeField]
        private UnityEvent OnDrop = null;

        [Header("Slider")]
        [SerializeField]
        private bool useAsSlider = false;

        [SerializeField]
        private TextMesh txtOutput_sliderValue = null;

        [SerializeField]
        private float slider_snapToNearestDecimal = 0.1f;
        #endregion

        #region Private variables
        private GameObject previewGameObject;
        private bool onlyEyeWarpOnRelease = true; // Only warp the currently grabbed target to the current look at location once the user releases the pinch gesture.        

        private float originalTransparency = -1;
        private bool originalUseGravity = false;
        private float originalDrag = 1;

        private bool onlyTransitionToPlausibleDestinations = true;
        private Vector3? plausibleLocation;
        private bool placePreviewAtHitPoint = true;

        private bool manualTargetManip = false;

        private Vector3 initalGazeDir;
        private static bool isManipulatingUsing_Hands = false;
        private static bool isManipulatingUsing_Voice = false;
        private Vector3 handPos_absolute;
        private Vector3 handPos_relative;
        private Vector3 initialHandPos;
        private static bool handIsPinching = false;
        private Handedness currEngagedHand = Handedness.None;
        private bool objIsGrabbed = false;

        private Ray? head_prevRay;
        private float head_deltaDirThresh = 0.05f;
        private float head_smoothf = 0.1f;
        private bool head_isInMotion;
        private float head_deltaDirf = 0;

        private int constrX { get { return ((freezeX) ? 0 : 1); } }
        private int constrY { get { return ((freezeY) ? 0 : 1); } }
        private int constrZ { get { return ((freezeZ) ? 0 : 1); } }
        private Vector3 constrMoveCtrl { get { return new Vector3(constrX, constrY, constrZ); } }

        Vector3? prevPreviewPos;

        // The values represent the maximum angle that the surface can be offset from the 'up' vector to be considered for horizontal placement.
        // For example, if a surface is slanted by 40 degrees targets may slid off and hence we may consider this an invalid offset angle.
        private float maxDiffAngleForHorizontalPlacem = 20;

        // The values represents the minimal angle that the surface must be offset from the 'up' vector to be considered for vertical placement.
        private float minDiffAngleForVerticalPlacem = 50;
        #endregion

        private void Start()
        {
            UpdateSliderTextOutput();
        }

        private void Update()
        {
            if (objIsGrabbed && useEyeSupportedTargetPlacement)
            {
                // Check whether the user is still looking within the proximity of the target
                if (IsLookingAwayFromTarget() && IsLookingAwayFromPreview())
                {
                    if ((EyeTrackingProvider?.GazeTarget != null) &&
                        (EyeTrackingProvider?.GazeTarget != gameObject))     // To prevent trying to place it on itself
                    {
                        plausibleLocation = null;

                        if (EyeTrackingProvider?.GazeTarget.GetComponent<SnapTo>() != null)
                        {
                            plausibleLocation = EyeTrackingProvider?.GazeTarget.transform.position;
                        }
                        // Determine the location to place the selected target at
                        else if (onlyTransitionToPlausibleDestinations)
                        {

                            if (IsDestinationPlausible())
                            {
                                plausibleLocation = EyeTrackingProvider?.HitPosition;
                            }
                            else
                            {
                                plausibleLocation = GetValidPlacemLocation(EyeTrackingProvider?.GazeTarget);
                            }
                        }
                        else
                        {
                            plausibleLocation = EyeTrackingProvider?.HitPosition;
                        }

                        // Only show preview and place target in plausible locations
                        if (plausibleLocation.HasValue)
                        {
                            ActivatePreview();

                            // Update preview position
                            if (placePreviewAtHitPoint)
                            {
                                previewGameObject.transform.position = plausibleLocation.Value; // EyeInputManager.Instance.HitPosition;
                            }
                            else
                            {
                                if (PlacementSurface == PlacementSurfaces.Horizontal)
                                {
                                    previewGameObject.transform.position = plausibleLocation.Value + previewGameObject.transform.localScale.y * new Vector3(0, 1, 0) / 2;  //EyeInputManager.Instance.HitPosition + pPreviewGameObj.transform.localScale.y * new Vector3(0, 1, 0);
                                }
                                else
                                {
                                    previewGameObject.transform.position = plausibleLocation.Value; //EyeInputManager.Instance.HitPosition;
                                }
                                prevPreviewPos = previewGameObject.transform.position;
                            }
                        }
                        else
                        {
                            DeactivatePreview();
                        }
                    }
                    else
                    {
                        DeactivatePreview();
                    }
                }
            }
            else
            {
                prevPreviewPos = null;
            }
        }

        #region Voice input handler
        void IMixedRealitySpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            if (voiceAction_PutThis == eventData.MixedRealityInputAction)
            {
                DragAndDrop_Start();
                MixedRealityToolkit.InputSystem.PushModalInputHandler(gameObject);
            }
            else if (voiceAction_OverHere == eventData.MixedRealityInputAction)
            {
                DragAndDrop_Finish();
                MixedRealityToolkit.InputSystem.PopModalInputHandler();
            }
        }
        #endregion

        #region Hand input handler
        void IMixedRealityHandJointHandler.OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
        {
            MixedRealityPose pose;
            eventData.InputData.TryGetValue(TrackedHandJoint.Palm, out pose);

            if ((pose != null) && (eventData.Handedness == currEngagedHand) && isManipulatingUsing_Hands)
            {
                if (handPos_absolute == Vector3.zero)
                {
                    handPos_absolute = pose.Position;
                }
                else
                {
                    Vector3 oldHandPos = handPos_absolute;
                    handPos_relative = new Vector3(oldHandPos.x - pose.Position.x, oldHandPos.y - pose.Position.y, oldHandPos.z - pose.Position.z);
                    handPos_absolute = pose.Position;

                    if (handIsPinching)
                    {
                        RelativeMoveUpdate(handPos_relative);
                    }
                }
            }
        }

        void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData) { }

        void IMixedRealitySourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            if (IsActiveHand(eventData.InputSource.SourceName))
            {
                HandDrag_Stop();
            }
        }

        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if (IsActiveHand(eventData.InputSource.SourceName))
            {
                HandDrag_Stop();
            }
        }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (SetActiveHand(eventData.InputSource.SourceName))
            {
                HandDrag_Start();
            }
        }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData) { }
        #endregion

        private bool SetActiveHand(string sourcename)
        {
            if (currEngagedHand == Handedness.None)
            {
                if ((sourcename == "Right Hand") || (sourcename == "Mixed Reality Controller Right"))
                {
                    currEngagedHand = Handedness.Right;
                }
                else if ((sourcename == "Left Hand") || (sourcename == "Mixed Reality Controller Left"))
                {
                    currEngagedHand = Handedness.Left;
                }

                if (currEngagedHand != Handedness.None)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsActiveHand(string sourcename)
        {

            if (((currEngagedHand == Handedness.Right) && ((sourcename == "Right Hand") || (sourcename == "Mixed Reality Controller Right"))) ||
                ((currEngagedHand == Handedness.Left) && ((sourcename == "Left Hand") || (sourcename == "Mixed Reality Controller Left"))))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Start moving the target using your hands.
        /// </summary>
        private void HandDrag_Start()
        {
            if ((handInputEnabled) && (!isManipulatingUsing_Hands) && (!isManipulatingUsing_Voice))
            {
                isManipulatingUsing_Hands = true;
                handIsPinching = true;
                handPos_relative = Vector3.zero;
                handPos_absolute = Vector3.zero;
                DragAndDrop_Start();
                MixedRealityToolkit.InputSystem.PushModalInputHandler(gameObject);
            }
        }

        /// <summary>
        /// Finish moving the target using your hands.
        /// </summary>
        private void HandDrag_Stop()
        {
            if (isManipulatingUsing_Hands)
            {
                isManipulatingUsing_Hands = false;
                handIsPinching = false;
                handPos_relative = Vector3.zero;
                DragAndDrop_Finish();
                MixedRealityToolkit.InputSystem.PopModalInputHandler();
                currEngagedHand = Handedness.None;
            }
        }

        /// <summary>
        /// Check whether the user is looking away from the target.
        /// </summary>
        /// <returns></returns>
        private bool IsLookingAwayFromTarget()
        {
            // First, let's check if the target is still hit by the eye gaze cursor
            if (EyeTrackingProvider?.GazeTarget == gameObject)
            {
                return false;
            }

            // Check whether the user is still looking within the proximity of the target
            float distanceBetweenTargetAndCurrHitPos = Angle_ToCurrHitTarget(gameObject);

            // ToDo: It would be better to make this dependent on the target's boundary instead of its center. 
            // The way it is implemented right now may cause problems for large-scale targets.
            if (distanceBetweenTargetAndCurrHitPos > minLookAwayDistToEnableEyeWarp)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determine whether the user is looking away from the preview. 
        /// </summary>
        /// <returns></returns>
        private bool IsLookingAwayFromPreview()
        {
            if (prevPreviewPos == null || EyeTrackingProvider == null)
            {
                return true;
            }

            Vector3 eyes2PrevPreview = prevPreviewPos.Value - EyeTrackingProvider.GazeOrigin;
            Vector3 eye2HitPos = EyeTrackingProvider.GazeDirection;

            float angle = Vector3.Angle(eyes2PrevPreview, eye2HitPos);
            float distance = EyeTrackingDemoUtils.VisAngleInDegreesToMeters(Vector3.Angle(eyes2PrevPreview, eye2HitPos), eye2HitPos.magnitude);

            if (distance < previewPlacemDistThresh)
            {
                return false;
            }

            // Check if the target is still hit by the eye gaze cursor
            if (EyeTrackingProvider.GazeTarget == previewGameObject)
            {
                return false;
            }

            // Check whether the user is still looking within the proximity of the target
            float distanceBetweenTargetAndCurrHitPos = Angle_ToCurrHitTarget(previewGameObject);
            if (distanceBetweenTargetAndCurrHitPos > minLookAwayDistToEnableEyeWarp)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the destination is plausible. For example, this means if the target is placeable
        /// on horizontal surfaces then only show a preview for (more or less) horizontal surfaces. 
        /// </summary>
        /// <returns>True if the target can be placed on this surface.</returns>
        private bool IsDestinationPlausible()
        {
            if (PlacementSurface == PlacementSurfaces.Horizontal)
            {
                float angle = Vector3.Angle(EyeTrackingProvider.HitNormal, Vector3.up);
                if (angle < maxDiffAngleForHorizontalPlacem) // If the angle is more than for example 20 degrees off from the up vector
                {
                    return true;
                }
            }

            else if (PlacementSurface == PlacementSurfaces.Vertical)
            {
                float angle = Vector3.Angle(EyeTrackingProvider.HitNormal, gameObject.transform.up);
                if (angle > minDiffAngleForVerticalPlacem)
                {
                    gameObject.transform.forward = -EyeTrackingProvider.HitNormal;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Retrieve a valid location for placing the target.
        /// </summary>
        /// <param name="hitobj"></param>
        /// <returns></returns>
        private Vector3 GetValidPlacemLocation(GameObject hitobj)
        {
            // Determine position
            Vector3 tCenter = hitobj.transform.position;
            Vector3 tScale = hitobj.transform.lossyScale;
            Vector3 vUp = Vector3.up;

            Vector3 newPos = (tCenter + tScale.y / 2 * Vector3.up);
            return newPos;
        }

        /// <summary>
        /// Turn on the preview of the to-be-placed target.
        /// </summary>
        private void ActivatePreview()
        {
            if (previewGameObject == null)
            {
                previewGameObject = Instantiate(gameObject);
                previewGameObject.GetComponent<Collider>().enabled = false;
                EyeTrackingDemoUtils.GameObject_ChangeTransparency(previewGameObject, transparency_preview);
                placePreviewAtHitPoint = false;
            }

            previewGameObject.SetActive(true);
        }

        /// <summary>
        /// Turn off the preview of the to-be-placed target.
        /// </summary>
        private void DeactivatePreview()
        {
            if (previewGameObject != null)
            {
                previewGameObject.SetActive(false);
                Destroy(previewGameObject);
                previewGameObject = null;
            }
        }

        /// <summary>
        /// Begin with the selection and movement of the focused target.
        /// </summary>
        public void DragAndDrop_Start()
        {
            if ((EyeTrackingProvider?.GazeTarget == gameObject) && (!objIsGrabbed))
            {
                if (AudioFeedbackPlayer.Instance != null)
                {
                    AudioFeedbackPlayer.Instance.PlaySound(audio_OnDragStart);
                }

                objIsGrabbed = true;
                EyeTrackingDemoUtils.GameObject_ChangeTransparency(gameObject, transparency_inTransition, ref originalTransparency);
                initialHandPos = handPos_absolute;
                initalGazeDir = new Vector3(EyeTrackingProvider.GazeDirection.x, EyeTrackingProvider.GazeDirection.y, EyeTrackingProvider.GazeDirection.z);

                Rigidbody rbody = GetComponent<Rigidbody>();
                if (rbody != null)
                {
                    originalUseGravity = rbody.useGravity;
                    originalDrag = rbody.drag;

                    rbody.useGravity = false;
                    rbody.drag = float.PositiveInfinity;
                }

                OnDragStart.Invoke();
            }
        }

        /// <summary>
        /// Finalize placing the currently selected target.
        /// </summary>
        public void DragAndDrop_Finish()
        {
            if (objIsGrabbed)
            {
                if (AudioFeedbackPlayer.Instance != null)
                {
                    AudioFeedbackPlayer.Instance.PlaySound(audio_OnDragStop);
                }

                if (onlyEyeWarpOnRelease)
                {
                    manualTargetManip = true;
                    if (plausibleLocation.HasValue)
                    {
                        MoveTargetTo(plausibleLocation.Value);
                    }
                }

                objIsGrabbed = false;
                DeactivatePreview();

                EyeTrackingDemoUtils.GameObject_ChangeTransparency(gameObject, originalTransparency);
                Rigidbody rbody = GetComponent<Rigidbody>();
                if (rbody != null)
                {
                    rbody.useGravity = originalUseGravity;
                    rbody.drag = originalDrag;
                }

                if (useAsSlider)
                {
                    OnDrop_SnapToClosestDecimal();
                }

                OnDrop.Invoke();
            }
        }

        /// <summary>
        /// Move the target using relative input values.
        /// </summary>
        /// <param name="relativeMovement"></param>
        private void RelativeMoveUpdate(Vector3 relativeMovement)
        {
            manualTargetManip = false;
            MoveTargetBy(relativeMovement);
        }

        /// <summary>
        /// Compute the angle between the initial (when selecting the target) and current eye gaze direction.
        /// </summary>
        /// <returns></returns>
        public float Angle_InitialGazeToCurrGazeDir()
        {
            return Vector3.Angle(initalGazeDir, EyeTrackingProvider.GazeDirection);
        }

        /// <summary>
        /// Compute angle between target center ( OR original targeting location??? ) and current targeting direction
        /// </summary>
        /// <returns></returns>
        public float Angle_ToCurrHitTarget(GameObject gobj)
        {
            if (EyeTrackingProvider?.GazeTarget != null)
            {
                // Target is currently hit
                if (EyeTrackingProvider?.GazeTarget == gobj)
                {
                    initalGazeDir = new Vector3(EyeTrackingProvider.GazeDirection.x, EyeTrackingProvider.GazeDirection.y, EyeTrackingProvider.GazeDirection.z);
                    return 0.0f;
                }

                float dist = Angle_InitialGazeToCurrGazeDir();
                return dist;
            }

            // No target currently hit
            return float.MinValue;
        }

        private bool HeadIsInMotion()
        {
            Vector3 pos = new Vector3(CameraCache.Main.transform.position.x, CameraCache.Main.transform.position.y, CameraCache.Main.transform.position.z);
            Vector3 forw = new Vector3(CameraCache.Main.transform.forward.x, CameraCache.Main.transform.forward.y, CameraCache.Main.transform.forward.z);

            if (head_prevRay != null)
            {
                float deltaPos = Vector3.Distance(head_prevRay.Value.origin, pos);
                float deltaDir = Vector3.Distance(head_prevRay.Value.direction, forw);
                if ((deltaPos != 0) && (deltaDir != 0))
                {
                    head_deltaDirf = deltaDir * head_smoothf + head_deltaDirf * (1 - head_smoothf);

                    if (head_deltaDirf > head_deltaDirThresh) // TODO: Combined metric of head location and rotation?
                        head_isInMotion = true;
                    else
                        head_isInMotion = false;
                }
            }
            else
                head_isInMotion = false;

            head_prevRay = new Ray(pos, forw);  // Update recent head move

            return head_isInMotion;
        }

        public void MoveTargetBy(Vector3 delta)
        {
            // Check that this game object is currently selected
            if (objIsGrabbed)
            {
                // Discrete gaze-supported target movement
                // Check whether the user is still looking within the proximity of the target
                bool headIsInMotion = HeadIsInMotion();
                if (headIsInMotion)
                {
                    initialHandPos = handPos_absolute;
                }

                float deltaHand = Vector3.Distance(initialHandPos, handPos_absolute);
                float angle_ToCurrHitTarget = Angle_ToCurrHitTarget(gameObject);

                // If manipulated via manual controller:
                if (ShouldObjBeWarped(deltaHand, angle_ToCurrHitTarget, headIsInMotion))
                {
                    // Discrete cursor-based target movement
                    Vector3 hitp = EyeTrackingProvider.HitPosition;

                    if (PlacementSurface == PlacementSurfaces.Horizontal)
                    {
                        hitp.y = hitp.y + gameObject.transform.localScale.y / 2;
                    }

                    Vector3 objp = gameObject.transform.position;

                    // Constrain in y-direction
                    gameObject.transform.position = new Vector3(
                        (((constrX + 1) % 2) * objp.x) + (constrX * hitp.x),
                        (((constrY + 1) % 2) * objp.y) + (constrY * hitp.y),
                        (((constrZ + 1) % 2) * objp.z) + (constrZ * hitp.z));

                    ConstrainMovement();

                    initialHandPos = handPos_absolute;
                }
                else
                {
                    // Continuous manual target movement
                    Vector3 oldPos = gameObject.transform.position;
                    Vector3 d = new Vector3(-delta.x * constrX, -delta.y * constrY, -delta.z * constrZ);
                    gameObject.transform.position = oldPos + d * handmapping;

                    ConstrainMovement();
                    UpdateSliderTextOutput();
                }
            }
        }

        private void UpdateSliderTextOutput()
        {
            if (txtOutput_sliderValue != null)
            {
                txtOutput_sliderValue.text = $"{((gameObject.transform.localPosition.x - LocalMinMax_X.x) / (LocalMinMax_X.y - LocalMinMax_X.x)): 0.00}";
            }
        }

        public void ConstrainMovement()
        {
            Vector3 locPos = gameObject.transform.localPosition;
            float rx, ry, rz;
            rx = Mathf.Clamp(locPos.x, LocalMinMax_X.x, LocalMinMax_X.y);
            ry = Mathf.Clamp(locPos.y, LocalMinMax_Y.x, LocalMinMax_Y.y);
            rz = Mathf.Clamp(locPos.z, LocalMinMax_Z.x, LocalMinMax_Z.y);

            gameObject.transform.localPosition = new Vector3(rx, ry, rz);
        }

        public void OnDrop_SnapToClosestDecimal()
        {
            if ((slider_snapToNearestDecimal != 0)
                && (!float.IsPositiveInfinity(LocalMinMax_X.x)) && (!float.IsNegativeInfinity(LocalMinMax_X.x))
                && (!float.IsPositiveInfinity(LocalMinMax_X.y)) && (!float.IsNegativeInfinity(LocalMinMax_X.y)))
            {
                Vector3 locPos = gameObject.transform.localPosition;
                float normalizedValue = (locPos.x - LocalMinMax_X.x) / (LocalMinMax_X.y - LocalMinMax_X.x);
                locPos.x = (Mathf.Round(normalizedValue / slider_snapToNearestDecimal) * slider_snapToNearestDecimal) * (LocalMinMax_X.y - LocalMinMax_X.x) + LocalMinMax_X.x;
                gameObject.transform.localPosition = locPos;
                UpdateSliderTextOutput();
            }
        }

        public void MoveTargetTo(Vector3 destination)
        {
            // Check that this game object is currently selected
            if (objIsGrabbed)
            {
                // Discrete gaze-supported target movement
                // Check whether the user is still looking within the proximity of the target
                bool headIsInMotion = HeadIsInMotion();
                if (headIsInMotion)
                {
                    initialHandPos = handPos_absolute;
                }

                float deltaHand = Vector3.Distance(initialHandPos, handPos_absolute);

                // Handle manipulation via hands/manual controllers
                if (ShouldObjBeWarped(deltaHand, Angle_ToCurrHitTarget(gameObject), headIsInMotion))
                {
                    // Discrete cursor-based target movement
                    if (PlacementSurface == PlacementSurfaces.Horizontal)
                    {
                        destination.y = destination.y + gameObject.transform.localScale.y / 2;
                    }

                    Vector3 objp = gameObject.transform.position;

                    // Constrain movement
                    gameObject.transform.position = new Vector3(
                        (((constrX + 1) % 2) * objp.x) + (constrX * destination.x),
                        (((constrY + 1) % 2) * objp.y) + (constrY * destination.y),
                        (((constrZ + 1) % 2) * objp.z) + (constrZ * destination.z));

                    initialHandPos = handPos_absolute;
                }
            }
        }

        private bool ShouldObjBeWarped(float deltaHand, float distTargetAndHitPos, bool headIsInMotion)
        {
            if ((manualTargetManip && (previewGameObject != null) && (previewGameObject.activeSelf)) ||
                ((!onlyEyeWarpOnRelease) &&
              // If manipulated via hands, head and eyes:
              (deltaHand > deltaHandMovemThresh) &&                // 1. Check that *hand* moved sufficiently to indicate the user's intent to move the target
              (distTargetAndHitPos > minLookAwayDistToEnableEyeWarp) &&       // 2. Check that *eye gaze* is sufficiently far away from the selected target
              !headIsInMotion))                               // 3. Check that *head* is not currently moving as this would otherwise cause the target to be moved automatically
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}