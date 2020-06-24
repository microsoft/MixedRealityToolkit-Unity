// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Linq;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tools.ContinuousIntegration
{
    /// <summary>
    /// Validates that the profile attributes are correctly configured to catch build time
    /// issues.
    /// </summary>
    /// <remarks>
    /// Checks that MixedRealityDataProviderAttribute has valid default profile paths that resolve.
    /// </remarks>
    public class ProfileAttributeValidator : IBaseValidator
    {
        /// <inheritdoc/>
        public bool Validate()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetLoadableTypes())
                .Where(type => HasInvalidAttribute(type))
                .Count() == 0;
        }

        /// <summary>
        /// Returns true of the given type has the MixedRealityDataProviderAttribute which has an invalid default profile path.
        /// </summary>
        private static bool HasInvalidAttribute(Type type)
        {
            // Filter out any types that don't derive from BaseService to reduce the work later in the function.
            if (!typeof(BaseService).IsAssignableFrom(type))
            {
                return false;
            }

            foreach (MixedRealityDataProviderAttribute attribute in type.GetCustomAttributes(typeof(MixedRealityDataProviderAttribute), true))
            {
                // Not all data providers are configured with default profile path - we only need to validate
                // those that explicitly set a path.
                if (string.IsNullOrEmpty(attribute.DefaultProfilePath))
                {
                    continue;
                }

                BaseMixedRealityProfile profile = attribute.DefaultProfile;
                if (profile == null)
                {
                    Debug.LogError($"Type {type} has a MixedRealityDataProviderAttribute with a default profile path that doesn't exist: {attribute.DefaultProfilePath}");
                    return true;
                }
                
            }

            return false;
        }
    }
}