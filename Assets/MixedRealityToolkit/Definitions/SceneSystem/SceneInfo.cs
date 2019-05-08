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

        [HideInInspector]
        public string Name;
        [HideInInspector]
        public string Path;
        [HideInInspector]
        public int BuildIndex;
#if UNITY_EDITOR
        [SceneAssetReference]
#endif
        public UnityEngine.Object Asset;
    }
}