// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Tests.Services
{
    internal class TestDataProvider1 : TestBaseDataProvider, ITestDataProvider1
    {
        [System.Obsolete()]
        public TestDataProvider1(
            IMixedRealityServiceRegistrar registrar, 
            IMixedRealityService service, 
            string name, 
            uint priority) : this(service, name, priority)
        {
            Registrar = registrar;
        }

        public TestDataProvider1(
            IMixedRealityService service,
            string name,
            uint priority) : base(service, name, priority) { }
    }
}