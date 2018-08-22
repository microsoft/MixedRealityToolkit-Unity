// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices
{
    /// <summary>
    /// Base Device manager to inherit from.
    /// </summary>
    public class BaseDeviceManager : IMixedRealityDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
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

        /// <summary>
        /// The current Input System, if any.
        /// </summary>
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

        /// <summary>
        /// Request an array of pointers for the controller type.
        /// </summary>
        /// <param name="controllerType">The controller type making the request for pointers.</param>
        /// <param name="controllingHand">The handedness of the controller making the request.</param>
        /// <returns></returns>
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

                    if ((pointerProfile.ControllerType.Type == null || pointerProfile.ControllerType.Type == controllerType.Type) &&
                        (pointerProfile.Handedness == Handedness.Any || pointerProfile.Handedness == Handedness.Both || pointerProfile.Handedness == controllingHand))
                    {
                        var pointerObject = Object.Instantiate(pointerProfile.PointerPrefab);
                        var pointer = pointerObject.GetComponent<IMixedRealityPointer>();
                        pointerObject.transform.SetParent(CameraCache.Main.transform.parent);

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

            return pointers.Count == 0 ? null : pointers.ToArray();
        }
    }
}
