// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    [Serializable]
    public struct SceneInfo
    {
        public static SceneInfo Empty { get { return empty; } }
        private static SceneInfo empty = default(SceneInfo);

        /// <summary>
        /// Scene asset is not set.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return Asset == null;
            }
        }

        /// <summary>
        /// Returns true if the asset is not null and if the scene has a valid build index. Doesn't respect whether scene is enabled in build settings.
        /// </summary>
        public bool IsInBuildSettings
        {
            get
            {
                return Asset != null && Included;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return IsInBuildSettings & BuildIndex >= 0;
            }
        }
        
        /// <summary>
        /// Name of the scene. Set by the property drawer.
        /// </summary>
        [HideInInspector]
        public string Name;

        /// <summary>
        /// Path of the scene. Set by the property drawer.
        /// </summary>
        [HideInInspector]
        public string Path;

        /// <summary>
        /// True if scene is included in build (NOT necessarily enabled)
        /// </summary>
        [HideInInspector]
        public bool Included;

        /// <summary>
        /// Build index of the scene. If included in build settings and enabled, this will be a value greater than zero.
        /// If not included or disabled, this will be -1
        /// </summary>
        [HideInInspector]
        public int BuildIndex;

        /// <summary>
        /// Optional tag used to load and unload scenes in groups.
        /// </summary>
#if UNITY_EDITOR
        [TagProperty]
#endif
        public string Tag;

#if UNITY_EDITOR
        [SceneAssetReference]
#endif
        /// <summary>
        /// SceneAsset reference. Since SceneAsset is an editor-only asset, we store an object reference instead.
        /// </summary>
        public UnityEngine.Object Asset;
    }
}