// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;

#if WINDOWS_UWP
using System.Collections.Generic;
using Windows.Gaming.Input;
#endif // WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.GamePads
{
    public class WindowsGamingDeviceManager : BaseManager
    {
        private IMixedRealityInputSystem inputSystem = null;
        public IMixedRealityInputSystem InputSystem => inputSystem ?? (inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>());

#if WINDOWS_UWP

        private readonly Dictionary<IMixedRealityInputSource, Gamepad> gamepadInputSources = new Dictionary<IMixedRealityInputSource, Gamepad>();

        private readonly Dictionary<IMixedRealityInputSource, ArcadeStick> arcadeStickInputSources = new Dictionary<IMixedRealityInputSource, ArcadeStick>();

        public override void Initialize()
        {
            Gamepad.GamepadAdded += OnGamepadDetected;
            Gamepad.GamepadRemoved += OnGamepadLost;

            ArcadeStick.ArcadeStickAdded += OnArcadeStickDetected;
            ArcadeStick.ArcadeStickRemoved += OnArcadeStickLost;

            FlightStick.FlightStickAdded += OnFlightStickDetected;
            FlightStick.FlightStickRemoved += OnFlightStickLost;

            RacingWheel.RacingWheelAdded += OnRacingWheelDetected;
            RacingWheel.RacingWheelRemoved += OnRacingWheelLost;
        }

        private void OnGamepadDetected(object sender, Gamepad gamepad)
        {
            var gamepadInputSource = InputSystem.RequestNewGenericInputSource($"Gamepad_{gamepadInputSources.Count + 1}");
            gamepadInputSources.Add(gamepadInputSource, gamepad);
        }

        private void OnGamepadLost(object sender, Gamepad gamepad)
        {
            foreach (var inputSource in gamepadInputSources)
            {
                if (inputSource.Value == gamepad)
                {
                    gamepadInputSources.Remove(inputSource.Key);
                    break;
                }
            }
        }

        private void OnArcadeStickDetected(object sender, ArcadeStick arcadeStick)
        {
            var arcadeStickInputSource = InputSystem.RequestNewGenericInputSource($"ArcadeStick_{gamepadInputSources.Count + 1}");
            arcadeStickInputSources.Add(arcadeStickInputSource, arcadeStick);
        }

        private void OnArcadeStickLost(object sender, ArcadeStick arcadeStick)
        {
            foreach (var inputSource in arcadeStickInputSources)
            {
                if (inputSource.Value == arcadeStick)
                {
                    arcadeStickInputSources.Remove(inputSource.Key);
                    break;
                }
            }
        }

        private void OnFlightStickDetected(object sender, FlightStick flightStick)
        {
        }

        private void OnFlightStickLost(object sender, FlightStick flightStick)
        {
        }

        private void OnRacingWheelDetected(object sender, RacingWheel racingWheel)
        {
        }

        private void OnRacingWheelLost(object sender, RacingWheel racingWheel)
        {
        }

#endif // WINDOWS_UWP
    }
}
