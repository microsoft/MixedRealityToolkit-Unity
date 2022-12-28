// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// The pose of an individual hand joint. Superset
    /// of a <see cref="UnityEngine.Pose"/>, adding a
    /// radius value.
    /// </summary>
    public struct HandJointPose : IEqualityComparer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="position">The position of the hand joint.</param>
        /// <param name="rotation">The rotation of the hand joint.</param>
        /// <param name="radius">The radius of the hand joint.</param>
        public HandJointPose(
            Vector3 position,
            Quaternion rotation,
            float radius)
        {
            this.pose = new Pose(position, rotation);
            this.radius = radius;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pose">The pose of the hand joint.</param>
        /// <param name="radius">The radius of the hand joint.</param>
        public HandJointPose(
            Pose pose,
            float radius)
        {
            this.pose = pose;
            this.radius = radius;
        }

        [SerializeField]
        [Tooltip("The pose of the hand joint.")]
        private Pose pose;

        /// <summary>
        /// The pose of the hand joint.
        /// </summary>
        public Pose Pose
        {
            get => pose;
            set => pose = value;
        }

        /// <summary>
        /// The position of the hand joint.
        /// </summary>
        public Vector3 Position
        {
            get => pose.position;
            set => pose.position = value;
        }

        /// <summary>
        /// The rotation of the hand joint.
        /// </summary>
        public Quaternion Rotation
        {
            get => pose.rotation;
            set => pose.rotation = value;
        }

        [SerializeField]
        [Tooltip("The radius of the hand joint.")]
        private float radius;

        /// <summary>
        /// The radius of the object.
        /// </summary>
        public float Radius { get => radius; set => radius = value; }

        /// <summary>
        /// The Z axis of the pose in world space.
        /// </summary>
        public Vector3 Forward => pose.forward;

        /// <summary>
        /// The Y axis of the pose in world space.
        /// </summary>
        public Vector3 Up => pose.up;

        /// <summary>
        /// The X axis of the pose in world space.
        /// </summary>
        public Vector3 Right => pose.right;

        public override string ToString()
        {
            return $"{pose} | {radius}";
        }

        #region IEqualityComparer Implementation

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object left, object right)
        {
            if (ReferenceEquals(null, left) || ReferenceEquals(null, right)) { return false; }
            if (!(left is HandJointPose) || !(right is HandJointPose)) { return false; }
            return ((HandJointPose)left).Equals((HandJointPose)right);
        }

        public bool Equals(HandJointPose other)
        {
            return Position == other.Position &&
                   Rotation.Equals(other.Rotation) &&
                   Radius.Equals(other.Radius);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            return obj is HandJointPose pose && Equals(pose);
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj is HandJointPose pose ? pose.GetHashCode() : 0;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(HandJointPose left, HandJointPose right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HandJointPose left, HandJointPose right)
        {
            return !(left == right);
        }

        #endregion IEqualityComparer Implementation

        #region Conversions

        public static implicit operator Pose(HandJointPose pose) => pose.pose;
        
        #endregion
    }
}
