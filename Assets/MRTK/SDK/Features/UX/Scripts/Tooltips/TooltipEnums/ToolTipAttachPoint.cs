// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.using System.Collections;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Used to find a pivot point that is closest to the 
    /// anchor. This ensures a natural-looking attachment where the connector line
    /// meets the label.
    /// </summary>
    /// <remarks>
    /// These double as array positions.
    /// </remarks>
    public enum ToolTipAttachPoint
    {
        #region Specific options

        BottomMiddle = 0,
        TopMiddle,
        RightMiddle,
        LeftMiddle,
        BottomRightCorner,
        BottomLeftCorner,
        TopRightCorner,
        TopLeftCorner,

        #endregion Specific options

        #region Automatic options

        Center,
        Closest,
        ClosestMiddle,
        ClosestCorner

        #endregion Automatic options
    }
}
