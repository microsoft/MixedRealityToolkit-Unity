// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem
{
    /// <summary>
    /// Data structure for mapping gestures to <see cref="MixedRealityInputAction"/>s that can be raised by the Input System.
    /// </summary>
    [Serializable]
    public struct MixedRealityGestureMapping
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="gestureType"></param>
        /// <param name="action"></param>
        public MixedRealityGestureMapping(string description, GestureInputType gestureType, MixedRealityInputAction action)
        {
            this.description = description;
            this.gestureType = gestureType;
            this.action = action;
        }

        [SerializeField]
        private string description;

        /// <summary>
        /// Simple, human readable description of the gesture.
        /// </summary>
        public string Description => description;

        [SerializeField]
        private GestureInputType gestureType;

        /// <summary>
        /// Type of Gesture.
        /// </summary>
        public GestureInputType GestureType => gestureType;

        [SerializeField]
        private MixedRealityInputAction action;

        /// <summary>
        /// Action for the associated gesture.
        /// </summary>
        public MixedRealityInputAction Action => action;
    }
}