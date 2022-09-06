// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Classes which implement IPoseSource contain a function which retrieves a Unity Pose (position and rotation)
    /// through some means. This can be the pose of a joint, the pose of an input action, or any other means which is appropriate.
    /// Designed to follow the Command Pattern https://en.wikipedia.org/wiki/Command_pattern
    /// </summary>
    public interface IPoseSource
    {
        /// <summary>
        /// Tries to get a Pose.
        /// </summary>
        /// <param name="pose">The value of the pose in world space</param>
        /// <returns>Whether or not retrieving the pose was successful. Some methods,
        /// like retrieving the pose from hand joint data, can fail if the data is not available</returns>
        bool TryGetPose(out Pose pose);
    }
}
