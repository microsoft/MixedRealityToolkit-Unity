// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Base class for obtaining and holding hand data.
    /// </summary>
    public abstract class HandDataContainer
    {
        /// <summary>
        /// The XRNode for the hand data.
        /// </summary>
        public XRNode HandNode { get; protected set; }

        /// <summary>
        /// Will be <see langword="true"/> if all the hand joint poses been queried.
        /// </summary>
        public bool AlreadyFullQueried { get; protected set; }

        /// <summary>
        /// Will be <see langword="true"/> if the the hand joint query as successful.
        /// </summary>
        public bool FullQueryValid { get; protected set; }

        /// <summary>
        /// The current <see cref="HandJointPose"/> for the trackable hand joints.
        /// </summary>
        protected HandJointPose[] HandJoints { get; set; } = new HandJointPose[(int)TrackedHandJoint.TotalJoints];

        /// <summary>
        /// Initializes a new instance of a <see cref="HandDataContainer"/> class.
        /// </summary>
        /// <param name="handNode">The XRNode the hand data pertains to.</param>
        public HandDataContainer(XRNode handNode)
        {
            HandNode = handNode;
        }

        /// <summary>
        /// Reset the hand data query status
        /// </summary>
        public void Reset()
        {
            AlreadyFullQueried = false;
            FullQueryValid = false;
        }


        /// <summary>   
        /// Implemented in derived classes.  This method gets all of the joint poses for the hand.
        /// </summary>
        /// <param name="joints"> The returned list of HandJointPoses</param>
        /// <returns><see langword="true"/> if the query was successful, otherwise <see langword="false"/>.</returns>
        public abstract bool TryGetEntireHand(out IReadOnlyList<HandJointPose> joints);

        /// <summary>   
        /// Implemented in derived classes.  This method gets the specified joint pose.
        /// </summary>
        /// <param name="joint">The TrackedHandJoint to retrieve the post for.</param>
        /// <param name="pose">The returned HandJointPose.</param>
        /// <returns><see langword="true"/> if the query was successful, otherwise <see langword="false"/>.</returns>
        public abstract bool TryGetJoint(TrackedHandJoint joint, out HandJointPose pose);
    }
}
