// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Editor.Inspectors")]
namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// List of the stages of the project configurator
    /// </summary>
    internal enum ConfigurationStage
    {
        Init = 0,
        SelectXRSDKPlugin = 100,
        InstallOpenXR = 101,
        InstallMSOpenXR = 102,
        InstallBuiltinPlugin = 150,
        ProjectConfiguration = 200,
        ImportTMP = 300,
        ShowExamples = 400,
        Done = 500
    };
}
