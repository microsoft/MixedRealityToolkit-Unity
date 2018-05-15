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

        private object rawDatum { get; set; }
        private bool boolDatum { get; set; }
        private float floatDatum { get; set; }
        private Vector2 vector2Datum { get; set; }
        private Vector3 positionDatum { get; set; }
        private Quaternion rotationDatum { get; set; }

        public bool Changed { get; set; }

        public object GetValue()
        {
            switch (AxisType)
            {
                case AxisType.Digital:
                    return boolDatum;
                case AxisType.SingleAxis:
                    return floatDatum;
                case AxisType.DualAxis:
                    return vector2Datum;
                case AxisType.ThreeDoF:
                    return rotationDatum;
                case AxisType.SixDoF:
                    return new Tuple<Vector3, Quaternion>(positionDatum, rotationDatum);
                case AxisType.Raw:
                case AxisType.None:
                default:
                    return rawDatum;
            }
        }

        public void SetValue(object value)
        {
            switch (AxisType)
            {
                case AxisType.Raw:
                    Changed = value == rawDatum;
                    rawDatum = value;
                    break;
                case AxisType.Digital:
                    bool newBoolValue;
                    Assert.IsTrue(bool.TryParse(value.ToString(), out newBoolValue));
                    Changed = newBoolValue == boolDatum;
                    boolDatum = newBoolValue;
                    break;
                case AxisType.SingleAxis:
                    float newFloatValue = float.NaN;
                    Assert.IsTrue(float.TryParse(value.ToString(), out newFloatValue));
                    if (!float.IsNaN(newFloatValue))
                    {
                        Changed = newFloatValue == floatDatum;
                        floatDatum = newFloatValue;
                    }
                    break;
                case AxisType.DualAxis:
                    Vector2 newVector2Value = (Vector2)value;
                    Assert.IsTrue(newVector2Value != null);
                    if (newVector2Value != null)
                    {
                        Changed = newVector2Value == vector2Datum;
                        vector2Datum = newVector2Value;
                    }
                    break;
                case AxisType.ThreeDoF:
                    Quaternion newRotationValue = (Quaternion)value;
                    Assert.IsTrue(newRotationValue != null);
                    if (newRotationValue != null)
                    {
                        Changed = newRotationValue == rotationDatum;
                        rotationDatum = newRotationValue;
                    }
                    break;
                case AxisType.SixDoF:
                    Tuple<Vector3, Quaternion> new6DoFvalue = (Tuple<Vector3, Quaternion>)value;
                    Assert.IsTrue(new6DoFvalue != null);
                    if (new6DoFvalue != null)
                    {
                        Changed = new6DoFvalue.Item1 == positionDatum && new6DoFvalue.Item2 == rotationDatum;
                        positionDatum = new6DoFvalue.Item1;
                        rotationDatum = new6DoFvalue.Item2;
                    }
                    break;
                case AxisType.None:
                default:
                    break;
            }
        }

    }
}