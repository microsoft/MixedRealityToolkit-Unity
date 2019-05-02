// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    [Serializable]
    public struct SceneInfo
    {
        public static SceneInfo Empty { get { return empty; } }
        private static SceneInfo empty;

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
        /// Scene asset is set, but other values are not.
        /// </summary>
        public bool IsIncomplete
        {
            get
            {
                return Asset != null && (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Path) || BuildIndex < 0);
            }
        }

        public string Name;
        public string Path;
        public int BuildIndex;
#if UNITY_EDITOR
        [SceneAssetReference]
#endif
        public UnityEngine.Object Asset;
    }
}