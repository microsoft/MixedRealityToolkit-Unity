// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.MSBuild
{
    public class CSProjectDependency<T>
    {
        public T Dependency { get; }

        public HashSet<BuildTarget> InEditorSupportedPlatforms { get; }

        public HashSet<BuildTarget> PlayerSupportedPlatforms { get; }

        public CSProjectDependency(T dependency, HashSet<BuildTarget> inEditorSupportedPlatforms, HashSet<BuildTarget> playerSupportedPlatforms)
        {
            Dependency = dependency;
            InEditorSupportedPlatforms = inEditorSupportedPlatforms;
            PlayerSupportedPlatforms = playerSupportedPlatforms;
        }
    }
}
#endif