// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A pose source composed computed from an ordered list of pose sources. Returns the result of the first pose source
    /// which successfully returns a pose.
    /// </summary>
    [Serializable]
    public class FallbackCompositePoseSource : IPoseSource
    {
        [SerializeReference]
        [InterfaceSelector]
        [Tooltip("An ordered list of pose sources to query.")]
        private IPoseSource[] poseSources;

        /// <summary>
        /// An ordered list of pose sources to query.
        /// </summary>
        protected IPoseSource[] PoseSources { get => poseSources; set => poseSources = value; }

        /// <summary>
        /// Tries to get a pose from each pose source in order, returning the result of the first pose source
        /// which returns a success.
        /// </summary>
        public bool TryGetPose(out Pose pose)
        {
            for (int i = 0; i < poseSources.Length; i++)
            {
                IPoseSource currentPoseSource = poseSources[i];
                if (currentPoseSource != null && currentPoseSource.TryGetPose(out pose))
                {
                    return true;
                }
            }

            pose = Pose.identity;
            return false;
        }
    }
}
