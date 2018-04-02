using System;

namespace MixedRealityToolkit.Build.WindowsDevicePortal.DataStructures
{
    [Serializable]
    public struct FileInfo
    {
        /// <summary>
        /// Folder under the requested known folder.
        /// </summary>
        public string CurrentDir;
        public int DateCreated;
        /// <summary>
        /// In bytes.
        /// </summary>
        public int FileSize;
        public string Id;
        public string Name;
        /// <summary>
        /// Present if this item is a folder, this is the name of the folder.
        /// </summary>
        public string SubPath;
        /// <summary>
        /// Folder==16 <para/>
        /// File==32
        /// </summary>
        public int Type;
    }
}