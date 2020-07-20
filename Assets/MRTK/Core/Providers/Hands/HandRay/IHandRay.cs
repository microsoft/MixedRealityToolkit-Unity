// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface defining a hand ray, which is used by far pointers to direct interactions.
    /// Implementations of this class are managed and updated by a BaseHand implementation.
    /// </summary>
    public interface IHandRay
    {
        /// <summary>
        /// Ray used by input system for Far Pointers.
        /// </summary>
        Ray Ray { get; }

        /// <summary>
        /// Check whether hand palm is angled in a way that hand rays should be used.
        /// </summary>
        bool ShouldShowRay { get; }

        /// <summary>
        /// Update data used by hand implementation, to compute next HandRay.
        /// </summary>
        /// <param name="handPosition">Position of hand</param>
        /// <param name="palmNormal">Palm normal</param>
        /// <param name="headTransform">Transform of CameraCache.main</param>
        /// <param name="sourceHandedness">Handedness of related hand</param>
        void Update(Vector3 handPosition, Vector3 palmNormal, Transform headTransform, Handedness sourceHandedness);
    }
}
