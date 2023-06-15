// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// Role (ex: is a button) for an object that may appear in the scene.
    /// </summary>
    [CreateAssetMenu(
        fileName = "ObjectRole.asset",
        menuName = "MRTK/Accessibility/Object Role")]
    public class ObjectRole : ScriptableObject
    {
        [SerializeField, Experimental]
        [Tooltip("Description of the role.")]
        private string description;

        /// <summary>
        /// Description of the role.
        /// </summary>
        public string Description
        {
            get => description;
            set => description = value;
        }

        [SerializeField]
        [Tooltip("Formal name of the role.")]
        private string formalName;

        /// <summary>
        /// Formal name of the role.
        /// </summary>
        public string FormalName
        {
            get => formalName;
            set => formalName = value;
        }

        [SerializeField]
        [Tooltip("Is this an ARIA role?")]
        private bool isAria = false;

        /// <summary>
        /// Is this an ARIA role?
        /// </summary>
        public bool IsAria
        {
            get => isAria;
            set => isAria = value;
        }

        [SerializeField]
        [Tooltip("Optional link to additional information about the role.")]
        private string referenceLink;


        /// <summary>
        /// Optional link to additional information about the role.
        /// </summary>
        public string ReferenceLink
        {
            get => referenceLink;
            set => referenceLink = value;
        }
    }
}
