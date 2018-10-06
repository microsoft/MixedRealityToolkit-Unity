// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using System.Collections.Generic;
using UnityEngine;

#if PLATFORM_LUMIN
using UnityEngine.XR.MagicLeap;
#endif

namespace Microsoft.MixedReality.Toolkit.Core.Devices.Lumin
{
    public class LuminDeviceManager : BaseDeviceManager, IMixedRealityComponent
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public LuminDeviceManager(string name, uint priority) : base(name, priority) { }

#if PLATFORM_LUMIN

        /// <summary>
        /// Dictionary to capture all active controllers detected
        /// </summary>
        private readonly Dictionary<byte, LuminController> activeControllers = new Dictionary<byte, LuminController>();

        /// <inheritdoc/>
        public override IMixedRealityController[] GetActiveControllers()
        {
            var list = new List<IMixedRealityController>();

            foreach (LuminController value in activeControllers.Values)
            {
                list.Add(value);
            }

            return list.ToArray();
        }

        public override void Enable()
        {
            var config = new MLInputConfiguration();
            var result = MLInput.Start(config);
            if (!result.IsOk)
            {
                Debug.LogError($"Error: ControllerConnectionHandler failed starting MLInput: {result}");
            }

            MLInput.OnControllerConnected += OnControllerConnected;
            MLInput.OnControllerDisconnected += OnControllerDisconnected;
        }

        public override void Update()
        {
            foreach (var controller in activeControllers)
            {
                controller.Value?.UpdateController();
            }
        }

        public override void Disable()
        {
            MLInput.OnControllerConnected -= OnControllerConnected;
            MLInput.OnControllerDisconnected -= OnControllerDisconnected;
            MLInput.Stop();
        }

        private LuminController GetController(byte controllerId, bool addController = true)
        {
            //If a device is already registered with the ID provided, just return it.
            if (activeControllers.ContainsKey(controllerId))
            {
                var controller = activeControllers[controllerId];
                Debug.Assert(controller != null);
                return controller;
            }

            if (!addController) { return null; }

            var mlController = MLInput.GetController(controllerId);

            if (mlController.Type == MLInputControllerType.None) { return null; }

            var controllingHand = Handedness.Any;

            if (mlController.Type == MLInputControllerType.Control)
            {
                switch (mlController.Hand)
                {
                    case MLInput.Hand.Left:
                        controllingHand = Handedness.Left;
                        break;
                    case MLInput.Hand.Right:
                        controllingHand = Handedness.Right;
                        break;
                }
            }

            var pointers = mlController.Type == MLInputControllerType.Control ? RequestPointers(typeof(LuminController), controllingHand) : null;
            var inputSource = MixedRealityManager.InputSystem?.RequestNewGenericInputSource($"Lumin Controller {controllingHand}", pointers);
            var detectedController = new LuminController(TrackingState.NotTracked, controllingHand, inputSource);

            if (!detectedController.SetupConfiguration(typeof(LuminController)))
            {
                // Controller failed to be setup correctly.
                // Return null so we don't raise the source detected.
                return null;
            }

            for (int i = 0; i < detectedController.InputSource?.Pointers?.Length; i++)
            {
                detectedController.InputSource.Pointers[i].Controller = detectedController;
            }

            detectedController.MlControllerReference = mlController;
            activeControllers.Add(controllerId, detectedController);
            return detectedController;
        }

        #region Controller Events

        private void OnControllerConnected(byte controllerId)
        {
            var controller = GetController(controllerId);

            if (controller != null)
            {
                MixedRealityManager.InputSystem?.RaiseSourceDetected(controller.InputSource, controller);
            }

            controller?.UpdateController();
        }

        private void OnControllerDisconnected(byte controllerId)
        {
            var controller = GetController(controllerId, false);

            if (controller != null)
            {
                MixedRealityManager.InputSystem?.RaiseSourceLost(controller.InputSource, controller);
            }

            activeControllers.Remove(controllerId);
        }

        #endregion Controller Events

#endif // PLATFORM_LUMIN
    }
}
