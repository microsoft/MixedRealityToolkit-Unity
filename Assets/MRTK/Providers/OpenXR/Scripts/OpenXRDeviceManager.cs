// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using System;
using Unity.Profiling;
using UnityEngine.XR;

#if UNITY_OPENXR
using UnityEngine.XR.OpenXR;
#endif // UNITY_OPENXR

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        (SupportedPlatforms)(-1),
        "OpenXR XRSDK Device Manager")]
    public class OpenXRDeviceManager : XRSDKDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public OpenXRDeviceManager(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile) { }

        private bool? isActiveLoader = null;
        private bool IsActiveLoader
        {
            get
            {
#if UNITY_OPENXR
                if (!isActiveLoader.HasValue)
                {
                    isActiveLoader = IsLoaderActive<OpenXRLoader>();
                }
#endif // UNITY_OPENXR

                return isActiveLoader ?? false;
            }
        }

        /// <inheritdoc />
        public override void Enable()
        {
            if (!IsActiveLoader)
            {
                IsEnabled = false;
                return;
            }

            base.Enable();
        }

        #region Controller Utilities

        private static readonly ProfilerMarker GetOrAddControllerPerfMarker = new ProfilerMarker("[MRTK] OpenXRDeviceManager.GetOrAddController");

        /// <summary>
        /// The OpenXR plug-in uses extensions to expose all possible data, which might be surfaced through multiple input devices.
        /// This method is overridden to account for multiple input devices and reuse MRTK controllers if a match is found.
        /// </summary>
        protected override GenericXRSDKController GetOrAddController(InputDevice inputDevice)
        {
            using (GetOrAddControllerPerfMarker.Auto())
            {
                // If this is a new input device, search if an existing input device has matching characteristics
                if (!ActiveControllers.ContainsKey(inputDevice))
                {
                    foreach (InputDevice device in ActiveControllers.Keys)
                    {
                        if (((device.characteristics.HasFlag(InputDeviceCharacteristics.Controller) && inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
                            || (device.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking) && inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking)))
                            && ((device.characteristics.HasFlag(InputDeviceCharacteristics.Left) && inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Left))
                            || (device.characteristics.HasFlag(InputDeviceCharacteristics.Right) && inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Right))))
                        {
                            ActiveControllers.Add(inputDevice, ActiveControllers[device]);
                            break;
                        }
                    }
                }

                if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking)
                    && inputDevice.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked)
                    && !isTracked)
                {
                    // If this is an input device from the Microsoft Hand Interaction profile, it doesn't go invalid but instead goes untracked. Ignore it if untracked.
                    return null;
                }

                return base.GetOrAddController(inputDevice);
            }
        }

        private static readonly ProfilerMarker RemoveControllerPerfMarker = new ProfilerMarker("[MRTK] OpenXRDeviceManager.RemoveController");

        /// <summary>
        /// The OpenXR plug-in uses extensions to expose all possible data, which might be surfaced through multiple input devices.
        /// This method is overridden to account for multiple input devices and reuse MRTK controllers if a match is found.
        /// </summary>
        protected override void RemoveController(InputDevice inputDevice)
        {
            using (RemoveControllerPerfMarker.Auto())
            {
                foreach (InputDevice device in ActiveControllers.Keys)
                {
                    if (device != inputDevice
                        && ((device.characteristics.HasFlag(InputDeviceCharacteristics.Controller) && inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
                        || (device.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking) && inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking)))
                        && ((device.characteristics.HasFlag(InputDeviceCharacteristics.Left) && inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Left))
                        || (device.characteristics.HasFlag(InputDeviceCharacteristics.Right) && inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Right))))
                    {
                        ActiveControllers.Remove(inputDevice);
                        // Since an additional device exists, return so a lost source isn't reported
                        return;
                    }
                }

                base.RemoveController(inputDevice);
            }
        }

        /// <inheritdoc />
        protected override Type GetControllerType(SupportedControllerType supportedControllerType)
        {
            switch (supportedControllerType)
            {
                case SupportedControllerType.WindowsMixedReality:
                    return typeof(MicrosoftMotionController);
                case SupportedControllerType.HPMotionController:
                    return typeof(HPReverbG2Controller);
                case SupportedControllerType.ArticulatedHand:
                    return typeof(MicrosoftArticulatedHand);
                default:
                    return base.GetControllerType(supportedControllerType);
            }
        }

        /// <inheritdoc />
        protected override InputSourceType GetInputSourceType(SupportedControllerType supportedControllerType)
        {
            switch (supportedControllerType)
            {
                case SupportedControllerType.WindowsMixedReality:
                case SupportedControllerType.HPMotionController:
                    return InputSourceType.Controller;
                case SupportedControllerType.ArticulatedHand:
                    return InputSourceType.Hand;
                default:
                    return base.GetInputSourceType(supportedControllerType);
            }
        }

        /// <inheritdoc />
        protected override SupportedControllerType GetCurrentControllerType(InputDevice inputDevice)
        {
            if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking))
            {
                return SupportedControllerType.ArticulatedHand;
            }

            if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
            {
                if (inputDevice.manufacturer == "HP")
                {
                    return SupportedControllerType.HPMotionController;
                }
                else // Fall back to the base WMR controller
                {
                    return SupportedControllerType.WindowsMixedReality;
                }
            }

            return base.GetCurrentControllerType(inputDevice);
        }

        #endregion Controller Utilities
    }
}
