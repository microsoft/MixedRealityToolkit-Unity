// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// An Input Action for mapping an action to an Input Sources Button, Joystick, Sensor, etc.
    /// </summary>
    [Serializable]
    public struct MixedRealityInputAction : IEqualityComparer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MixedRealityInputAction(uint id, string description, AxisType axisConstraint = AxisType.None)
        {
            this.id = id;
            this.description = description;
            this.axisConstraint = axisConstraint;
        }

        public static MixedRealityInputAction None { get; } = new MixedRealityInputAction(0, "None");

        /// <summary>
        /// The Unique Id of this Input Action.
        /// </summary>
        public uint Id => id;

        [SerializeField]
        private uint id;

        /// <summary>
        /// A short description of the Input Action.
        /// </summary>
        public string Description => description;

        [SerializeField]
        private string description;

        /// <summary>
        /// The Axis constraint for the Input Action
        /// </summary>
        public AxisType AxisConstraint => axisConstraint;

        [SerializeField]
        private AxisType axisConstraint;

        public static bool operator ==(MixedRealityInputAction left, MixedRealityInputAction right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MixedRealityInputAction left, MixedRealityInputAction right)
        {
            return !left.Equals(right);
        }

        #region IEqualityComparer Implementation

        bool IEqualityComparer.Equals(object left, object right)
        {
            if (ReferenceEquals(null, left) || ReferenceEquals(null, right)) { return false; }
            if (!(left is MixedRealityInputAction) || !(right is MixedRealityInputAction)) { return false; }
            return ((MixedRealityInputAction)left).Equals((MixedRealityInputAction)right);
        }

        public bool Equals(MixedRealityInputAction other)
        {
            return Id == other.Id &&
                   AxisConstraint == other.AxisConstraint;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            return obj is MixedRealityInputAction && Equals((MixedRealityInputAction)obj);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj is MixedRealityInputAction ? ((MixedRealityInputAction)obj).GetHashCode() : 0;
        }

        public override int GetHashCode()
        {
            return $"{Id}.{AxisConstraint}".GetHashCode();
        }

        #endregion IEqualityComparer Implementation
    }
}
