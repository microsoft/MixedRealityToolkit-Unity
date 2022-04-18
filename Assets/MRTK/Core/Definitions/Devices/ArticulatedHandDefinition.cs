// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Defines the interactions and data that an articulated hand can provide.
    /// </summary>
    public class ArticulatedHandDefinition : BaseInputSourceDefinition
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="source">The input source backing this definition instance. Used for raising events.</param>
        /// <param name="handedness">The handedness that this definition instance represents.</param>
        public ArticulatedHandDefinition(IMixedRealityInputSource source, Handedness handedness) : base(handedness)
        {
            InputSource = source;
        }

        /// <summary>
        /// The input source backing this definition instance.
        /// </summary>
        protected IMixedRealityInputSource InputSource { get; }

        private readonly float cursorBeamBackwardTolerance = 0.5f;
        private readonly float cursorBeamUpTolerance = 0.8f;

        private Dictionary<TrackedHandJoint, MixedRealityPose> unityJointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();
        private MixedRealityPose currentIndexPose = MixedRealityPose.ZeroIdentity;

        // Minimum distance between the index and the thumb tip required to enter a pinch
        private readonly float minimumPinchDistance = 0.015f;

        // Maximum distance between the index and thumb tip required to exit the pinch gesture
        private readonly float maximumPinchDistance = 0.1f;

        // Default enterPinchDistance value
        private float enterPinchDistance = 0.02f;

        /// <summary>
        /// The distance between the index finger tip and the thumb tip required to enter the pinch/air tap selection gesture.
        /// The pinch gesture enter will be registered for all values less than the EnterPinchDistance. The default EnterPinchDistance value is 0.02 and must be between 0.015 and 0.1. 
        /// </summary>
        public float EnterPinchDistance
        {
            get => enterPinchDistance;
            set
            {
                if (value >= minimumPinchDistance && value <= maximumPinchDistance)
                {
                    enterPinchDistance = value;
                }
                else
                {
                    Debug.LogError("EnterPinchDistance must be between 0.015 and 0.1, please change Enter Pinch Distance in the Leap Motion Device Manager Profile");
                }
            }
        }

        // Default exitPinchDistance value
        private float exitPinchDistance = 0.05f;

        /// <summary>
        /// The distance between the index finger tip and the thumb tip required to exit the pinch/air tap gesture.
        /// The pinch gesture exit will be registered for all values greater than the ExitPinchDistance. The default ExitPinchDistance value is 0.05 and must be between 0.015 and 0.1. 
        /// </summary>
        public float ExitPinchDistance
        {
            get => exitPinchDistance;
            set
            {
                if (value >= minimumPinchDistance && value <= maximumPinchDistance)
                {
                    exitPinchDistance = value;
                }
                else
                {
                    Debug.LogError("ExitPinchDistance must be between 0.015 and 0.1, please change Exit Pinch Distance in the Leap Motion Device Manager Profile");
                }
            }
        }

        /// <summary>
        /// The articulated hands default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right articulated hands.</remarks>
        [System.Obsolete("Call GetDefaultMappings(Handedness) instead.")]
        public MixedRealityInteractionMapping[] DefaultInteractions
        {
            get
            {
                MixedRealityInteractionMapping[] defaultInteractions = new MixedRealityInteractionMapping[DefaultMappings.Length];
                for (int i = 0; i < DefaultMappings.Length; i++)
                {
                    defaultInteractions[i] = new MixedRealityInteractionMapping((uint)i, DefaultMappings[i]);
                }
                return defaultInteractions;
            }
        }

        /// <summary>
        /// The articulated hands default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right articulated hands.</remarks>
        protected override MixedRealityInputActionMapping[] DefaultMappings => new[]
        {
            new MixedRealityInputActionMapping("Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInputActionMapping("Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip),
            new MixedRealityInputActionMapping("Select", AxisType.Digital, DeviceInputType.Select),
            new MixedRealityInputActionMapping("Grab", AxisType.SingleAxis, DeviceInputType.GripPress),
            new MixedRealityInputActionMapping("Index Finger Pose", AxisType.SixDof, DeviceInputType.IndexFinger),
            new MixedRealityInputActionMapping("Teleport Pose", AxisType.DualAxis, DeviceInputType.ThumbStick),
        };


        // Internal calculation of what the HandRay should be
        // Useful as a fallback for hand ray data
        protected virtual IHandRay HandRay { get; } = new HandRay();

        /// <summary>
        /// Calculates whether the current pose allows for pointing/distant interactions.
        /// Equivalent to the HandRay's ShouldShowRay implementation <see cref="HandRay.ShouldShowRay"/>
        /// </summary>
        public bool IsInPointingPose
        {
            get
            {
                MixedRealityPose palmJoint;
                if (unityJointPoses.TryGetValue(TrackedHandJoint.Palm, out palmJoint))
                {
                    Vector3 palmNormal = palmJoint.Rotation * (-1 * Vector3.up);
                    if (cursorBeamBackwardTolerance >= 0 && CameraCache.Main != null)
                    {
                        if (Vector3.Dot(palmNormal.normalized, -CameraCache.Main.transform.forward) > cursorBeamBackwardTolerance)
                        {
                            return false;
                        }
                    }
                    if (cursorBeamUpTolerance >= 0)
                    {
                        if (Vector3.Dot(palmNormal, Vector3.up) > cursorBeamUpTolerance)
                        {
                            return false;
                        }
                    }
                }
                return !IsInTeleportPose;
            }
        }

        /// <summary>
        /// Calculates whether the current pose is the one to start a teleport action
        /// </summary>
        protected bool IsInTeleportPose
        {
            get
            {
                if (!unityJointPoses.TryGetValue(TrackedHandJoint.Palm, out var palmPose)) return false;

                Camera mainCamera = CameraCache.Main;

                if (mainCamera == null)
                {
                    return false;
                }

                Transform cameraTransform = mainCamera.transform;

                Vector3 palmNormal = -palmPose.Up;
                // We check if the palm up is roughly in line with the camera up
                return Vector3.Dot(palmNormal, cameraTransform.up) > 0.6f
                       // Thumb must be extended, and middle must be grabbing
                       && !isThumbGrabbing && isMiddleGrabbing;
            }
        }

        /// <summary>
        /// A bool tracking whether the hand definition is pinch or not
        /// </summary>
        private bool isPinching = false;

        /// <summary>
        /// Calculates whether the current the current joint pose is selecting (air tap gesture).
        /// </summary>
        public bool IsPinching
        {
            get
            {
                MixedRealityPose thumbTip;
                MixedRealityPose indexTip;
                if (unityJointPoses.TryGetValue(TrackedHandJoint.ThumbTip, out thumbTip) && unityJointPoses.TryGetValue(TrackedHandJoint.IndexTip, out indexTip))
                {
                    float distance = Vector3.Distance(thumbTip.Position, indexTip.Position);

                    if (isPinching && distance > ExitPinchDistance)
                    {
                        isPinching = false;
                    }
                    else if (!isPinching && distance < EnterPinchDistance)
                    {
                        isPinching = true;
                    }
                }
                else
                {
                    isPinching = false;
                }

                return isPinching;
            }
        }

        public bool IsGrabbing => isIndexGrabbing && isMiddleGrabbing;

        private bool isIndexGrabbing;
        private bool isMiddleGrabbing;
        private bool isThumbGrabbing;

        // Velocity internal states
        private float deltaTimeStart;
        private const int velocityUpdateInterval = 6;
        private int frameOn = 0;

        private readonly Vector3[] velocityPositionsCache = new Vector3[velocityUpdateInterval];
        private readonly Vector3[] velocityNormalsCache = new Vector3[velocityUpdateInterval];
        private Vector3 velocityPositionsSum = Vector3.zero;
        private Vector3 velocityNormalsSum = Vector3.zero;

        public Vector3 AngularVelocity { get; protected set; }

        public Vector3 Velocity { get; protected set; }


        private static readonly ProfilerMarker UpdateHandJointsPerfMarker = new ProfilerMarker("[MRTK] ArticulatedHandDefinition.UpdateHandJoints");

        #region Hand Definition Update functions

        /// <summary>
        /// Updates the current hand joints with new data.
        /// </summary>
        /// <param name="jointPoses">The new joint poses.</param>
        public void UpdateHandJoints(Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses)
        {
            using (UpdateHandJointsPerfMarker.Auto())
            {
                unityJointPoses = jointPoses;
                CoreServices.InputSystem?.RaiseHandJointsUpdated(InputSource, Handedness, unityJointPoses);
            }
        }

        private static readonly ProfilerMarker UpdateCurrentIndexPosePerfMarker = new ProfilerMarker("[MRTK] ArticulatedHandDefinition.UpdateCurrentIndexPose");

        /// <summary>
        /// Updates the MixedRealityInteractionMapping with the latest index pose and fires a corresponding pose event.
        /// </summary>
        /// <param name="interactionMapping">The index finger's interaction mapping.</param>
        public void UpdateCurrentIndexPose(MixedRealityInteractionMapping interactionMapping)
        {
            using (UpdateCurrentIndexPosePerfMarker.Auto())
            {
                if (unityJointPoses.TryGetValue(TrackedHandJoint.IndexTip, out currentIndexPose))
                {
                    // Update the interaction data source
                    interactionMapping.PoseData = currentIndexPose;

                    // If our value changed raise it
                    if (interactionMapping.Changed)
                    {
                        // Raise input system event if it's enabled
                        CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, Handedness, interactionMapping.MixedRealityInputAction, currentIndexPose);
                    }
                }
            }
        }

        // Used to track the input that was last raised
        private bool previousReadyToTeleport = false;

        private IMixedRealityTeleportPointer teleportPointer;

        private static readonly ProfilerMarker UpdateCurrentTeleportPosePerfMarker = new ProfilerMarker("[MRTK] ArticulatedHandDefinition.UpdateCurrentTeleportPose");

        /// <summary>
        /// Updates the MixedRealityInteractionMapping with the lastest teleport pose status and fires an event when appropriate
        /// </summary>
        /// <param name="interactionMapping">The teleport action's interaction mapping.</param>
        public void UpdateCurrentTeleportPose(MixedRealityInteractionMapping interactionMapping)
        {
            using (UpdateCurrentTeleportPosePerfMarker.Auto())
            {
                // Check if we're focus locked or near something interactive to avoid teleporting unintentionally.
                bool anyPointersLockedWithHand = false;
                for (int i = 0; i < InputSource?.Pointers?.Length; i++)
                {
                    IMixedRealityPointer mixedRealityPointer = InputSource.Pointers[i];
                    if (mixedRealityPointer.IsNull()) continue;
                    if (mixedRealityPointer is IMixedRealityNearPointer nearPointer)
                    {
                        anyPointersLockedWithHand |= nearPointer.IsNearObject;
                    }
                    anyPointersLockedWithHand |= mixedRealityPointer.IsFocusLocked;

                    // If official teleport mode and we have a teleport pointer registered, we get the input action to trigger it.
                    if (teleportPointer == null && mixedRealityPointer is IMixedRealityTeleportPointer pointer)
                    {
                        teleportPointer = pointer;
                    }
                }

                // We close middle finger to signal spider-man gesture, and as being ready for teleport
                isIndexGrabbing = HandPoseUtils.IsIndexGrabbing(Handedness);
                isMiddleGrabbing = HandPoseUtils.IsMiddleGrabbing(Handedness);
                isThumbGrabbing = HandPoseUtils.IsThumbGrabbing(Handedness);
                bool isReadyForTeleport = !anyPointersLockedWithHand && IsInTeleportPose;

                // Tracks the input vector that should be sent out based on the gesture that is made
                Vector2 stickInput = (isReadyForTeleport && !isIndexGrabbing) ? Vector2.up : Vector2.zero;

                // The teleport event needs to be canceled if we have not completed the teleport motion and we were previously ready to teleport, but for some reason we
                // are no longer doing the ready to teleport gesture
                bool teleportCanceled = previousReadyToTeleport && !isReadyForTeleport && !isIndexGrabbing;
                if (teleportCanceled && teleportPointer != null)
                {
                    CoreServices.TeleportSystem?.RaiseTeleportCanceled(teleportPointer, null);
                    previousReadyToTeleport = isReadyForTeleport;
                    return;
                }

                // Update the interaction data source
                interactionMapping.Vector2Data = stickInput;

                // If our value changed raise it
                if (interactionMapping.Changed)
                {
                    CoreServices.InputSystem?.RaisePositionInputChanged(InputSource, Handedness, interactionMapping.MixedRealityInputAction, stickInput);
                }

                previousReadyToTeleport = isReadyForTeleport;
            }
        }

        /// <summary>
        /// Updates the MixedRealityInteractionMapping with the lastest pointer pose status and fires a corresponding pose event.
        /// </summary>
        /// <param name="interactionMapping">The pointer pose's interaction mapping.</param>
        public void UpdatePointerPose(MixedRealityInteractionMapping interactionMapping)
        {
            if (!unityJointPoses.TryGetValue(TrackedHandJoint.IndexKnuckle, out var knucklePose)) return;
            if (!unityJointPoses.TryGetValue(TrackedHandJoint.Palm, out var palmPose)) return;

            Vector3 rayPosition = knucklePose.Position;
            Vector3 palmNormal = -palmPose.Up;

            HandRay.Update(rayPosition, palmNormal, CameraCache.Main.transform, Handedness);
            Ray ray = HandRay.Ray;

            MixedRealityPose pointerPose = new MixedRealityPose();
            pointerPose.Position = ray.origin;
            pointerPose.Rotation = Quaternion.LookRotation(ray.direction);

            // Update the interaction data source
            interactionMapping.PoseData = pointerPose;

            // If our value changed raise it
            if (interactionMapping.Changed)
            {
                // Raise input system event if it's enabled
                CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, Handedness, interactionMapping.MixedRealityInputAction, pointerPose);
            }
        }

        /// <summary>
        /// Updates the hand definition with its velocity
        /// </summary>
        public void UpdateVelocity()
        {
            if (unityJointPoses.TryGetValue(TrackedHandJoint.Palm, out var currentPalmPose))
            {
                Vector3 palmPosition = currentPalmPose.Position;
                Vector3 palmNormal = -currentPalmPose.Up;

                if (frameOn < velocityUpdateInterval)
                {
                    velocityPositionsCache[frameOn] = palmPosition;
                    velocityPositionsSum += velocityPositionsCache[frameOn];
                    velocityNormalsCache[frameOn] = palmNormal;
                    velocityNormalsSum += velocityNormalsCache[frameOn];
                }
                else
                {
                    int frameIndex = frameOn % velocityUpdateInterval;

                    float deltaTime = Time.unscaledTime - deltaTimeStart;

                    Vector3 newPositionsSum = velocityPositionsSum - velocityPositionsCache[frameIndex] + palmPosition;
                    Vector3 newNormalsSum = velocityNormalsSum - velocityNormalsCache[frameIndex] + palmNormal;

                    Velocity = (newPositionsSum - velocityPositionsSum) / deltaTime / velocityUpdateInterval;

                    Quaternion rotation = Quaternion.FromToRotation(velocityNormalsSum / velocityUpdateInterval, newNormalsSum / velocityUpdateInterval);
                    Vector3 rotationRate = rotation.eulerAngles * Mathf.Deg2Rad;
                    AngularVelocity = rotationRate / deltaTime;

                    velocityPositionsCache[frameIndex] = palmPosition;
                    velocityNormalsCache[frameIndex] = palmNormal;
                    velocityPositionsSum = newPositionsSum;
                    velocityNormalsSum = newNormalsSum;
                }

                deltaTimeStart = Time.unscaledTime;
                frameOn++;
            }
        }

        #endregion
    }
}
