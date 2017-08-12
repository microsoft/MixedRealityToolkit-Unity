using UnityEngine;

namespace HoloToolkit.Unity
{
    public static class CameraCache
    {
        private static Camera cachedCamera;

        /// <summary>
        /// Returns a cached reference to the main camera and uses Camera.main if it hasn't been cached yet.
        /// </summary>
        public static Camera main
        {
            get
            {
                return cachedCamera ?? CacheMain(Camera.main);
            }
        }

        /// <summary>
        /// Set the cached camera to a new reference and return it
        /// </summary>
        /// <param name="newMain">New main camera to cache</param>
        /// <returns></returns>
        private static Camera CacheMain(Camera newMain)
        {
            return cachedCamera = newMain;
        }
    }
}
