// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Defines a pointer option to assign to a controller.
    /// </summary>
    [Serializable]
    public struct PointerOption : ISerializationCallbackReceiver
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PointerOption(SupportedControllerType controllerType, Handedness handedness, GameObject pointerPrefab, LayerMask[] prioritizedLayerMasks = null)
        {
            this.controllerType = controllerType;
            this.handedness = handedness;
            this.pointerPrefab = pointerPrefab;
            this.prioritizedLayerMasks = prioritizedLayerMasks ?? new LayerMask[1] { UnityEngine.Physics.DefaultRaycastLayers };
        }

        [EnumFlags]
        [SerializeField]
        [Tooltip("The type of Controller this pointer can be attached to at runtime.")]
        private SupportedControllerType controllerType;

        /// <summary>
        /// The type of Controller this pointer can be attached to at runtime.
        /// </summary>
        /// <remarks>If <see cref="Microsoft.MixedReality.Toolkit.Utilities.Handedness.None"/> is selected, then it will attach to any controller type</remarks>
        public SupportedControllerType ControllerType => controllerType;

        [SerializeField]
        [Tooltip("Defines valid hand(s) to create the pointer prefab on.")]
        private Handedness handedness;

        /// <summary>
        /// Defines valid hand(s) to create the pointer prefab on.
        /// </summary>
        public Handedness Handedness => handedness;

        [SerializeField]
        [Tooltip("The prefab with an IMixedRealityPointer component to create when a valid controller becomes available.")]
        private GameObject pointerPrefab;

        /// <summary>
        /// The prefab with an <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityPointer"/> component to create when a valid controller becomes available. 
        /// </summary>
        public GameObject PointerPrefab => pointerPrefab;

        [SerializeField]
        [Tooltip("The LayerMasks, in prioritized order, which are used to determine the target.")]
        private LayerMask[] prioritizedLayerMasks;

        /// <summary>
        /// The LayerMasks, in prioritized order, which are used to determine the target
        /// </summary>
        public LayerMask[] PrioritizedLayerMasks
        {
            get => prioritizedLayerMasks;
            internal set => prioritizedLayerMasks = value;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (pointerPrefab == null)
            {
                return;
            }

            IMixedRealityPointer pointer = pointerPrefab.GetComponent<IMixedRealityPointer>();
            if (pointer.IsNull()
                || pointer.PrioritizedLayerMasksOverride == null
                || pointer.PrioritizedLayerMasksOverride.Length == 0
                || (prioritizedLayerMasks != null && pointer.PrioritizedLayerMasksOverride.SequenceEqual(prioritizedLayerMasks)))
            {
                return;
            }

            int pointerPrioritizedLayerMasksOverrideCount = pointer.PrioritizedLayerMasksOverride.Length;
            if (prioritizedLayerMasks?.Length != pointerPrioritizedLayerMasksOverrideCount)
            {
                prioritizedLayerMasks = new LayerMask[pointerPrioritizedLayerMasksOverrideCount];
            }
            Array.Copy(pointer.PrioritizedLayerMasksOverride, prioritizedLayerMasks, pointerPrioritizedLayerMasksOverrideCount);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() { }
    }
}
