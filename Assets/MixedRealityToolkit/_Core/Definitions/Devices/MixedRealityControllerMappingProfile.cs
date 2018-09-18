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
    public class MixedRealityControllerMappingProfile : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The list of controller templates your application can use.")]
        private MixedRealityControllerMapping[] mixedRealityControllerMappingProfiles =
        {
            new MixedRealityControllerMapping(0, "Mouse Input", typeof(MouseController), Handedness.Any),
            new MixedRealityControllerMapping(1, "Touch Screen Input", typeof(UnityTouchController), Handedness.Any),
            new MixedRealityControllerMapping(2, "Xbox Controller", typeof(XboxController)),
            new MixedRealityControllerMapping(3, "Windows Mixed Reality Motion Controller Left", typeof(WindowsMixedRealityController), Handedness.Left),
            new MixedRealityControllerMapping(4, "Windows Mixed Reality Motion Controller Right", typeof(WindowsMixedRealityController), Handedness.Right),
            new MixedRealityControllerMapping(5, "Open VR Motion Controller Left", typeof(WindowsMixedRealityOpenVRMotionController), Handedness.Left),
            new MixedRealityControllerMapping(6, "Open VR Motion Controller Right", typeof(WindowsMixedRealityOpenVRMotionController), Handedness.Right),
            new MixedRealityControllerMapping(7, "Windows Mixed Reality Hand Gestures", typeof(WindowsMixedRealityController)),
            new MixedRealityControllerMapping(8, "Vive Wand Controller Left", typeof(ViveWandController), Handedness.Left),
            new MixedRealityControllerMapping(9, "Vive Wand Controller Right", typeof(ViveWandController), Handedness.Right),
            new MixedRealityControllerMapping(10, "Oculus Touch Controller Left", typeof(OculusTouchController), Handedness.Left),
            new MixedRealityControllerMapping(11, "Oculus Touch Controller Right", typeof(OculusTouchController), Handedness.Right),
            new MixedRealityControllerMapping(12, "Oculus Remote Controller", typeof(OculusRemoteController)),
            new MixedRealityControllerMapping(13, "Generic OpenVR Controller Left", typeof(GenericOpenVRController), Handedness.Left, true),
            new MixedRealityControllerMapping(14, "Generic OpenVR Controller Right", typeof(GenericOpenVRController), Handedness.Right, true),
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