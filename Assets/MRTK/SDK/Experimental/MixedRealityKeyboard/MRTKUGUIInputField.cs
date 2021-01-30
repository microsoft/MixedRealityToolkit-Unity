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
        [SerializeField]
        private bool disableUGUIWorkaround = false;

        /// <summary>
        /// Currently there is a Unity bug that needs a workaround. Please keep this setting to be false until an announcement of the version of Unity/UGUI that resolves the issue is made.
        /// </summary>
        public bool DisableUGUIWorkaround
        {
            get => disableUGUIWorkaround;
            set => disableUGUIWorkaround = value;
        }

        protected override void LateUpdate()
        {
            if (!DisableUGUIWorkaround && isFocused && m_Keyboard != null && (UnityEngine.Input.GetKeyDown(KeyCode.Backspace)))
            {
                m_Keyboard.text = m_Text;
            }
            base.LateUpdate();
        }
#else
        public int SelectionPosition
        {
            get => caretSelectPositionInternal;
            set => caretSelectPositionInternal = value;
        }
        public override void OnUpdateSelected(BaseEventData eventData) { }
#endif
    }
}