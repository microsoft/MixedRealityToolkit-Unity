// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Services
{
    internal class TestExtensionService2 : BaseTestExtensionService, ITestExtensionService2
    {
        public TestExtensionService2(
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(name, priority, profile) { }
    }
}