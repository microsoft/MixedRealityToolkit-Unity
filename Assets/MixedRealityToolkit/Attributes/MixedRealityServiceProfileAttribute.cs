// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Attribute that defines which service a profile is meant to be consumed by.
    /// Only applies to profiles that are consumed by types implementing IMixedRealityService.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class MixedRealityServiceProfileAttribute : Attribute
    {
        public MixedRealityServiceProfileAttribute(Type serviceType) { ServiceType = serviceType; }

        public Type ServiceType { get; private set; }
    }
}