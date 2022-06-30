// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine.Scripting;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// A subsystem that exposes information about the user's hands.
    /// </summary>
    [Preserve]
    public class HandsSubsystem :
        MRTKSubsystem<HandsSubsystem, HandsSubsystemDescriptor, HandsSubsystem.Provider>,
        IHandsSubsystem
    {
        /// <summary>
        /// Construct the <c>HandsSubsystem</c>.
        /// </summary>
        public HandsSubsystem()
        { }

        /// <summary>
        /// Interface for providing hand functionality for the implementation.
        /// </summary>
        public abstract class Provider : MRTKSubsystemProvider<HandsSubsystem>, IHandsSubsystem
        {
            #region IHandsSubsystem implementation

            ///<inheritdoc/>
            public abstract bool TryGetEntireHand(XRNode hand, out IReadOnlyList<HandJointPose> jointPoses);

            ///<inheritdoc/>
            public abstract bool TryGetJoint(TrackedHandJoint joint, XRNode hand, out HandJointPose jointPose);

            #endregion IHandsSubsystem implementation
        }

        #region IHandsSubsystem implementation

        ///<inheritdoc/>
        public bool TryGetEntireHand(XRNode hand, out IReadOnlyList<HandJointPose> jointPoses)
            => provider.TryGetEntireHand(hand, out jointPoses);

        ///<inheritdoc/>
        public bool TryGetJoint(TrackedHandJoint joint, XRNode hand, out HandJointPose jointPose)
            => provider.TryGetJoint(joint, hand, out jointPose);

        #endregion IHandsSubsystem implementation

        /// <summary>
        /// Registers a hands subsystem implementation based on the given subsystem parameters.
        /// </summary>
        /// <param name="handsSubsystemParams">The parameters defining the hands subsystem functionality implemented
        /// by the subsystem provider.</param>
        /// <returns>
        /// <c>true</c> if the subsystem implementation is registered. Otherwise, <c>false</c>.
        /// </returns>
        public static bool Register(HandsSubsystemCinfo handsSubsystemParams)
        {
            var descriptor = HandsSubsystemDescriptor.Create(handsSubsystemParams);
            SubsystemDescriptorStore.RegisterDescriptor(descriptor);
            return true;
        }
    }
}
