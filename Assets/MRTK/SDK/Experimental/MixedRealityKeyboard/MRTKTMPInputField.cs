// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro;

#if !UNITY_2019_3_OR_NEWER
using UnityEngine.EventSystems;
#endif // !UNITY_2019_3_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// A derived class of TMP's InputField to workaround with some issues of typing on HoloLens 2 specific to Unity 2018.4
    /// </summary>
    /// <remarks>
    /// <para>If using Unity 2019 or 2020, make sure the version >= 2019.4.25 or 2020.3.2 to ensure the latest fixes for Unity keyboard bugs are present.</para>
    /// <para>There is a known Unity/TMP issue preventing the caret from showing up. Please see https://github.com/microsoft/MixedRealityToolkit-Unity/issues/9056 for updates.</para>
    /// </remarks>
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