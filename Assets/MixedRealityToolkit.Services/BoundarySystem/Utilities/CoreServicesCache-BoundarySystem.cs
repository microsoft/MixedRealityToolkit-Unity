// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Boundary;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public partial class CoreServices : BaseCoreServices
    {
        /// <summary>
        /// Cached reference to the active instance of the boundary system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityBoundarySystem BoundarySystem
        {
            get
            {
                return GetService<IMixedRealityBoundarySystem>();
            }
        }
    }
}