// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    public interface IMixedRealitySceneSystem : IMixedRealityEventSystem, IMixedRealityEventSource, IMixedRealityDataProvider
    {
        /// <summary>
        /// True if a load or unload scene operation is in progress.
        /// </summary>
        bool SceneOpInProgress { get; }

        /// <summary>
        /// Progress of the current scene operation, from 0-1.
        /// </summary>
        float SceneOpProgress { get; }

        /// <summary>
        /// Async method to load the scenes provided. Will do nothing if a scene operation is already in progress.
        /// </summary>
        /// <param name="scenesToLoad"></param>
        /// <returns></returns>
        Task LoadScenes(IEnumerable<string> scenesToLoad);

        /// <summary>
        /// Async method to unload the scenes provided. Will do nothing if a scene operation is already in progress.
        /// </summary>
        /// <param name="scenesToLoad"></param>
        /// <returns></returns>
        Task UnloadScenes(IEnumerable<string> scenesToUnload);
    }
}