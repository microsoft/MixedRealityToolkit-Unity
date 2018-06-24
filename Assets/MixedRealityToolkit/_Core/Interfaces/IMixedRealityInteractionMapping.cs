// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces
{
    /// <summary>
    /// Mixed Reality Toolkit Interaction Mapping interface. Provides the abstraction for the different types of mapping available.
    /// </summary>
    public interface IMixedRealityInteractionMapping
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
        IMixedRealityInputAction InputAction { get; }

        /// <summary>
        /// Has the value changed since the last reading.
        /// </summary>
        bool Changed { get; }

        object GetRawValue();
        bool GetBooleanValue();
        float GetFloatValue();
        Vector2 GetVector2Value();
        Vector3 GetPosition();
        Quaternion GetRotation();
        SixDof GetSixDof();

        void SetValue(object newValue);
        void SetValue(bool newValue);
        void SetValue(float newValue);
        void SetValue(Vector2 newValue);
        void SetValue(Vector3 newValue);
        void SetValue(Quaternion newValue);
        void SetValue(SixDof newValue);

    }
}