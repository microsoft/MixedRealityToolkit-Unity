// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    using Input;
    using Subsystems;
    using UnityEngine.XR;
    using UnityEngine.XR.Interaction.Toolkit;

    public enum PlacementSurfaces
    {
        Horizontal,
        Vertical
    }

    [AddComponentMenu("Scripts/MRTK/Examples/MoveObjByEyeGaze")]
    public class MoveObjByEyeGaze : StatefulInteractable
    {
        [SerializeField]
        private FuzzyGazeInteractor _gazeInteractor;

        #region Serialized variables

        [Header("Eyes")]
        [SerializeField]
        private bool _useEyeSupportedTargetPlacement = false;

        [Tooltip("The user has to look away at least this far to enable the eye-supported target placement. " +
                 "This is to allow for local manual positioning using hand input.")]
        [SerializeField]
        [Range(1f, 10f)]
        private float _minLookAwayDistToEnableEyeWarp = 5f;

        [Header("Hands")]
        [Tooltip("Use this to enforce only voice commands to move targets.")]
        [SerializeField]
        private bool _handInputEnabled = true;

        [Tooltip(
            "To control whether the hand motion is used 1:1 to move a target or to use different gains to allow for smaller hand motions.")]
        [SerializeField]
        private float _handmapping = 1f;

        [Tooltip("Minimal amount of hand movement to trigger target repositioning.")]
        [SerializeField]
        private float _deltaHandMovemThresh = 0.05f;

        [Header("Transitioning")]
        [Tooltip("Transparency of the target itself while dragging is active.")]
        [SerializeField]
        [Range(0, 1)]
        private float _transparencyInTransition = 130 / 255f;

        [Tooltip("Transparency of the target preview while dragging it around.")]
        [SerializeField]
        [Range(0, 1)]
        private float _transparencyPreview = 50 / 255f;

        [Tooltip(
            "Minimal distance between the old and new preview. This is to prevent the preview to always follow the eye gaze immediately. " +
            "The value should depend on the size of the target.")]
        [SerializeField]
        [Range(0, 1)]
        private float _previewPlacemDistThresh = 0.05f;

        [Header("Constrained Movement")]
        [SerializeField]
        private bool _freezeX = false;

        [SerializeField] private bool _freezeY = false;

        [SerializeField] private bool _freezeZ = false;

        public Vector2 LocalMinMaxX = new Vector2(float.NegativeInfinity, float.PositiveInfinity);
        public Vector2 LocalMinMaxY = new Vector2(float.NegativeInfinity, float.PositiveInfinity);
        public Vector2 LocalMinMaxZ = new Vector2(float.NegativeInfinity, float.PositiveInfinity);

        public PlacementSurfaces PlacementSurface = PlacementSurfaces.Horizontal;

        [SerializeField]
        private UnityEvent OnDrop = null;

        #endregion

        #region Private variables

        private GameObject _previewGameObject;

        private bool
            _onlyEyeWarpOnRelease =
                true; // Only warp the currently grabbed target to the current look at location once the user releases the pinch gesture.        

        private float _originalTransparency = -1f;
        private bool _originalUseGravity = false;
        private float _originalDrag = 1f;

        private bool _onlyTransitionToPlausibleDestinations = true;
        private Vector3? _plausibleLocation;
        private bool _placePreviewAtHitPoint = true;

        private bool _manualTargetManip = false;

        private Vector3 _initalGazeDir;
        private static bool _isManipulatingUsingHands = false;
        private static bool _isManipulatingUsingVoice = false;
        private Vector3 _handPosAbsolute;
        private Vector3 _handPosRelative;
        private Vector3 _initialHandPos;
        private static bool _handIsPinching = false;
        private Handedness _currEngagedHand = Handedness.None;
        private bool _objIsGrabbed = false;

        private Ray? _headPrevRay;
        private float _headDeltaDirThresh = 0.05f;
        private float _headSmoothf = 0.1f;
        private bool _headIsInMotion;
        private float _headDeltaDirf = 0f;

        private int ConstrX => _freezeX ? 0 : 1;
        private int ConstrY => _freezeY ? 0 : 1;
        private int ConstrZ => _freezeZ ? 0 : 1;

        Vector3? _prevPreviewPos;

        // The values represent the maximum angle that the surface can be offset from the 'up' vector to be considered for horizontal placement.
        // For example, if a surface is slanted by 40 degrees targets may slid off and hence we may consider this an invalid offset angle.
        private readonly float _maxDiffAngleForHorizontalPlacem = 20f;

        // The values represents the minimal angle that the surface must be offset from the 'up' vector to be considered for vertical placement.
        private readonly float _minDiffAngleForVerticalPlacem = 50f;

        #endregion

        public override float Selectedness()
        {
            return _objIsGrabbed ? 1f : base.Selectedness();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            // Dynamic is effectively just your normal Update().
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                if (_isManipulatingUsingHands && _handInputEnabled)
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
                                if (_handPosAbsolute == Vector3.zero)
                                {
                                    _currEngagedHand = handedness;
                                    _handPosAbsolute = palmJointPose.Pose.position;
                                }
                                else
                                {
                                    Vector3 oldHandPos = _handPosAbsolute;
                                    _handPosRelative = new Vector3(oldHandPos.x - palmJointPose.Pose.position.x,
                                        oldHandPos.y - palmJointPose.Pose.position.y,
                                        oldHandPos.z - palmJointPose.Pose.position.z);
                                    _handPosAbsolute = palmJointPose.Pose.position;

                                    if (_handIsPinching && _currEngagedHand == handedness)
                                    {
                                        RelativeMoveUpdate(_handPosRelative);
                                    }
                                }
                            }
                        }
                    }
                }

                if (_objIsGrabbed && _useEyeSupportedTargetPlacement)
                {
                    // Check whether the user is still looking within the proximity of the target
                    if (IsLookingAwayFromTarget() && IsLookingAwayFromPreview())
                    {
                        if (_gazeInteractor.PreciseHitResult.targetInteractable != null &&
                            _gazeInteractor.PreciseHitResult.raycastHit.transform.gameObject !=
                            this) // To prevent trying to place it on itself
                        {
                            _plausibleLocation = null;

                            if (_onlyTransitionToPlausibleDestinations)
                            {
                                if (IsDestinationPlausible())
                                {
                                    _plausibleLocation = _gazeInteractor.PreciseHitResult.raycastHit.point;
                                }
                                else
                                {
                                    _plausibleLocation = GetValidPlacementLocation(_gazeInteractor.PreciseHitResult.raycastHit.transform.gameObject);
                                }
                            }
                            else
                            {
                                _plausibleLocation = _gazeInteractor.PreciseHitResult.raycastHit.point;
                            }

                            // Only show preview and place target in plausible locations
                            if (_plausibleLocation.HasValue)
                            {
                                ActivatePreview();

                                // Update preview position
                                if (_placePreviewAtHitPoint)
                                {
                                    _previewGameObject.transform.position = _plausibleLocation.Value;
                                }
                                else
                                {
                                    if (PlacementSurface == PlacementSurfaces.Horizontal)
                                    {
                                        _previewGameObject.transform.position = _plausibleLocation.Value +
                                            _previewGameObject.transform.localScale.y * Vector3.up /
                                            2;
                                    }
                                    else
                                    {
                                        _previewGameObject.transform.position =
                                            _plausibleLocation.Value;
                                    }

                                    _prevPreviewPos = _previewGameObject.transform.position;
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
                    _prevPreviewPos = null;
                }
            }
        }

        #region Hand input handler
        public void OnGrabSelectedEntered()
        {
            HandDrag_Start();
        }

        public void OnGrabSelectedExited()
        {
            HandDrag_Stop();
        }
        #endregion

        #region Voice input handler
        public void OnVoiceSelectedEntered()
        {
            _isManipulatingUsingVoice = true;
            DragAndDrop_Start();
        }

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
            if (_handInputEnabled && !_isManipulatingUsingHands && !_isManipulatingUsingVoice)
            {
                _isManipulatingUsingHands = true;
                _handIsPinching = true;
                _handPosRelative = Vector3.zero;
                _handPosAbsolute = Vector3.zero;
                DragAndDrop_Start();
            }
        }

        /// <summary>
        /// Finish moving the target using your hands.
        /// </summary>
        private void HandDrag_Stop()
        {
            if (_isManipulatingUsingHands)
            {
                _isManipulatingUsingHands = false;
                _handIsPinching = false;
                _handPosRelative = Vector3.zero;
                DragAndDrop_Finish();
                _currEngagedHand = Handedness.None;
            }
        }

        /// <summary>
        /// Check whether the user is looking away from the target.
        /// </summary>
        private bool IsLookingAwayFromTarget()
        {
            // First, let's check if the target is still hit by the eye gaze cursor
            if (_gazeInteractor.PreciseHitResult.targetInteractable.transform.gameObject == this)
            {
                return false;
            }

            // Check whether the user is still looking within the proximity of the target
            float distanceBetweenTargetAndCurrHitPos = Angle_ToCurrHitTarget(gameObject);

            // **Note for potential improvement**: It would be better to make this dependent on the target's boundary 
            // instead of its center. The way it is implemented right now may cause problems for large-scale targets.
            return distanceBetweenTargetAndCurrHitPos > _minLookAwayDistToEnableEyeWarp;
        }

        /// <summary>
        /// Determine whether the user is looking away from the preview. 
        /// </summary>
        private bool IsLookingAwayFromPreview()
        {
            if (_prevPreviewPos == null)
            {
                return true;
            }

            Vector3 eyes2PrevPreview = _prevPreviewPos.Value - Camera.main.transform.position;
            Vector3 eye2HitPos = Camera.main.transform.forward;

            float distance = EyeTrackingDemoUtils.VisAngleInDegreesToMeters(Vector3.Angle(eyes2PrevPreview, eye2HitPos), eye2HitPos.magnitude);

            if (distance < _previewPlacemDistThresh)
            {
                return false;
            }

            // Check if the target is still hit by the eye gaze cursor
            if (_gazeInteractor.PreciseHitResult.raycastHit.transform.gameObject == _previewGameObject)
            {
                return false;
            }

            // Check whether the user is still looking within the proximity of the target
            float distanceBetweenTargetAndCurrHitPos = Angle_ToCurrHitTarget(_previewGameObject);
            if (distanceBetweenTargetAndCurrHitPos > _minLookAwayDistToEnableEyeWarp)
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
                float angle = Vector3.Angle(_gazeInteractor.PreciseHitResult.raycastHit.normal, Vector3.up);
                if (angle < _maxDiffAngleForHorizontalPlacem) // If the angle is more than for example 20 degrees off from the up vector
                {
                    return true;
                }
            }

            else if (PlacementSurface == PlacementSurfaces.Vertical)
            {
                float angle = Vector3.Angle(_gazeInteractor.PreciseHitResult.raycastHit.normal, gameObject.transform.up);
                if (angle > _minDiffAngleForVerticalPlacem)
                {
                    gameObject.transform.forward = -_gazeInteractor.PreciseHitResult.raycastHit.normal;
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
            if (_previewGameObject == null)
            {
                _previewGameObject = Instantiate(gameObject);
                _previewGameObject.GetComponent<Collider>().enabled = false;
                EyeTrackingDemoUtils.GameObject_ChangeTransparency(_previewGameObject, _transparencyPreview);
                _placePreviewAtHitPoint = false;
            }

            _previewGameObject.SetActive(true);
        }

        /// <summary>
        /// Turn off the preview of the to-be-placed target.
        /// </summary>
        private void DeactivatePreview()
        {
            if (_previewGameObject != null)
            {
                _previewGameObject.SetActive(false);
                Destroy(_previewGameObject);
                _previewGameObject = null;
            }
        }

        /// <summary>
        /// Begin with the selection and movement of the focused target.
        /// </summary>
        public void DragAndDrop_Start()
        {
            if (!_objIsGrabbed && isHovered)
            {
                _objIsGrabbed = true;
                EyeTrackingDemoUtils.GameObject_ChangeTransparency(gameObject, _transparencyInTransition, ref _originalTransparency);
                _initialHandPos = _handPosAbsolute;
                _initalGazeDir = new Vector3(Camera.main.transform.forward.x, Camera.main.transform.forward.y, Camera.main.transform.forward.z);

                if (TryGetComponent<Rigidbody>(out var rbody))
                {
                    _originalUseGravity = rbody.useGravity;
                    _originalDrag = rbody.drag;

                    rbody.useGravity = false;
                    rbody.drag = float.PositiveInfinity;
                }
            }
        }

        /// <summary>
        /// Finalize placing the currently selected target.
        /// </summary>
        public void DragAndDrop_Finish()
        {
            if (_objIsGrabbed)
            {
                if (_onlyEyeWarpOnRelease)
                {
                    _manualTargetManip = true;
                    if (_plausibleLocation.HasValue)
                    {
                        MoveTargetTo(_plausibleLocation.Value);
                    }
                }

                _isManipulatingUsingVoice = false;
                _objIsGrabbed = false;
                DeactivatePreview();

                EyeTrackingDemoUtils.GameObject_ChangeTransparency(gameObject, _originalTransparency);
                if (TryGetComponent<Rigidbody>(out var rbody))
                {
                    rbody.useGravity = _originalUseGravity;
                    rbody.drag = _originalDrag;
                }

                OnDrop.Invoke();
            }
        }

        /// <summary>
        /// Move the target using relative input values.
        /// </summary>
        private void RelativeMoveUpdate(Vector3 relativeMovement)
        {
            _manualTargetManip = false;
            MoveTargetBy(relativeMovement);
        }

        /// <summary>
        /// Compute the angle between the initial (when selecting the target) and current eye gaze direction.
        /// </summary>
        public float Angle_InitialGazeToCurrGazeDir()
        {
            return Vector3.Angle(_initalGazeDir, Camera.main.transform.forward);
        }

        /// <summary>
        /// Compute angle between target center ( OR original targeting location??? ) and current targeting direction
        /// </summary>
        public float Angle_ToCurrHitTarget(GameObject gobj)
        {
            if (_gazeInteractor.PreciseHitResult.targetInteractable != null)
            {
                // Target is currently hit
                if (_gazeInteractor.PreciseHitResult.targetInteractable.transform.gameObject == gobj)
                {
                    _initalGazeDir = new Vector3(Camera.main.transform.forward.x, Camera.main.transform.forward.y, Camera.main.transform.forward.z);
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
            Vector3 pos = Camera.main.transform.position;
            Vector3 forward = Camera.main.transform.forward;

            if (_headPrevRay != null)
            {
                float deltaPos = Vector3.Distance(_headPrevRay.Value.origin, pos);
                float deltaDir = Vector3.Distance(_headPrevRay.Value.direction, forward);
                if (deltaPos != 0f && deltaDir != 0f)
                {
                    _headDeltaDirf = deltaDir * _headSmoothf + _headDeltaDirf * (1f - _headSmoothf);

                    _headIsInMotion = _headDeltaDirf > _headDeltaDirThresh;
                }
            }
            else
                _headIsInMotion = false;

            _headPrevRay = new Ray(pos, forward);  // Update recent head move

            return _headIsInMotion;
        }

        public void MoveTargetBy(Vector3 delta)
        {
            // Check that this game object is currently selected
            if (_objIsGrabbed)
            {
                // Discrete gaze-supported target movement
                // Check whether the user is still looking within the proximity of the target
                bool headIsInMotion = HeadIsInMotion();
                if (headIsInMotion)
                {
                    _initialHandPos = _handPosAbsolute;
                }

                float deltaHand = Vector3.Distance(_initialHandPos, _handPosAbsolute);
                float angleToCurrHitTarget = Angle_ToCurrHitTarget(gameObject);

                // If manipulated via manual controller:
                if (ShouldObjBeWarped(deltaHand, angleToCurrHitTarget, headIsInMotion))
                {
                    Vector3 hitPosition = _gazeInteractor.PreciseHitResult.raycastHit.point;

                    // Discrete cursor-based target movement
                    if (PlacementSurface == PlacementSurfaces.Horizontal)
                    {
                        hitPosition.y += gameObject.transform.localScale.y * 0.5f;
                    }

                    Vector3 objp = gameObject.transform.position;

                    // Constrain in y-direction
                    gameObject.transform.position = new Vector3(
                        (((ConstrX + 1) % 2) * objp.x) + (ConstrX * hitPosition.x),
                        (((ConstrY + 1) % 2) * objp.y) + (ConstrY * hitPosition.y),
                        (((ConstrZ + 1) % 2) * objp.z) + (ConstrZ * hitPosition.z));

                    ConstrainMovement();

                    _initialHandPos = _handPosAbsolute;
                }
                else
                {
                    // Continuous manual target movement
                    Vector3 oldPos = gameObject.transform.position;
                    Vector3 d = new Vector3(-delta.x * ConstrX, -delta.y * ConstrY, -delta.z * ConstrZ);
                    gameObject.transform.position = oldPos + d * _handmapping;

                    ConstrainMovement();
                }
            }
        }

        public void ConstrainMovement()
        {
            Vector3 locPos = gameObject.transform.localPosition;
            float rx = Mathf.Clamp(locPos.x, LocalMinMaxX.x, LocalMinMaxX.y);
            float ry = Mathf.Clamp(locPos.y, LocalMinMaxY.x, LocalMinMaxY.y);
            float rz = Mathf.Clamp(locPos.z, LocalMinMaxZ.x, LocalMinMaxZ.y);

            gameObject.transform.localPosition = new Vector3(rx, ry, rz);
        }

        /*public void OnDrop_SnapToClosestDecimal()
        {
            if (_sliderSnapToNearestDecimal != 0f
                && !float.IsPositiveInfinity(LocalMinMaxX.x) && !float.IsNegativeInfinity(LocalMinMaxX.x)
                && !float.IsPositiveInfinity(LocalMinMaxX.y) && !float.IsNegativeInfinity(LocalMinMaxX.y))
            {
                Vector3 locPos = gameObject.transform.localPosition;
                float normalizedValue = (locPos.x - LocalMinMaxX.x) / (LocalMinMaxX.y - LocalMinMaxX.x);
                locPos.x = (Mathf.Round(normalizedValue / _sliderSnapToNearestDecimal) * _sliderSnapToNearestDecimal) * (LocalMinMaxX.y - LocalMinMaxX.x) + LocalMinMaxX.x;
                gameObject.transform.localPosition = locPos;
            }
        }*/

        public void MoveTargetTo(Vector3 destination)
        {
            // Check that this game object is currently selected
            if (_objIsGrabbed)
            {
                // Discrete gaze-supported target movement
                // Check whether the user is still looking within the proximity of the target
                bool headIsInMotion = HeadIsInMotion();
                if (headIsInMotion)
                {
                    _initialHandPos = _handPosAbsolute;
                }

                float deltaHand = Vector3.Distance(_initialHandPos, _handPosAbsolute);

                // Handle manipulation via hands/manual controllers
                if (ShouldObjBeWarped(deltaHand, Angle_ToCurrHitTarget(gameObject), headIsInMotion))
                {
                    // Discrete cursor-based target movement
                    if (PlacementSurface == PlacementSurfaces.Horizontal)
                    {
                        destination.y += gameObject.transform.localScale.y * 0.5f;
                    }

                    Vector3 objp = gameObject.transform.position;

                    // Constrain movement
                    gameObject.transform.position = new Vector3(
                        ((ConstrX + 1) % 2 * objp.x) + (ConstrX * destination.x),
                        ((ConstrY + 1) % 2 * objp.y) + (ConstrY * destination.y),
                        ((ConstrZ + 1) % 2 * objp.z) + (ConstrZ * destination.z));

                    _initialHandPos = _handPosAbsolute;
                }
            }
        }

        private bool ShouldObjBeWarped(float deltaHand, float distTargetAndHitPos, bool headIsInMotion)
        {
            return (_manualTargetManip && _previewGameObject != null && _previewGameObject.activeSelf) ||
                   (!_onlyEyeWarpOnRelease &&
                    // If manipulated via hands, head and eyes:
                    (deltaHand >
                     _deltaHandMovemThresh) && // 1. Check that *hand* moved sufficiently to indicate the user's intent to move the target
                    (distTargetAndHitPos >
                     _minLookAwayDistToEnableEyeWarp) && // 2. Check that *eye gaze* is sufficiently far away from the selected target
                    !headIsInMotion); // 3. Check that *head* is not currently moving as this would otherwise cause the target to be moved automatically
        }
    }
}
