// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Defines a documentation link for a service.
    /// Used primarily by service inspector facades.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DocLinkAttribute : Attribute
    {
        public DocLinkAttribute(string url) { URL = url; }

        public string URL { get; private set; }
    }
}