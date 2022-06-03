// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;

#if MROPENXR_PRESENT
using Microsoft.MixedReality.OpenXR;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR;
#endif // MROPENXR_PRESENT

namespace Microsoft.MixedReality.Toolkit.Input
{
#if MROPENXR_PRESENT
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
#if MROPENXR_PRESENT
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

                        Transform playspaceTransform = PlayspaceUtilities.ReferenceTransform;
                        if (playspaceTransform == null)
                        {
                            pose = handJoints[index];
                            return false;
                        }

                        thisQueryValid |= TryUpdateJoint(index, HandJointLocations[HandJointIndexFromTrackedHandJointIndex[index]], playspaceTransform);
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

            /// <summary/>
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
                    Transform playspaceTransform = PlayspaceUtilities.ReferenceTransform;
                    if (playspaceTransform == null)
                    {
                        return;
                    }

                    FullQueryValid = true;
                    for (int i = 0; i < HandTracker.JointCount; i++)
                    {
                        FullQueryValid &= TryUpdateJoint(TrackedHandJointIndexFromHandJointIndex[i], HandJointLocations[i], playspaceTransform);
                    }

                    // Mark this hand as having been fully queried this frame.
                    // If any joint is queried again this frame, we'll reuse the
                    // information to avoid extra work.
                    AlreadyFullQueried = true;
                }
            }

            private static readonly ProfilerMarker TryUpdateJointPerfMarker =
                new ProfilerMarker("[MRTK] OpenXRHandsSubsystem.TryUpdateJoint");

            /// <summary/>
            /// Given a destination jointID, apply the Bone info to the correct struct
            /// in the handJoints collection.
            /// </summary>
            private bool TryUpdateJoint(int jointIndex, in HandJointLocation handJointLocation, Transform playspaceTransform)
            {
                using (TryUpdateJointPerfMarker.Auto())
                {
                    handJoints[jointIndex] = new HandJointPose(
                        playspaceTransform.TransformPoint(handJointLocation.Pose.position),
                        playspaceTransform.rotation * handJointLocation.Pose.rotation,
                        handJointLocation.Radius);

                    return true;
                }
            }

            private static readonly int[] TrackedHandJointIndexFromHandJointIndex = new int[]
            {
                HandsUtils.ConvertToIndex(TrackedHandJoint.Palm),
                HandsUtils.ConvertToIndex(TrackedHandJoint.Wrist),

                HandsUtils.ConvertToIndex(TrackedHandJoint.ThumbMetacarpalJoint),
                HandsUtils.ConvertToIndex(TrackedHandJoint.ThumbProximalJoint),
                HandsUtils.ConvertToIndex(TrackedHandJoint.ThumbDistalJoint),
                HandsUtils.ConvertToIndex(TrackedHandJoint.ThumbTip),

                HandsUtils.ConvertToIndex(TrackedHandJoint.IndexMetacarpal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.IndexKnuckle),
                HandsUtils.ConvertToIndex(TrackedHandJoint.IndexMiddleJoint),
                HandsUtils.ConvertToIndex(TrackedHandJoint.IndexDistalJoint),
                HandsUtils.ConvertToIndex(TrackedHandJoint.IndexTip),

                HandsUtils.ConvertToIndex(TrackedHandJoint.MiddleMetacarpal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.MiddleKnuckle),
                HandsUtils.ConvertToIndex(TrackedHandJoint.MiddleMiddleJoint),
                HandsUtils.ConvertToIndex(TrackedHandJoint.MiddleDistalJoint),
                HandsUtils.ConvertToIndex(TrackedHandJoint.MiddleTip),

                HandsUtils.ConvertToIndex(TrackedHandJoint.RingMetacarpal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.RingKnuckle),
                HandsUtils.ConvertToIndex(TrackedHandJoint.RingMiddleJoint),
                HandsUtils.ConvertToIndex(TrackedHandJoint.RingDistalJoint),
                HandsUtils.ConvertToIndex(TrackedHandJoint.RingTip),

                HandsUtils.ConvertToIndex(TrackedHandJoint.PinkyMetacarpal),
                HandsUtils.ConvertToIndex(TrackedHandJoint.PinkyKnuckle),
                HandsUtils.ConvertToIndex(TrackedHandJoint.PinkyMiddleJoint),
                HandsUtils.ConvertToIndex(TrackedHandJoint.PinkyDistalJoint),
                HandsUtils.ConvertToIndex(TrackedHandJoint.PinkyTip),
            };

            private static readonly int[] HandJointIndexFromTrackedHandJointIndex = new int[]
            {
                (int)HandJoint.Wrist,
                (int)HandJoint.Palm,

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

        [Preserve]
        private class OpenXRProvider : Provider, IHandsSubsystem
        {
            private Dictionary<XRNode, OpenXRHandContainer> hands = new Dictionary<XRNode, OpenXRHandContainer>
            {
                { XRNode.LeftHand, new OpenXRHandContainer(XRNode.LeftHand) },
                { XRNode.RightHand, new OpenXRHandContainer(XRNode.RightHand) }
            };

            public override void Update()
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
