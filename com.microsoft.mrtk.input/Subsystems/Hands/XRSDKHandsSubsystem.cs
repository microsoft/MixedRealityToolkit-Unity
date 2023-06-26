// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;
using UnityEngine.XR;
using CommonUsages = UnityEngine.XR.CommonUsages;
using InputDevice = UnityEngine.XR.InputDevice;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A Unity subsystem that extends <see cref="Microsoft.MixedReality.Toolkit.Subsystems.HandsSubsystem">HandsSubsystem</see>, and 
    /// obtains hand joint poses from the Unity Engine's XR <see href="https://docs.unity3d.com/ScriptReference/XR.Hand.html">Hand</see> class.
    /// </summary>
    [Preserve]
    [MRTKSubsystem(
        Name = "com.microsoft.mixedreality.xrsdkhands",
        DisplayName = "Subsystem for XRSDK Hands API",
        Author = "Microsoft",
        ProviderType = typeof(XRSDKProvider),
        SubsystemTypeOverride = typeof(XRSDKHandsSubsystem),
        ConfigType = typeof(BaseSubsystemConfig))]
    public class XRSDKHandsSubsystem : HandsSubsystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Register()
        {
            // Fetch subsystem metadata from the attribute.
            var cinfo = XRSubsystemHelpers.ConstructCinfo<XRSDKHandsSubsystem, HandsSubsystemCinfo>();

            // Populate remaining cinfo field.
            cinfo.IsPhysicalData = true;

            if (!Register(cinfo))
            {
                Debug.LogError($"Failed to register the {cinfo.Name} subsystem.");
            }
        }

        /// <summary>
        /// A class that extends <see cref="Microsoft.MixedReality.Toolkit.Input.HandDataContainer">HandDataContainer</see>, and 
        /// obtains hand joint poses from the Unity Engine's XR <see href="https://docs.unity3d.com/ScriptReference/XR.Hand.html">Hand</see> class.
        /// </summary>
        private class XRSDKHandContainer : HandDataContainer
        {
            // The cached reference to the XRSDK tracked hand.
            // Is re-queried/TryGetFeatureValue'd each frame,
            // as the presence (or absence) of this reference
            // indicates tracking state.
            private Hand? handDevice;

            public XRSDKHandContainer(XRNode handNode) : base(handNode)
            {
                handDevice = GetTrackedHand();
            }

            private static readonly ProfilerMarker TryGetEntireHandPerfMarker =
                new ProfilerMarker("[MRTK] XRSDKHandsSubsystem.TryGetEntireHand");

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
                new ProfilerMarker("[MRTK] XRSDKHandsSubsystem.TryGetJoint");

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
                        handDevice = GetTrackedHand();

                        // If the tracked hand is null, we obviously have no data,
                        // and return immediately.
                        if (!handDevice.HasValue)
                        {
                            pose = handJoints[HandsUtils.ConvertToIndex(joint)];
                            return false;
                        }

                        // Joints are relative to the camera floor offset object.
                        Transform origin = PlayspaceUtilities.XROrigin.CameraFloorOffsetObject.transform;
                        if (origin == null)
                        {
                            pose = handJoints[HandsUtils.ConvertToIndex(joint)];
                            return false;
                        }

                        // Otherwise, we need to deal with palm/root vs finger separately
                        if (joint == TrackedHandJoint.Palm)
                        {
                            handDevice.Value.TryGetRootBone(out Bone rootBone);
                            thisQueryValid |= TryUpdateJoint(TrackedHandJoint.Palm, rootBone, origin);
                        }
                        else
                        {
                            HandFinger finger = HandsUtils.GetFingerFromJoint(joint);
                            if (handDevice.Value.TryGetFingerBones(finger, fingerBones))
                            {
                                Bone bone = fingerBones[HandsUtils.GetOffsetFromBase(joint)];
                                thisQueryValid |= TryUpdateJoint(joint, bone, origin);
                            }
                        }
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

            // Scratchpad for reading out devices, to reduce allocs.
            private readonly List<InputDevice> handDevices = new List<InputDevice>(2);

            private static readonly ProfilerMarker GetTrackedHandPerfMarker =
                new ProfilerMarker("[MRTK] XRSDKHandsSubsystem.GetTrackedHand");

            /// <summary>
            /// Obtains a reference to the actual Hand object representing the tracked hand
            /// functionality present on handNode. Returns null if no Hand reference available.
            /// </summary>
            private Hand? GetTrackedHand()
            {
                using (GetTrackedHandPerfMarker.Auto())
                {
                    InputDevices.GetDevicesWithCharacteristics(HandNode == XRNode.LeftHand ? HandsUtils.LeftHandCharacteristics : HandsUtils.RightHandCharacteristics, handDevices);

                    if (handDevices.Count == 0)
                    {
                        // No hand devices detected at this hand.
                        return null;
                    }
                    else
                    {
                        foreach (InputDevice device in handDevices)
                        {
                            if (device.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked)
                                && isTracked
                                && device.TryGetFeatureValue(CommonUsages.handData, out Hand handRef))
                            {
                                // We've found our device that supports CommonUsages.handData, and
                                // the specific Hand object that we can return.
                                return handRef;
                            }
                        }

                        // None of the devices on this hand are tracked and/or support CommonUsages.handData.
                        // This will happen when the platform doesn't support hand tracking,
                        // or the hand is not visible enough to return a tracking solution.
                        return null;
                    }
                }
            }

            // Scratchpad for reading out finger bones, to reduce allocs.
            private readonly List<Bone> fingerBones = new List<Bone>();

            private static readonly ProfilerMarker TryCalculateEntireHandPerfMarker =
                new ProfilerMarker("[MRTK] XRSDKHandsSubsystem.TryCalculateEntireHand");

            /// <summary>
            /// For a certain hand, query every Bone in the hand, and write all results to the
            /// handJoints collection. This will also mark handsQueriedThisFrame[handNode] = true.
            /// </summary>
            private void TryCalculateEntireHand()
            {
                using (TryCalculateEntireHandPerfMarker.Auto())
                {
                    handDevice = GetTrackedHand();

                    if (!handDevice.HasValue)
                    {
                        // No articulated hand device available this frame.
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

                    FullQueryValid = true;
                    foreach (HandFinger finger in HandsUtils.HandFingers)
                    {
                        if (handDevice.Value.TryGetFingerBones(finger, fingerBones))
                        {
                            for (int i = 0; i < fingerBones.Count; i++)
                            {
                                FullQueryValid &= TryUpdateJoint(HandsUtils.ConvertToTrackedHandJoint(finger, i), fingerBones[i], origin);
                            }
                        }
                    }

                    // Write root bone into handJoints as palm joint.
                    handDevice.Value.TryGetRootBone(out Bone rootBone);
                    FullQueryValid &= TryUpdateJoint(TrackedHandJoint.Palm, rootBone, origin);

                    // Mark this hand as having been fully queried this frame.
                    // If any joint is queried again this frame, we'll reuse the
                    // information to avoid extra work.
                    AlreadyFullQueried = true;
                }
            }

            private static readonly ProfilerMarker TryUpdateJointPerfMarker =
                new ProfilerMarker("[MRTK] XRSDKHandsSubsystem.TryUpdateJoint");

            /// <summary>
            /// Given a destination jointID, apply the Bone info to the correct struct
            /// in the handJoints collection.
            /// </summary>
            private bool TryUpdateJoint(TrackedHandJoint jointID, Bone bone, Transform playspaceTransform)
            {
                using (TryUpdateJointPerfMarker.Auto())
                {
                    bool gotData = true;
                    gotData &= bone.TryGetPosition(out Vector3 position);
                    gotData &= bone.TryGetRotation(out Quaternion rotation);

                    if (!gotData)
                    {
                        return false;
                    }

                    // XRSDK does not return joint radius. 0.5cm default.
                    handJoints[HandsUtils.ConvertToIndex(jointID)] = new HandJointPose(
                        playspaceTransform.TransformPoint(position),
                        playspaceTransform.rotation * rotation,
                        0.005f);

                    return true;
                }
            }
        }

        [Preserve]
        private class XRSDKProvider : Provider
        {
            private Dictionary<XRNode, XRSDKHandContainer> hands = null;

            public override void Start()
            {
                base.Start();

                hands ??= new Dictionary<XRNode, XRSDKHandContainer>
                {
                    { XRNode.LeftHand, new XRSDKHandContainer(XRNode.LeftHand) },
                    { XRNode.RightHand, new XRSDKHandContainer(XRNode.RightHand) }
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
    }
}
