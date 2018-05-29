// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices
{
    /// <summary>
    /// Maps the capabilities of controllers, one definition should exist for each physical device input.<para/>
    /// <remarks>Interactions can be any input the controller supports such as buttons, triggers, joysticks, dpads, and more.</remarks>
    /// </summary>
    [Serializable]
    public struct InteractionMapping
    {
        public InteractionMapping(uint id, AxisType axisType, DeviceInputType inputType, InputAction inputAction) : this()
        {
            Id = id;
            AxisType = axisType;
            InputType = inputType;
            InputAction = inputAction;
            positionData = Vector3.zero;
            rotationData = Quaternion.identity;
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
        public DeviceInputType InputType { get; }

        /// <summary>
        /// Action to be raised to the Input Manager when the input data has changed.
        /// </summary>
        public InputAction InputAction { get; }

        /// <summary>
        /// Has the value changed since the last reading.
        /// </summary>
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

        #region Definition Data items

        [SerializeField]
        private object rawData;

        [SerializeField]
        private bool boolData;

        [SerializeField]
        private float floatData;

        [SerializeField]
        private Vector2 vector2Data;

        [SerializeField]
        private Vector3 positionData;

        [SerializeField]
        private Quaternion rotationData;

        [SerializeField]
        private Tuple<Vector3, Quaternion> transformData;

        [SerializeField]
        private bool changed;

        #endregion Definition Data items

        #region Get Operators

        public T GetValue<T>()
        {
            switch (AxisType)
            {
                case AxisType.None:
                case AxisType.Raw:
                    return (T)Convert.ChangeType(rawData, typeof(T));
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
                    return (T)Convert.ChangeType(transformData, typeof(T));
                default:
                    throw new ArgumentOutOfRangeException();
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
            return transformData;
        }

        #endregion Get Operators

        #region Set Operators

        public void SetValue<T>(T newValue)
        {
            switch (AxisType)
            {
                case AxisType.None:
                case AxisType.Raw:
                    SetValue((object)newValue);
                    break;
                case AxisType.Digital:
                    SetValue((bool)Convert.ChangeType(newValue, typeof(bool)));
                    break;
                case AxisType.SingleAxis:
                    SetValue((float)Convert.ChangeType(newValue, typeof(float)));
                    break;
                case AxisType.DualAxis:
                    SetValue((Vector2)Convert.ChangeType(newValue, typeof(Vector2)));
                    break;
                case AxisType.ThreeDoFPosition:
                    SetValue((Vector3)Convert.ChangeType(newValue, typeof(Vector3)));
                    break;
                case AxisType.ThreeDoFRotation:
                    SetValue((Quaternion)Convert.ChangeType(newValue, typeof(Quaternion)));
                    break;
                case AxisType.SixDoF:
                    SetValue((Tuple<Vector3, Quaternion>)Convert.ChangeType(newValue, typeof(Tuple<Vector3, Quaternion>)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetValue(object newValue)
        {
            if (AxisType == AxisType.Raw)
            {
                Changed = rawData != newValue;
                rawData = newValue;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void SetValue(bool newValue)
        {
            if (AxisType == AxisType.Digital)
            {
                Changed = boolData != newValue;
                boolData = newValue;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void SetValue(float newValue)
        {
            if (AxisType == AxisType.SingleAxis)
            {
                Changed = !floatData.Equals(newValue);
                floatData = newValue;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void SetValue(Vector2 newValue)
        {
            if (AxisType == AxisType.DualAxis)
            {
                Changed = vector2Data != newValue;
                vector2Data = newValue;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void SetValue(Vector3 newValue)
        {
            if (AxisType == AxisType.ThreeDoFPosition)
            {
                Changed = positionData != newValue;
                positionData = newValue;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void SetValue(Quaternion newValue)
        {
            if (AxisType == AxisType.ThreeDoFRotation)
            {
                Changed = rotationData != newValue;
                rotationData = newValue;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void SetValue(Tuple<Vector3, Quaternion> newValue)
        {
            if (AxisType == AxisType.SixDoF)
            {
                Changed = transformData == null && newValue != null ||
                          transformData != null && newValue == null ||
                          transformData != null && newValue != null &&
                          (transformData.Item1 != newValue.Item1 || transformData.Item2 != newValue.Item2);

                transformData = newValue;

                if (transformData != null)
                {
                    positionData = transformData.Item1;
                    rotationData = transformData.Item2;
                }
                else
                {
                    positionData = Vector3.zero;
                    rotationData = Quaternion.identity;
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        #endregion Set Operators
    }
}