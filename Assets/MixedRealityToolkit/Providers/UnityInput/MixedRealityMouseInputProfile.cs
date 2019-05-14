// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input.UnityInput;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Mouse Input Profile", fileName = "MixedRealityMouseInputProfile", order = (int)CreateProfileMenuItemIndices.MouseInput)]
    [MixedRealityServiceProfile(typeof(MouseDeviceManager))]
    public class MixedRealityMouseInputProfile : BaseMixedRealityProfile
    {
        [Header("Mouse Input Settings")]
        [SerializeField]
        [Range(0.1f, 10f)]
        [Tooltip("Mouse cursor speed multiplier that gets applied to the mouse delta.")]
        private float mouseSpeed = 0.25f;
        /// <summary>
        /// Defines the mouse cursor speed. 
        /// Multiplier that gets applied to the mouse delta before converting to world space.
        /// </summary>
        public float MouseSpeed => mouseSpeed;      
    }
}