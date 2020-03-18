// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Services
{
    /// <summary>
    /// Base class for test data providers
    /// </summary>
    public class TestBaseDataProvider : BaseDataProvider<IMixedRealityService>
    {
        public TestBaseDataProvider(
            IMixedRealityService service,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null)
        : base(service, name, priority, profile) { }

        public bool IsInitialized { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            IsInitialized = true;
        }

        public override void Reset()
        {
            base.Reset();
            Debug.Log("TestDataProvider Reset");
            IsInitialized = false;
        }
    }
}
