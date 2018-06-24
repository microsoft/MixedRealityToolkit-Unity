// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities
{
    [Serializable]
    public struct SixDof : IEqualityComparer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public SixDof(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="position"></param>
        public SixDof(Vector3 position)
        {
            this.position = position;
            rotation = Quaternion.identity;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rotation"></param>
        public SixDof(Quaternion rotation)
        {
            position = Vector3.zero;
            this.rotation = rotation;
        }

        /// <summary>
        /// The default value for a Six Dof Transform.
        /// </summary>
        /// <returns>
        /// <see cref="Vector3.zero"/> and <see cref="Quaternion.identity"/>.
        /// </returns>
        public static SixDof ZeroIdentity { get; } = new SixDof(Vector3.zero, Quaternion.identity);

        [SerializeField]
        private Vector3 position;

        public Vector3 Position { get { return position; } set { position = value; } }

        [SerializeField]
        private Quaternion rotation;

        public Quaternion Rotation { get { return rotation; } set { rotation = value; } }

        public static SixDof operator +(SixDof left, SixDof right)
        {
            return new SixDof(left.Position + right.Position, left.Rotation * right.Rotation);
        }

        public static bool operator ==(SixDof left, SixDof right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SixDof left, SixDof right)
        {
            return !left.Equals(right);
        }

        #region IEqualityComparer Implementation

        bool IEqualityComparer.Equals(object left, object right)
        {
            if (ReferenceEquals(null, left) || ReferenceEquals(null, right)) { return false; }
            if (!(left is SixDof) || !(right is SixDof)) { return false; }
            return ((SixDof)left).Equals((SixDof)right);
        }

        public bool Equals(SixDof other)
        {
            return Position == other.Position &&
                   Rotation == other.Rotation;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            return obj is SixDof && Equals((SixDof)obj);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj is SixDof ? ((SixDof)obj).GetHashCode() : 0;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion IEqualityComparer Implementation
    }
}
