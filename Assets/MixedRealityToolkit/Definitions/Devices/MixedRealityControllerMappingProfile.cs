// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Devices.OpenVR;
using Microsoft.MixedReality.Toolkit.Core.Devices.UnityInput;
using Microsoft.MixedReality.Toolkit.Core.Devices.WindowsMixedReality;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Devices
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Controller Mapping Profile", fileName = "MixedRealityControllerMappingProfile", order = (int)CreateProfileMenuItemIndices.ControllerMapping)]
    public class MixedRealityControllerMappingProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("The list of controller templates your application can use.")]
        private MixedRealityControllerMapping[] mixedRealityControllerMappingProfiles =
        {
            new MixedRealityControllerMapping("Mouse Input", typeof(MouseController), Handedness.Any),
            new MixedRealityControllerMapping("Touch Screen Input", typeof(UnityTouchController), Handedness.Any),
            new MixedRealityControllerMapping("Xbox Controller", typeof(XboxController)),
            new MixedRealityControllerMapping("Windows Mixed Reality Motion Controller Left", typeof(WindowsMixedRealityController), Handedness.Left),
            new MixedRealityControllerMapping("Windows Mixed Reality Motion Controller Right", typeof(WindowsMixedRealityController), Handedness.Right),
            new MixedRealityControllerMapping("Open VR Motion Controller Left", typeof(WindowsMixedRealityOpenVRMotionController), Handedness.Left),
            new MixedRealityControllerMapping("Open VR Motion Controller Right", typeof(WindowsMixedRealityOpenVRMotionController), Handedness.Right),
            new MixedRealityControllerMapping("Windows Mixed Reality Hand Gestures", typeof(WindowsMixedRealityController)),
            new MixedRealityControllerMapping("Vive Wand Controller Left", typeof(ViveWandController), Handedness.Left),
            new MixedRealityControllerMapping("Vive Wand Controller Right", typeof(ViveWandController), Handedness.Right),
            new MixedRealityControllerMapping("Oculus Touch Controller Left", typeof(OculusTouchController), Handedness.Left),
            new MixedRealityControllerMapping("Oculus Touch Controller Right", typeof(OculusTouchController), Handedness.Right),
            new MixedRealityControllerMapping("Oculus Remote Controller", typeof(OculusRemoteController)),
            new MixedRealityControllerMapping("Generic OpenVR Controller Left", typeof(GenericOpenVRController), Handedness.Left, true),
            new MixedRealityControllerMapping("Generic OpenVR Controller Right", typeof(GenericOpenVRController), Handedness.Right, true),
        };

        public MixedRealityControllerMapping[] MixedRealityControllerMappingProfiles => mixedRealityControllerMappingProfiles;


        private void Awake()
        {
            for (int i = 0; i < mixedRealityControllerMappingProfiles.Length; i++)
            {
                mixedRealityControllerMappingProfiles[i].SetDefaultInteractionMapping();
            }
        }
    }
}