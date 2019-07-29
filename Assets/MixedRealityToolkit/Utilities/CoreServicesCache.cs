// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public partial class CoreServices : BaseCoreServices
    {
        private CoreServices() { }
    }

    public abstract class BaseCoreServices
    {
        /// <summary>
        /// Resets all cached system references to null
        /// </summary>
        public static void ResetCacheReferences()
        {
            serviceCache.Clear();
        }

        private static Dictionary<Type, IMixedRealityService> serviceCache = new Dictionary<Type, IMixedRealityService>();

        protected static T GetService<T>() where T : IMixedRealityService
        {
            Type serviceType = typeof(T);

            if (serviceCache.ContainsKey(serviceType))
            {
                return (T)serviceCache[serviceType];
            }
            else
            {
                T service;
                MixedRealityServiceRegistry.TryGetService<T>(out service);
                if (service == null)
                {
                    return service;
                }

                serviceCache.Add(typeof(T), service as IMixedRealityService);
                return service;
            }
        }
    }
}