// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public abstract class HandDataContainer
    {
        public XRNode HandNode { get; protected set; }

        public bool AlreadyFullQueried { get; protected set; }

        public bool FullQueryValid { get; protected set; }

        protected HandJointPose[] handJoints = new HandJointPose[(int)TrackedHandJoint.TotalJoints];

        public HandDataContainer(XRNode handNode)
        {
            HandNode = handNode;
        }

        public void Reset()
        {
            AlreadyFullQueried = false;
            FullQueryValid = false;
        }

        public abstract bool TryGetEntireHand(out IReadOnlyList<HandJointPose> joints);

        public abstract bool TryGetJoint(TrackedHandJoint joint, out HandJointPose pose);
    }
}