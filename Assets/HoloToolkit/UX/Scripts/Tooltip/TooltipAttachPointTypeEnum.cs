using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.UX.ToolTips
{
    /// <summary>
    /// Used to find a pivot point that is closest to the 
    /// anchor. This ensures a natural-looking attachment where the connector line
    /// meets the label.
    /// </summary>
    public enum ToolTipAttachPointType
    {
        // Specific options
        // These double as array positions
        BotMiddle = 0,
        TopMiddle = 1,
        RightMiddle = 2,
        LeftMiddle = 3,
        BotRightCorner = 4,
        BotLeftCorner = 5,
        TopRightCorner = 6,
        TopLeftCorner = 7,
        // Automatic options
        Center,
        Closest,
        ClosestMiddle,
        ClosestCorner,
        // Smoothly interpolate between positions
        // (UNIMPLEMENTED)
        //Continuous,
    }
}
