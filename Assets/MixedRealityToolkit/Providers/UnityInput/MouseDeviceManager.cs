// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace Microsoft.MixedReality.Toolkit.Input.UnityInput
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        (SupportedPlatforms)(-1), // All platforms supported by Unity
        "Unity Mouse Device Manager")]  
    public class MouseDeviceManager : BaseInputDeviceManager, IMixedRealityMouseDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public MouseDeviceManager(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(registrar, inputSystem, name, priority, profile) { }

        /// <summary>
        /// Current Mouse Controller.
        /// </summary>
        public MouseController Controller { get; private set; }

        /// <summary>
        /// Return the service profile and ensure that the type is correct
        /// </summary>
        public MixedRealityMouseInputProfile MouseInputProfile => ConfigurationProfile as MixedRealityMouseInputProfile;

        /// <inheritdoc />
        public override void Enable()
        {
            if (!UInput.mousePresent)
            {
                Disable();
                return;
            }

            if (Controller != null)
            {
                // device manager has already been set up
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

            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;

            if (inputSystem != null)
            {
                var pointers = RequestPointers(SupportedControllerType.Mouse, handedness);
                mouseInputSource = inputSystem.RequestNewGenericInputSource("Mouse Input", pointers);
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
            inputSystem?.RaiseSourceDetected(Controller.InputSource, Controller);
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (UInput.mousePresent && Controller == null) { Enable(); }

            Controller?.Update();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
            if (Controller != null)
            {
                inputSystem?.RaiseSourceLost(Controller.InputSource, Controller);
                Controller = null;
            }
        }

    }
}
