// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    /// <summary>
    /// Interface for managing scenes in Unity.
    /// Scenes are divided into three categories: Manager, Lighting and Content.
    ///
    /// The Manager scene is loaded first and remains loaded for the duration of the app.
    /// Only one Manager scene is ever loaded, and no scene operation will ever unload it.
    ///
    /// The Lighting scene is a largely empty scene which controls lighting settings.
    /// Ambient lighting, skybox, sun direction, etc. A default lighting scene is loaded on initialization.
    /// After that the active lighting scene may be changed at any time via SetLightingScene.
    /// Only one lighting scene can ever be loaded at a time.
    ///
    /// Content scenes are everything else. These can be loaded and unloaded at will in any combination.
    ///
    /// The scene actions provided improve on unity's SceneManagement events by ensuring that scenes
    /// are considered valid before the action is invoked.
    /// </summary>
    public interface IMixedRealitySceneSystem : IMixedRealityEventSystem, IMixedRealityEventSource
    {
        #region Actions

        /// <summary>
        /// Called just before a set of content scenes is loaded.
        /// Includes names of all scenes about to be loaded.
        /// </summary>
        Action<IEnumerable<string>> OnWillLoadContent { get; set; }

        /// <summary>
        /// Called when a set of content scenes have been loaded, activated and are valid.
        /// Includes names of all scenes loaded.
        /// </summary>
        Action<IEnumerable<string>> OnContentLoaded { get; set; }

        /// <summary>
        /// Called just before a set of content scenes will be unloaded.
        /// Includes names of all scenes about to be unloaded.
        /// </summary>
        Action<IEnumerable<string>> OnWillUnloadContent { get; set; }

        /// <summary>
        /// Called after a set of content scenes have been completely unloaded.
        /// Includes names of all scenes about to be unloaded.
        /// </summary>
        Action<IEnumerable<string>> OnContentUnloaded { get; set; }

        /// <summary>
        /// Called just before a lighting scene is loaded.
        /// Includes name of scene.
        /// </summary>
        Action<string> OnWillLoadLighting { get; set; }

        /// <summary>
        /// Called when a lighting scene has been loaded, activated and is valid.
        /// Includes scene name.
        /// </summary>
        Action<string> OnLightingLoaded { get; set; }

        /// <summary>
        /// Called just before a lighting scene unload operation begins.
        /// Includes scene name.
        /// </summary>
        Action<string> OnWillUnloadLighting { get; set; }

        /// <summary>
        /// Called after a lighting scene has been completely unloaded.
        /// Includes scene name.
        /// </summary>
        Action<string> OnLightingUnloaded { get; set; }

        /// <summary>
        /// Called just before a scene is loaded.
        /// Called for all scene types (content, lighting and manager)
        /// Includes scene name
        /// </summary>
        Action<string> OnWillLoadScene { get; set; }

        /// <summary>
        /// Called when scene has been loaded, activated and is valid.
        /// Called for all scene types (content, lighting and manager)
        /// Includes scene name
        /// </summary>
        Action<string> OnSceneLoaded { get; set; }

        /// <summary>
        /// Called just before a scene will be unloaded
        /// Called for all scene types (content, lighting and manager)
        /// Includes scene name
        /// </summary>
        Action<string> OnWillUnloadScene { get; set; }

        /// <summary>
        /// Called when scene has been unloaded
        /// Called for all scene types (content, lighting and manager)
        /// Includes scene name
        /// </summary>
        Action<string> OnSceneUnloaded { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// True if the scene system is loading or unloading content scenes.
        /// Manager and lighting scenes are ignored.
        /// </summary>
        bool SceneOperationInProgress { get; }

        /// <summary>
        /// Progress of the current scene operation, from 0-1.
        /// A scene operation may include multiple concurrently loaded scenes.
        /// </summary>
        float SceneOperationProgress { get; }

        /// <summary>
        /// True if the scene system is transitioning from one lighting scene to another.
        /// Lighting operations will not impede other operations.
        /// </summary>
        bool LightingOperationInProgress { get; }

        /// <summary>
        /// Progress of current lighting operation, from 0-1
        /// </summary>
        float LightingOperationProgress { get; }

        /// <summary>
        /// True when content has been loaded with an activation token and AllowSceneActivation has not been set to true.
        /// Useful for existing entities that shouldn't act until a newly loaded scene is actually activated.
        /// </summary>
        bool WaitingToProceed { get; }

        /// <summary>
        /// Name of the currently loaded lighting scene.
        /// If a transition is in progress, this reports the target lighting scene we're transitioning to.
        /// </summary>
        string ActiveLightingScene { get; }

        /// <summary>
        /// Returns true if a content scene appears in build settings PRIOR to the latest loaded build index.
        /// Use to verify that LoadPrevContent can be performed without wrapping.
        /// </summary>
        bool PrevContentExists { get; }

        /// <summary>
        /// Returns true if a content scene appears in build settings AFTER the latest loaded build index.
        /// Use to verify that LoadNextContent can be performed without wrapping.
        /// </summary>
        bool NextContentExists { get; }

        /// <summary>
        /// An array of content scenes available to load / unload. Order in array matches build order.
        /// Useful if you want to present an ordered list of options, or if you want to track which scenes are loaded via IsContentLoaded.
        /// </summary>
        string[] ContentSceneNames { get; }

        #endregion

        #region Scene Operations

        /// <summary>
        /// Async method to load the scenes by name.
        /// If a scene operation is in progress, no action will be taken.
        /// </summary>
        /// <param name="scenesToLoad">Names of content scenes to load. Invalid scenes will be ignored.</param>
        /// <param name="mode">Additive mode will load the content additively. Single mode will first unload all loaded content scenes first.</param>
        /// <param name="activationToken">
        /// Optional token for manual scene activation. Useful for loading screens and multiplayer.
        /// If not null, operation will wait until activationToken's AllowSceneActivation value is true before activating scene objects.
        /// </param>
        /// <returns>Task</returns>
        Task LoadContent(IEnumerable<string> scenesToLoad, LoadSceneMode mode = LoadSceneMode.Additive, SceneActivationToken activationToken = null);

        /// <summary>
        /// Async method to unload scenes by name.
        /// If a scene is not loaded, it will be ignored.
        /// If a scene operation is in progress, no action will be taken.
        /// </summary>
        /// <returns>Task</returns>
        Task UnloadContent(IEnumerable<string> scenesToUnload);

        /// <summary>
        /// Async method to load a single scene by name.
        /// If a scene operation is in progress, no action will be taken.
        /// </summary>
        /// <param name="sceneToLoad">Name of content scene to load. Invalid scenes will be ignored.</param>
        /// <param name="mode">Additive mode will load the content additively. Single mode will first unload all loaded content scenes first.</param>
        /// <param name="activationToken">
        /// Optional token for manual scene activation. Useful for loading screens and multiplayer.
        /// If not null, operation will wait until activationToken's AllowSceneActivation value is true before activating scene objects.
        /// </param>
        /// <returns>Task</returns>
        Task LoadContent(string sceneToLoad, LoadSceneMode mode = LoadSceneMode.Additive, SceneActivationToken activationToken = null);

        /// <summary>
        /// Async method to unload a single scene by name.
        /// If the scene is not loaded, no action will be taken.
        /// If a scene operation is in progress, no action will be taken.
        /// </summary>
        /// <returns>Task</returns>
        Task UnloadContent(string sceneToUnload);

        /// <summary>
        /// Async method to load content scenes by tag. All scenes with the supplied tag will be loaded.
        /// If no scenes with this tag are found, no action will be taken.
        /// If a scene operation is in progress, no action will be taken.
        /// </summary>
        /// <param name="tag">Scene tag.</param>
        /// <param name="mode">Additive mode will load the content additively. Single mode will first unload all loaded content scenes first.</param>
        /// <param name="activationToken">
        /// Optional token for manual scene activation. Useful for loading screens and multiplayer.
        /// If not null, operation will wait until activationToken's AllowSceneActivation value is true before activating scene objects.
        /// </param>
        /// <returns>Task</returns>
        Task LoadContentByTag(string tag, LoadSceneMode mode = LoadSceneMode.Additive, SceneActivationToken activationToken = null);

        /// <summary>
        /// Async method to unload scenes by name.
        /// If a scene is not loaded, it will be ignored.
        /// If a scene operation is in progress, no action will be taken.
        /// </summary>
        /// <param name="tag">Scene tag</param>
        /// <returns>Task</returns>
        Task UnloadContentByTag(string tag);

        /// <summary>
        /// Loads the next content scene according to build index.
        /// Uses the last-loaded content scene as previous build index.
        /// If no next content exists, and wrap is false, no action is taken.
        /// Use NextContentExists to verify that this operation is possible (if not using wrap).
        /// </summary>
        /// <param name="wrap">If true, if the current scene is the LAST content scene, the FIRST content scene will be loaded.</param>
        /// <param name="mode">Additive mode will load the content additively. Single mode will first unload all loaded content scenes first.</param>
        /// <param name="activationToken">
        /// Optional token for manual scene activation. Useful for loading screens and multiplayer.
        /// If not null, operation will wait until activationToken's AllowSceneActivation value is true before activating scene objects.
        /// </param>
        /// <returns>Task</returns>
        Task LoadNextContent(bool wrap = false, LoadSceneMode mode = LoadSceneMode.Single, SceneActivationToken activationToken = null);

        /// <summary>
        /// Loads the previous content scene according to build index.
        /// Uses the loaded content scene with the smallest build index as previous build index.
        /// If no previous content exists, and wrap is false, no action is taken.
        /// Use PrevContentExists to verify that this operation is possible (if not using wrap).
        /// </summary>
        /// <param name="wrap">If true, if the current scene is the FIRST content scene, the LAST content scene will be loaded.</param>
        /// <param name="mode">Additive mode will load the content additively. Single mode will first unload all loaded content scenes first.</param>
        /// <param name="activationToken">
        /// Optional token for manual scene activation. Useful for loading screens and multiplayer.
        /// If not null, operation will wait until activationToken's AllowSceneActivation value is true before activating scene objects.
        /// </param>
        /// <returns>Task</returns>
        Task LoadPrevContent(bool wrap = false, LoadSceneMode mode = LoadSceneMode.Single, SceneActivationToken activationToken = null);

        /// <summary>
        /// Returns true if a content scene is fully loaded.
        /// </summary>
        bool IsContentLoaded(string sceneName);

        /// <summary>
        /// Sets the current lighting scene. The lighting scene determines ambient light and skybox settings. It can optionally contain light objects.
        /// If the lighting scene is already loaded, no action will be taken.
        /// If a lighting scene transition is in progress, request will be queued and executed when the transition is complete.
        /// </summary>
        /// <param name="lightingSceneName">The name of the lighting scene.</param>
        /// <param name="transitionType">The transition type to use. See LightingSceneTransitionType for information about each transition type.</param>
        /// <param name="transitionDuration">The duration of the transition (if not None).</param>
        void SetLightingScene(string newLightingSceneName, LightingSceneTransitionType transitionType = LightingSceneTransitionType.None, float transitionDuration = 1f);

        #endregion

        #region Utilities

        /// <summary>
        /// Returns a set of scenes by name.
        /// Useful for processing events.
        /// </summary>
        IEnumerable<Scene> GetScenes(IEnumerable<string> sceneNames);

        /// <summary>
        /// Returns a scene by name.
        /// Useful for processing events.
        /// </summary>
        Scene GetScene(string sceneName);

        #endregion
    }
}
