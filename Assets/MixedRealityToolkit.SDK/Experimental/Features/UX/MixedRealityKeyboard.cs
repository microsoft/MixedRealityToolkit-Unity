// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;

#if !UNITY_EDITOR && UNITY_WSA
using Windows.UI.ViewManagement;
#endif 

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// Class with simple access to the UWP system keyboards text
    /// </summary>
    public class MixedRealityKeyboard : MixedRealityKeyboardBase
    {
        [Experimental]
        #region Properties

        private string text;
        public string Text
        {
            get
            {
                return text;
            }

            private set
            {
                if (text != value)
                {
                    text = value;
                }
            }
        }

        #endregion Properties

        protected override void UpdateText(string text) => Text = text;
    }
}
