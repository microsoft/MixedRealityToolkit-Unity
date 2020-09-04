// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Services
{
    internal interface ITestService : IMixedRealityService
    {
        bool IsEnabled { get; }
    }
}