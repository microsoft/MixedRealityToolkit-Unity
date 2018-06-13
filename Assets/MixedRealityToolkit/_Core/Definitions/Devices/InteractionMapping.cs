// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices
{
    /// <summary>
    /// Maps the capabilities of controllers, linking the Physical inputs of a controller to a Logical construct in a runtime project<para/>
    /// <remarks>One definition should exist for each physical device input, such as buttons, triggers, joysticks, dpads, and more.</remarks>
    /// </summary>
    [Serializable]
    public struct InteractionMapping
    {
        public InteractionMapping(uint id, AxisType axisType, DeviceInputType inputType, InputAction inputAction) : this()
        {
            this.id = id;
            this.axisType = axisType;
            this.inputType = inputType;
            this.inputAction = inputAction;
            positionData = Vector3.zero;
            rotationData = Quaternion.identity;
        }

        #region Interaction Properties

        /// <summary>
        /// The Id assigned to the Interaction.
        /// </summary>
        public uint Id => id;

        [SerializeField]
        private uint id;

        /// <summary>
        /// The axis type of the button, e.g. Analogue, Digital, etc.
        /// </summary>
        public AxisType AxisType => axisType;

        [SerializeField]
        [Tooltip("The axis type of the button, e.g. Analogue, Digital, etc.")]
        private AxisType axisType;

        /// <summary>
        /// The primary action of the input as defined by the controller SDK.
        /// </summary>
        public DeviceInputType InputType => inputType;

        [SerializeField]
        [Tooltip("The primary action of the input as defined by the controller SDK.")]
        private DeviceInputType inputType;

        /// <summary>
        /// Action to be raised to the Input Manager when the input data has changed.
        /// </summary>
        public InputAction InputAction => inputAction;

        [SerializeField]
        [Tooltip("Action to be raised to the Input Manager when the input data has changed.")]
        private InputAction inputAction;

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

        private object rawData;

        private bool boolData;

        private float floatData;

        private Vector2 vector2Data;

        private Vector3 positionData;

        private Quaternion rotationData;

        private Tuple<Vector3, Quaternion> transformData;

        private bool changed;

        #endregion Definition Data items

        #region Get Operators

        public object GetRawValue()
        {
            return rawData;
        }

        public bool GetBooleanValue()
        {
            return boolData;
        }

        public float GetFloatValue()
        {
            return floatData;
        }

        public Vector2 GetVector2Value()
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

        public void SetValue(object newValue)
        {
            if (AxisType != AxisType.Raw)
            {
                Debug.LogError("SetValue(object) is only valid for AxisType.Raw InteractionMappings");
            }

            Changed = rawData != newValue;
            rawData = newValue;
        }

        public void SetValue(bool newValue)
        {
            if (AxisType != AxisType.Digital)
            {
                Debug.LogError("SetValue(bool) is only valid for AxisType.Digital InteractionMappings");
            }

            Changed = boolData != newValue;
            boolData = newValue;
        }

        public void SetValue(float newValue)
        {
            if (AxisType != AxisType.SingleAxis)
            {
                Debug.LogError("SetValue(float) is only valid for AxisType.SingleAxis InteractionMappings");
            }

            Changed = !floatData.Equals(newValue);
            floatData = newValue;
        }

        public void SetValue(Vector2 newValue)
        {
            if (AxisType != AxisType.DualAxis)
            {
                Debug.LogError("SetValue(Vector2) is only valid for AxisType.DualAxis InteractionMappings");
            }

            Changed = vector2Data != newValue;
            vector2Data = newValue;
        }

        public void SetValue(Vector3 newValue)
        {
            if (AxisType != AxisType.ThreeDoFPosition)
            {
                {
                    Debug.LogError("SetValue(Vector3) is only valid for AxisType.ThreeDoFPosition InteractionMappings");
                }
            }

            Changed = positionData != newValue;
            positionData = newValue;
        }

        public void SetValue(Quaternion newValue)
        {
            if (AxisType != AxisType.ThreeDoFRotation)
            {
                Debug.LogError("SetValue(Quaternion) is only valid for AxisType.ThreeDoFRotation InteractionMappings");
            }

            Changed = rotationData != newValue;
            rotationData = newValue;
        }

        public void SetValue(Tuple<Vector3, Quaternion> newValue)
        {
            if (AxisType != AxisType.SixDoF)
            {
                Debug.LogError("SetValue(Tuple<Vector3, Quaternion>) is only valid for AxisType.SixDoF InteractionMappings");
            }

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

        #endregion Set Operators
    }

    public class Test
    {
        System.Collections.Generic.List<IInteractionMapping> list = new System.Collections.Generic.List<IInteractionMapping>();
        void helloWorld()
        {
            var interaction = new InteractionMapping<float>(0, AxisType.SingleAxis, DeviceInputType.None, new InputAction(1, "None"));
            var value = interaction.GetData();
            interaction.SetData(0f);
            list.Add(interaction);
            InteractionMapping<float> interaction2 = (InteractionMapping<float>)list[0];
            var value2 = interaction2.GetData();
            interaction2.SetData(0.1f);
        }
    }

    public interface IInteractionMapping
    {
        /// <summary>
        /// The Id assigned to the Interaction.
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// The axis type of the button, e.g. Analogue, Digital, etc.
        /// </summary>
        AxisType AxisType { get; }

        /// <summary>
        /// The primary action of the input as defined by the controller SDK.
        /// </summary>
        DeviceInputType InputType { get; }

        /// <summary>
        /// Action to be raised to the Input Manager when the input data has changed.
        /// </summary>
        InputAction InputAction { get; }

        /// <summary>
        /// Has the value changed since the last reading.
        /// </summary>
        bool Changed { get; }
    }

    public class InteractionMapping<TReadingType> : IInteractionMapping
    {
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

        public TReadingType GetData() => currentReading;

        public void SetData(TReadingType newValue)
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