// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Defines a documentation link for a service.
    /// Used primarily by service inspector facades.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [Obsolete("Use HelpURLAttribute from Unity instead")]
    public class DocLinkAttribute : Attribute
    {
        public DocLinkAttribute(string url) { URL = url; }

        public string URL { get; private set; }
    }
}