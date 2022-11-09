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
    public class FallbackCompositePoseSource : IPoseSource, ISerializationCallbackReceiver
    {
        [SerializeReference]
        [InterfaceSelector]
        [Tooltip("An ordered list of pose sources to query.")]
        private IPoseSource[] poseSourceList;

        [SerializeField]
        [Tooltip("An ordered list of pose sources to query.")]
        [Obsolete, HideInInspector]
        private PoseSourceWrapper[] poseSources;

        /// <summary>
        /// An ordered list of pose sources to query.
        /// </summary>
        protected IPoseSource[] PoseSources { get => poseSourceList; set => poseSourceList = value; }

        /// <summary>
        /// Tries to get a pose from each pose source in order, returning the result of the first pose source
        /// which returns a success.
        /// </summary>
        public bool TryGetPose(out Pose pose)
        {
            for (int i = 0; i < poseSourceList.Length; i++)
            {
                IPoseSource currentPoseSource = poseSourceList[i];
                if (currentPoseSource != null && currentPoseSource.TryGetPose(out pose))
                {
                    return true;
                }
            }

            pose = Pose.identity;
            return false;
        }

        [Obsolete]
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (poseSources != null && poseSources.Length > 0)
            {
                poseSourceList = new IPoseSource[poseSources.Length];

                for (int i = 0; i < poseSources.Length; i++)
                {
                    PoseSourceWrapper poseSource = poseSources[i];
                    poseSourceList[i] = poseSource.source;
                }

                poseSources = null;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        [Serializable, Obsolete]
        private struct PoseSourceWrapper
        {
            [SerializeReference]
            [InterfaceSelector]
            [Tooltip("The pose source we are trying to get the pose of")]
            public IPoseSource source;
        }
    }
}
