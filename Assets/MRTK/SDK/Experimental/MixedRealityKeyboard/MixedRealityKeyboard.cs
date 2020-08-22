// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// Class that can launch and hide a system keyboard specifically for Windows Mixed Reality
    /// devices (HoloLens 2, Windows Mixed Reality).
    /// 
    /// Implements a workaround for UWP TouchScreenKeyboard bug which prevents
    /// UWP keyboard from showing up again after it is closed.
    /// Unity bug tracking the issue https://fogbugz.unity3d.com/default.asp?1137074_rttdnt8t1lccmtd3
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Experimental/Keyboard/MixedRealityKeyboard")]
    public class MixedRealityKeyboard : MixedRealityKeyboardBase
    {
        /// <summary>
        /// Returns the committed text.
        /// </summary>
        public override string Text
        {
            get;
            protected set;
        } = string.Empty;
    }
}
