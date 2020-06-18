// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Tools.Build
{
    /// <summary>
    /// A representation of a Unity assembly definition file.
    /// The structure of this data is defined by the JSON file format here:
    /// https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html
    /// </summary>
    public struct AssemblyDefinition
    {
        public string name;
        public string[] references;
        public string[] optionalUnityReferences;
        public string[] includePlatforms;
        public string[] excludePlatforms;
        public bool allowUnsafeCode;
        public bool overrideReferences;
        public string[] precompiledReferences;
        public bool autoReferenced;
        public string[] defineConstraints;
    }
}