// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input.UnityInput;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CreateAssetMenu(
        menuName = "Mixed Reality/Toolkit/Profiles/Mixed Reality Mouse Input Profile",
        fileName = "MixedRealityMouseInputProfile",
        order = (int)CreateProfileMenuItemIndices.MouseInput)]
    [MixedRealityServiceProfile(typeof(MouseDeviceManager))]
    public class MixedRealityMouseInputProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Range(0.1f, 10f)]
        [Tooltip("Mouse cursor speed multiplier.")]
        [FormerlySerializedAsAttribute("mouseSpeed")]
        private float cursorSpeed = 1.0f;

        /// <summary>
        /// Defines the mouse cursor speed multiplier used to scale the mouse cursor delta.
        /// </summary>
        public float CursorSpeed => cursorSpeed;

        [SerializeField]
        [Range(0.1f, 10f)]
        [Tooltip("Mouse wheel speed multiplier.")]
        private float wheelSpeed = 1.0f;

        /// <summary>
        /// Defines the mouse wheel speed multiplier used to scale the scroll wheel delta.
        /// </summary>
        public float WheelSpeed => wheelSpeed;

    }
}