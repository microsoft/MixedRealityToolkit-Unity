// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Represents the subset of data held by a <see cref="MixedRealityInteractionMapping"/> that represents Unity's legacy input system.
    /// </summary>
    public struct MixedRealityInteractionMappingLegacyInput
    {
        /// <summary>
        /// Optional KeyCode value to get input from Unity's old input system.
        /// </summary>
        public KeyCode KeyCode { get; }

        /// <summary>
        /// Optional horizontal or single axis value to get axis data from Unity's old input system.
        /// </summary>
        public string AxisCodeX { get; }

        /// <summary>
        /// Optional vertical axis value to get axis data from Unity's old input system.
        /// </summary>
        public string AxisCodeY { get; }

        /// <summary>
        /// Should the X axis be inverted?
        /// </summary>
        public bool InvertXAxis { get; }

        /// <summary>
        /// Should the Y axis be inverted?
        /// </summary>
        public bool InvertYAxis { get; }

        public MixedRealityInteractionMappingLegacyInput(KeyCode keyCode = KeyCode.None, string axisCodeX = "", string axisCodeY = "", bool invertXAxis = false, bool invertYAxis = false)
        {
            KeyCode = keyCode;
            AxisCodeX = axisCodeX;
            AxisCodeY = axisCodeY;
            InvertXAxis = invertXAxis;
            InvertYAxis = invertYAxis;
        }
    }
}
