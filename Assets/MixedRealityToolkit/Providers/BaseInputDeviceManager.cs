// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Providers 
{
    /// <summary>
    /// Base input device manager to inherit from.
    /// </summary>
    public class BaseInputDeviceManager : BaseDataProvider, IMixedRealityInputDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public BaseInputDeviceManager(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name, 
            uint priority, 
            MixedRealityInputSystemProfile profile /*,
            Transform playspace*/): base(registrar, inputSystem, name, priority, profile)
        {
            if (inputSystem == null)
            {
                Debug.LogError($"The {name} data provider requires a valid input system instance.");
            }

            if (profile == null)
            {
                Debug.LogError($"The {name} data provider requires a valid input system profile.");
            }

            //if (playspace == null)
            //{
            //    Debug.LogError($"The {name} data provider requires a playspace Transform.");
            //}
            //Playspace = playspace;
        }

        /// <summary>
        /// Transform used to parent controllers and pointers so that they move correctly with the user during teleportation.
        /// </summary>
        private Transform Playspace = null;

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

            MixedRealityInputSystemProfile profile = ConfigurationProfile as MixedRealityInputSystemProfile;

            if ((Service != null) &&
                (profile != null) &&
                profile.PointerProfile != null)
            {
                for (int i = 0; i < profile.PointerProfile.PointerOptions.Length; i++)
                {
                    var pointerProfile = profile.PointerProfile.PointerOptions[i];

                    if (((useSpecificType && pointerProfile.ControllerType.Type == controllerType.Type) || (!useSpecificType && pointerProfile.ControllerType.Type == null)) &&
                        (pointerProfile.Handedness == Handedness.Any || pointerProfile.Handedness == Handedness.Both || pointerProfile.Handedness == controllingHand))
                    {
                        var pointerObject = Object.Instantiate(pointerProfile.PointerPrefab);
                        var pointer = pointerObject.GetComponent<IMixedRealityPointer>();
                        // todo: do this right.... before PR
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
