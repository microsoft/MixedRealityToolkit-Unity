// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem
{
    /// <summary>
    /// An Input Action for mapping an action to an Input Sources Button, Joystick, Sensor, etc.
    /// </summary>
    [Serializable]
    public struct MixedRealityInputAction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="axisConstraint"></param>
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
    }
}
