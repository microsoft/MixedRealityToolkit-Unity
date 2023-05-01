// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Speech.Windows
{
    /// <summary>
    /// The en-US voices that can be used by <see cref="TextToSpeech"/>. Voices for all other locales are categorized as Other.
    /// </summary>
    public enum TextToSpeechVoice
    {
        /// <summary>
        /// The default system voice.
        /// </summary>
        Default,

        /// <summary>
        /// Microsoft David voice
        /// </summary>
        David,

        /// <summary>
        /// Microsoft Mark voice
        /// </summary>
        Mark,

        /// <summary>
        /// Microsoft Zira voice
        /// </summary>
        Zira,

        /// <summary>
        /// Voice not listed above (for non en-US languages)
        /// </summary>
        Other
    }
}
