// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Controllers
{
    /// <summary>
    /// The Mixed Reality Visualization component is primarily responsible for synchronizing the user's current input with controller models.
    /// </summary>
    /// <seealso cref="Core.Definitions.Devices.MixedRealityControllerMappingProfile"/>
    public class MixedRealityControllerVisualizer : ControllerPoseSynchronizer, IMixedRealityControllerVisualizer
    {
        // TODO wire up input actions to controller transform nodes / animations

        /// <inheritdoc />
        public GameObject GameObjectProxy => gameObject;

        #region IMixedRealityInputHandler Implementation

        /// <summary>
        /// Visualize digital and single axis controls down state on the controller model, if supported
        /// </summary>
        /// <remarks>
        /// Reserved for future implementation
        /// </remarks>
        /// <param name="eventData"></param>
        public override void OnInputDown(InputEventData eventData)
        {
            base.OnInputDown(eventData);
            // TODO Visualize digital and single axis controls down state
        }

        /// <summary>
        /// Visualize digital and single axis controls up state on the controller model, if supported
        /// </summary>
        /// <remarks>
        /// Reserved for future implementation
        /// </remarks>
        /// <param name="eventData"></param>
        public override void OnInputUp(InputEventData eventData)
        {
            base.OnInputUp(eventData);
            // TODO Visualize digital and single axis controls up state
        }

        /// <summary>
        /// Visualize single axis controls on the controller model, if supported
        /// </summary>
        /// <remarks>
        /// Reserved for future implementation
        /// </remarks>
        /// <param name="eventData"></param>
        public override void OnInputChanged(InputEventData<float> eventData)
        {
            base.OnInputChanged(eventData);
            // TODO Visualize single axis controls
        }

        /// <summary>
        /// Visualize the movement of a dual axis input on the controller model, if supported
        /// </summary>
        /// <remarks>
        /// Reserved for future implementation
        /// </remarks>
        /// <param name="eventData"></param>
        public override void OnInputChanged(InputEventData<Vector2> eventData)
        {
            base.OnInputChanged(eventData);
            // TODO Visualize dual axis controls
        }

        #endregion IMixedRealityInputHandler Implementation
    }
}