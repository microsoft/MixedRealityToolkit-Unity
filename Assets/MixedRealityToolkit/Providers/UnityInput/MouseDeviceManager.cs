// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Attributes;
using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Providers.UnityInput
{
    [MixedRealityDataProvider(
        typeof(Interfaces.InputSystem.IMixedRealityInputSystem),
        (SupportedPlatforms)(-1))]  // All platforms supported by Unity
    public class MouseDeviceManager : BaseDeviceManager, IMixedRealityExtensionService
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public MouseDeviceManager(string name, uint priority, BaseMixedRealityProfile profile) : base(name, priority, profile) { }

        /// <summary>
        /// Current Mouse Controller.
        /// </summary>
        public MouseController Controller { get; private set; }

        /// <inheritdoc />
        public override void Enable()
        {
            if (!Input.mousePresent)
            {
                Disable();
                return;
            }

#if UNITY_EDITOR
            if (UnityEditor.EditorWindow.focusedWindow != null)
            {
                UnityEditor.EditorWindow.focusedWindow.ShowNotification(new GUIContent("Press \"ESC\" to regain mouse control"));
            }
#endif

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            IMixedRealityInputSource mouseInputSource = null;

            MixedRealityRaycaster.DebugEnabled = true;

            const Handedness handedness = Handedness.Any;
            System.Type controllerType = typeof(MouseController);

            // Make sure that the handedness declared in the controller attribute matches what we expect
            {
                var controllerAttribute = MixedRealityControllerAttribute.Find(controllerType);
                if (controllerAttribute != null)
                {
                    Handedness[] handednesses = controllerAttribute.SupportedHandedness;
                    Debug.Assert(handednesses.Length == 1 && handednesses[0] == Handedness.Any, "Unexpected mouse handedness declared in MixedRealityControllerAttribute");
                }
            }

            if (MixedRealityToolkit.InputSystem != null)
            {
                var pointers = RequestPointers(new SystemType(controllerType), handedness, true);
                mouseInputSource = MixedRealityToolkit.InputSystem.RequestNewGenericInputSource("Mouse Input", pointers);
            }

            Controller = new MouseController(TrackingState.NotApplicable, handedness, mouseInputSource);

            if (mouseInputSource != null)
            {
                for (int i = 0; i < mouseInputSource.Pointers.Length; i++)
                {
                    mouseInputSource.Pointers[i].Controller = Controller;
                }
            }

            Controller.SetupConfiguration(typeof(MouseController));
            MixedRealityToolkit.InputSystem?.RaiseSourceDetected(Controller.InputSource, Controller);
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (Input.mousePresent && Controller == null) { Enable(); }

            Controller?.Update();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            if (Controller != null)
            {
                MixedRealityToolkit.InputSystem?.RaiseSourceLost(Controller.InputSource, Controller);
            }
        }
    }
}
