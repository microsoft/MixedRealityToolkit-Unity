// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.CompilerServices;

#if ARSUBSYSTEMS_ENABLED
using UnityEngine.XR.ARSubsystems;
#endif // ARSUBSYSTEMS_ENABLED

namespace Microsoft.MixedReality.Toolkit
{
    public static class SpatialAwarenessSurfaceTypeExtensions
    {
#if ARSUBSYSTEMS_ENABLED
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static PlaneClassification ToPlaneClassification(this SpatialAwarenessSurfaceType)
        {

        }
#endif // ARSUBSYSTEMS_ENABLED
    }
}
