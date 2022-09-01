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
        [SerializeField]
        [Tooltip("An ordered list of pose sources to query.")]
        private PoseSourceWrapper[] poseSources;

        /// <summary>
        /// An ordered list of pose sources to query.
        /// </summary>
        protected PoseSourceWrapper[] PoseSources { get => poseSources; set => poseSources = value; }

        /// <summary>
        /// Tries to get a pose from each pose source in order, returning the result of the first pose source
        /// which returns a success.
        /// </summary>
        public bool TryGetPose(out Pose pose)
        {
            pose = Pose.identity;
            bool poseRetrieved = false;

            for (int i = 0; i < poseSources.Length; i++)
            {
                IPoseSource currentPoseSource = poseSources[i].source;
                poseRetrieved = currentPoseSource != null && currentPoseSource.TryGetPose(out pose);
                if (poseRetrieved)
                {
                    break;
                }
            }

            return poseRetrieved;
        }

        /// <summary>
        /// An internal wrapper class which is required to allow the pose source to be properly selectable in editor
        /// This is needed because GenericMenu's cannot be set as the active context when embedded inside other GUI contexts,
        /// which in this particular instance, is the list element's container context.
        /// Reference: https://forum.unity.com/threads/genericmenu-used-as-context-inside-a-menuitem.330235/
        /// </summary>
        [Serializable]
        protected struct PoseSourceWrapper
        {
            [SerializeReference]
            [InterfaceSelector]
            [Tooltip("The pose source we are trying to get the pose of")]
            public IPoseSource source;
        }
    }
}
