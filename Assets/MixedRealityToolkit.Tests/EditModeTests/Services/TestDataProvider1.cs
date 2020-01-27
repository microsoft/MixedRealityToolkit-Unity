// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Services
{
    internal class TestDataProvider1 : TestBaseDataProvider, ITestDataProvider1
    {
        public TestDataProvider1(
            IMixedRealityService service,
            string name,
            uint priority) : base(service, name, priority) { }
    }
}