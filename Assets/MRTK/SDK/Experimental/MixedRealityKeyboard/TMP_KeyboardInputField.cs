// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// A component that can be added to InputField to make them work with Windows Mixed Reality's system keyboard.
    /// No longer used in Unity 2019.3 and later versions.
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