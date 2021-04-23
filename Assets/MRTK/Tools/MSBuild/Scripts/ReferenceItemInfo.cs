// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.MSBuild
{
    /// <summary>
    /// A common base class for reference items such as C# Projects and DLLs to be added to MSBuild.
    /// </summary>
    public class ReferenceItemInfo
    {
        /// <summary>
        /// Gets the instance of the parsed project information.
        /// </summary>
        protected UnityProjectInfo UnityProjectInfo { get; }

        /// <summary>
        /// Gets the Guid associated with the reference.
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// Gets the output path to the reference.
        /// </summary>
        public Uri ReferencePath { get; }

        /// <summary>
        /// Gets name of the reference item.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a set of platforms supported for the InEditor configuration.
        /// </summary>
        /// <remarks>
        /// <para>In the editor, we can support all platforms if it's a pre-defined assembly, or an asmdef with Editor platform checked. 
        /// Otherwise we fallback to just the platforms specified in the editor.</para>
        /// </remarks>
        public IReadOnlyDictionary<BuildTarget, CompilationPlatformInfo> InEditorPlatforms { get; protected set; }

        /// <summary>
        /// Gets a set of platforms supported for the Player configuration.
        /// </summary>
        /// <remarks>
        /// In the player, we support any platform if pre-defined assembly, or the ones explicitly specified in the AsmDef player.
        /// </remarks>
        public IReadOnlyDictionary<BuildTarget, CompilationPlatformInfo> PlayerPlatforms { get; protected set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="unityProjectInfo">Instance of parsed unity project info.</param>
        /// <param name="guid">The unique Guid of this reference item.</param>
        /// <param name="referencePath">The output path to the reference item.</param>
        /// <param name="name">The name of the reference.</param>
        protected ReferenceItemInfo(UnityProjectInfo unityProjectInfo, Guid guid, Uri referencePath, string name)
        {
            UnityProjectInfo = unityProjectInfo;
            Guid = guid;
            ReferencePath = referencePath;
            Name = name;
        }

        /// <summary>
        /// A much more readable string representation of this reference item info.
        /// </summary>
        public override string ToString()
        {
            return $"{GetType().Name}: {Name}";
        }
    }
}
