// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Attribute that defines which service a profile is meant to be consumed by.
    /// Only applies to profiles that are consumed by types implementing IMixedRealityService.
    /// A service must implement all required types and no excluded types to be considered compatible with the profile.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class MixedRealityServiceProfileAttribute : Attribute
    {
        public MixedRealityServiceProfileAttribute(Type requiredType, Type excludedType = null)
        {
            RequiredTypes = new Type[] { requiredType };
            ExcludedTypes = excludedType != null ? new Type[] { excludedType } : new Type[0];
        }

        public MixedRealityServiceProfileAttribute(Type[] requiredTypes, Type[] excludedTypes = null)
        {
            RequiredTypes = requiredTypes;
            ExcludedTypes = excludedTypes != null ? excludedTypes : new Type[0];
        }

        public Type[] RequiredTypes { get; private set; }
        public Type[] ExcludedTypes { get; private set; }
    }
}