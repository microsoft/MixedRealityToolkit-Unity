// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity.Buttons
{
    /// <summary>
    /// State enum for buttons.
    /// </summary>
    public enum ButtonStateEnum
    {
        /// <summary>
        /// Looking at button and pressed.
        /// </summary>
        Pressed,
        /// <summary>
        /// Looking at button and spatial input source present.
        /// </summary>
        Targeted,
        /// <summary>
        /// Not looking at button and spatial input source present.
        /// </summary>
        Interactive,
        /// <summary>
        /// Looking at button and no spatial input source present.
        /// </summary> 
        ObservationTargeted,
        /// <summary>
        /// Not looking at button and no spatial input source present.
        /// </summary>
        Observation,
        /// <summary>
        /// Button in a disabled state.
        /// </summary>
        Disabled,
    }
}