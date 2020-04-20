// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// A component that can be added to InputField to make them work with Windows Mixed Reality's system keyboard
    /// </summary>
    [RequireComponent(typeof(InputField))]
    [AddComponentMenu("Scripts/MRTK/Experimental/MixedRealityKeyboard")]
    public class UI_KeyboardInputField : KeyboardInputFieldBase<InputField>
    {
        public override string Text { get => inputField.text; protected set => inputField.text = value; }
        protected override Graphic TextGraphic(InputField inputField) => inputField.textComponent;
        protected override Graphic PlaceHolderGraphic(InputField inputField) => inputField.placeholder;
    }
}