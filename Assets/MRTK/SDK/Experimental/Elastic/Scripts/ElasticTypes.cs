// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Physics
{
    /// <summary>
    /// Properties of a linear, one-dimensional extent
    /// in which a damped harmonic oscillator is free to move.
    /// </summary>
    [Serializable]
    public struct LinearElasticExtent
    {
        /// <value>
        /// Represents the lower bound of the extent.
        /// </value>
        [SerializeField]
        public float MinStretch;

        /// <value>
        /// Represents the upper bound of the extent.
        /// </value>
        [SerializeField]
        public float MaxStretch;

        /// <value>
        /// Whether the system, when approaching the upper bound,
        /// will treat the end limits like snap points and magnetize to them.
        /// </value>
        [SerializeField]
        public bool SnapToEnds;

        /// <value>
        /// Points inside the extent to which the system will snap.
        /// </value>
        [SerializeField]
        public float[] SnapPoints;

        /// <value>
        /// Distance at which snap points begin forcing the spring.
        /// </value>
        [SerializeField]
        public float SnapRadius;
    }

    /// <summary>
    /// Properties of a three-dimensional extent
    /// in which a damped harmonic oscillator is free to move.
    /// </summary>
    [Serializable]
    public struct VolumeElasticExtent
    {
        /// <value>
        /// Represents the lower bound of the extent.
        /// </value>
        [SerializeField]
        public Bounds StretchBounds;

        /// <value>
        /// Whether the system, when approaching the extent bounds,
        /// will treat the bounds like snap points and magnetize to them.
        /// </value>
        [SerializeField]
        public bool SnapToEnds;

        /// <value>
        /// Points inside the extent to which the system will snap.
        /// </value>
        [SerializeField]
        public Vector3[] SnapPoints;

        /// <value>
        /// Should the SnapPoints be repeated to infinity to serve
        /// as regular snapping intervals?
        /// </value>
        public bool repeatSnapPoints;

        /// <value>
        /// Distance at which snap points begin forcing the spring.
        /// </value>
        [SerializeField]
        public float SnapRadius;
    }

    /// <summary>
    /// Properties of a four-dimensional extent
    /// in which a damped harmonic oscillator is free to rotate.
    /// </summary>
    [Serializable]
    public struct QuaternionElasticExtent
    {
        /// <value>
        /// Points inside the extent to which the system will snap.
        /// </value>
        [SerializeField]
        public Quaternion[] SnapPoints;

        /// <value>
        /// Should the SnapPoints be repeated to infinity to serve
        /// as regular snapping intervals?
        /// </value>
        public bool repeatSnapPoints;

        /// <value>
        /// Distance at which snap points begin forcing the spring.
        /// </value>
        [SerializeField]
        public float SnapRadius;
    }

    /// <summary>
    /// Properties of the damped harmonic oscillator differential system.
    /// </summary>
    [Serializable]
    public struct ElasticProperties
    {
        /// <value>
        /// Mass of the simulated oscillator element
        /// </value>
        [SerializeField]
        public float Mass;

        /// <value>
        /// Hand spring constant
        /// </value>
        [SerializeField]
        public float HandK;

        /// <value>
        /// End cap spring constant
        /// </value>
        [SerializeField]
        public float EndK;

        /// <value>
        /// Snap point spring constant
        /// </value>
        [SerializeField]
        public float SnapK;

        /// <value>
        /// Drag/damper factor, proportional to velocity.
        /// </value>
        [SerializeField]
        public float Drag;
    }
}
