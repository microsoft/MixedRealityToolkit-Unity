// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Snapshot of simulated hand data.
    /// </summary>
    [System.Serializable]
    public class SimulatedHandData
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        // Timestamp of hand data, as FileTime, e.g. DateTime.UtcNow.ToFileTime()
        private long timestamp = 0;
        public long Timestamp => timestamp;

        [SerializeField]
        private bool isTracked = false;
        public bool IsTracked => isTracked;
        [SerializeField]
        private MixedRealityPose[] joints = new MixedRealityPose[jointCount];
        public MixedRealityPose[] Joints => joints;
        [SerializeField]
        private bool isPinching = false;
        public bool IsPinching => isPinching;

        public delegate void HandJointDataGenerator(MixedRealityPose[] jointPoses);

        private IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        private IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                }
                return inputSystem;
            }
        }

        public void Copy(SimulatedHandData other)
        {
            timestamp = other.timestamp;
            isTracked = other.isTracked;
            isPinching = other.isPinching; 
            for (int i = 0; i < jointCount; ++i)
            {
                joints[i] = other.joints[i];
            }
        }

        /// <summary>
        /// Replace the hand data with the given values.
        /// </summary>
        /// <returns>True if the hand data has been changed.</returns>
        /// <param name="isTrackedNew">True if the hand is currently tracked.</param>
        /// <param name="isPinchingNew">True if the hand is in a pinching pose that causes a "Select" action.</param>
        /// <param name="generator">Generator function that produces joint positions and rotations. The joint data generator is only used when the hand is tracked.</param>
        /// <remarks>The timestamp of the hand data will be the current time, see [DateTime.UtcNow](https://docs.microsoft.com/en-us/dotnet/api/system.datetime.utcnow?view=netframework-4.8).</remarks>
        public bool Update(bool isTrackedNew, bool isPinchingNew, HandJointDataGenerator generator)
        {
            // TODO: DateTime.UtcNow can be quite imprecise, better use Stopwatch.GetTimestamp
            // https://stackoverflow.com/questions/2143140/c-sharp-datetime-now-precision
            return UpdateWithTimestamp(DateTime.UtcNow.Ticks, isTrackedNew, isPinchingNew, generator);
        }

        /// <summary>
        /// Replace the hand data with the given values.
        /// </summary>
        /// <returns>True if the hand data has been changed.</returns>
        /// <param name="timestampNew">The timestamp for the new hand data. No change will occur if the time stamp is the same as the current one.</param>
        /// <param name="isTrackedNew">True if the hand is currently tracked.</param>
        /// <param name="isPinchingNew">True if the hand is in a pinching pose that causes a "Select" action.</param>
        /// <param name="generator">Generator function that produces joint positions and rotations. The joint data generator is only used when the hand is tracked.</param>
        public bool UpdateWithTimestamp(long timestampNew, bool isTrackedNew, bool isPinchingNew, HandJointDataGenerator generator)
        {
            bool handDataChanged = false;

            if (isTracked != isTrackedNew || isPinching != isPinchingNew)
            {
                isTracked = isTrackedNew;
                isPinching = isPinchingNew;
                handDataChanged = true;
            }

            if (timestamp != timestampNew)
            {
                timestamp = timestampNew;
                if (isTracked)
                {
                    generator(Joints);
                    handDataChanged = true;
                }
            }

            return handDataChanged;
        }
    }

    public abstract class SimulatedHand : BaseHand
    {
        public abstract HandSimulationMode SimulationMode { get; }

        protected static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        protected readonly Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        protected SimulatedHand(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {}

        public override bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose)
        {
            return jointPoses.TryGetValue(joint, out pose);
        }

        public void UpdateState(SimulatedHandData handData)
        {
            for (int i = 0; i < jointCount; i++)
            {
                TrackedHandJoint handJoint = (TrackedHandJoint)i;

                if (!jointPoses.ContainsKey(handJoint))
                {
                    jointPoses.Add(handJoint, handData.Joints[i]);
                }
                else
                {
                    jointPoses[handJoint] = handData.Joints[i];
                }
            }

            InputSystem?.RaiseHandJointsUpdated(InputSource, ControllerHandedness, jointPoses);

            UpdateVelocity();

            UpdateInteractions(handData);
        }

        protected abstract void UpdateInteractions(SimulatedHandData handData);
    }
}