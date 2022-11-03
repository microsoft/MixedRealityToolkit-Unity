// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)                                           

using System;
using System.Runtime.InteropServices;

namespace Microsoft.MixedReality.Toolkit.Speech.Windows
{
    public static class WinRTTextToSpeechPInvokes
    {
        [DllImport(
            "WinRTTextToSpeech.dll",
            EntryPoint = "TrySynthesizePhrase",
            CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool TrySynthesizePhrase(
            string phrase,
            out IntPtr data,
            out int length);

        [DllImport(
            "WinRTTextToSpeech.dll",
            EntryPoint = "FreeSynthesizedData",
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool FreeSynthesizedData(IntPtr data);
    }
}
#endif
