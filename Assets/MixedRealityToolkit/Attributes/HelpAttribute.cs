// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// A PropertyAttribute for showing a collapsable Help section.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property, AllowMultiple = false)]
    public class HelpAttribute : PropertyAttribute
    {
        /// <summary>
        /// The help text
        /// </summary>
        public string Text;

        /// <summary>
        /// The help header foldout text
        /// </summary>
        public string Header;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="helpText">The help text to display</param>
        /// <param name="helpHeader">The help header foldout text</param>
        public HelpAttribute(string helpText, string helpHeader="Help")
        {
            Text = helpText;
            Header = helpHeader;
        }
    }
}