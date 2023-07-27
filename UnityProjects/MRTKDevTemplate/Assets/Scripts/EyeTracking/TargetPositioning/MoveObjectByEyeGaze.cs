// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Specifies whether the <see cref="GameObject"/> moves on horizontal or vertical surfaces.
    /// </summary>
    internal enum PlacementSurfaces
    {
        /// <summary>
        /// The <see cref="GameObject"/> can moves on horizontal surfaces.
        /// </summary>
        Horizontal,

        /// <summary>
        /// The <see cref="GameObject"/> can moves on vertical surfaces.
        /// </summary>
        Vertical
    }

    /// <summary>
    /// This script allows the user to move a GameObject, using ray or grab interactors to directly move
    /// the GameObject, or eye gaze to place the GameObject at the preview object's location.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/MoveObjectByEyeGaze")]
    public class MoveObjectByEyeGaze : StatefulInteractable
    {
        [Tooltip("Reference to the FuzzyGazeInteractor in the scene")]
        [SerializeField]
        private FuzzyGazeInteractor gazeInteractor;

        #region Serialized variables

        [Header("Eyes")]
        [Tooltip("Use this to toggle eye gaze interactions.")]
        [SerializeField]
        private bool useEyeSupportedTargetPlacement = false;

        [Tooltip("The user has to look away at least this far to enable the eye-supported target placement. " +
                 "This is to allow for local manual positioning using hand input.")]
        [SerializeField]
        [Range(1f, 10f)]
        private float minLookAwayDistToEnableEyeWarp = 5f;

        [Header("Hands")]
        [Tooltip("Use this to toggle hand based interactions.")]
        [SerializeField]
        private bool handInputEnabled = true;

        [Tooltip("To control whether the hand motion is used 1:1 to move a target or to use different gains to allow for smaller hand motions.")]
        [SerializeField]
        private float handMapping = 1f;

        [Tooltip("Minimal amount of hand movement to trigger target repositioning.")]
        [SerializeField]
        private float deltaHandMovementThreshold = 0.05f;

        [Header("Transitioning")]
        [Tooltip("Transparency of the target itself while dragging is active.")]
        [SerializeField]
        [Range(0, 1)]
        private float transparencyInTransition = 130 / 255f;

        [Tooltip("Transparency of the target preview while dragging it around.")]
        [SerializeField]
        [Range(0, 1)]
        private float transparencyPreview = 50 / 255f;

        [Tooltip(
            "Minimal distance between the old and new preview. This is to prevent the preview to always follow the eye gaze immediately. " +
            "The value should depend on the size of the target.")]
        [SerializeField]
        [Range(0, 1)]
        private float previewPlacementDistanceThreshold = 0.05f;

        [Header("Constrained Movement")]
        [Tooltip("Freeze the X position of the GameObject.")]
        [SerializeField]
        private bool freezeX = false;

        [Tooltip("Freeze the Y position of the GameObject.")]
        [SerializeField]
        private bool freezeY = false;

        [Tooltip("Freeze the Z position of the GameObject.")]
        [SerializeField]
        private bool freezeZ = false;

        [Tooltip("Specifies whether the GameObject moves on horizontal or vertical surfaces.")]
        [SerializeField]
        private PlacementSurfaces placementSurface = PlacementSurfaces.Horizontal;

        /// <summary>
        /// Limits the X position of the GameObject to the specified minimum and maximum.
        /// </summary>
        public Vector2 LocalMinMaxX = new Vector2(float.NegativeInfinity, float.PositiveInfinity);

        /// <summary>
        /// Limits the Y position of the GameObject to the specified minimum and maximum.
        /// </summary>
        public Vector2 LocalMinMaxY = new Vector2(float.NegativeInfinity, float.PositiveInfinity);

        /// <summary>
        /// Limits the Z position of the GameObject to the specified minimum and maximum.
        /// </summary>
        public Vector2 LocalMinMaxZ = new Vector2(float.NegativeInfinity, float.PositiveInfinity);
        
        [Tooltip("Fired when the GameObject is dropped.")]
        [SerializeField]
        private UnityEvent onDrop = null;
        #endregion

        #region Private variables
        private GameObject previewGameObject;

        private bool
            onlyEyeWarpOnRelease =
                true; // Only warp the currently grabbed target to the current look at location once the user releases the pinch gesture.        

        private float originalTransparency = -1f;
        private bool originalUseGravity = false;
        private float originalDrag = 1f;

        private bool onlyTransitionToPlausibleDestinations = true;
        private Vector3? plausibleLocation;
        private bool placePreviewAtHitPoint = true;

        private bool manualTargetManipulation = false;

        private Vector3 initialGazeDirection;
        private static bool isManipulatingUsingHands = false;
        private static bool isManipulatingUsingVoice = false;
        private Vector3 handPositionAbsolute;
        private Vector3 handPositionRelative;
        private Vector3 initialHandPosition;
        private static bool handIsPinching = false;
        private Handedness currentEngagedHand = Handedness.None;
        private bool objectIsGrabbed = false;

        private Ray? headPreviousRay;
        private float headDeltaDirectionThreshold = 0.05f;
        private float headSmooth = 0.1f;
        private float headDeltaDirection = 0f;

        private int ConstraintX => freezeX ? 0 : 1;
        private int ConstraintY => freezeY ? 0 : 1;
        private int ConstraintZ => freezeZ ? 0 : 1;

        Vector3? previousPreviewPosition;

        // The values represent the maximum angle that the surface can be offset from the 'up' vector to be considered for horizontal placement.
        // For example, if a surface is slanted by 40 degrees targets may slid off and hence we may consider this an invalid offset angle.
        private readonly float maxDiffAngleForHorizontalPlacement = 20f;

        // The values represents the minimal angle that the surface must be offset from the 'up' vector to be considered for vertical placement.
        private readonly float minDiffAngleForVerticalPlacement = 50f;
        #endregion

        /// <inheritdoc />
        public override float GetSelectionProgress()
        {
            return objectIsGrabbed ? 1f :  base.GetSelectionProgress();
        }

        /// <summary>
        /// Moves the GameObject while it is grabbed using hand interactors if enabled, and places a preview object at the eye gaze
        /// position if eye gaze interaction is enabled.
        /// </summary>
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            // Dynamic is effectively just your normal Update().
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                if (isManipulatingUsingHands && handInputEnabled)
                {
                    var handsSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<HandsSubsystem>();
                    if (handsSubsystem != null)
                    {
                        XRNode[] hands = { XRNode.RightHand, XRNode.LeftHand };
                        foreach (var hand in hands)
                        {
                            if (handsSubsystem.TryGetJoint(TrackedHandJoint.IndexTip, hand, out var palmJointPose))
                            {
                                Handedness handedness = hand == XRNode.RightHand ? Handedness.Right : Handedness.Left;
                                if (handPositionAbsolute == Vector3.zero)
                                {
                                    currentEngagedHand = handedness;
                                    handPositionAbsolute = palmJointPose.Pose.position;
                                }
                                else
                                {
                                    Vector3 oldHandPos = handPositionAbsolute;
                                    handPositionRelative = new Vector3(oldHandPos.x - palmJointPose.Pose.position.x,
                                        oldHandPos.y - palmJointPose.Pose.position.y,
                                        oldHandPos.z - palmJointPose.Pose.position.z);
                                    handPositionAbsolute = palmJointPose.Pose.position;

                                    if (handIsPinching && currentEngagedHand == handedness)
                                    {
                                        RelativeMoveUpdate(handPositionRelative);
                                    }
                                }
                            }
                        }
                    }
                }

                if (objectIsGrabbed && useEyeSupportedTargetPlacement)
                {
                    // Check whether the user is still looking within the proximity of the target
                    if (IsLookingAwayFromTarget() && IsLookingAwayFromPreview())
                    {
                        if (gazeInteractor.PreciseHitResult.targetInteractable != null &&
                            gazeInteractor.PreciseHitResult.raycastHit.transform.gameObject !=
                            this) // To prevent trying to place it on itself
                        {
                            plausibleLocation = null;

                            if (onlyTransitionToPlausibleDestinations)
                            {
                                if (IsDestinationPlausible())
                                {
                                    plausibleLocation = gazeInteractor.PreciseHitResult.raycastHit.point;
                                }
                                else
                                {
                                    plausibleLocation = GetValidPlacementLocation(gazeInteractor.PreciseHitResult.raycastHit.transform.gameObject);
                                }
                            }
                            else
                            {
                                plausibleLocation = gazeInteractor.PreciseHitResult.raycastHit.point;
                            }

                            // Only show preview and place target in plausible locations
                            if (plausibleLocation.HasValue)
                            {
                                ActivatePreview();

                                // Update preview position
                                if (placePreviewAtHitPoint)
                                {
                                    previewGameObject.transform.position = plausibleLocation.Value;
                                }
                                else
                                {
                                    if (placementSurface == PlacementSurfaces.Horizontal)
                                    {
                                        previewGameObject.transform.position = plausibleLocation.Value +
                                            previewGameObject.transform.localScale.y * Vector3.up /
                                            2;
                                    }
                                    else
                                    {
                                        previewGameObject.transform.position =
                                            plausibleLocation.Value;
                                    }

                                    previousPreviewPosition = previewGameObject.transform.position;
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
                    previousPreviewPosition = null;
                }
            }
        }

        #region Hand input handler
        /// <summary>
        /// Called when a user selects the GameObject using a hand based interactor.
        /// </summary>
        public void OnGrabSelectedEntered()
        {
            HandDrag_Start();
        }

        /// <summary>
        /// Called when a user deselects the GameObject using a hand based interactor.
        /// </summary>
        public void OnGrabSelectedExited()
        {
            HandDrag_Stop();
        }
        #endregion

        #region Voice input handler
        /// <summary>
        /// Called when a user selects the GameObject using a voice based interactor
        /// </summary>
        public void OnVoiceSelectedEntered()
        {
            isManipulatingUsingVoice = true;
            DragAndDrop_Start();
        }

        /// <summary>
        /// Called when a user deselects the GameObject using a voice based interactor
        /// </summary>
        public void OnVoiceSelectedExited()
        {
            DragAndDrop_Finish();
        }
        #endregion

        /// <summary>
        /// Start moving the target using your hands.
        /// </summary>
        private void HandDrag_Start()
        {
            if (handInputEnabled && !isManipulatingUsingHands && !isManipulatingUsingVoice)
            {
                isManipulatingUsingHands = true;
                handIsPinching = true;
                handPositionRelative = Vector3.zero;
                handPositionAbsolute = Vector3.zero;
                DragAndDrop_Start();
            }
        }

        /// <summary>
        /// Finish moving the target using your hands.
        /// </summary>
        private void HandDrag_Stop()
        {
            if (isManipulatingUsingHands)
            {
                isManipulatingUsingHands = false;
                handIsPinching = false;
                handPositionRelative = Vector3.zero;
                DragAndDrop_Finish();
                currentEngagedHand = Handedness.None;
            }
        }

        /// <summary>
        /// Check whether the user is looking away from the target.
        /// </summary>
        private bool IsLookingAwayFromTarget()
        {
            // First, let's check if the target is still hit by the eye gaze cursor
            if (gazeInteractor.PreciseHitResult.targetInteractable != null && gazeInteractor.PreciseHitResult.targetInteractable.transform.gameObject == this)
            {
                return false;
            }

            // Check whether the user is still looking within the proximity of the target
            float distanceBetweenTargetAndCurrHitPos = Angle_ToCurrHitTarget(gameObject);

            // **Note for potential improvement**: It would be better to make this dependent on the target's boundary 
            // instead of its center. The way it is implemented right now may cause problems for large-scale targets.
            return distanceBetweenTargetAndCurrHitPos > minLookAwayDistToEnableEyeWarp;
        }

        /// <summary>
        /// Determine whether the user is looking away from the preview. 
        /// </summary>
        private bool IsLookingAwayFromPreview()
        {
            if (previousPreviewPosition == null)
            {
                return true;
            }

            Vector3 eyes2PreviousPreview = previousPreviewPosition.Value - Camera.main.transform.position;
            Vector3 eye2HitPosition = Camera.main.transform.forward;

            float distance = EyeTrackingUtilities.VisAngleInDegreesToMeters(Vector3.Angle(eyes2PreviousPreview, eye2HitPosition), eye2HitPosition.magnitude);

            if (distance < previewPlacementDistanceThreshold)
            {
                return false;
            }

            // Check if the target is still hit by the eye gaze cursor
            if (gazeInteractor.PreciseHitResult.raycastHit.transform.gameObject == previewGameObject)
            {
                return false;
            }

            // Check whether the user is still looking within the proximity of the target
            float distanceBetweenTargetAndCurrHitPos = Angle_ToCurrHitTarget(previewGameObject);

            return distanceBetweenTargetAndCurrHitPos > minLookAwayDistToEnableEyeWarp;
        }

        /// <summary>
        /// Check if the destination is plausible. For example, this means if the target is placeable
        /// on horizontal surfaces then only show a preview for (more or less) horizontal surfaces. 
        /// </summary>
        /// <returns>True if the target can be placed on this surface.</returns>
        private bool IsDestinationPlausible()
        {
            if (placementSurface == PlacementSurfaces.Horizontal)
            {
                float angle = Vector3.Angle(gazeInteractor.PreciseHitResult.raycastHit.normal, Vector3.up);
                if (angle < maxDiffAngleForHorizontalPlacement) // If the angle is more than for example 20 degrees off from the up vector
                {
                    return true;
                }
            }

            else if (placementSurface == PlacementSurfaces.Vertical)
            {
                float angle = Vector3.Angle(gazeInteractor.PreciseHitResult.raycastHit.normal, gameObject.transform.up);
                if (angle > minDiffAngleForVerticalPlacement)
                {
                    gameObject.transform.forward = -gazeInteractor.PreciseHitResult.raycastHit.normal;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Retrieve a valid location for placing the target.
        /// </summary>
        private Vector3 GetValidPlacementLocation(GameObject hitObj)
        {
            // Determine position
            Vector3 tCenter = hitObj.transform.position;
            Vector3 tScale = hitObj.transform.lossyScale;

            Vector3 newPos = tCenter + tScale.y * 0.5f * Vector3.up;
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
                EyeTrackingUtilities.SetGameObjectTransparency(previewGameObject, transparencyPreview);
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
        private void DragAndDrop_Start()
        {
            if (!objectIsGrabbed && isHovered)
            {
                objectIsGrabbed = true;
                EyeTrackingUtilities.SetGameObjectTransparency(gameObject, transparencyInTransition, ref originalTransparency);
                initialHandPosition = handPositionAbsolute;
                initialGazeDirection = new Vector3(Camera.main.transform.forward.x, Camera.main.transform.forward.y, Camera.main.transform.forward.z);

                if (TryGetComponent<Rigidbody>(out var rigidbody))
                {
                    originalUseGravity = rigidbody.useGravity;
                    originalDrag = rigidbody.drag;

                    rigidbody.useGravity = false;
                    rigidbody.drag = float.PositiveInfinity;
                }
            }
        }

        /// <summary>
        /// Finalize placing the currently selected target.
        /// </summary>
        private void DragAndDrop_Finish()
        {
            if (objectIsGrabbed)
            {
                if (onlyEyeWarpOnRelease)
                {
                    manualTargetManipulation = true;
                    if (plausibleLocation.HasValue)
                    {
                        MoveTargetTo(plausibleLocation.Value);
                    }
                }

                isManipulatingUsingVoice = false;
                objectIsGrabbed = false;
                DeactivatePreview();

                EyeTrackingUtilities.SetGameObjectTransparency(gameObject, originalTransparency);
                if (TryGetComponent<Rigidbody>(out var rigidbody))
                {
                    rigidbody.useGravity = originalUseGravity;
                    rigidbody.drag = originalDrag;
                }

                onDrop.Invoke();
            }
        }

        /// <summary>
        /// Move the target using relative input values.
        /// </summary>
        private void RelativeMoveUpdate(Vector3 relativeMovement)
        {
            manualTargetManipulation = false;
            MoveTargetBy(relativeMovement);
        }

        /// <summary>
        /// Compute the angle between the initial (when selecting the target) and current eye gaze direction.
        /// </summary>
        private float Angle_InitialGazeToCurrGazeDir()
        {
            return Vector3.Angle(initialGazeDirection, Camera.main.transform.forward);
        }

        /// <summary>
        /// Compute angle between target center and current targeting direction
        /// </summary>
        private float Angle_ToCurrHitTarget(GameObject target)
        {
            if (gazeInteractor.PreciseHitResult.targetInteractable != null)
            {
                // Target is currently hit
                if (gazeInteractor.PreciseHitResult.targetInteractable.transform.gameObject == target)
                {
                    initialGazeDirection = new Vector3(Camera.main.transform.forward.x, Camera.main.transform.forward.y, Camera.main.transform.forward.z);
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
            bool headIsInMotion = false;

            Vector3 pos = Camera.main.transform.position;
            Vector3 forward = Camera.main.transform.forward;

            if (headPreviousRay != null)
            {
                float deltaPos = Vector3.Distance(headPreviousRay.Value.origin, pos);
                float deltaDir = Vector3.Distance(headPreviousRay.Value.direction, forward);
                if (deltaPos != 0f && deltaDir != 0f)
                {
                    headDeltaDirection = deltaDir * headSmooth + headDeltaDirection * (1f - headSmooth);

                    headIsInMotion = headDeltaDirection > headDeltaDirectionThreshold;
                }
            }

            headPreviousRay = new Ray(pos, forward);  // Update recent head move

            return headIsInMotion;
        }

        private void MoveTargetBy(Vector3 delta)
        {
            // Check that this game object is currently selected
            if (objectIsGrabbed)
            {
                // Discrete gaze-supported target movement
                // Check whether the user is still looking within the proximity of the target
                bool headIsInMotion = HeadIsInMotion();
                if (headIsInMotion)
                {
                    initialHandPosition = handPositionAbsolute;
                }

                float deltaHand = Vector3.Distance(initialHandPosition, handPositionAbsolute);
                float angleToCurrHitTarget = Angle_ToCurrHitTarget(gameObject);

                // If manipulated via manual controller:
                if (ShouldObjBeWarped(deltaHand, angleToCurrHitTarget, headIsInMotion))
                {
                    Vector3 hitPosition = gazeInteractor.PreciseHitResult.raycastHit.point;

                    // Discrete cursor-based target movement
                    if (placementSurface == PlacementSurfaces.Horizontal)
                    {
                        hitPosition.y += gameObject.transform.localScale.y * 0.5f;
                    }

                    Vector3 objectPosition = gameObject.transform.position;

                    // Constrain in y-direction
                    gameObject.transform.position = new Vector3(
                        (((ConstraintX + 1) % 2) * objectPosition.x) + (ConstraintX * hitPosition.x),
                        (((ConstraintY + 1) % 2) * objectPosition.y) + (ConstraintY * hitPosition.y),
                        (((ConstraintZ + 1) % 2) * objectPosition.z) + (ConstraintZ * hitPosition.z));

                    ConstrainMovement();

                    initialHandPosition = handPositionAbsolute;
                }
                else
                {
                    // Continuous manual target movement
                    Vector3 oldPos = gameObject.transform.position;
                    Vector3 d = new Vector3(-delta.x * ConstraintX, -delta.y * ConstraintY, -delta.z * ConstraintZ);
                    gameObject.transform.position = oldPos + d * handMapping;

                    ConstrainMovement();
                }
            }
        }

        private void ConstrainMovement()
        {
            Vector3 locPos = gameObject.transform.localPosition;
            float rx = Mathf.Clamp(locPos.x, LocalMinMaxX.x, LocalMinMaxX.y);
            float ry = Mathf.Clamp(locPos.y, LocalMinMaxY.x, LocalMinMaxY.y);
            float rz = Mathf.Clamp(locPos.z, LocalMinMaxZ.x, LocalMinMaxZ.y);

            gameObject.transform.localPosition = new Vector3(rx, ry, rz);
        }

        private void MoveTargetTo(Vector3 destination)
        {
            // Check that this game object is currently selected
            if (objectIsGrabbed)
            {
                // Discrete gaze-supported target movement
                // Check whether the user is still looking within the proximity of the target
                bool headIsInMotion = HeadIsInMotion();
                if (headIsInMotion)
                {
                    initialHandPosition = handPositionAbsolute;
                }

                float deltaHand = Vector3.Distance(initialHandPosition, handPositionAbsolute);

                // Handle manipulation via hands/manual controllers
                if (ShouldObjBeWarped(deltaHand, Angle_ToCurrHitTarget(gameObject), headIsInMotion))
                {
                    // Discrete cursor-based target movement
                    if (placementSurface == PlacementSurfaces.Horizontal)
                    {
                        destination.y += gameObject.transform.localScale.y * 0.5f;
                    }

                    Vector3 objectPosition = gameObject.transform.position;

                    // Constrain movement
                    gameObject.transform.position = new Vector3(
                        ((ConstraintX + 1) % 2 * objectPosition.x) + (ConstraintX * destination.x),
                        ((ConstraintY + 1) % 2 * objectPosition.y) + (ConstraintY * destination.y),
                        ((ConstraintZ + 1) % 2 * objectPosition.z) + (ConstraintZ * destination.z));

                    initialHandPosition = handPositionAbsolute;
                }
            }
        }

        private bool ShouldObjBeWarped(float deltaHand, float distTargetAndHitPos, bool headIsInMotion)
        {
            return (manualTargetManipulation && previewGameObject != null && previewGameObject.activeSelf) ||
                   (!onlyEyeWarpOnRelease &&
                    // If manipulated via hands, head and eyes:
                    (deltaHand >
                     deltaHandMovementThreshold) && // 1. Check that *hand* moved sufficiently to indicate the user's intent to move the target
                    (distTargetAndHitPos >
                     minLookAwayDistToEnableEyeWarp) && // 2. Check that *eye gaze* is sufficiently far away from the selected target
                    !headIsInMotion); // 3. Check that *head* is not currently moving as this would otherwise cause the target to be moved automatically
        }
    }
}
