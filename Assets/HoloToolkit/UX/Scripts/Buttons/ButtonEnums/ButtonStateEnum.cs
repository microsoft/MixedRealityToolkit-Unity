//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

namespace HoloToolkit.Unity.Buttons
{
    /// <summary>
    /// State enum for buttons
    /// </summary>
    public enum ButtonStateEnum
    {
        /// <summary>
        /// Looking at and Pressed
        /// </summary>
        Pressed,
        /// <summary>
        /// Looking at and finger up
        /// </summary>
        Targeted,
        /// <summary>
        /// Not looking at it and finger is up
        /// </summary>
        Interactive,
        /// <summary>
        /// Looking at button finger down
        /// </summary> 
        ObservationTargeted,
        /// <summary>
        /// Not looking at it and finger down
        /// </summary>
        Observation,
        /// <summary>
        /// Button in a disabled state
        /// </summary>
        Disabled,
    }
}