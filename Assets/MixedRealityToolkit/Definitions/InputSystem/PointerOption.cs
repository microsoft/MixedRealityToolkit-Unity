// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Attributes;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem
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
        public PointerOption(IMixedRealityController controllerType, Handedness handedness, GameObject pointerPrefab)
        {
            this.controllerType = new SystemType(controllerType.GetType());
            this.handedness = handedness;
            this.pointerPrefab = pointerPrefab;
        }

        [SerializeField]
        [Tooltip("The type of Controller this pointer will attach itself to at runtime.\nNote: If None is selected, then it will attach to any controller type.")]
        [Implements(typeof(IMixedRealityController), TypeGrouping.ByNamespaceFlat)]
        private SystemType controllerType;

        /// <summary>
        /// The type of Controller this pointer will attach itself to at runtime.
        /// </summary>
        /// <remarks>If <see cref="Utilities.Handedness.None"/> is selected, then it will attach to any controller type</remarks>
        public SystemType ControllerType => controllerType;

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