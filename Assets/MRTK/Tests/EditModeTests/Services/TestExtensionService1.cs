// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Services
{
    internal class TestExtensionService1 : BaseTestExtensionService, ITestExtensionService1
    {
        public TestExtensionService1(
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(name, priority, profile) { }
    }
}