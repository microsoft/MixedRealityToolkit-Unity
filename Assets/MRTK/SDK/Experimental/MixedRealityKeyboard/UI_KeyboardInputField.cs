// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// A component that can be added to InputField to make them work with Windows Mixed Reality's system keyboard
    /// </summary>
    [RequireComponent(typeof(InputField))]
    [AddComponentMenu("Scripts/MRTK/Experimental/Keyboard/UI_KeyboardInputField")]
    public class UI_KeyboardInputField : KeyboardInputFieldBase<InputField>
    {
        public override string Text { get => inputField.text; protected set => inputField.text = value; }
        protected override Graphic TextGraphic(InputField inputField) => inputField.textComponent;
        protected override Graphic PlaceHolderGraphic(InputField inputField) => inputField.placeholder;
    }
}