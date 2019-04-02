// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Defines a pointer option to assign to a controller.
    /// </summary>
    [Serializable]
    public struct PointerOption
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controllerType"></param>
        /// <param name="handedness"></param>
        /// <param name="pointerPrefab"></param>
        public PointerOption(SupportedControllerType controllerType, Handedness handedness, GameObject pointerPrefab)
        {
            this.controllerType = controllerType;
            this.handedness = handedness;
            this.pointerPrefab = pointerPrefab;
        }

        [EnumFlags]
        [SerializeField]
        [Tooltip("The controller this pointer will attach itself to at runtime.")]
        private SupportedControllerType controllerType;

        /// <summary>
        /// The type of Controller this pointer will attach itself to at runtime.
        /// </summary>
        /// <remarks>If <see cref="Microsoft.MixedReality.Toolkit.Utilities.Handedness.None"/> is selected, then it will attach to any controller type</remarks>
        public SupportedControllerType ControllerType => controllerType;

        [SerializeField]
        [Tooltip("Defines which hand to create the pointer prefab on")]
        private Handedness handedness;

        /// <summary>
        /// Defines which hand to create the pointer prefab on.
        /// </summary>
        public Handedness Handedness => handedness;

        [SerializeField]
        private GameObject pointerPrefab;

        public GameObject PointerPrefab => pointerPrefab;
    }
}