// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// A component that can be added to InputField to make it work with Windows Mixed Reality's system keyboard.
    /// Required when using Unity 2018.4.
    /// No longer used in Unity 2019.3 and later versions (becomes an empty MonoBehaviour and is only around for compatibility) and you can safely remove it if you wish
    /// </summary>
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