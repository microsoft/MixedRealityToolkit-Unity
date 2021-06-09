// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// A PropertyAttribute for showing a collapsible Help section.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class HelpAttribute : PropertyAttribute
    {
        /// <summary>
        /// The help text
        /// </summary>
        public string Text;

        /// <summary>
        /// The help header foldout text
        /// </summary>
        /// <remarks>
        /// If Collapsible is false, then this header text will not be shown.
        /// </remarks>
        public string Header;

        /// <summary>
        /// If true, this will be a collapsible help section. Defaults to true.
        /// </summary>
        public bool Collapsible;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="helpText">The help text to display</param>
        /// <param name="helpHeader">The help header foldout text</param>
        /// <param name="collapsible">If true, this help drawer will be collapsible</param>
        public HelpAttribute(string helpText, string helpHeader = "Help", bool collapsible = true)
        {
            Text = helpText;
            Header = helpHeader;
            Collapsible = collapsible;
        }
    }
}