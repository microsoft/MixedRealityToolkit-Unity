// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices
{
    public class BaseController : IMixedRealityController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controllerState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public BaseController(ControllerState controllerState,
                              Handedness controllerHandedness,
                              IMixedRealityInputSource inputSource = null,
                              List<IInteractionMapping> interactions = null)
        {
            ControllerState = controllerState;
            ControllerHandedness = controllerHandedness;
            InputSource = inputSource;
            Interactions = interactions ?? new List<IInteractionMapping>();
        }

        /// <inheritdoc />
        public ControllerState ControllerState { get; protected set; }

        /// <inheritdoc />
        public Handedness ControllerHandedness { get; }

        /// <inheritdoc />
        public IMixedRealityInputSource InputSource { get; }

        /// <inheritdoc />
        public List<IInteractionMapping> Interactions { get; }

        /// <summary>
        /// Returns the current Input System if enabled, otherwise null.
        /// </summary>
        protected IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null && MixedRealityManager.Instance.ActiveProfile.EnableInputSystem)
                {
                    inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();
                }

                return inputSystem;
            }
        }

        private IMixedRealityInputSystem inputSystem;
    }
}
