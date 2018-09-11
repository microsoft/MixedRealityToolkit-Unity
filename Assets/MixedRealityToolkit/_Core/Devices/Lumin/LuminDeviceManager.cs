// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces;

namespace Microsoft.MixedReality.Toolkit.Core.Devices.Lumin
{
    public class LuminDeviceManager : BaseDeviceManager, IMixedRealityComponent
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public LuminDeviceManager(string name, uint priority) : base(name, priority) { }

#if PLATFORM_LUMIN

        public override void Enable()
        {
        }

        public override void Disable()
        {
        }

#endif // PLATFORM_LUMIN
    }
}
