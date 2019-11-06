// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    public class SymbolKeyboard : MonoBehaviour
    {
        [Experimental]
        [SerializeField]
        private UnityEngine.UI.Button m_PageBck = null;

        [SerializeField]
        private UnityEngine.UI.Button m_PageFwd = null;

        private void Update()
        {
            // Visual reflection of state.
            m_PageBck.interactable = NoneNativeKeyboard.Instance.IsShifted;
            m_PageFwd.interactable = !NoneNativeKeyboard.Instance.IsShifted;
        }
    }
}
