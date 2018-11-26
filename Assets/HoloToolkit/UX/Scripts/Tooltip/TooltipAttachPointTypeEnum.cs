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
        TopMiddle,
        RightMiddle,
        LeftMiddle,
        BotRightCorner,
        BotLeftCorner,
        TopRightCorner,
        TopLeftCorner,
        // Automatic options
        Center,
        Closest,
        ClosestMiddle,
        ClosestCorner
    }
}
