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
        public PointerOption(Handedness handedness, GameObject pointerPrefab, params Type[] controllerType)
        {
            controllerTypes = new SystemType[controllerType.Length];
            for (int i = 0; i < controllerType.Length; i++)
            {
                controllerTypes[i] = new SystemType(controllerType[i]);
            }
            this.handedness = handedness;
            this.pointerPrefab = pointerPrefab;
        }

        [SerializeField]
        [Tooltip("The controller this pointer will attach itself to at runtime.")]
        [Implements(typeof(IMixedRealityController), TypeGrouping.ByNamespaceFlat)]
        private SystemType[] controllerTypes;

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

        public bool Contains(SystemType type)
        {
            foreach (var controllertype in controllerTypes)
            {
                if (controllertype.Equals(type))
                {
                    return true;
                }
            }

            return false;
        }
    }
}