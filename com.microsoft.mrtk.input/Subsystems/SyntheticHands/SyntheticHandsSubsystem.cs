// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;
using UnityEngine.XR;

using GestureId = Microsoft.MixedReality.Toolkit.Input.GestureTypes.GestureId;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [Preserve]
    [MRTKSubsystem(
        Name = "com.microsoft.mixedreality.synthhands",
        DisplayName = "Subsystem for Hand Synthesis",
        Author = "Microsoft",
        ProviderType = typeof(SynthesisProvider),
        SubsystemTypeOverride = typeof(SyntheticHandsSubsystem),
        ConfigType = typeof(SyntheticHandsConfig))]
    public class SyntheticHandsSubsystem : HandsSubsystem
    {
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
        public GestureId GetNeutralPose(XRNode handNode)
        {
            return (provider as SynthesisProvider).GetNeutralPose(handNode);
        }

        /// <summary>
        /// Sets the neutral pose for the specified hand.
        /// </summary>
        /// <param name="handNode">The hand for which the neutral pose is being set.</param>
        /// <param name="poseId">The desired hand pose.</param>
        public void SetNeutralPose(XRNode handNode, GestureId poseId)
        {
            (provider as SynthesisProvider).SetNeutralPose(handNode, poseId);
        }

        private class SyntheticHandContainer : HandDataContainer
        {
            // The current gesture in hand-space, untransformed.
            private HandJointPose[] currentGesture = new HandJointPose[(int)TrackedHandJoint.TotalJoints];

            // The 'neutral' hand pose (ex: flat or ready position) to be displayed.
            private GestureId neutralHandPose = GestureId.Flat;

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
                                        GestureId baseHandPose,
                                        InputActionProperty positionAction,
                                        InputActionProperty rotationAction,
                                        InputActionProperty selectAction,
                                        Vector3 poseOffset) : base(handNode)
            {
                this.neutralHandPose = baseHandPose;
                this.positionAction = positionAction;
                this.rotationAction = rotationAction;
                this.selectAction = selectAction;
                this.poseOffset = poseOffset;
            }

            private static readonly ProfilerMarker TryGetEntireHandPerfMarker =
                new ProfilerMarker("[MRTK] SyntheticHandsSubsystem.TryGetEntireHand");

            /// <summary>
            /// Gets or sets the synthetic hand's neutral pose.
            /// </summary>
            public GestureId NeutralPose
            {
                get => neutralHandPose;
                set => neutralHandPose = value;
            }

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

            /// <summary/>
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

                    UpdateGesture();

                    if (positionAction.action?.activeControl?.device is TrackedDevice positionTrackedDevice)
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

                    if (rotationAction.action?.activeControl?.device is TrackedDevice rotationTrackedDevice)
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
                    Transform playspaceTransform = PlayspaceUtilities.ReferenceTransform;
                    if (playspaceTransform == null)
                    {
                        return;
                    }

                    for (int i = 0; i < JointCount; i++)
                    {
                        handJoints[i] = new HandJointPose(
                            playspaceTransform.TransformPoint((handRotation * currentGesture[i].Position) + handPosition),
                            playspaceTransform.rotation * (handRotation * currentGesture[i].Rotation),
                            currentGesture[i].Radius);
                    }
                }
            }

            private static readonly ProfilerMarker UpdateGesturePerfMarker =
                new ProfilerMarker("[MRTK] SyntheticHandsSubsystem.UpdateGesture");

            /// <summary/>
            /// Given the current state of the various input actions,
            /// write the current local-space gesture into the currentGesture array.
            /// </summary>
            private void UpdateGesture()
            {
                using (UpdateGesturePerfMarker.Auto())
                {
                    SimulatedArticulatedHandPoses.GetGesturePose(neutralHandPose, out HandJointPose[] baseData);
                    SimulatedArticulatedHandPoses.GetGesturePose(GestureId.Pinch, out HandJointPose[] pinchData);

                    selectAmount = selectAction.action.ReadValue<float>();

                    for (int i = 0; i < JointCount; i++)
                    {
                        currentGesture[i].Position = Vector3.Lerp(baseData[i].Position, pinchData[i].Position, selectAmount) + poseOffset;
                        currentGesture[i].Rotation = Quaternion.Slerp(baseData[i].Rotation, pinchData[i].Rotation, selectAmount);
                        currentGesture[i].Radius = Mathf.Lerp(baseData[i].Radius, pinchData[i].Radius, selectAmount);
                        currentGesture[i].Position = Vector3.Scale(currentGesture[i].Position, new Vector3(HandNode == XRNode.LeftHand ? -1 : 1, 1, 1));
                    }
                }
            }
        }

        [Preserve]
        class SynthesisProvider : Provider
        {
            protected SyntheticHandsConfig config;

            private Dictionary<XRNode, SyntheticHandContainer> hands;

            public SynthesisProvider() : base()
            {
                config = XRSubsystemHelpers.GetConfiguration<SyntheticHandsConfig, SyntheticHandsSubsystem>();

                hands = new Dictionary<XRNode, SyntheticHandContainer>
                {
                    { XRNode.LeftHand, new SyntheticHandContainer(
                                                                XRNode.LeftHand,
                                                                GestureId.Flat,
                                                                config.LeftHandPosition,
                                                                config.LeftHandRotation,
                                                                config.LeftHandSelect,
                                                                config.PoseOffset) },
                    { XRNode.RightHand, new SyntheticHandContainer(
                                                                XRNode.RightHand,
                                                                GestureId.Flat,
                                                                config.RightHandPosition,
                                                                config.RightHandRotation,
                                                                config.RightHandSelect,
                                                                config.PoseOffset) }
                };
            }

            public override void Update()
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
            public GestureId GetNeutralPose(XRNode handNode)
            {
                return hands[handNode].NeutralPose;
            }

            /// <summary>
            /// Sets the neutral pose for the specified hand.
            /// </summary>
            /// <param name="handNode">The hand for which the neutral pose is being set.</param>
            /// <param name="poseId">The desired hand pose.</param>
            public void SetNeutralPose(XRNode handNode, GestureId poseId)
            {
                hands[handNode].NeutralPose = poseId;
            }

            #region IHandsSubsystem implementation

            ///<inheritdoc/>
            public override bool TryGetEntireHand(XRNode handNode, out IReadOnlyList<HandJointPose> jointPoses)
            {
                Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, "Non-hand XRNode used in GetHand query");

                if (!config.ShouldSynthesize())
                {
                    jointPoses = System.Array.Empty<HandJointPose>();
                    return false;
                }

                return hands[handNode].TryGetEntireHand(out jointPoses);
            }

            ///<inheritdoc/>
            public override bool TryGetJoint(TrackedHandJoint joint, XRNode handNode, out HandJointPose jointPose)
            {
                Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, "Non-hand XRNode used in GetHand query");

                if (!config.ShouldSynthesize())
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
