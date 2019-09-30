// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Used to tag fields by subject to make profiles easier to search.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SubjectAttribute : Attribute
    {
        public SubjectAttribute(SubjectTag tags) { Tags = tags; }

        public SubjectTag Tags { get; private set; }
    }
}