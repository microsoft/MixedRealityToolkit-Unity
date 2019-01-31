// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Providers 
{
    /// <summary>
    /// Base Device manager to inherit from.
    /// </summary>
    public class BaseDeviceManager : BaseExtensionService, IMixedRealityDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public BaseDeviceManager(string name, uint priority, BaseMixedRealityProfile profile) : base(name, priority, profile) { }

        /// <inheritdoc />
        public virtual IMixedRealityController[] GetActiveControllers() => new IMixedRealityController[0];

        /// <summary>
        /// Request an array of pointers for the controller type.
        /// </summary>
        /// <param name="controllerType">The controller type making the request for pointers.</param>
        /// <param name="controllingHand">The handedness of the controller making the request.</param>
        /// <param name="useSpecificType">Only register pointers with a specific type.</param>
        /// <returns></returns>
        protected virtual IMixedRealityPointer[] RequestPointers(SystemType controllerType, Handedness controllingHand, bool useSpecificType = false)
        {
            var pointers = new List<IMixedRealityPointer>();

            if (MixedRealityToolkit.HasActiveProfile &&
                MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled &&
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile != null)
            {
                for (int i = 0; i < MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile.PointerOptions.Length; i++)
                {
                    var pointerProfile = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile.PointerOptions[i];

                    if (((useSpecificType && pointerProfile.ControllerType.Type == controllerType.Type) || (!useSpecificType && pointerProfile.ControllerType.Type == null)) &&
                        (pointerProfile.Handedness == Handedness.Any || pointerProfile.Handedness == Handedness.Both || pointerProfile.Handedness == controllingHand))
                    {
                        var pointerObject = Object.Instantiate(pointerProfile.PointerPrefab);
                        var pointer = pointerObject.GetComponent<IMixedRealityPointer>();
                        pointerObject.transform.SetParent(MixedRealityToolkit.Instance.MixedRealityPlayspace);

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
