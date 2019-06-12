// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


#if !UNITY_EDITOR && UNITY_WSA
#endif 


namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    public class KeyboardTextData
    {
        public KeyboardTextData(string oldValue, string newValue, MixedRealityKeyboard keyboard)
        {
            OldValue = oldValue;
            NewValue = newValue;
            Keyboard = keyboard;
        }

        public string OldValue { get; private set; }
        public string NewValue { get; private set; }
        public MixedRealityKeyboard Keyboard { get; private set; }
    }
}