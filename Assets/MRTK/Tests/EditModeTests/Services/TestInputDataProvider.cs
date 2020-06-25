// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Services
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