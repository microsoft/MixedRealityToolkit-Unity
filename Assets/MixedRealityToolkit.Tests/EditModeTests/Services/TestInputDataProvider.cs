// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Runtime.InteropServices;

namespace Microsoft.MixedReality.Toolkit.Tests.Services
{
    /// <summary>
    /// Dummy test IMixedRealityInputDeviceManager implementation only used for testing
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        (SupportedPlatforms)(-1), // All platforms supported by Unity
        "Test Input Data Provider")]
    public class TestInputDataProvider : TestBaseDataProvider, ITestInputDataProvider
    {
        [System.Obsolete()]
        public TestInputDataProvider(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : this(inputSystem, name, priority, profile) 
        {
            Registrar = registrar;
        }

        public TestInputDataProvider(
            IMixedRealityInputSystem inputSystem,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(inputSystem, name, priority, profile) { }

        public IMixedRealityController[] GetActiveControllers()
        {
            return null;
        }
    }
}