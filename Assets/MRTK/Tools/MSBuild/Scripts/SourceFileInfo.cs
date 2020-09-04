// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;

namespace Microsoft.MixedReality.Toolkit.MSBuild
{
    /// <summary>
    /// Parsed information for a source file.
    /// </summary>
    public class SourceFileInfo
    {
        /// <summary>
        /// Parses the source file at a given path.
        /// </summary>
        public static SourceFileInfo Parse(FileInfo path, Type classType = null)
        {
            if (path.Extension != ".cs")
            {
                throw new ArgumentException($"Given file '{path.FullName}' is not a C# source file.");
            }
            else if (!path.Exists)
            {
                throw new ArgumentException($"Given file '{path.FullName}' does not exist.");
            }

            if (!Utilities.TryGetGuidForAsset(path, out Guid guid))
            {
                throw new InvalidOperationException($"Couldn't get guid for source asset '{path.FullName}'.");
            }

            return new SourceFileInfo(path, guid, Utilities.GetAssetLocation(path), classType);
        }

        /// <summary>
        /// Gets the file on disk.
        /// </summary>
        public FileInfo File { get; }

        /// <summary>
        /// Gets the Asset Guid for this source file.
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// Gets the asset location of this source file.
        /// </summary>
        public AssetLocation AssetLocation { get; }

        /// <summary>
        /// Gets the class type of this source file. May be null, if the file was not inside the Unity project.
        /// </summary>
        public Type ClassType { get; }

        private SourceFileInfo(FileInfo fileInfo, Guid guid, AssetLocation assetLocation, Type classType)
        {
            File = fileInfo;
            Guid = guid;
            AssetLocation = assetLocation;
            ClassType = classType;
        }
    }
}
