// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloToolkit.Unity
{
    // Provides a clickable link to documentation in the inspector header
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DocLinkAttribute : Attribute
    {

        public string DocURL { get; private set; }
        public string Description { get; private set; }

        public DocLinkAttribute(string docURL, string description = null)
        {
            DocURL = docURL;
            Description = description;
        }
    }
}