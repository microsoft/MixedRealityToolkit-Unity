// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// A InteractionDefinition maps the capabilities of controllers, one definition should exist for each interaction profile.<para/>
    /// <remarks>Interactions can be any input the controller supports such as buttons, triggers, joysticks, dpads, and more.</remarks>
    /// </summary>
    public struct InteractionDefinition
    {
        public InteractionDefinition(uint id, AxisType axisType, InputType inputType) : this()
        {
            Id = id;
            AxisType = axisType;
            InputType = inputType;
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
        public InputType InputType { get; }

        /// <summary>
        /// Has the value changed since the last reading.
        /// </summary>
        public bool Changed { get; private set; }

        #endregion Interaction Properties

        #region Definition Data items

        /// <summary>
        /// The data storage for a Raw / None Axis type.
        /// </summary>
        private object RawData { get; set; }

        /// <summary>
        /// The data storage for a Digital Axis type.
        /// </summary>
        private bool BoolData { get; set; }

        /// <summary>
        /// The data storage for a Single Axis type.
        /// </summary>
        private float FloatData { get; set; }

        /// <summary>
        /// The data storage for a Dual Axis type.
        /// </summary>
        private Vector2 Vector2Data { get; set; }

        /// <summary>
        /// The position data storage for a 3DoF or 6DoF Axis type.
        /// </summary>
        private Vector3 PositionData { get; set; }

        /// <summary>
        /// The rotation data storage for a 3DoF or 6DoF Axis type.
        /// </summary>
        private Quaternion RotationData { get; set; }

        private Tuple<Vector3, Quaternion> TransformData { get; set; }

        #endregion Definition Data items

        public T GetValue<T>()
        {
            switch (AxisType)
            {
                case AxisType.None:
                case AxisType.Raw:
                    return (T)Convert.ChangeType(RawData, typeof(T));
                case AxisType.Digital:
                    return (T)Convert.ChangeType(BoolData, typeof(T));
                case AxisType.SingleAxis:
                    return (T)Convert.ChangeType(FloatData, typeof(T));
                case AxisType.DualAxis:
                    return (T)Convert.ChangeType(Vector2Data, typeof(T));
                case AxisType.ThreeDoFPosition:
                    return (T)Convert.ChangeType(PositionData, typeof(T));
                case AxisType.ThreeDoFRotation:
                    return (T)Convert.ChangeType(RotationData, typeof(T));
                case AxisType.SixDoF:
                    return (T)Convert.ChangeType(TransformData, typeof(T));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetValue<T>(T newValue)
        {
            switch (AxisType)
            {
                case AxisType.None:
                case AxisType.Raw:
                    Changed = Equals(newValue, RawData);
                    RawData = newValue;
                    break;
                case AxisType.Digital:
                    var newBool = (bool)Convert.ChangeType(newValue, typeof(T));
                    Changed = newBool != BoolData;
                    BoolData = newBool;
                    break;
                case AxisType.SingleAxis:
                    var newFloat = (float)Convert.ChangeType(newValue, typeof(T));
                    Changed = !newFloat.Equals(FloatData);
                    FloatData = newFloat;
                    break;
                case AxisType.DualAxis:
                    var newVec2 = (Vector2)Convert.ChangeType(newValue, typeof(T));
                    Changed = newVec2 != Vector2Data;
                    Vector2Data = newVec2;
                    break;
                case AxisType.ThreeDoFPosition:
                    var newVec3 = (Vector3)Convert.ChangeType(newValue, typeof(T));
                    Changed = newVec3 != PositionData;
                    PositionData = newVec3;
                    break;
                case AxisType.ThreeDoFRotation:
                    var newQuat = (Quaternion)Convert.ChangeType(newValue, typeof(T));
                    Changed = newQuat != RotationData;
                    RotationData = newQuat;
                    break;
                case AxisType.SixDoF:
                    var newTransform = (Tuple<Vector3, Quaternion>)Convert.ChangeType(newValue, typeof(T));
                    Changed = newTransform.Item1 != TransformData.Item1 || newTransform.Item2 != TransformData.Item2;
                    TransformData = newTransform;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(AxisType));
            }
        }
    }
}