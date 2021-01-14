// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// A component that can be added to InputField to make it work with Windows Mixed Reality's system keyboard.
    /// Will no longer be necessary in Unity 2019.3 and later versions after a Unity/UGUI bug is fixed (see disableUGUIWorkaround).
    /// </summary>
    [RequireComponent(typeof(MRTKUGUIInputField))]
    [AddComponentMenu("Scripts/MRTK/Experimental/Keyboard/UI_KeyboardInputField")]
    public class UI_KeyboardInputField :
#if UNITY_2019_3_OR_NEWER
        MonoBehaviour
#else
        KeyboardInputFieldBase<MRTKUGUIInputField>
#endif
    {
#if !UNITY_2019_3_OR_NEWER
        public override string Text { get => inputField.text; protected set => inputField.text = value; }
        protected override Graphic TextGraphic(MRTKUGUIInputField inputField) => inputField.textComponent;
        protected override Graphic PlaceHolderGraphic(MRTKUGUIInputField inputField) => inputField.placeholder;
        protected override void SyncCaret()
        {
            inputField.caretPosition = CaretIndex;
            inputField.SelectionPosition = CaretIndex;
        }
#else
        [SerializeField, Tooltip("Currently there is a Unity bug that needs a workaround. Please keep this setting to be false until an announcement of the version of Unity/UGUI that resolves the issue is made.")]
        private bool disableUGUIWorkaround = false;

        private MRTKUGUIInputField inputField;
        
        private void OnValidate()
        {
            if (inputField == null)
            {
                inputField = GetComponent<MRTKUGUIInputField>();
            }
            inputField.DisableUGUIWorkaround = disableUGUIWorkaround;
        }
#endif
    }
}