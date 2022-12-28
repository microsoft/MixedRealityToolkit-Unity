// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// Script that applies accessibility enhancements to an attached Text Mesh Pro object.
    /// </summary>
    [AddComponentMenu("MRTK/Accessibility/Text Accessibility")]
    public class TextAccessibility : MonoBehaviour
    {
        private AccessibilitySubsystem accessibilitySubsystem = null;
        private Material material = null;

        /// <summary>
        /// Apply the initial configuration settings to the object.
        /// </summary>
        private void ApplyInitialConfiguration()
        {
            OnInvertTextColorChanged(accessibilitySubsystem.InvertTextColor);
        }

        /// <summary>
        /// Get the material that will be used to change the text color.
        /// </summary>
        /// <returns>
        /// The material controlling the appearance of the text, or null.
        /// </returns>
        private void GetTextMaterial()
        {
            if (TryGetComponent(out TMP_Text tmpText))
            {
                material = tmpText.fontMaterial;
                return;
            }

            // The object was not the appropriate type.
            Debug.LogError($"{nameof(TextAccessibility)} requires being attached to a TextMeshPro or TextMeshProUGUI object");
        }

        /// <summary>
        /// Handles <see cref="AccessibilitySubsystem.InvertTextColorChanged"/> events.
        /// </summary>
        /// <param name="invert">
        /// TRue to apply text color inversion, or false.
        /// </param>
        private void OnInvertTextColorChanged(bool invert)
        {
            if (material == null) { return; }
            accessibilitySubsystem?.ApplyTextColorInversion(material, invert);
        }

        /// <summary>
        /// Registers AccessibilitySubsystem event handlers.
        /// </summary>
        /// <remarks>
        /// Prior to registration, the initial configuration settings will
        /// be applied.
        /// </remarks>
        private void RegisterHandlers()
        {
            if (accessibilitySubsystem != null)
            {
                ApplyInitialConfiguration();
                accessibilitySubsystem.InvertTextColorChanged += OnInvertTextColorChanged;
            }
        }

        /// <summary>
        /// Unregisters AccessibilitySubsystem event handlers.
        /// </summary>
        private void UnregisterHandlers()
        {
            if (accessibilitySubsystem != null)
            {
                accessibilitySubsystem.InvertTextColorChanged -= OnInvertTextColorChanged;
            }
        }

        #region MonoBehaviour

        private void Start()
        {
            GetTextMaterial();
            accessibilitySubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<AccessibilitySubsystem>();
            RegisterHandlers();
        }

        private void OnEnable()
        {
            RegisterHandlers();
        }

        private void OnDisable()
        {
            UnregisterHandlers();
        }

        private void OnDestroy()
        {
            accessibilitySubsystem = null;
        }

        #endregion MonoBehaviour
    }
}
