// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Services
{
    internal class TestExtensionService3 : BaseTestExtensionService, ITestExtensionService3
    {
        public TestExtensionService3(
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base( name, priority, profile) { }
    }
}