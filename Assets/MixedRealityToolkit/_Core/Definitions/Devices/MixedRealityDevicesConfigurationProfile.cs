// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Devices Configuration Profile", fileName = "MixedRealityDevicesConfigurationProfile", order = 2)]
    public class MixedRealityDevicesConfigurationProfile : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The list of devices templates your application can use.")]
        private DeviceTemplate[] deviceTemplates =
        {
            new DeviceTemplate(1,"Windows Spatial Controller - Left", SDKType.WindowsMR, Handedness.Left, new []
            {
                new InteractionMapping(1, AxisType.SixDoF, DeviceInputType.PointerPosition, new InputAction(1, "Pointer Position", AxisType.SixDoF)),
            }),
        };

        public DeviceTemplate[] DeviceTemplates => deviceTemplates;
    }
}