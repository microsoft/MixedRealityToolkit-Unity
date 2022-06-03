// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    internal static class XRDisplaySubsystemHelpers
    {
        private static readonly List<XRDisplaySubsystem> DisplaySubsystems = new List<XRDisplaySubsystem>();

        private static int lastQueriedFrame = -1;

        /// <summary>
        /// Helper for detecting an active XR display. Caches the result per-frame to reduce duplicate queries.
        /// </summary>
        /// <remarks>
        /// This method will not provide a different result within the same frame.
        /// Therefore, if you're starting a subsystem as a result of this method,
        /// don't use this method to detect it running until the subsequent frame.
        /// </remarks>
        /// <returns>Whether any XR displays are currently active.</returns>
        public static bool AreAnyActive()
        {
            int currentFrame = Time.frameCount;

            if (lastQueriedFrame != currentFrame)
            {
                lastQueriedFrame = currentFrame;
                SubsystemManager.GetSubsystems(DisplaySubsystems);
            }

            return DisplaySubsystems.Count > 0;
        }
    }
}
