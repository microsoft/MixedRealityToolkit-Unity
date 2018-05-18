// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// A InteractionDefinition maps the capabilities of controllers, one definition should exist for each interaction profile.<para/>
    /// <remarks>Interactions can be any input the controller supports such as buttons, triggers, joysticks, dpads, and more.</remarks>
    /// </summary>
    public struct InteractionDefinition
    {

        #region Interaction Properties

        /// <summary>
        /// The ID assigned to the Input
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The axis type of the button, e.g. Analogue, Digital, etc.
        /// </summary>
        public AxisType AxisType { get; set; }

        /// <summary>
        /// The primary action of the button as defined by the controller SDK.
        /// </summary>
        public InputType InputType { get; set; }

        #endregion Interaction Properties

        #region Definition Datums

        /// <summary>
        /// The data storage for a Raw / None Axis type.
        /// </summary>
        private object rawDatum { get; set; }

        /// <summary>
        /// The data storage for a Digital Axis type.
        /// </summary>
        private bool boolDatum { get; set; }

        /// <summary>
        /// The data storage for a Single Axis type.
        /// </summary>
        private float floatDatum { get; set; }

        /// <summary>
        /// The data storage for a Dual Axis type.
        /// </summary>
        private Vector2 vector2Datum { get; set; }

        /// <summary>
        /// The position data storage for a 3DoF or 6DoF Axis type.
        /// </summary>
        private Vector3 positionDatum { get; set; }

        /// <summary>
        /// The rotation data storage for a 3DoF or 6DoF Axis type.
        /// </summary>
        private Quaternion rotationDatum { get; set; }

        /// <summary>
        /// Has the value changed since the last reading
        /// </summary>
        public bool Changed { get; set; }

        #endregion Definition Datums

        #region Get Operators

        public T GetValue<T>()
        {
            switch (AxisType)
            {
                case AxisType.Digital:
                    return (T)Convert.ChangeType(boolDatum,typeof(T));
                case AxisType.SingleAxis:
                    return (T)Convert.ChangeType(floatDatum, typeof(T));
                case AxisType.DualAxis:
                    return (T)Convert.ChangeType(vector2Datum, typeof(T));
                case AxisType.ThreeDoFPosition:
                    return (T)Convert.ChangeType(positionDatum, typeof(T));
                case AxisType.ThreeDoFRotation:
                    return (T)Convert.ChangeType(rotationDatum, typeof(T));
                case AxisType.SixDoF:
                    return (T)Convert.ChangeType(new Tuple<Vector3, Quaternion>(positionDatum, rotationDatum), typeof(T));
                case AxisType.Raw:
                case AxisType.None:
                default:
                    return (T)Convert.ChangeType(rawDatum,typeof(T));
            }
        }
        #endregion Get Operators

        #region Set Operators

        public void SetValue(object newValue)
        {
            if (AxisType == AxisType.Digital)
            {
                Changed = newValue == rawDatum;
                rawDatum = newValue;
            }
        }

        public void SetValue(bool newValue)
        {
            if (AxisType == AxisType.Digital)
            {
                Changed = newValue == boolDatum;
                boolDatum = newValue;
            }
        }

        public void SetValue(float newValue)
        {
            if (AxisType == AxisType.SingleAxis)
            {
                Changed = newValue == floatDatum;
                floatDatum = newValue;
            }
        }

        public void SetValue(Vector2 newValue)
        {
            if (AxisType == AxisType.DualAxis)
            {
                Changed = newValue == vector2Datum;
                vector2Datum = newValue;
            }
        }

        public void SetValue(Vector3 newValue)
        {
            if (AxisType == AxisType.ThreeDoF)
            {
                Changed = newValue == positionDatum;
                positionDatum = newValue;
            }
        }

        public void SetValue(Quaternion newValue)
        {
            if (AxisType == AxisType.ThreeDoF)
            {
                Changed = newValue == rotationDatum;
                rotationDatum = newValue;
            }
        }

        public void SetValue(Tuple<Vector3, Quaternion> newValue)
        {
            if (AxisType == AxisType.SixDoF)
            {
                Changed = newValue.Item1 == positionDatum && newValue.Item2 == rotationDatum;
                positionDatum = newValue.Item1;
                rotationDatum = newValue.Item2;
            }
        }

        #endregion Set Operators

    }
}