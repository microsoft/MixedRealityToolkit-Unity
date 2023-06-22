// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// Class enabling assistive technologies to provide descriptive data (ex: purpose,
    /// description, role, etc.) for an object in the scene.
    /// </summary>
    [AddComponentMenu("MRTK/Accessibility/Accessible Object")]
    public class AccessibleObject : MonoBehaviour
    {
        [SerializeField, Experimental]
        [Tooltip("What is the classification (ex: person, place, ui element, etc.) of this object?")]
        private AccessibleObjectClassification classification = null;

        /// <summary>
        /// What is the classification (ex: person, place, ui element, etc.) of this object?
        /// </summary>
        public AccessibleObjectClassification Classification => classification;

        [SerializeField]
        [Tooltip("The full contents of the object.")]
        private string contents = string.Empty;

        /// <summary>
        /// The contents of the accessible object.
        /// </summary>
        /// <remarks>
        /// The contents of the object are generally the complete, readable text. For
        /// example, the .text field of a TextMeshPro object and a button's label are
        /// considered the object's contents.
        /// </remarks>
        public string Contents => contents;

        [SerializeField]
        [Tooltip("Summary of the object's contents.")]
        private string contentSummary = string.Empty;

        /// <summary>
        /// A short version of the accessible object's contents.
        /// </summary>
        /// <remarks>
        /// A good summary provides a clear and concise version of the data contained in
        /// the <see cref="Contents"/> property.
        /// </remarks>
        public string ContentSummary => contentSummary;

        [SerializeField]
        [Tooltip("A rich description of the object.")]
        private string description = string.Empty;

        /// <summary>
        /// A rich description of the object.
        /// </summary>
        /// <remarks>
        /// A good description will provide a rich explanation of the object as well as
        /// information that solidifies its place in the world.
        /// <para/>
        /// A summary description should be provided in the <see cref="Semantic"/> property,
        /// to allow a user to rapidly recognize the object.
        /// </remarks>
        public string Description => description;

        [SerializeField]
        [Tooltip("Instructions on how to interact with the object.")]
        private string instructions = string.Empty;

        /// <summary>
        /// Instructions, if any, for how to interact with the object.
        /// </summary>
        /// <remarks>
        /// Where possible, instructions should be brief and easily understood, for example
        /// "Select the Start button to begin."
        /// </remarks>
        public string Instructions => instructions;

        [SerializeField]
        [Tooltip("Is the object relevant to the current context of the experience?")]
        private bool isContextuallyRelevant = true;

        /// <summary>
        /// Is the object relevant to the current context of the experience?
        /// </summary>
        public bool IsContextuallyRelevant
        {
            get => isContextuallyRelevant;
            set => isContextuallyRelevant = value;
        }

        [SerializeField]
        [Tooltip("Information provided to assistive technologies to describe the role of the object within the scene.")]
        private ObjectRole role = null;

        /// <summary>
        /// Information provided to assistive technologies to describe the role (ex: a progress
        /// bar) of the object within the scene.
        /// </summary>
        public ObjectRole Role => role;

        [SerializeField]
        [Tooltip("The semantic (ex: 'antique rocking chair') of the object.")]
        // todo: need better field/property names (FriendlyCategory?)
        private string semantic = string.Empty;

        /// <summary>
        /// A string describing the semantic usage of the object. Common semantics include:
        /// "login button", "rocking chair", etc.
        /// </summary>
        public string Semantic => semantic;

        #region Monobehaviour methods

        private static bool suppressSubsystemNotFound = false;

        private void OnEnable()
        {
            if ((AccessibilityHelpers.Subsystem == null) && !suppressSubsystemNotFound)
            {
                Debug.LogWarning("The accessibility subsystem is not enabled or has not yet started.");
                suppressSubsystemNotFound = true;
                return;
            }

            if (!AccessibilityHelpers.Subsystem.TryRegisterAccessibleObject(gameObject, Classification))
            {
                Debug.LogError($"Failed to register {gameObject.name} with the accessibility subsystem.");
            }
        }

        private void OnDisable()
        {
            if (AccessibilityHelpers.Subsystem == null) { return; }
            if (!AccessibilityHelpers.Subsystem.TryUnregisterAccessibleObject(gameObject, Classification))
            {
                Debug.LogError($"Failed to unregister {gameObject.name} with the accessibility subsystem.");
            }
        }

        #endregion Monobehavior methods
    }
}
