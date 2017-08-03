// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.UI.Keyboard
{
    public class SymbolKeyboard : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Button m_PageBck = null;

        [SerializeField]
        private UnityEngine.UI.Button m_PageFwd = null;

        private void Update()
        {
            // Visual reflection of state.
            m_PageBck.interactable = Keyboard.Instance.IsShifted;
            m_PageFwd.interactable = !Keyboard.Instance.IsShifted;
        }
    }
}
