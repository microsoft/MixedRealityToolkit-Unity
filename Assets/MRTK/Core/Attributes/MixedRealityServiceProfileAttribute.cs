// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Attribute that defines the relationships between profiles and services.
    /// 
    /// When used to mark a profile:
    /// Applies to profiles that are consumed by types implementing IMixedRealityService.
    /// A service must implement all required types and no excluded types to be considered compatible with the profile.
    /// 
    /// When used to mark a service:
    /// Only profiles specified in requiredTypes are allowed for configuring that service.
    /// </summary>
    /// <remarks>
    /// There are a few reasons for the complexity here allowing bidirectional declarations:
    /// 1. To allow for a profile to apply to different services (which all implement a specific interface or type)
    /// 2. To allow for a service to declare a dependency on a specific profile, so that the profile can live in
    ///    separate assembly definition to guarantee consistent profile deserialization for all platforms.
    /// </remarks>
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