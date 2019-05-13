// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

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
        /// Async method to load the scenes by name. Will do nothing if a scene operation is already in progress.
        /// </summary>
        /// <param name="scenesToLoad"></param>
        /// <returns></returns>
        Task LoadContent(IEnumerable<string> scenesToLoad, LoadSceneMode mode = LoadSceneMode.Additive);

        /// <summary>
        /// Async method to unload the scenes by name. Will do nothing if a scene operation is already in progress.
        /// </summary>
        /// <param name="scenesToLoad"></param>
        /// <returns></returns>
        Task UnloadContent(IEnumerable<string> scenesToUnload);

        /// <summary>
        /// Async method to load a single scene by name. Will do nothing if a scene operation is already in progress.
        /// </summary>
        /// <param name="sceneToLoad"></param>
        /// <returns></returns>
        Task LoadContent(string sceneToLoad, LoadSceneMode mode = LoadSceneMode.Additive);

        /// <summary>
        /// Async method to unload a single scene by name. Will do nothing if a scene operation is already in progress.
        /// </summary>
        /// <param name="sceneToUnload"></param>
        /// <returns></returns>
        Task UnloadContent(string sceneToUnload);

        /// <summary>
        // Async method to load scenes by tag. Will do nothing if a scene operation is already in progress.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        Task LoadContentByTag(string tag, LoadSceneMode mode = LoadSceneMode.Additive);

        /// <summary>
        /// Async method to unload scenes by tag. Will do nothing if a scene operation is already in progress.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        Task UnloadContentByTag(string tag);
    }
}