// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine.XR.WSA.Input;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality
{
    // TODO - Only scaffold at present
    public struct WindowsMixedRealityGestureController : IMixedRealityController
    {
        #region Private properties

        #endregion Private properties

        public WindowsMixedRealityGestureController(ControllerState controllerState, Handedness controllerHandedness, IMixedRealityInputSource inputSource, Dictionary<DeviceInputType, IMixedRealityInteractionMapping> interactions = null) : this()
        {
            ControllerState = controllerState;
            ControllerHandedness = controllerHandedness;
            InputSource = inputSource;
            Interactions = interactions ?? new Dictionary<DeviceInputType, IMixedRealityInteractionMapping>();
        }

        #region IMixedRealityController Interface Members

        /// <inheritdoc/>
        public ControllerState ControllerState { get; private set; }

        /// <inheritdoc/>
        public Handedness ControllerHandedness { get; }

        /// <inheritdoc/>
        public IMixedRealityInputSource InputSource { get; private set; }

        /// <inheritdoc/>
        public Dictionary<DeviceInputType, IMixedRealityInteractionMapping> Interactions { get; private set; }

        /// <inheritdoc/>
        public void SetupInputSource<T>(T state)
        {
            InteractionSourceState interactionSourceState = CheckIfValidInteractionSourceState(state);
            SetupFromInteractionSource(interactionSourceState);
        }

        /// <inheritdoc/>
        public void UpdateInputSource<T>(T state)
        {
            InteractionSourceState interactionSourceState = CheckIfValidInteractionSourceState(state);
            UpdateFromInteractionSource(interactionSourceState);
        }

        #endregion IMixedRealityInputSource Interface Members

        #region Setup and Update functions

        /// <summary>
        /// Read the Interaction Source State information and initialize an instance of a Windows Mixed Reality controller
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        public void SetupFromInteractionSource(InteractionSourceState interactionSourceState)
        {
            //Update the Tracked state of the controller
            UpdateControllerData(interactionSourceState);

            // TODO Addressed in next PR
            //var controllerMapping = Managers.MixedRealityManager.Instance.ActiveProfile.GetControllerMapping(typeof(WindowsMixedRealityController),ControllerHandedness);
            ////MixedRealityControllerMappingProfile controllerMapping = Managers.MixedRealityManager.Instance.ActiveProfile.GetControllerMapping<WindowsMixedRealityController>(ControllerHandedness);
            //if (controllerMapping.Interactions?.Length > 0)
            //{
            //    SetupFromMapping(controllerMapping.Interactions);
            //}
            //else
            //{
            //    SetupWMRControllerDefaults(interactionSourceState);
            //}
        }

        /// <summary>
        /// Load the Interaction mappings for this controller from the configured Controller Mapping profile
        /// </summary>
        /// <param name="mappings">Configured mappings from a controller mapping profile</param>
        private void SetupFromMapping(MixedRealityInteractionMapping[] mappings)
        {
            for (uint i = 0; i < mappings.Length; i++)
            {
                // Add interaction for Mapping
                Interactions.Add(mappings[i].InputType, new MixedRealityInteractionMapping(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].InputAction));
            }
        }

        /// <summary>
        /// Create Interaction mappings from a device specific default set of action mappings
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void SetupWMRControllerDefaults(InteractionSourceState interactionSourceState)
        {
            InputAction[] inputActions = Managers.MixedRealityManager.Instance.ActiveProfile.InputActionsProfile.InputActions;
            if (inputActions == null)
            {
                return;
            }
            
            // TODO - add setup for Gesture
        }

        /// <summary>
        /// Update the controller data from the provided platform state
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        public void UpdateFromInteractionSource(InteractionSourceState interactionSourceState)
        {
            UpdateControllerData(interactionSourceState);

            // TODO - add defaults for Gesture
        }

        #region Update data functions

        /// <summary>
        /// Update the "Controller" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdateControllerData(InteractionSourceState interactionSourceState)
        {
            // TODO - add update for Gesture
        }

        #endregion Update data functions

        #endregion Setup and Update functions

        #region Utilities
        private static InteractionSourceState CheckIfValidInteractionSourceState<T>(T state)
        {
            InteractionSourceState interactionSourceState = (InteractionSourceState)Convert.ChangeType(state, typeof(InteractionSourceState));
            if (interactionSourceState.source.id == 0) { throw new ArgumentOutOfRangeException(nameof(state), "Incorrect state type provided to controller,did you send an InteractionSourceState?"); }

            return interactionSourceState;
        }
        #endregion Utilities
    }
}