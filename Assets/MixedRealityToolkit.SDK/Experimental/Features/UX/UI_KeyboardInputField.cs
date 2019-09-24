// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    [RequireComponent(typeof(InputField))]
    public class UI_KeyboardInputField : KeyboardInputFieldBase<InputField>
    {
#if WINDOWS_UWP
        public override void ShowKeyboard()
        {
            base.ShowKeyboard();
            KeyboardText = inputField.text;
        }
#endif
        protected override void UpdateText(string text) => inputField.text = text;

        protected override Graphic Text(InputField inputField) => inputField.textComponent;
        protected override Graphic PlaceHolder(InputField inputField) => inputField.placeholder;
    }
}