// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine.InputSystem;

#if MROPENXR_PRESENT && (UNITY_EDITOR_WIN || UNITY_WSA || UNITY_STANDALONE_WIN || UNITY_ANDROID)
using Microsoft.MixedReality.OpenXR;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR;
#endif // MROPENXR_PRESENT

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A Unity subsystem that extends <see cref="Microsoft.MixedReality.Toolkit.Subsystems.HandsSubsystem">HandsSubsystem</see>, and 
    /// obtains hand joint poses from the Microsoft.MixedReality.OpenXR.HandTracker class.
    /// </summary>
#if MROPENXR_PRESENT && (UNITY_EDITOR_WIN || UNITY_WSA || UNITY_STANDALONE_WIN || UNITY_ANDROID)
    [Preserve]
    [MRTKSubsystem(
        Name = "com.microsoft.mixedreality.openxrhands",
        DisplayName = "Subsystem for OpenXR Hands API",
        Author = "Microsoft",
        ProviderType = typeof(OpenXRProvider),
        SubsystemTypeOverride = typeof(OpenXRHandsSubsystem),
        ConfigType = typeof(BaseSubsystemConfig))]
#endif // MROPENXR_PRESENT
    public class OpenXRHandsSubsystem : HandsSubsystem
    {
#if MROPENXR_PRESENT && (UNITY_EDITOR_WIN || UNITY_WSA || UNITY_STANDALONE_WIN || UNITY_ANDROID)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Register()
        {
            // Fetch subsystem metadata from the attribute.
            var cinfo = XRSubsystemHelpers.ConstructCinfo<OpenXRHandsSubsystem, HandsSubsystemCinfo>();

            // Populate remaining cinfo field.
            cinfo.IsPhysicalData = true;

            if (!Register(cinfo))
            {
                Debug.LogError($"Failed to register the {cinfo.Name} subsystem.");
            }
        }

        /// <summary>
        /// A class that extends <see cref="Microsoft.MixedReality.Toolkit.Input.HandDataContainer">HandDataContainer</see>, and 
        /// obtains hand joint poses from the Microsoft.MixedReality.OpenXR.HandTracker class.
        /// </summary>
        private class OpenXRHandContainer : HandDataContainer
        {
            public OpenXRHandContainer(XRNode handNode) : base(handNode)
            {
                handTracker = handNode == XRNode.LeftHand ? HandTracker.Left : HandTracker.Right;
            }

            private readonly HandTracker handTracker;

            // Scratchpad for reading out hand data, to reduce allocs.
            private static readonly HandJointLocation[] HandJointLocations = new HandJointLocation[HandTracker.JointCount];

            private static readonly ProfilerMarker TryGetEntireHandPerfMarker =
                new ProfilerMarker("[MRTK] OpenXRHandsSubsystem.TryGetEntireHand");

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
                new ProfilerMarker("[MRTK] OpenXRHandsSubsystem.TryGetJoint");

            /// <inheritdoc/>
            public override bool TryGetJoint(TrackedHandJoint joint, out HandJointPose pose)
            {
                using (TryGetJointPerfMarker.Auto())
                {
                    bool thisQueryValid = false;
                    int index = HandsUtils.ConvertToIndex(joint);

                    // If we happened to have already queried the entire
                    // hand data this frame, we don't need to re-query for
                    // just the joint. If we haven't, we do still need to
                    // query for the single joint.
                    if (!AlreadyFullQueried)
                    {
                        if (!handTracker.TryLocateHandJoints(FrameTime.OnUpdate, HandJointLocations))
                        {
                            pose = handJoints[index];
                            return false;
                        }

                        // Joints are relative to the camera floor offset object.
                        Transform origin = PlayspaceUtilities.XROrigin.CameraFloorOffsetObject.transform;
                        if (origin == null)
                        {
                            pose = handJoints[index];
                            return false;
                        }

                        var jointLocation = HandJointLocations[HandJointIndexFromTrackedHandJointIndex[index]];
                        UpdateJoint(index, jointLocation, origin);
                        thisQueryValid = true;
                    }
                    else
                    {
                        // If we've already run a full-hand query, this single joint query
                        // is just as valid as the full query.
                        thisQueryValid = FullQueryValid;
                    }

                    pose = handJoints[index];
                    return thisQueryValid;
                }
            }

            private static readonly ProfilerMarker TryCalculateEntireHandPerfMarker =
                new ProfilerMarker("[MRTK] OpenXRHandsSubsystem.TryCalculateEntireHand");

            /// <summary>
            /// For a certain hand, query every Bone in the hand, and write all results to the
            /// handJoints collection. This will also mark handsQueriedThisFrame[handNode] = true.
            /// </summary>
            private void TryCalculateEntireHand()
            {
                using (TryCalculateEntireHandPerfMarker.Auto())
                {
                    if (!handTracker.TryLocateHandJoints(FrameTime.OnUpdate, HandJointLocations))
                    {
                        // No articulated hand data available this frame.
                        FullQueryValid = false;
                        AlreadyFullQueried = true;
                        return;
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

                    for (int i = 0; i < HandTracker.JointCount; i++)
                    {
                        UpdateJoint(TrackedHandJointIndexFromHandJointIndex[i], HandJointLocations[i], origin);
                    }

                    // Mark this hand as having been fully queried this frame.
                    // If any joint is queried again this frame, we'll reuse the
                    // information to avoid extra work.
                    FullQueryValid = true;
                    AlreadyFullQueried = true;
                }
            }

            private static readonly ProfilerMarker UpdateJointPerfMarker =
                new ProfilerMarker("[MRTK] OpenXRHandsSubsystem.UpdateJoint");

            /// <summary>
            /// Given a destination jointID, apply the Bone info to the correct struct
            /// in the handJoints collection.
            /// </summary>
            private void UpdateJoint(int jointIndex, in HandJointLocation handJointLocation, Transform playspaceTransform)
            {
                using (UpdateJointPerfMarker.Auto())
                {
                    handJoints[jointIndex] = new HandJointPose(
                        playspaceTransform.TransformPoint(handJointLocation.Pose.position),
                        playspaceTransform.rotation * handJointLocation.Pose.rotation,
                        handJointLocation.Radius);
                }
            }

            private static readonly int[] TrackedHandJointIndexFromHandJointIndex = new int[]
            {
                HandsUtils.ConvertToIndex(TrackedHandJoint.Palm),
                HandsUtils.ConvertToIndex(TrackedHandJoint.Wrist),

                HandsUtils.ConvertToIndex(TrackedHandJoint.ThumbMetacarpal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.ThumbProximal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.ThumbDistal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.ThumbTip),

                HandsUtils.ConvertToIndex(TrackedHandJoint.IndexMetacarpal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.IndexProximal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.IndexIntermediate),
                HandsUtils.ConvertToIndex(TrackedHandJoint.IndexDistal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.IndexTip),

                HandsUtils.ConvertToIndex(TrackedHandJoint.MiddleMetacarpal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.MiddleProximal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.MiddleIntermediate),
                HandsUtils.ConvertToIndex(TrackedHandJoint.MiddleDistal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.MiddleTip),

                HandsUtils.ConvertToIndex(TrackedHandJoint.RingMetacarpal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.RingProximal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.RingIntermediate),
                HandsUtils.ConvertToIndex(TrackedHandJoint.RingDistal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.RingTip),

                HandsUtils.ConvertToIndex(TrackedHandJoint.LittleMetacarpal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.LittleProximal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.LittleIntermediate),
                HandsUtils.ConvertToIndex(TrackedHandJoint.LittleDistal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.LittleTip),
            };

            private static readonly int[] HandJointIndexFromTrackedHandJointIndex = new int[]
            {
                (int)HandJoint.Palm,
                (int)HandJoint.Wrist,

                (int)HandJoint.ThumbMetacarpal,
                (int)HandJoint.ThumbProximal,
                (int)HandJoint.ThumbDistal,
                (int)HandJoint.ThumbTip,

                (int)HandJoint.IndexMetacarpal,
                (int)HandJoint.IndexProximal,
                (int)HandJoint.IndexIntermediate,
                (int)HandJoint.IndexDistal,
                (int)HandJoint.IndexTip,

                (int)HandJoint.MiddleMetacarpal,
                (int)HandJoint.MiddleProximal,
                (int)HandJoint.MiddleIntermediate,
                (int)HandJoint.MiddleDistal,
                (int)HandJoint.MiddleTip,

                (int)HandJoint.RingMetacarpal,
                (int)HandJoint.RingProximal,
                (int)HandJoint.RingIntermediate,
                (int)HandJoint.RingDistal,
                (int)HandJoint.RingTip,

                (int)HandJoint.LittleMetacarpal,
                (int)HandJoint.LittleProximal,
                (int)HandJoint.LittleIntermediate,
                (int)HandJoint.LittleDistal,
                (int)HandJoint.LittleTip,
            };
        }

        /// <summary>
        /// A hand subsystem that extends the <see cref="Microsoft.MixedReality.Toolkit.Subsystems.HandSubsystem.Provider">Provider</see> class, and 
        /// obtains hand joint poses from the <see cref="OpenXRHandContainer"/> class.
        /// </summary>
        [Preserve]
        private class OpenXRProvider : Provider, IHandsSubsystem
        {
            private Dictionary<XRNode, OpenXRHandContainer> hands = null;

            public override void Start()
            {
                base.Start();

                hands ??= new Dictionary<XRNode, OpenXRHandContainer>
                {
                    { XRNode.LeftHand, new OpenXRHandContainer(XRNode.LeftHand) },
                    { XRNode.RightHand, new OpenXRHandContainer(XRNode.RightHand) }
                };

                InputSystem.onBeforeUpdate += ResetHands;
            }

            public override void Stop()
            {
                ResetHands();
                InputSystem.onBeforeUpdate -= ResetHands;
                base.Stop();
            }

            private void ResetHands()
            {
                hands[XRNode.LeftHand].Reset();
                hands[XRNode.RightHand].Reset();
            }

            #region IHandsSubsystem implementation

            /// <inheritdoc/>
            public override bool TryGetEntireHand(XRNode handNode, out IReadOnlyList<HandJointPose> jointPoses)
            {
                Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, "Non-hand XRNode used in TryGetEntireHand query");

                return hands[handNode].TryGetEntireHand(out jointPoses);
            }

            /// <inheritdoc/>
            public override bool TryGetJoint(TrackedHandJoint joint, XRNode handNode, out HandJointPose jointPose)
            {
                Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, "Non-hand XRNode used in TryGetJoint query");

                return hands[handNode].TryGetJoint(joint, out jointPose);
            }

            #endregion IHandsSubsystem implementation
        }
#endif // MROPENXR_PRESENT
    }
}
