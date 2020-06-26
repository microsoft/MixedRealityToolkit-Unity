// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// The Mixed Reality Visualization component is primarily responsible for synchronizing the user's current input with controller models.
    /// </summary>
    /// <seealso cref="MixedRealityControllerMappingProfile"/>
    [AddComponentMenu("Scripts/MRTK/SDK/MixedRealityControllerVisualizer")]
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
        public override void OnInputChanged(InputEventData<Vector2> eventData)
        {
            base.OnInputChanged(eventData);
            // TODO Visualize dual axis controls
        }

        #endregion IMixedRealityInputHandler Implementation
    }
}