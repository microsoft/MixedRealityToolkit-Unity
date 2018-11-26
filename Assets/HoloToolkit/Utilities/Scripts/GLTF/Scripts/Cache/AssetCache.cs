using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGLTF.Cache
{
    /// <summary>
    /// Caches data in order to construct a unity object
    /// </summary>
    public class AssetCache : IDisposable
    {
        /// <summary>
        /// Raw loaded images
        /// </summary>
        public Texture2D[] ImageCache { get; private set; }

        /// <summary>
        /// Textures to be used for assets. Textures from image cache with samplers applied
        /// </summary>
        public Texture[] TextureCache { get; private set; }

        /// <summary>
        /// Cache for materials to be applied to the meshes
        /// </summary>
        public MaterialCacheData[] MaterialCache { get; private set; }

        /// <summary>
        /// Byte buffers that represent the binary contents that get parsed
        /// </summary>
        public Dictionary<int, BufferCacheData> BufferCache { get; private set; }

        /// <summary>
        /// Cache of loaded meshes
        /// </summary>
        public List<MeshCacheData[]> MeshCache { get; private set; }

        /// <summary>
        /// Creates an asset cache which caches objects used in scene
        /// </summary>
        /// <param name="imageCacheSize"></param>
        /// <param name="textureCacheSize"></param>
        /// <param name="materialCacheSize"></param>
        /// <param name="bufferCacheSize"></param>
        /// <param name="meshCacheSize"></param>
        public AssetCache(int imageCacheSize, int textureCacheSize, int materialCacheSize, int bufferCacheSize,
            int meshCacheSize)
        {
            // todo: add optimization to set size to be the JSON size
            ImageCache = new Texture2D[imageCacheSize];
            TextureCache = new Texture[textureCacheSize];
            MaterialCache = new MaterialCacheData[materialCacheSize];
            BufferCache = new Dictionary<int, BufferCacheData>(bufferCacheSize);
            MeshCache = new List<MeshCacheData[]>(meshCacheSize);
            for (int i = 0; i < meshCacheSize; ++i)
            {
                MeshCache.Add(null);
            }
        }

        public void Dispose()
        {
            ImageCache = null;
            TextureCache = null;
            MaterialCache = null;
            BufferCache.Clear();
            MeshCache = null;
        }
    }
}
