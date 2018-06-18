// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices
{
    /// <summary>
    /// Maps the capabilities of controllers, linking the Physical inputs of a controller to a Logical construct in a runtime project<para/>
    /// <remarks>One definition should exist for each physical device input, such as buttons, triggers, joysticks, dpads, and more.</remarks>
    /// </summary>
    [Serializable]
    public class InteractionMapping<TReadingType> : IInteractionMapping
    {
        /// <summary>
        /// The constructor for a new Interaction Mapping definition
        /// </summary>
        /// <param name="id">Identity for mapping</param>
        /// <param name="axisType">The axis that the mapping operates on, also denotes the data type for the mapping</param>
        /// <param name="inputType">The physical input device / control</param>
        /// <param name="inputAction">The logical InputAction that this input performs</param>
        public InteractionMapping(uint id, AxisType axisType, DeviceInputType inputType, InputAction inputAction)
        {
            this.id = id;
            this.axisType = axisType;
            this.inputType = inputType;
            this.inputAction = inputAction;
            changed = false;
        }

        #region Interaction Properties

        [SerializeField]
        private uint id;

        /// <inheritdoc/>
        public uint Id { get { return id; } private set { id = value; } }

        [SerializeField]
        [Tooltip("The axis type of the button, e.g. Analogue, Digital, etc.")]
        private AxisType axisType;

        /// <inheritdoc/>
        public AxisType AxisType { get { return axisType; } private set { axisType = value; } }

        [SerializeField]
        [Tooltip("The primary action of the input as defined by the controller SDK.")]
        private DeviceInputType inputType;

        /// <inheritdoc/>
        public DeviceInputType InputType { get { return inputType; } private set { inputType = value; } }

        [SerializeField]
        [Tooltip("Action to be raised to the Input Manager when the input data has changed.")]
        private InputAction inputAction;

        /// <inheritdoc/>
        public InputAction InputAction { get { return inputAction; } private set { inputAction = value; } }

        private TReadingType currentReading;

        private bool changed;

        /// <inheritdoc/>
        public bool Changed
        {
            get
            {
                bool returnValue = changed;

                if (changed)
                {
                    changed = false;
                }

                return returnValue;
            }
            private set
            {
                changed = value;
            }
        }

        #endregion Interaction Properties

        #region Operators

        /// <summary>
        /// Set the value of the Interaction Mapping data
        /// </summary>
        /// <returns></returns>
        public TReadingType GetValue() => currentReading;

        /// <summary>
        /// Get the value of the Interaction Mapping data
        /// </summary>
        /// <param name="newValue"></param>
        public void GetValue(TReadingType newValue)
        {
            Changed = currentReading == null && newValue != null ||
            currentReading != null && newValue == null ||
            currentReading != null && newValue != null &&
            !currentReading.Equals(newValue);

            currentReading = newValue;
        }

        #endregion Operators
    }
}