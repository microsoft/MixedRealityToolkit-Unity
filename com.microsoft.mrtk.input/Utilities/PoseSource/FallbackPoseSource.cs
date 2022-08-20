// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{

    /// <summary>
    /// A pose source composed of other pose sources. Tries to get a pose from each pose source in order, returning the result of the first pose source
    /// which returns a success.
    /// </summary>
    [Serializable]
    public class FallbackPoseSource : IPoseSource
    {
        [SerializeReference]
        [InterfaceSelector]
        [Tooltip("A list of pose sources to query in order")]
        // TODO: Needs some custom inspector work because you the IPoseSource dropdown menu is not selectable within the list context
        // https://forum.unity.com/threads/genericmenu-used-as-context-inside-a-menuitem.330235/ has to do with the list element being it's own context
        private IPoseSource[] poseSources = new IPoseSource[] { new PinchPoseSource() };

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
                IPoseSource currentPoseSource = poseSources[i];
                if (currentPoseSource != null && currentPoseSource.TryGetPose(out pose))
                {
                    break;
                }
            }

            return poseRetrieved;
        }
    }
}
