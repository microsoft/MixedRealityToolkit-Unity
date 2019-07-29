// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Teleport;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public partial class CoreServices : BaseCoreServices
    {
        /// <summary>
        /// Cached reference to the active instance of the teleport system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityTeleportSystem TeleportSystem
        {
            get
            {
                return GetService<IMixedRealityTeleportSystem>();
            }
        }
    }
}