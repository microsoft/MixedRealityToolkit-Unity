// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.DataProviders.Controllers;
using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.SDK.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.InputSystem
{
    public static class InputSystemTestUtilities
    {
        public static MixedRealityInputSystemProfile CreateInputSystemProfile()
        {
            // Create a Input System Profiles
            var inputSystemProfile = ScriptableObject.CreateInstance<MixedRealityInputSystemProfile>();
            inputSystemProfile.FocusProviderType = typeof(FocusProvider);
            inputSystemProfile.InputActionsProfile = ScriptableObject.CreateInstance<MixedRealityInputActionsProfile>();
            inputSystemProfile.InputActionRulesProfile = ScriptableObject.CreateInstance<MixedRealityInputActionRulesProfile>();
            inputSystemProfile.PointerProfile = ScriptableObject.CreateInstance<MixedRealityPointerProfile>();
            inputSystemProfile.PointerProfile.GazeProviderType = typeof(GazeProvider);
            inputSystemProfile.GesturesProfile = ScriptableObject.CreateInstance<MixedRealityGesturesProfile>();
            inputSystemProfile.SpeechCommandsProfile = ScriptableObject.CreateInstance<MixedRealitySpeechCommandsProfile>();
            inputSystemProfile.ControllerVisualizationProfile = ScriptableObject.CreateInstance<MixedRealityControllerVisualizationProfile>();
            inputSystemProfile.ControllerMappingProfiles = ScriptableObject.CreateInstance<MixedRealityControllerMappingProfiles>();
            return inputSystemProfile;
        }
    }
}