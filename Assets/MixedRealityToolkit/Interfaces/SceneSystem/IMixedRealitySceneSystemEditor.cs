// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.SceneSystem;

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    /// <summary>
    /// Optional editor-only interface for use with facade inspectors.
    /// If a scene system service does not implement this interface, the facade will not be rendered.
    /// </summary>
    public interface IMixedRealitySceneSystemEditor
    {
#if UNITY_EDITOR
        /// <summary>
        /// Returns all content scene tags in the scene service profile.
        /// </summary>
        IEnumerable<string> ContentTags { get; }

        /// <summary>
        /// Returns the content scenes in the scene service profile.
        /// </summary>
        SceneInfo[] ContentScenes { get; }

        /// <summary>
        ///  Returns the lighting scenes in the scene service profile.
        /// </summary>
        SceneInfo[] LightingScenes { get; }

        /// <summary>
        /// Loads the next content scene in-editor. Use instead of LoadNextContent while not in play mode.
        /// </summary>
        /// <param name="wrap"></param>
        void EditorLoadNextContent(bool wrap = false);

        /// <summary>
        /// Loads the prev content scene in-editor. Use instead of LoadPrevContent while not in play mode.
        /// </summary>
        /// <param name="wrap"></param>
        void EditorLoadPrevContent(bool wrap = false);
#endif
    }
}
