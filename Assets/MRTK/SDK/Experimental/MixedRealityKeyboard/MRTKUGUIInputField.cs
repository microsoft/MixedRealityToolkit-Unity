// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.UI;

#if !UNITY_2019_3_OR_NEWER
using UnityEngine.EventSystems;
#endif // !UNITY_2019_3_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// A derived class of UGUI's InputField to workaround with some issues of typing on HoloLens 2 specific to Unity 2018.4
    /// </summary>
    /// <remarks>
    /// <para>If using Unity 2019 or 2020, make sure the version >= 2019.4.25 or 2020.3.2 to ensure the latest fixes for Unity keyboard bugs are present.</para>
    /// </remarks>
    public class MRTKUGUIInputField : InputField
    {
#if !UNITY_2019_3_OR_NEWER
        public int SelectionPosition
        {
            get => caretSelectPositionInternal;
            set => caretSelectPositionInternal = value;
        }
        public override void OnUpdateSelected(BaseEventData eventData) { }
#endif // !UNITY_2019_3_OR_NEWER
    }
}