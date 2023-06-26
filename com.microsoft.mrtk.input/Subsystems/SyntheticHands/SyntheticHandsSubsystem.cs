// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using HandshapeId = Microsoft.MixedReality.Toolkit.Input.HandshapeTypes.HandshapeId;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A Unity subsystem that enables articulated hand simulation.
    /// </summary>    
    [Preserve]
    [MRTKSubsystem(
        Name = "com.microsoft.mixedreality.synthhands",
        DisplayName = "Subsystem for Hand Synthesis",
        Author = "Microsoft",
        ProviderType = typeof(SyntheticHandsProvider),
        SubsystemTypeOverride = typeof(SyntheticHandsSubsystem),
        ConfigType = typeof(SyntheticHandsConfig))]
    public class SyntheticHandsSubsystem : HandsSubsystem
    {
        private SyntheticHandsProvider syntheticProvider;

        /// <summary>
        /// Get the subsystem provider for this object.
        /// </summary>
        private SyntheticHandsProvider SyntheticProvider
        {
            get
            {
                if (syntheticProvider == null || syntheticProvider != provider)
                {
                    syntheticProvider = provider as SyntheticHandsProvider;
                }
                return syntheticProvider;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Register()
        {
            SyntheticHandsConfig config = XRSubsystemHelpers.GetConfiguration<SyntheticHandsConfig, SyntheticHandsSubsystem>();
            if (config != null && !config.ShouldSynthesize())
            {
                // If we're not configured for this scenario, don't run.
                return;
            }

            // Fetch subsystem metadata from the attribute.
            var cinfo = XRSubsystemHelpers.ConstructCinfo<SyntheticHandsSubsystem, HandsSubsystemCinfo>();

            // Populate remaining cinfo field.
            cinfo.IsPhysicalData = false;

            if (!Register(cinfo))
            {
                Debug.LogError($"Failed to register the {cinfo.Name} subsystem.");
            }
        }

        /// <summary>
        /// Requests the neutral pose for the specified hand.
        /// </summary>
        /// <param name="handNode">The hand for which the neutral pose is being requested.</param>
        /// <returns>
        /// Identifier representing the hand pose.
        /// </returns>
        [Obsolete("Please use the GetNeutralHandshape(handNode) instead.")]
        public HandshapeId GetNeutralPose(XRNode handNode) => GetNeutralHandshape(handNode);

        /// <summary>
        /// Sets the neutral pose for the specified hand.
        /// </summary>
        /// <param name="handNode">The hand for which the neutral pose is being set.</param>
        /// <param name="poseId">The desired hand pose.</param>
        [Obsolete("Please use the SetNeutralHandshape(handNode, handshapeId) instead.")]
        public void SetNeutralPose(XRNode handNode, HandshapeId poseId) => SetNeutralHandshape(handNode, poseId);

        /// <summary>
        /// Requests the selection pose for the specified hand.
        /// </summary>
        /// <param name="handNode">The hand for which the selection pose is being requested.</param>
        /// <returns>
        /// Identifier representing the hand pose.
        /// </returns>
        [Obsolete("Please use the GetSelectionHandshape(handNode) instead.")]
        public HandshapeId GetSelectionPose(XRNode handNode) => GetSelectionHandshape(handNode);

        /// <summary>
        /// Sets the selection pose for the specified hand.
        /// </summary>
        /// <param name="handNode">The hand for which the selection pose is being set.</param>
        /// <param name="poseId">The desired hand pose.</param>
        [Obsolete("Please use the SetSelectionHandshape(handNode, handshapeId) instead.")]
        public void SetSelectionPose(XRNode handNode, HandshapeId poseId) => SetSelectionHandshape(handNode, poseId);

        /// <summary>
        /// Requests the neutral handshape for the specified hand.
        /// </summary>
        /// <param name="handNode">The hand for which the neutral handshape is being requested.</param>
        /// <returns>
        /// Identifier representing the hand handshape.
        /// </returns>
        public HandshapeId GetNeutralHandshape(XRNode handNode)
        {
            return SyntheticProvider.GetNeutralHandshape(handNode);
        }

        /// <summary>
        /// Sets the neutral handshape for the specified hand.
        /// </summary>
        /// <param name="handNode">The hand for which the neutral handshape is being set.</param>
        /// <param name="handshapeId">The desired hand handshape.</param>
        public void SetNeutralHandshape(XRNode handNode, HandshapeId handshapeId)
        {
            SyntheticProvider.SetNeutralHandshape(handNode, handshapeId);
        }

        /// <summary>
        /// Requests the selection handshape for the specified hand.
        /// </summary>
        /// <param name="handNode">The hand for which the selection handshape is being requested.</param>
        /// <returns>
        /// Identifier representing the hand handshape.
        /// </returns>
        public HandshapeId GetSelectionHandshape(XRNode handNode)
        {
            return SyntheticProvider.GetSelectionHandshape(handNode);
        }

        /// <summary>
        /// Sets the selection handshape for the specified hand.
        /// </summary>
        /// <param name="handNode">The hand for which the selection handshape is being set.</param>
        /// <param name="handshapeId">The desired hand handshape.</param>
        public void SetSelectionHandshape(XRNode handNode, HandshapeId handshapeId)
        {
            SyntheticProvider.SetSelectionHandshape(handNode, handshapeId);
        }

        private class SyntheticHandContainer : HandDataContainer
        {
            // The current handshape in hand-space, untransformed.
            private HandJointPose[] currentHandshape = new HandJointPose[(int)TrackedHandJoint.TotalJoints];

            // The Input Action associated with the root position of this hand.
            private InputActionProperty positionAction;

            // The Input Action associated with the root rotation of this hand.
            private InputActionProperty rotationAction;

            // The Input Action associated with the XRI selection action for this hand.
            private InputActionProperty selectAction;

            // The stored position for this hand.
            // Used as default/stale value when no new value is available.
            private Vector3 handPosition;

            // The stored rotation for this hand.
            // Used as default/stale value when no new value is available.
            private Quaternion handRotation;

            // The stored pinch progress for this hand.
            // Used as default/stale value when no new value is available.
            private float selectAmount;

            // The static, local-space offset from the hand position to the origin
            // of the stored/serialized simulated pose. Populated by the subsystem config.
            // TODO: Should eventually be replaced by actually re-serializing the
            // hand poses with the correct origin.
            private Vector3 poseOffset;

            public SyntheticHandContainer(
                                        XRNode handNode,
                                        HandshapeId baseHandshape,
                                        InputActionProperty positionAction,
                                        InputActionProperty rotationAction,
                                        InputActionProperty selectAction,
                                        Vector3 poseOffset) : base(handNode)
            {
                NeturalHandshape = baseHandshape;

                this.positionAction = positionAction;
                this.rotationAction = rotationAction;
                this.selectAction = selectAction;
                this.poseOffset = poseOffset;
            }

            public void Start()
            {
                positionAction.EnableDirectAction();
                rotationAction.EnableDirectAction();
                selectAction.EnableDirectAction();
            }

            public void Stop()
            {
                positionAction.DisableDirectAction();
                rotationAction.DisableDirectAction();
                selectAction.DisableDirectAction();
            }

            private static readonly ProfilerMarker TryGetEntireHandPerfMarker =
                new ProfilerMarker("[MRTK] SyntheticHandsSubsystem.TryGetEntireHand");

            /// <summary>
            /// Gets or sets the synthetic hand's neutral hand shape.
            /// </summary>
            public HandshapeId NeturalHandshape { get; set; } = HandshapeId.Open;

            /// <summary>
            /// Gets or sets the synthetic hand's selection hand shape.
            /// </summary>
            public HandshapeId SelectionHandshape { get; set; } = HandshapeId.Pinch;

            /// <inheritdoc/>
            public override bool TryGetEntireHand(out IReadOnlyList<HandJointPose> result)
            {
                using (TryGetEntireHandPerfMarker.Auto())
                {
                    if (!AlreadyFullQueried)
                    {
                        TryCalculateEntireHand();
                    }

                    result = handJoints;
                    return FullQueryValid;
                }
            }

            private static readonly ProfilerMarker TryGetJointPerfMarker =
                new ProfilerMarker("[MRTK] SyntheticHandsSubsystem.TryGetJoint");

            /// <inheritdoc/>
            public override bool TryGetJoint(TrackedHandJoint joint, out HandJointPose pose)
            {
                using (TryGetJointPerfMarker.Auto())
                {
                    bool thisQueryValid = false;

                    // If we happened to have already queried the entire
                    // hand data this frame, we don't need to re-query for
                    // just the joint. If we haven't, we do still need to
                    // query for the single joint.
                    if (!AlreadyFullQueried)
                    {
                        // No single-joint queries for simulated hands (yet)
                        TryCalculateEntireHand();
                        thisQueryValid = FullQueryValid;
                    }
                    else
                    {
                        // If we've already run a full-hand query, this single joint query
                        // is just as valid as the full query.
                        thisQueryValid = FullQueryValid;
                    }

                    pose = handJoints[HandsUtils.ConvertToIndex(joint)];
                    return thisQueryValid;
                }
            }

            private static readonly int JointCount = (int)TrackedHandJoint.TotalJoints;

            private static readonly ProfilerMarker TryCalculateEntireHandPerfMarker =
                new ProfilerMarker("[MRTK] SyntheticHandsSubsystem.TryCalculateEntireHand");

            /// <summary>
            /// For a certain hand, query every Bone in the hand, and write all results to the
            /// handJoints collection. This will also mark handsQueriedThisFrame[handNode] = true.
            /// </summary>
            private void TryCalculateEntireHand()
            {
                using (TryCalculateEntireHandPerfMarker.Auto())
                {
                    FullQueryValid = true;

                    // Mark this hand as having been fully queried this frame.
                    // If any joint is queried again this frame, we'll reuse the
                    // information to avoid extra work.
                    AlreadyFullQueried = true;

                    UpdateHandshape();

                    // ActiveControl will be null if the position/pose has not yet been "actuated".
                    // Thus, if there's no "recently actuated control", we just manually resolve the
                    // binding and grab the control list ourselves, and assume the first control is
                    // the one we want.
                    InputControl positionControl = positionAction.action?.activeControl ??
                        (positionAction.action.HasAnyControls() ? positionAction.action?.controls[0] : null);

                    if (positionControl?.device is TrackedDevice positionTrackedDevice)
                    {
                        var trackingState = (InputTrackingState)positionTrackedDevice.trackingState.ReadValue();
                        if ((trackingState & InputTrackingState.Position) != 0)
                        {
                            handPosition = positionAction.action.ReadValue<Vector3>();
                        }
                        else
                        {
                            FullQueryValid = false;
                        }
                    }
                    else
                    {
                        FullQueryValid = false;
                    }

                    // ActiveControl will be null if the rotation/pose has not yet been "actuated".
                    // Thus, if there's no "recently actuated control", we just manually resolve the
                    // binding and grab the control list ourselves, and assume the first control is
                    // the one we want.
                    InputControl rotationControl = rotationAction.action?.activeControl ??
                        (rotationAction.action.HasAnyControls() ? rotationAction.action.controls[0] : null);

                    if (rotationControl?.device is TrackedDevice rotationTrackedDevice)
                    {
                        var trackingState = (InputTrackingState)rotationTrackedDevice.trackingState.ReadValue();
                        if ((trackingState & InputTrackingState.Position) != 0)
                        {
                            handRotation = rotationAction.action.ReadValue<Quaternion>();
                        }
                        else
                        {
                            FullQueryValid = false;
                        }
                    }
                    else
                    {
                        FullQueryValid = false;
                    }

                    // Null checks against Unity objects can be expensive, especially when you do
                    // it 52 times per frame (26 hand joints across 2 hands). Instead, we manage
                    // the playspace transformation internally for hand joints.
                    // Joints are relative to the camera floor offset object.
                    Transform origin = PlayspaceUtilities.XROrigin.CameraFloorOffsetObject.transform;
                    if (origin == null)
                    {
                        return;
                    }

                    for (int i = 0; i < JointCount; i++)
                    {
                        // Joints are recorded for the right hand. If we're the left
                        // hand, mirror the joint pose!
                        if (HandNode == XRNode.LeftHand)
                        {
                            MirrorJoint(ref currentHandshape[i]);
                        }

                        handJoints[i] = new HandJointPose(
                            origin.TransformPoint((handRotation * currentHandshape[i].Position) + handPosition),
                            origin.rotation * (handRotation * currentHandshape[i].Rotation),
                            currentHandshape[i].Radius);
                    }
                }
            }

            private static readonly ProfilerMarker UpdatehandshapePerfMarker =
                new ProfilerMarker("[MRTK] SyntheticHandsSubsystem.Updatehandshape");

            /// <summary>
            /// Given the current state of the various input actions,
            /// write the current local-space handshape into the currentHandshape array.
            /// </summary>
            private void UpdateHandshape()
            {
                using (UpdatehandshapePerfMarker.Auto())
                {
                    SimulatedArticulatedHandshapes.GetHandshapeJointPoseData(NeturalHandshape, out HandJointPose[] baseData);
                    SimulatedArticulatedHandshapes.GetHandshapeJointPoseData(SelectionHandshape, out HandJointPose[] pinchData);

                    selectAmount = selectAction.action.ReadValue<float>();

                    for (int i = 0; i < JointCount; i++)
                    {
                        currentHandshape[i].Position = Vector3.Lerp(baseData[i].Position, pinchData[i].Position, selectAmount) + poseOffset;
                        currentHandshape[i].Rotation = Quaternion.Slerp(baseData[i].Rotation, pinchData[i].Rotation, selectAmount);
                        currentHandshape[i].Radius = Mathf.Lerp(baseData[i].Radius, pinchData[i].Radius, selectAmount);
                    }
                }
            }

            // Flips/rotates a hand joint pose. This mirrors the joint's position across
            // the x-axis, and applies a mirror transformation to the joint quaternions (x,-y,-z,w).
            private static void MirrorJoint(ref HandJointPose pose)
            {
                pose.Position = Vector3.Scale(pose.Position, new Vector3(-1, 1, 1));
                pose.Rotation = new Quaternion(pose.Rotation.x, -pose.Rotation.y, -pose.Rotation.z, pose.Rotation.w);
            }
        }

        /// <summary>
        /// A subsystem provider for <see cref="SyntheticHandsSubsystem"/> that provides methods for getting and setting
        /// the simulated hand poses, as well as methods for obtain hand joint poses.
        /// </summary>
        [Preserve]
        class SyntheticHandsProvider : Provider
        {
            protected SyntheticHandsConfig Config { get; private set; }

            private Dictionary<XRNode, SyntheticHandContainer> hands = null;

            public override void Start()
            {
                base.Start();

                Config = XRSubsystemHelpers.GetConfiguration<SyntheticHandsConfig, SyntheticHandsSubsystem>();

                hands ??= new Dictionary<XRNode, SyntheticHandContainer>
                {
                    { XRNode.LeftHand, new SyntheticHandContainer(
                                                                XRNode.LeftHand,
                                                                HandshapeId.Flat,
                                                                Config.LeftHandPosition,
                                                                Config.LeftHandRotation,
                                                                Config.LeftHandSelect,
                                                                Config.PoseOffset) },
                    { XRNode.RightHand, new SyntheticHandContainer(
                                                                XRNode.RightHand,
                                                                HandshapeId.Flat,
                                                                Config.RightHandPosition,
                                                                Config.RightHandRotation,
                                                                Config.RightHandSelect,
                                                                Config.PoseOffset) }
                };

                hands[XRNode.LeftHand].Start();
                hands[XRNode.RightHand].Start();

                InputSystem.onBeforeUpdate += ResetHands;
            }

            public override void Stop()
            {
                ResetHands();
                InputSystem.onBeforeUpdate -= ResetHands;

                hands[XRNode.LeftHand].Stop();
                hands[XRNode.RightHand].Stop();

                base.Stop();
            }

            private void ResetHands()
            {
                hands[XRNode.LeftHand].Reset();
                hands[XRNode.RightHand].Reset();
            }

            /// <summary>
            /// Requests the neutral pose for the specified hand.
            /// </summary>
            /// <param name="handNode">The hand for which the neutral pose is being requested.</param>
            /// <returns>
            /// Identifier representing the hand pose.
            /// </returns>
            [Obsolete("Please use the GetNeutralHandshape(handNode) instead.")]
            public HandshapeId GetNeutralPose(XRNode handNode) => GetNeutralHandshape(handNode);

            /// <summary>
            /// Sets the neutral pose for the specified hand.
            /// </summary>
            /// <param name="handNode">The hand for which the neutral pose is being set.</param>
            /// <param name="poseId">The desired hand pose.</param>
            [Obsolete("Please use the SetNeutralHandshape(handNode, handshapeId) instead.")]
            public void SetNeutralPose(XRNode handNode, HandshapeId poseId) => SetNeutralHandshape(handNode, poseId);

            /// <summary>
            /// Requests the selection pose for the specified hand.
            /// </summary>
            /// <param name="handNode">The hand for which the selection pose is being requested.</param>
            /// <returns>
            /// Identifier representing the hand pose.
            /// </returns>
            [Obsolete("Please use the GetSelectionHandshape(handNode) instead.")]
            public HandshapeId GetSelectionPose(XRNode handNode) => GetSelectionHandshape(handNode);

            /// <summary>
            /// Sets the selection pose for the specified hand.
            /// </summary>
            /// <param name="handNode">The hand for which the selection pose is being set.</param>
            /// <param name="poseId">The desired hand pose.</param>
            [Obsolete("Please use the SetSelectionHandshape(handNode, handshapeId) instead.")]
            public void SetSelectionPose(XRNode handNode, HandshapeId poseId) => SetSelectionHandshape(handNode, poseId);

            /// <summary>
            /// Requests the neutral handshape for the specified hand.
            /// </summary>
            /// <param name="handNode">The hand for which the neutral handshape is being requested.</param>
            /// <returns>
            /// handshapeId for the desired handshape.
            /// </returns>
            public HandshapeId GetNeutralHandshape(XRNode handNode)
            {
                return hands[handNode].NeturalHandshape;
            }

            /// <summary>
            /// Sets the neutral handshape for the specified hand.
            /// </summary>
            /// <param name="handNode">The hand for which the neutral handshape is being set.</param>
            /// <param name="handshapeId">The desired hand handshape.</param>
            public void SetNeutralHandshape(XRNode handNode, HandshapeId handshapeId)
            {
                hands[handNode].NeturalHandshape = handshapeId;
            }

            /// <summary>
            /// Requests the selection handshape for the specified hand.
            /// </summary>
            /// <param name="handNode">The hand for which the selection handshape is being requested.</param>
            /// <returns>
            /// Identifier representing the hand handshape.
            /// </returns>
            public HandshapeId GetSelectionHandshape(XRNode handNode)
            {
                return hands[handNode].SelectionHandshape;
            }

            /// <summary>
            /// Sets the selection handshape for the specified hand.
            /// </summary>
            /// <param name="handNode">The hand for which the selection handshape is being set.</param>
            /// <param name="handshapeId">The desired hand handshape.</param>
            public void SetSelectionHandshape(XRNode handNode, HandshapeId handshapeId)
            {
                hands[handNode].SelectionHandshape = handshapeId;
            }

            #region IHandsSubsystem implementation

            ///<inheritdoc/>
            public override bool TryGetEntireHand(XRNode handNode, out IReadOnlyList<HandJointPose> jointPoses)
            {
                Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, "Non-hand XRNode used in GetHand query");

                if (!Config.ShouldSynthesize())
                {
                    jointPoses = Array.Empty<HandJointPose>();
                    return false;
                }

                return hands[handNode].TryGetEntireHand(out jointPoses);
            }

            ///<inheritdoc/>
            public override bool TryGetJoint(TrackedHandJoint joint, XRNode handNode, out HandJointPose jointPose)
            {
                Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, "Non-hand XRNode used in GetHand query");

                if (!Config.ShouldSynthesize())
                {
                    jointPose = default;
                    return false;
                }

                return hands[handNode].TryGetJoint(joint, out jointPose);
            }

            #endregion IHandsSubsystem implementation
        }
    }
}
