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
    public class MixedRealityInteractionMapping
    {
        /// <summary>
        /// The constructor for a new Interaction Mapping definition
        /// </summary>
        /// <param name="id">Identity for mapping</param>
        /// <param name="description">The description of the interaction mapping.</param> 
        /// <param name="axisType">The axis that the mapping operates on, also denotes the data type for the mapping</param>
        /// <param name="inputType">The physical input device / control</param>
        /// <param name="inputAction">The logical MixedRealityInputAction that this input performs</param>
        /// <param name="keyCode">Optional KeyCode value to get input from Unity's old input system</param>
        /// <param name="axisCodeX">Optional horizontal or single axis value to get axis data from Unity's old input system.</param>
        /// <param name="axisCodeY">Optional vertical axis value to get axis data from Unity's old input system.</param> 
        public MixedRealityInteractionMapping(uint id, string description, AxisType axisType, DeviceInputType inputType, MixedRealityInputAction inputAction, KeyCode keyCode = KeyCode.None, string axisCodeX = "", string axisCodeY = "")
        {
            this.id = id;
            this.description = description;
            this.axisType = axisType;
            this.inputType = inputType;
            this.inputAction = inputAction;
            this.keyCode = keyCode;
            this.axisCodeX = axisCodeX;
            this.axisCodeY = axisCodeY;
            rawData = null;
            boolData = false;
            floatData = 0f;
            vector2Data = Vector2.zero;
            positionData = Vector3.zero;
            rotationData = Quaternion.identity;
            poseData = MixedRealityPose.ZeroIdentity;
            changed = false;
        }

        #region Interaction Properties

        [SerializeField]
        [Tooltip("The Id assigned to the Interaction.")]
        private uint id;

        /// <summary>
        /// The Id assigned to the Interaction.
        /// </summary>
        public uint Id => id;

        [SerializeField]
        [Tooltip("The description of the interaction mapping.")]
        private string description;

        /// <summary>
        /// The description of the interaction mapping.
        /// </summary>
        public string Description => description;

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

        [SerializeField]
        [Tooltip("Action to be raised to the Input Manager when the input data has changed.")]
        private MixedRealityInputAction inputAction;

        /// <summary>
        /// Action to be raised to the Input Manager when the input data has changed.
        /// </summary>
        public MixedRealityInputAction MixedRealityInputAction => inputAction;

        [SerializeField]
        [Tooltip("Optional KeyCode value to get input from Unity's old input system.")]
        private KeyCode keyCode;

        /// <summary>
        /// Optional KeyCode value to get input from Unity's old input system.
        /// </summary>
        public KeyCode KeyCode => keyCode;

        [SerializeField]
        [Tooltip("Optional horizontal or single axis value to get axis data from Unity's old input system.")]
        private string axisCodeX;

        /// <summary>
        /// Optional horizontal or single axis value to get axis data from Unity's old input system.
        /// </summary>
        public string AxisCodeX => axisCodeX;

        [SerializeField]
        [Tooltip("Optional vertical axis value to get axis data from Unity's old input system.")]
        private string axisCodeY;

        /// <summary>
        /// Optional vertical axis value to get axis data from Unity's old input system.
        /// </summary>
        public string AxisCodeY => axisCodeY;

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

        #region Definition Data Items

        private object rawData;

        private bool boolData;

        private float floatData;

        private Vector2 vector2Data;

        private Vector3 positionData;

        private Quaternion rotationData;

        private MixedRealityPose poseData;

        #endregion Definition Data Items

        #region Data Properties

        /// <summary>
        /// The Raw (object) data value.
        /// </summary>
        /// <remarks>Only supported for a Raw mapping axis type</remarks>
        public object RawData
        {
            get
            {
                return rawData;
            }

            set
            {
                if (AxisType != AxisType.Raw)
                {
                    Debug.LogError($"SetRawValue is only valid for AxisType.Raw InteractionMappings\nPlease check the {inputType} mapping for the current controller");
                }

                Changed = rawData != value;
                rawData = value;
            }
        }

        /// <summary>
        /// The Bool data value.
        /// </summary>
        /// <remarks>Only supported for a Digital mapping axis type</remarks>
        public bool BoolData
        {
            get
            {
                return boolData;
            }

            set
            {
                if (AxisType != AxisType.Digital)
                {
                    Debug.LogError($"SetBoolValue is only valid for AxisType.Digital InteractionMappings\nPlease check the {inputType} mapping for the current controller");
                }

                Changed = boolData != value;
                boolData = value;
            }
        }

        /// <summary>
        /// The Float data value.
        /// </summary>
        /// <remarks>Only supported for a SingleAxis mapping axis type</remarks>
        public float FloatData
        {
            get
            {
                return floatData;
            }

            set
            {
                if (AxisType != AxisType.SingleAxis)
                {
                    Debug.LogError($"SetFloatValue is only valid for AxisType.SingleAxis InteractionMappings\nPlease check the {inputType} mapping for the current controller");
                }

                Changed = !floatData.Equals(value);
                floatData = value;
            }
        }

        /// <summary>
        /// The Vector2 data value.
        /// </summary>
        /// <remarks>Only supported for a DualAxis mapping axis type</remarks>
        public Vector2 Vector2Data
        {
            get
            {
                return vector2Data;
            }

            set
            {
                if (AxisType != AxisType.DualAxis)
                {
                    Debug.LogError($"SetVector2Value is only valid for AxisType.DualAxis InteractionMappings\nPlease check the {inputType} mapping for the current controller");
                }

                Changed = vector2Data != value;
                vector2Data = value;
            }
        }

        /// <summary>
        /// The ThreeDof Vector3 Position data value.
        /// </summary>
        /// <remarks>Only supported for a ThreeDof mapping axis type</remarks>
        public Vector3 PositionData
        {
            get
            {
                return positionData;
            }

            set
            {
                if (AxisType != AxisType.ThreeDofPosition)
                {
                    {
                        Debug.LogError($"SetPositionValue is only valid for AxisType.ThreeDoFPosition InteractionMappings\nPlease check the {inputType} mapping for the current controller");
                    }
                }

                Changed = positionData != value;
                positionData = value;
            }
        }

        /// <summary>
        /// The ThreeDof Quaternion Rotation data value.
        /// </summary>
        /// <remarks>Only supported for a ThreeDof mapping axis type</remarks>
        public Quaternion RotationData
        {
            get
            {
                return rotationData;
            }

            set
            {
                if (AxisType != AxisType.ThreeDofRotation)
                {
                    Debug.LogError($"SetRotationValue is only valid for AxisType.ThreeDoFRotation InteractionMappings\nPlease check the {inputType} mapping for the current controller");
                }

                Changed = rotationData != value;
                rotationData = value;
            }
        }

        /// <summary>
        /// The Pose data value.
        /// </summary>
        /// <remarks>Only supported for a SixDof mapping axis type</remarks>
        public MixedRealityPose PoseData
        {
            get
            {
                return poseData;
            }
            set
            {
                if (AxisType != AxisType.SixDof)
                {
                    Debug.LogError($"SetPoseValue is only valid for AxisType.SixDoF InteractionMappings\nPlease check the {inputType} mapping for the current controller");
                }

                Changed = poseData != value;

                poseData = value;
                positionData = poseData.Position;
                rotationData = poseData.Rotation;
            }
        }

        #endregion Data Properties

    }
}