// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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