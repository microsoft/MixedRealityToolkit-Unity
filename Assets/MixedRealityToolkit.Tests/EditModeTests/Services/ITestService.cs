// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace Microsoft.MixedReality.Toolkit.Tests.Services
{
    internal interface ITestService : IMixedRealityService
    {
        bool IsEnabled { get; }
    }
}