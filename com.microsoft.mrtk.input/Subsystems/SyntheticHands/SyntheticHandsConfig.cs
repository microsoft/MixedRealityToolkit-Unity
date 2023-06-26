// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A configuration object for <see cref="Microsoft.MixedReality.Toolkit.Input.SyntheticHandsSubsystem">SyntheticHandsSubsystem</see>.
    /// This class defines the Unity input actions that <see cref="Microsoft.MixedReality.Toolkit.Input.SyntheticHandsSubsystem">SyntheticHandsSubsystem</see>
    /// uses when simulating articulated hands.
    /// </summary>
    [CreateAssetMenu(fileName = "MRTKSyntheticHandsConfig.asset", menuName = "MRTK/Subsystems/MRTK Synthetic Hands Config")]
    public class SyntheticHandsConfig : BaseSubsystemConfig
    {
        [SerializeField, Tooltip("Determines if this subsystem should run even if a device is present."), LabelWidth(200)]
        private bool synthesizeWhenDevicePresent;

        /// <summary>
        /// Determines if this subsystem should run even based on the current settings.
        /// </summary>
        internal bool ShouldSynthesize()
        {
            return synthesizeWhenDevicePresent || !XRDisplaySubsystemHelpers.AreAnyActive();
        }

        [SerializeField, Tooltip("Hand-local pose origin offset.")]
        private Vector3 poseOffset;

        /// <summary>
        /// Hand-local pose origin offset.
        /// </summary>
        public Vector3 PoseOffset => poseOffset;

        [SerializeField, Tooltip("The Input System action to use for the left hand position.")]
        private InputActionProperty leftHandPosition;

        /// <summary>
        /// The Input System action to use for the left hand position.
        /// </summary>
        public InputActionProperty LeftHandPosition => leftHandPosition;

        [SerializeField, Tooltip("The Input System action to use for the left hand rotation.")]
        private InputActionProperty leftHandRotation;

        /// <summary>
        /// The Input System action to use for the left hand rotation.
        /// </summary>
        public InputActionProperty LeftHandRotation => leftHandRotation;

        [SerializeField, Tooltip("The Input System action to use for the left hand selection.")]
        private InputActionProperty leftHandSelect;

        /// <summary>
        /// The Input System action to use for the left hand selection.
        /// </summary>
        public InputActionProperty LeftHandSelect => leftHandSelect;

        [SerializeField, Tooltip("The Input System action to use for the right hand position.")]
        private InputActionProperty rightHandPosition;

        /// <summary>
        /// The Input System action to use for the right hand position.
        /// </summary>
        public InputActionProperty RightHandPosition => rightHandPosition;

        [SerializeField, Tooltip("The Input System action to use for the right hand rotation.")]
        private InputActionProperty rightHandRotation;

        /// <summary>
        /// The Input System action to use for the right hand rotation.
        /// </summary>
        public InputActionProperty RightHandRotation => rightHandRotation;

        [SerializeField, Tooltip("The Input System action to use for the left hand selection.")]
        private InputActionProperty rightHandSelect;

        /// <summary>
        /// The Input System action to use for the left hand selection.
        /// </summary>
        public InputActionProperty RightHandSelect => rightHandSelect;
    }
}
