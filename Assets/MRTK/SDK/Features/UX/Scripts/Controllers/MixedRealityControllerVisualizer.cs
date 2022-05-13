// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
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
        /// <summary>
        /// The amount to offset this controller visualizer's rotation from the input pose
        /// </summary>
        [SerializeField]
        [Tooltip("The amount to offset this controller visualizer's rotation from the input pose")]
        protected Quaternion rotationOffset = Quaternion.identity;

        protected virtual Quaternion RotationOffset => rotationOffset;

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


        /// <inheritdoc />
        public override void OnSourcePoseChanged(SourcePoseEventData<MixedRealityPose> eventData)
        {
            if (UseSourcePoseData &&
                eventData.SourceId == Controller?.InputSource.SourceId)
            {
                base.OnSourcePoseChanged(eventData);
                transform.localRotation *= RotationOffset;
            }
        }

        #endregion IMixedRealityInputHandler Implementation
    }
}