// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// The configuration object for all AccessibilitySubsystems
    /// </summary>
    [CreateAssetMenu(
        fileName = "AccessibilitySubsystemConfig.asset",
        menuName = "MRTK/Subsystems/Accessibility Config")]
    public class AccessibilitySubsystemConfig : BaseSubsystemConfig
    {
        [SerializeField]
        [Tooltip("Should registered TextMesh Pro objects have their color inverted to contrast with the background?")]
        private bool invertTextColor = default;

        /// <summary>
        /// Indicates whether registered TextMesh Pro objects are to
        /// have their color inverted to contrast with the background.
        /// </summary>
        public bool InvertTextColor
        {
            get => invertTextColor;
            set => invertTextColor = value;
        }

        /// <summary>
        /// Reset the configuration.
        /// </summary>
        protected virtual void Reset()
        {
            invertTextColor = default;
        }
    }
}
