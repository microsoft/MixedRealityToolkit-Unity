using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.SDK.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.InputSystem
{
    public static class InputSystemTestUtilities
    {
        public static MixedRealityInputSystemProfile SetupInputSystemProfile()
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
            inputSystemProfile.ControllerMappingProfile = ScriptableObject.CreateInstance<MixedRealityControllerMappingProfile>();

            return inputSystemProfile;
        }
    }
}