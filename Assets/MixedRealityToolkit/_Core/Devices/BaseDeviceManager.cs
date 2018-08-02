// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using System.Collections.Generic;
using UnityEngine;

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
        /// <inheritdoc />
        public uint Priority { get; }

        /// <inheritdoc />
        public virtual void Initialize() { }
        /// <inheritdoc />
        public virtual void Reset() { }
        /// <inheritdoc />
        public virtual void Enable() { }
        /// <inheritdoc />
        public virtual void Update() { }
        /// <inheritdoc />
        public virtual void Disable() { }
        /// <inheritdoc />
        public virtual void Destroy() { }
        /// <inheritdoc />
        public virtual IMixedRealityController[] GetActiveControllers() => new IMixedRealityController[0];

        protected IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null && MixedRealityManager.Instance.ActiveProfile.IsInputSystemEnabled)
                {
                    inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();
                }

                return inputSystem;
            }
        }
        private IMixedRealityInputSystem inputSystem;

        protected virtual IMixedRealityPointer[] RequestPointers(SystemType controllerType, Handedness controllingHand)
        {
            var pointers = new List<IMixedRealityPointer>();

            if (MixedRealityManager.HasActiveProfile &&
                MixedRealityManager.Instance.ActiveProfile.IsInputSystemEnabled &&
                MixedRealityManager.Instance.ActiveProfile.PointerProfile != null)
            {
                for (int i = 0; i < MixedRealityManager.Instance.ActiveProfile.PointerProfile.PointerOptions.Length; i++)
                {
                    var pointerProfile = MixedRealityManager.Instance.ActiveProfile.PointerProfile.PointerOptions[i];

                    if (pointerProfile.ControllerType.Type == null ||
                        pointerProfile.ControllerType == controllerType)
                    {
                        if (pointerProfile.Handedness == Handedness.Any ||
                            pointerProfile.Handedness == Handedness.Both ||
                            pointerProfile.Handedness == controllingHand)
                        {
                            var pointerObject = UnityEngine.Object.Instantiate(pointerProfile.PointerPrefab);
                            var pointer = pointerObject.GetComponent<IMixedRealityPointer>();

                            if (pointer != null)
                            {
                                pointers.Add(pointer);
                            }
                            else
                            {
                                Debug.LogWarning($"Failed to attach {pointerProfile.PointerPrefab.name} to {controllerType.Type.Name}.");
                            }
                        }
                    }
                }
            }

            return pointers.Count == 0 ? null : pointers.ToArray();
        }
    }
}
