// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    public enum ConfigurationStage
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
