using System.Collections;
using System.Collections.Generic;
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

        public override void Enable()
        {
            base.Enable();

            IsEnabled = true;
        }

        public override void Disable()
        {
            base.Disable();

            IsEnabled = false;
        }

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}
