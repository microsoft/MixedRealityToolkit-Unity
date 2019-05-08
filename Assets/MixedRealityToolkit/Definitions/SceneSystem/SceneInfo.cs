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
        /// Scene asset was set at some point, but is now gone
        /// </summary>
        public bool IsMissing
        {
            get
            {
                return Asset == null && !string.IsNullOrEmpty(Name);
            }
        }

        /// <summary>
        /// Returns true if the asset is not null and if the scene is in our build settings.
        /// </summary>
        public bool IsInBuildSettings
        {
            get
            {
                return Asset != null && BuildIndex >= 0;
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
        /// True if scene is included in build (not just added to build settings, but also checked)
        /// </summary>
        [HideInInspector]
        public bool Included;

        /// <summary>
        /// Build index of the scene. Set by the property drawer.
        /// </summary>
        [HideInInspector]
        public int BuildIndex;
#if UNITY_EDITOR
        [SceneAssetReference]
#endif
        /// <summary>
        /// SceneAsset reference. Since SceneAsset is an editor-only asset, we store an object reference instead.
        /// </summary>
        public UnityEngine.Object Asset;
    }
}