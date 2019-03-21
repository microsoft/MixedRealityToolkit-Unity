// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace Microsoft.MixedReality.Toolkit.Tests.Services
{
    internal class TestDataProvider2 : BaseDataProvider, ITestDataProvider2
    {
        public TestDataProvider2(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityService service, 
            string name,
            uint priority) : base(registrar, service, name, priority) { }

    public bool IsEnabled { get; private set; }

        public override void Enable()
        {
            IsEnabled = true;
        }

        public override void Disable()
        {
            IsEnabled = false;
        }

        public override void Destroy()
        {
        }
    }
}