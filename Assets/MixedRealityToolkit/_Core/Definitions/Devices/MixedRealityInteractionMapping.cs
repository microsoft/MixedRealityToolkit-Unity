// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices
{
    /// <summary>
    /// Maps the capabilities of controllers, linking the Physical inputs of a controller to a Logical construct in a runtime project<para/>
    /// <remarks>One definition should exist for each physical device input, such as buttons, triggers, joysticks, dpads, and more.</remarks>
    /// </summary>
    [Serializable]
    public struct MixedRealityInteractionMapping
    {
        /// <summary>
        /// The constructor for a new Interaction Mapping definition
        /// </summary>
        /// <param name="id">Identity for mapping</param>
        /// <param name="axisType">The axis that the mapping operates on, also denotes the data type for the mapping</param>
        /// <param name="inputType">The physical input device / control</param>
        /// <param name="inputAction">The logical InputAction that this input performs</param>
        public MixedRealityInteractionMapping(uint id, AxisType axisType, DeviceInputType inputType, IMixedRealityInputAction inputAction)
        {
            this.id = id;
            this.axisType = axisType;
            this.inputType = inputType;
            this.inputAction = inputAction;
            rawData = null;
            boolData = false;
            floatData = 0f;
            vector2Data = Vector2.zero;
            positionData = Vector3.zero;
            rotationData = Quaternion.identity;
            sixDofData = SixDof.ZeroIdentity;
            changed = false;
        }

        #region Interaction Properties

        [SerializeField]
        private uint id;

        /// <summary>
        /// The Id assigned to the Interaction.
        /// </summary>
        public uint Id => id;

        [SerializeField]
        [Tooltip("The axis type of the button, e.g. Analogue, Digital, etc.")]
        private AxisType axisType;

        /// <summary>
        /// The axis type of the button, e.g. Analogue, Digital, etc.
        /// </summary>
        public AxisType AxisType => axisType;

        [SerializeField]
        [Tooltip("The primary action of the input as defined by the controller SDK.")]
        private DeviceInputType inputType;

        /// <summary>
        /// The primary action of the input as defined by the controller SDK.
        /// </summary>
        public DeviceInputType InputType => inputType;

        /// <summary>
        /// Action to be raised to the Input Manager when the input data has changed.
        /// </summary>
        public IMixedRealityInputAction InputAction => inputAction;

        [SerializeField]
        [Tooltip("Action to be raised to the Input Manager when the input data has changed.")]
        private IMixedRealityInputAction inputAction;

        private bool changed;

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

        private SixDof sixDofData;

        #endregion Definition Data items

        #region Get Operators
        /// <summary>
        /// Get the Raw (object) data value.
        /// </summary>
        public object GetRawValue()
        {
            return rawData;
        }

        /// <summary>
        /// Get the Boolean data value.
        /// </summary>
        public bool GetBooleanValue()
        {
            return boolData;
        }

        /// <summary>
        /// Get the Float data value.
        /// </summary>
        public float GetFloatValue()
        {
            return floatData;
        }

        /// <summary>
        /// Get the Vector2 data value.
        /// </summary>
        public Vector2 GetVector2Value()
        {
            return vector2Data;
        }

        /// <summary>
        /// Get the ThreeDof Vector3 Position data value.
        /// </summary>
        public Vector3 GetPositionValue()
        {
            return positionData;
        }

        /// <summary>
        /// Get the ThreeDof Quaternion Rotation data value.
        /// </summary>
        public Quaternion GetRotationValue()
        {
            return rotationData;
        }

        /// <summary>
        /// Get the SixDof data value.
        /// </summary>
        public SixDof GetSixDofValue()
        {
            return sixDofData;
        }

        #endregion Get Operators

        #region Set Operators

        /// <summary>
        /// Set the Raw (object) data value.
        /// </summary>
        /// <remarks>Only supported for a Raw mapping axis type</remarks>
        /// <param name="newValue">Raw (object) value to set</param>
        public void SetRawValue(object newValue)
        {
            if (AxisType != AxisType.Raw)
            {
                Debug.LogError("SetRawValue(object) is only valid for AxisType.Raw InteractionMappings");
            }

            Changed = rawData != newValue;
            rawData = newValue;
        }

        /// <summary>
        /// Set the Bool data value.
        /// </summary>
        /// <remarks>Only supported for a Digital mapping axis type</remarks>
        /// <param name="newValue">Bool value to set</param>
        public void SetBoolValue(bool newValue)
        {
            if (AxisType != AxisType.Digital)
            {
                Debug.LogError("SetRawValue(bool) is only valid for AxisType.Digital InteractionMappings");
            }

            Changed = boolData != newValue;
            boolData = newValue;
        }

        /// <summary>
        /// Set the Float data value.
        /// </summary>
        /// <remarks>Only supported for a SingleAxis mapping axis type</remarks>
        /// <param name="newValue">Float value to set</param>
        public void SetFloatValue(float newValue)
        {
            if (AxisType != AxisType.SingleAxis)
            {
                Debug.LogError("SetRawValue(float) is only valid for AxisType.SingleAxis InteractionMappings");
            }

            Changed = !floatData.Equals(newValue);
            floatData = newValue;
        }

        /// <summary>
        /// Set the Vector2 data value.
        /// </summary>
        /// <remarks>Only supported for a DualAxis mapping axis type</remarks>
        /// <param name="newValue">Vector2 value to set</param>
        public void SetVector2Value(Vector2 newValue)
        {
            if (AxisType != AxisType.DualAxis)
            {
                Debug.LogError("SetRawValue(Vector2) is only valid for AxisType.DualAxis InteractionMappings");
            }

            Changed = vector2Data != newValue;
            vector2Data = newValue;
        }

        /// <summary>
        /// Set the ThreeDof Vector3 Position data value.
        /// </summary>
        /// <remarks>Only supported for a ThreeDof mapping axis type</remarks>
        /// <param name="newValue">Vector3 value to set</param>
        public void SetPositionValue(Vector3 newValue)
        {
            if (AxisType != AxisType.ThreeDofPosition)
            {
                {
                    Debug.LogError("SetRawValue(Vector3) is only valid for AxisType.ThreeDoFPosition InteractionMappings");
                }
            }

            Changed = positionData != newValue;
            positionData = newValue;
        }

        /// <summary>
        /// Set the ThreeDof Quaternion Rotation data value.
        /// </summary>
        /// <remarks>Only supported for a ThreeDof mapping axis type</remarks>
        /// <param name="newValue">Quaternion value to set</param>
        public void SetRotationValue(Quaternion newValue)
        {
            if (AxisType != AxisType.ThreeDofRotation)
            {
                Debug.LogError("SetRawValue(Quaternion) is only valid for AxisType.ThreeDoFRotation InteractionMappings");
            }

            Changed = rotationData != newValue;
            rotationData = newValue;
        }

        /// <summary>
        /// Set the SixDof data value.
        /// </summary>
        /// <remarks>Only supported for a SixDof mapping axis type</remarks>
        /// <param name="newValue">SixDof value to set</param>
        public void SetSixDofValue(SixDof newValue)
        {
            if (AxisType != AxisType.SixDof)
            {
                Debug.LogError("SetRawValue(SixDof) is only valid for AxisType.SixDoF InteractionMappings");
            }

            Changed = sixDofData != newValue;

            sixDofData = newValue;
            positionData = sixDofData.Position;
            rotationData = sixDofData.Rotation;
        }

        #endregion Set Operators
    }
}