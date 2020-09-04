// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Services
{
    /// <summary>
    /// A base class for test extension services.
    /// </summary>
    internal abstract class BaseTestExtensionService : BaseExtensionService
    {
        public BaseTestExtensionService(
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(name, priority, profile) { }

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