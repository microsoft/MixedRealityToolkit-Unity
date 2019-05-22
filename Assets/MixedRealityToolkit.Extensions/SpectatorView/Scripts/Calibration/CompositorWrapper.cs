// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class CompositorWrapper : Singleton<CompositorWrapper>
    {
        /// <summary>
        /// CompositionManager used to obtain a DSLR stream
        /// </summary>
        [Tooltip("CompositionManager used to obtain the DSLR stream")]
        [SerializeField]
        private CompositionManager compositionManager = null;

        public RenderTexture GetVideoCameraFeed()
        {
#if UNITY_EDITOR
            // Obtain DSLR Feed
            if (compositionManager == null)
            {
                Debug.LogWarning("CompositionManager not assigned for CompositorWrapper");
                return null;
            }

            if (compositionManager.TextureManager == null)
            {
                // TextureManager hasn't been created yet
                return null;
            }

            return compositionManager.TextureManager.compositeTexture; // Note: figure out what is the correct texture to use here in the new compositor wrapper
#else
            Debug.LogWarning("CompositorWrapper not supported on current platform, returning null texture");
            return null;
#endif
        }

        public Texture2D GetVideoCameraTexture()
        {
#if UNITY_EDITOR
            if (compositionManager == null)
            {
                Debug.LogWarning("CompositionManager not assigned for CompositorWrapper");
                return null;
            }

            if (compositionManager.TextureManager == null)
            {
                // TextureManager hasn't been created yet
                return null;
            }

            // Obtain DSLR Image
            Texture2D dslrTexture = new Texture2D(
                compositionManager.TextureManager.colorRGBTexture.width,
                compositionManager.TextureManager.colorRGBTexture.height,
                TextureFormat.RGB24,
                false);

            var previousActive = RenderTexture.active;
            RenderTexture.active = compositionManager.TextureManager.colorRGBTexture;
            dslrTexture.ReadPixels(new Rect(0, 0, dslrTexture.width, dslrTexture.height), 0, 0);
            dslrTexture.Apply();
            RenderTexture.active = previousActive;

            return dslrTexture;
#else
            Debug.LogWarning("CompositorWrapper not supported on current platform, returning null texture");
            return null;
#endif
        }
    }
}