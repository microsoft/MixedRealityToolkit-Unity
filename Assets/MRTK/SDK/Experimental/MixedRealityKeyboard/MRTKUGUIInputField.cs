// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// A derived class of UGUI's InputField to workaround with some issues of typing on HoloLens 2
    /// </summary>
    public class MRTKUGUIInputField : InputField
    {
#if UNITY_2019_3_OR_NEWER
        [SerializeField, Tooltip("Currently there is a Unity bug that needs a workaround. Please keep this setting to be true until an announcement of the version of Unity/UGUI that resolves the issue is made.")]
        private bool enableUGUIWorkaround = false;
#endif
#if !UNITY_2019_3_OR_NEWER
        public int SelectionPosition
        {
            get => caretSelectPositionInternal;
            set => caretSelectPositionInternal = value;
        }
        public override void OnUpdateSelected(BaseEventData eventData) { }
#else
        protected override void LateUpdate()
        {
            if (enableUGUIWorkaround && isFocused && m_Keyboard != null && (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKey(KeyCode.Backspace)))
            {
                m_Keyboard.text = m_Text;
            }
            base.LateUpdate();
        }
#endif
    }
}