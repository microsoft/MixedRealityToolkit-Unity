// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices
{
    public class BaseDeviceManager : IMixedRealityDeviceManager
    {
        public BaseDeviceManager(string name, uint priority)
        {
            Name = name;
            Priority = priority;
        }

        public string Name { get; }
        public uint Priority { get; }

        public virtual void Initialize() { }
        public virtual void Reset() { }
        public virtual void Enable() { }
        public virtual void Update() { }
        public virtual void Disable() { }
        public virtual void Destroy() { }
        public virtual IMixedRealityController[] GetActiveControllers() => new IMixedRealityController[0];

        protected IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null && MixedRealityManager.Instance.ActiveProfile.EnableInputSystem)
                {
                    inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();
                }

                return inputSystem;
            }
        }
        private IMixedRealityInputSystem inputSystem;
    }
}
