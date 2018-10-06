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
        /// Visualize the pressed button in the controller model, if supported
        /// </summary>
        /// <remarks>
        /// Reserved for future implementation
        /// </remarks>
        /// <param name="eventData"></param>
        public override void OnInputDown(InputEventData eventData)
        {
            base.OnInputDown(eventData);
            // TODO Visualize button down
        }

        /// <summary>
        /// Visualize the released button in the controller model, if supported
        /// </summary>
        /// <remarks>
        /// Reserved for future implementation
        /// </remarks>
        /// <param name="eventData"></param>
        public override void OnInputUp(InputEventData eventData)
        {
            base.OnInputUp(eventData);
            // TODO Visualize button up
        }

        /// <summary>
        /// Visualize the held trigger in the controller model, if supported
        /// </summary>
        /// <remarks>
        /// Reserved for future implementation
        /// </remarks>
        /// <param name="eventData"></param>
        public override void OnInputPressed(InputEventData<float> eventData)
        {
            base.OnInputPressed(eventData);
            // TODO Visualize single axis controls
        }

        /// <summary>
        /// Visualize the movement of a dual axis input in the controller model, if supported
        /// </summary>
        /// <remarks>
        /// Reserved for future implementation
        /// </remarks>
        /// <param name="eventData"></param>
        public override void OnPositionInputChanged(InputEventData<Vector2> eventData)
        {
            base.OnPositionInputChanged(eventData);
            // TODO Visualize dual axis controls
        }

        #endregion IMixedRealityInputHandler Implementation
    }
}