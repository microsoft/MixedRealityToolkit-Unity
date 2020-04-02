// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// A component that can be added to TMP_InputField to make them work with Windows Mixed Reality's system keyboard
    /// </summary>
    [RequireComponent(typeof(TMP_InputField))]
    [AddComponentMenu("Scripts/MRTK/Experimental/MixedRealityKeyboard")]
    public class TMP_KeyboardInputField : KeyboardInputFieldBase<TMP_InputField>
    {
        public override string Text { get => inputField.text; protected set => inputField.text = value; }
        protected override Graphic TextGraphic(TMP_InputField inputField) => inputField.textComponent;
        protected override Graphic PlaceHolderGraphic(TMP_InputField inputField) => inputField.placeholder;
    }
}