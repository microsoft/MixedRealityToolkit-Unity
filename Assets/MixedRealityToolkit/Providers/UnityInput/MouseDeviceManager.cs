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
        "Unity Mouse Device Manager",
        "Profiles/DefaultMixedRealityMouseInputProfile.asset",
        "MixedRealityToolkit.SDK")]  
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
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public MouseDeviceManager(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : this(inputSystem, name, priority, profile)
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public MouseDeviceManager(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile)
        { }

        // Values defining the range of the cursor and wheel speed multipliers
        private const float MinSpeedMultiplier = 0.1f;
        private const float MaxSpeedMultiplier = 10.0f;

        /// <inheritdoc />
        public MixedRealityMouseInputProfile MouseInputProfile => ConfigurationProfile as MixedRealityMouseInputProfile;

        private float cursorSpeed = 1.0f;

        /// <inheritdoc />
        public float CursorSpeed
        {
            get => cursorSpeed;

            set
            {
                if (value != cursorSpeed)
                {
                    cursorSpeed = Mathf.Clamp(value, MinSpeedMultiplier, MaxSpeedMultiplier);
                }
            }
        }

        private float wheelSpeed = 1.0f;

        /// <inheritdoc />
        public float WheelSpeed
        {
            get => wheelSpeed;

            set
            {
                if (value != wheelSpeed)
                {
                    wheelSpeed = Mathf.Clamp(value, MinSpeedMultiplier, MaxSpeedMultiplier);
                }
            }
        }

        /// <summary>
        /// Current Mouse Controller.
        /// </summary>
        public MouseController Controller { get; private set; }

        private void ReadProfile()
        {
            MixedRealityMouseInputProfile profile = ConfigurationProfile as MixedRealityMouseInputProfile;

            CursorSpeed = profile.CursorSpeed;
            WheelSpeed = profile.WheelSpeed;
        }

        public override void Initialize()
        {
            ReadProfile();
        }

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

            IMixedRealityInputSource mouseInputSource = null;

            MixedRealityRaycaster.DebugEnabled = true;

            const Handedness handedness = Handedness.Any;
            System.Type controllerType = typeof(MouseController);

            // Make sure that the handedness declared in the controller attribute matches what we expect
            var controllerAttribute = MixedRealityControllerAttribute.Find(controllerType);
            if (controllerAttribute != null)
            {
                Handedness[] handednesses = controllerAttribute.SupportedHandedness;
                Debug.Assert(
                    handednesses.Length == 1 && handednesses[0] == Handedness.Any, 
                    "Unexpected mouse handedness declared in MixedRealityControllerAttribute");
            }

            if (InputSystem != null)
            {
                var pointers = RequestPointers(SupportedControllerType.Mouse, handedness);
                mouseInputSource = InputSystem.RequestNewGenericInputSource("Mouse Input", pointers);
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
            InputSystem?.RaiseSourceDetected(Controller.InputSource, Controller);
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
            if (Controller != null)
            {
                InputSystem?.RaiseSourceLost(Controller.InputSource, Controller);
                Controller = null;
            }
        }
    }
}
