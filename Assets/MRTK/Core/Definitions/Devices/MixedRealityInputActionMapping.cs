// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Maps the capabilities of controllers, defining the physical inputs of a controller.
    /// </summary>
    /// <remarks>
    /// One definition should exist for each physical device input, such as buttons, triggers, joysticks, dpads, etc.
    /// </remarks>
    [Serializable]
    public class MixedRealityInputActionMapping
    {
        /// <summary>
        /// The constructor for a new MixedRealityInputActionMapping definition
        /// </summary>
        /// <param name="description">The description of the interaction mapping.</param> 
        /// <param name="inputAction">The logical MixedRealityInputAction that this input performs</param>
        public MixedRealityInputActionMapping(string description, AxisType axisType, DeviceInputType inputType) :
            this(description, axisType, inputType, MixedRealityInputAction.None)
        {
        }

        /// <summary>
        /// The constructor for a new MixedRealityInputActionMapping definition
        /// </summary>
        /// <param name="description">The description of the interaction mapping.</param> 
        /// <param name="inputAction">The logical MixedRealityInputAction that this input performs</param>
        public MixedRealityInputActionMapping(string description, AxisType axisType, DeviceInputType inputType, MixedRealityInputAction inputAction)
        {
            this.description = description;
            this.axisType = axisType;
            this.inputType = inputType;
            this.inputAction = inputAction;
        }

        [SerializeField]
        [Tooltip("The description of the interaction mapping.")]
        private string description;

        /// <summary>
        /// The description of the input action mapping.
        /// </summary>
        public string Description => description;

        [SerializeField]
        [Tooltip("The axis type of the button, e.g. Analogue, Digital, etc.")]
        private AxisType axisType;

        /// <summary>
        /// The axis type of the button, e.g. Analog, Digital, etc.
        /// </summary>
        public AxisType AxisType => axisType;

        [SerializeField]
        [Tooltip("The primary action of the input as defined by the controller SDK.")]
        private DeviceInputType inputType;

        /// <summary>
        /// The primary action of the input as defined by the controller SDK.
        /// </summary>
        public DeviceInputType InputType => inputType;

        [SerializeField]
        [Tooltip("Action to be raised to the Input Manager when the input data has changed.")]
        private MixedRealityInputAction inputAction;

        /// <summary>
        /// Action to be raised when the input data has changed.
        /// </summary>
        public MixedRealityInputAction InputAction
        {
            get { return inputAction; }
            internal set { inputAction = value; }
        }
    }
}
