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
    /// Properties of the extent in which a damped
    /// harmonic oscillator is free to move.
    /// </summary>
    [Serializable]
    public struct ElasticExtentProperties<T>
    {
        /// <value>
        /// Represents the lower bound of the extent,
        /// specified as the norm of the n-dimensional extent
        /// </value>
        [SerializeField]
        public float MinStretch;

        /// <value>
        /// Represents the upper bound of the extent,
        /// specified as the norm of the n-dimensional extent
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
        public T[] SnapPoints;
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
        /// Extent at which snap points begin forcing the spring.
        /// </value>
        [SerializeField]
        public float SnapRadius;

        /// <value>
        /// Drag/damper factor, proportional to velocity.
        /// </value>
        [SerializeField]
        public float Drag;
    }
}
