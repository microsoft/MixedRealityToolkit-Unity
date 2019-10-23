// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    [Serializable]
    public struct MixedRealityTransform : IEqualityComparer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MixedRealityTransform(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.pose = new MixedRealityPose(position, rotation);
            this.scale = scale;
        }

        /// <summary>
        /// Create a transform with only given position
        /// </summary>
        public static MixedRealityTransform NewTranslate(Vector3 position)
        {
            return new MixedRealityTransform(position, Quaternion.identity, Vector3.one);
        }

        /// <summary>
        /// Create a transform with only given rotation
        /// </summary>
        public static MixedRealityTransform NewRotate(Quaternion rotation)
        {
            return new MixedRealityTransform(Vector3.zero, rotation, Vector3.one);
        }

        /// <summary>
        /// Create a transform with only given scale
        /// </summary>
        public static MixedRealityTransform NewScale(Vector3 scale)
        {
            return new MixedRealityTransform(Vector3.zero, Quaternion.identity, scale);
        }

        /// <summary>
        /// The default value for a Six Dof Transform.
        /// </summary>
        public static MixedRealityTransform Identity { get; } = new MixedRealityTransform(Vector3.zero, Quaternion.identity, Vector3.one);

        [SerializeField]
        [Tooltip("The pose (position and rotation) of the transform")]
        private MixedRealityPose pose;

        /// <summary>
        /// The position of the transform.
        /// </summary>
        public Vector3 Position { get { return pose.Position; } set { pose.Position = value; } }

        /// <summary>
        /// The rotation of the transform.
        /// </summary>
        public Quaternion Rotation { get { return pose.Rotation; } set { pose.Rotation = value; } }

        [SerializeField]
        [Tooltip("The scale of the transform.")]
        private Vector3 scale;

        /// <summary>
        /// The scale of the transform.
        /// </summary>
        public Vector3 Scale { get { return scale; } set { scale = value; } }

        public static MixedRealityTransform operator +(MixedRealityTransform left, MixedRealityTransform right)
        {
            return new MixedRealityTransform(left.Position + right.Position, left.Rotation * right.Rotation, Vector3.Scale(left.Scale, right.Scale));
        }

        public static bool operator ==(MixedRealityTransform left, MixedRealityTransform right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MixedRealityTransform left, MixedRealityTransform right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"{pose.Position} | {pose.Rotation}";
        }

        /// <summary>
        /// The Z axis of the pose in world space.
        /// </summary>
        public Vector3 Forward => (pose.Rotation * Vector3.Scale(scale, Vector3.forward)).normalized;

        /// <summary>
        /// The Y axis of the pose in world space.
        /// </summary>
        public Vector3 Up => (pose.Rotation * Vector3.Scale(scale, Vector3.up)).normalized;

        /// <summary>
        /// The X axis of the pose in world space.
        /// </summary>
        public Vector3 Right => (pose.Rotation * Vector3.Scale(scale, Vector3.right)).normalized;

        #region IEqualityComparer Implementation

        bool IEqualityComparer.Equals(object left, object right)
        {
            if (ReferenceEquals(null, left) || ReferenceEquals(null, right)) { return false; }
            if (!(left is MixedRealityTransform) || !(right is MixedRealityTransform)) { return false; }
            return ((MixedRealityTransform)left).Equals((MixedRealityTransform)right);
        }

        public bool Equals(MixedRealityTransform other)
        {
            return Position == other.Position &&
                   Rotation.Equals(other.Rotation);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            return obj is MixedRealityTransform && Equals((MixedRealityTransform)obj);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj is MixedRealityTransform ? ((MixedRealityTransform)obj).GetHashCode() : 0;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion IEqualityComparer Implementation
    }
}
