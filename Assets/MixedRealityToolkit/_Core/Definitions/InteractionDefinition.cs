// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// Maps the capabilities of controllers, one definition should exist for each interaction profile.<para/>
    /// <remarks>Interactions can be any input the controller supports such as buttons, triggers, joysticks, dpads, and more.</remarks>
    /// </summary>
    public struct InteractionDefinition
    {
        public InteractionDefinition(uint id, AxisType axisType, Devices.DeviceInputType inputType) : this()
        {
            Id = id;
            AxisType = axisType;
            InputType = inputType;
        }

        public InteractionDefinition(uint id, AxisType axisType, Devices.DeviceInputType inputType, InputAction inputAction) : this()
        {
            Id = id;
            AxisType = axisType;
            InputType = inputType;
            InputAction = inputAction;
        }

        public InteractionDefinition(uint id, AxisType axisType, Devices.DeviceInputType inputType, InputAction inputAction, InputAction inputHoldAction) : this()
        {
            Id = id;
            AxisType = axisType;
            InputType = inputType;
            InputAction = inputAction;
            InputHoldAction = inputHoldAction;
        }

        #region Interaction Properties

        /// <summary>
        /// The Id assigned to the Interaction.
        /// </summary>
        public uint Id { get; }

        /// <summary>
        /// The axis type of the button, e.g. Analogue, Digital, etc.
        /// </summary>
        public AxisType AxisType { get; }

        /// <summary>
        /// The primary action of the input as defined by the controller SDK.
        /// </summary>
        public Devices.DeviceInputType InputType { get; }

        /// <summary>
        /// Action to be raised to the Input Manager when the input data has changed.
        /// </summary>
        public InputAction InputAction { get; set; }

        /// <summary>
        /// Action to be raised to the Input Manager when the input data has changed.
        /// </summary>
        public InputAction InputHoldAction { get; set; }
        /// <summary>
        /// Has the value changed since the last reading.
        /// </summary>
        public bool Changed { get; private set; }

        #endregion Interaction Properties

        #region Definition Data items

        /// <summary>
        /// The data storage for a Raw / None Axis type.
        /// </summary>
        private object rawData;

        /// <summary>
        /// The data storage for a Digital Axis type.
        /// </summary>
        private bool boolData;

        /// <summary>
        /// The data storage for a Single Axis type.
        /// </summary>
        private float floatData;

        /// <summary>
        /// The data storage for a Dual Axis type.
        /// </summary>
        private Vector2 vector2Data;

        /// <summary>
        /// The position data storage for a 3DoF type.
        /// </summary>
        private Vector3 positionData;

        /// <summary>
        /// The rotation data storage for a 3DoF type.
        /// </summary>
        private Quaternion rotationData;

        #endregion Definition Data items

        #region Get Operators

        public T GetValue<T>()
        {
            switch (AxisType)
            {
                case AxisType.Digital:
                    return (T)Convert.ChangeType(boolData, typeof(T));
                case AxisType.SingleAxis:
                    return (T)Convert.ChangeType(floatData, typeof(T));
                case AxisType.DualAxis:
                    return (T)Convert.ChangeType(vector2Data, typeof(T));
                case AxisType.ThreeDoFPosition:
                    return (T)Convert.ChangeType(positionData, typeof(T));
                case AxisType.ThreeDoFRotation:
                    return (T)Convert.ChangeType(rotationData, typeof(T));
                case AxisType.SixDoF:
                    return (T)Convert.ChangeType(GetTransform(), typeof(T));
                case AxisType.Raw:
                case AxisType.None:
                default:
                    return (T)Convert.ChangeType(rawData, typeof(T));
            }
        }

        public object GetRaw()
        {
            return rawData;
        }

        public bool GetBool()
        {
            return boolData;
        }

        public float GetFloat()
        {
            return floatData;
        }

        public Vector2 GetVector2()
        {
            return vector2Data;
        }

        public Vector3 GetPosition()
        {
            return positionData;
        }

        public Quaternion GetRotation()
        {
            return rotationData;
        }

        public Tuple<Vector3, Quaternion> GetTransform()
        {
            return new Tuple<Vector3, Quaternion>(positionData,rotationData);
        }

        #endregion Get Operators

        #region Set Operators

        public void SetValue(object newValue)
        {
            if (AxisType == AxisType.Raw)
            {
                Changed = newValue != rawData;
                rawData = newValue;
            }
        }

        public void SetValue(bool newValue)
        {
            if (AxisType == AxisType.Digital)
            {
                Changed = newValue != boolData;
                boolData = newValue;
            }
        }

        public void SetValue(float newValue)
        {
            if (AxisType == AxisType.SingleAxis)
            {
                Changed = !newValue.Equals(floatData);
                floatData = newValue;
            }
        }

        public void SetValue(Vector2 newValue)
        {
            if (AxisType == AxisType.DualAxis)
            {
                Changed = newValue != vector2Data;
                vector2Data = newValue;
            }
        }

        public void SetValue(Vector3 newValue)
        {
            if (AxisType == AxisType.ThreeDoFPosition)
            {
                Changed = newValue != positionData;
                positionData = newValue;
            }
        }

        public void SetValue(Quaternion newValue)
        {
            if (AxisType == AxisType.ThreeDoFRotation)
            {
                Changed = newValue != rotationData;
                rotationData = newValue;
            }
        }

        public void SetValue(Tuple<Vector3, Quaternion> newValue)
        {
            if (AxisType == AxisType.SixDoF)
            {
                Changed = newValue.Item1 != positionData || newValue.Item2 != rotationData;
                positionData = newValue.Item1;
                rotationData = newValue.Item2;
            }
        }

        #endregion Set Operators
    }
}