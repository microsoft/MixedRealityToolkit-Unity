// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Base input device manager to inherit from.
    /// </summary>
    public abstract class BaseInputDeviceManager : BaseDataProvider, IMixedRealityInputDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="inputSystemProfile">The input system configuration profile.</param>
        /// <param name="playspace">The <see href="https://docs.unity3d.com/ScriptReference/Transform.html">Transform</see> of the playspace object.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public BaseInputDeviceManager(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            MixedRealityInputSystemProfile inputSystemProfile,
            Transform playspace,
            string name, 
            uint priority, 
            BaseMixedRealityProfile profile): base(registrar, inputSystem, name, priority, profile)
        {
            if (inputSystem == null)
            {
                Debug.LogError($"{name} requires a valid input system instance.");
            }

            if (inputSystemProfile == null)
            {
                Debug.LogError($"{name} requires a valid input system profile.");
            }
            InputSystemProfile = inputSystemProfile;

            if (playspace == null)
            {
                Debug.LogError($"{name} requires a playspace Transform.");
            }
            Playspace = playspace;
        }

        /// <summary>
        /// The input system configuration profile in use in the application.
        /// </summary>
        protected MixedRealityInputSystemProfile InputSystemProfile = null;

        /// <summary>
        /// Transform used to parent controllers and pointers so that they move correctly with the user during teleportation.
        /// </summary>
        protected Transform Playspace = null;

        /// <inheritdoc />
        public virtual IMixedRealityController[] GetActiveControllers() => new IMixedRealityController[0];

        /// <summary>
        /// Request an array of pointers for the controller type.
        /// </summary>
        /// <param name="controllerType">The controller type making the request for pointers.</param>
        /// <param name="controllingHand">The handedness of the controller making the request.</param>
        /// <param name="useSpecificType">Only register pointers with a specific type.</param>
        /// <returns></returns>
        protected virtual IMixedRealityPointer[] RequestPointers(SupportedControllerType controllerType, Handedness controllingHand)
        {
            var pointers = new List<IMixedRealityPointer>();

            if ((Service != null) &&
                (InputSystemProfile != null) &&
                InputSystemProfile.PointerProfile != null)
            {
                for (int i = 0; i < InputSystemProfile.PointerProfile.PointerOptions.Length; i++)
                {
                    var pointerProfile = InputSystemProfile.PointerProfile.PointerOptions[i];

                    if (((pointerProfile.ControllerType & controllerType) != 0) &&
                        (pointerProfile.Handedness == Handedness.Any || pointerProfile.Handedness == Handedness.Both || pointerProfile.Handedness == controllingHand))
                    {
                        var pointerObject = Object.Instantiate(pointerProfile.PointerPrefab, Playspace);
                        var pointer = pointerObject.GetComponent<IMixedRealityPointer>();
                        pointerObject.transform.SetParent(Playspace);

                        if (pointer != null)
                        {
                            pointers.Add(pointer);
                        }
                        else
                        {
                            Debug.LogWarning($"Failed to attach {pointerProfile.PointerPrefab.name} to {controllerType}.");
                        }
                    }
                }
            }

            return pointers.Count == 0 ? null : pointers.ToArray();
        }
    }
}
