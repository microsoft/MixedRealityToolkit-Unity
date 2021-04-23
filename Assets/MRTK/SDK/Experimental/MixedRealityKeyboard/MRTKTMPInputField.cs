// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// A derived class of TMP's InputField to workaround with some issues of typing on HoloLens 2
    /// No longer used in Unity 2019.3 and later versions.
    /// </summary>
    public class MRTKTMPInputField : TMP_InputField
    {
#if !UNITY_2019_3_OR_NEWER
        public int SelectionPosition
        {
            get => caretSelectPositionInternal;
            set
            {
                caretSelectPositionInternal = value;
                selectionStringFocusPosition = value;
                selectionStringAnchorPosition = value;
            }
        }
        public override void OnUpdateSelected(BaseEventData eventData) { }
#endif
    }
}