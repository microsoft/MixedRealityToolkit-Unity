// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Services
{
    internal class TestExtensionService3 : BaseTestExtensionService, ITestExtensionService3
    {
        public TestExtensionService3(
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(name, priority, profile) { }
    }
}