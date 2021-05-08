// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

#if !UNITY_2019_3_OR_NEWER
using UnityEngine.UI;
#endif // !UNITY_2019_3_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// A component that can be added to InputField to make it work with Windows Mixed Reality's system keyboard.
    /// Only used in Unity 2018.4.
    /// No longer used in Unity 2019.3 and later versions (becomes an empty MonoBehaviour and is only around for compatibility) and you can safely remove it if you wish
    /// </summary>
    /// <remarks>
    /// <para>If using Unity 2019 or 2020, make sure the version >= 2019.4.25 or 2020.3.2 to ensure the latest fixes for Unity keyboard bugs are present.</para>
    /// <para>There is a known Unity/TMP issue preventing the caret from showing up. Please see https://github.com/microsoft/MixedRealityToolkit-Unity/issues/9056 for updates.</para>
    /// </remarks>
#if !UNITY_2019_3_OR_NEWER
    [RequireComponent(typeof(MRTKTMPInputField))]
    [AddComponentMenu("Scripts/MRTK/Experimental/Keyboard/TMP_KeyboardInputField")]
#endif
    public class TMP_KeyboardInputField :
#if UNITY_2019_3_OR_NEWER
        MonoBehaviour
#else
        KeyboardInputFieldBase<MRTKTMPInputField>
#endif
    {
#if !UNITY_2019_3_OR_NEWER
        public override string Text { get => inputField.text; protected set => inputField.text = value; }
        protected override Graphic TextGraphic(MRTKTMPInputField inputField) => inputField.textComponent;
        protected override Graphic PlaceHolderGraphic(MRTKTMPInputField inputField) => inputField.placeholder;
        protected override void SyncCaret()
        {
            inputField.caretPosition = CaretIndex;
            inputField.SelectionPosition = CaretIndex;
        }
#endif
    }
}