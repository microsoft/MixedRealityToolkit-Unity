// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    /// <summary>
    /// Used by scene system to control when newly loaded scenes are activated.
    /// </summary>
    public class SceneActivationToken
    {
        /// <summary>
        /// When true, the operation is waiting on AllowSceneActivation to be set to true before proceeding.
        /// </summary>
        public bool ReadyToProceed { get; private set; } = false;

        /// <summary>
        /// Setting this to true grants permission for scene operation to activate loaded scenes.
        /// </summary>
        public bool AllowSceneActivation { get; set; } = false;

        /// <summary>
        /// Sets ReadyToProceed value
        /// </summary>
        public void SetReadyToProceed(bool readyToProceed)
        {
            ReadyToProceed = readyToProceed;
        }
    }
}