// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for samples. While nice to have, this XML documentation is not required for samples.
#pragma warning disable CS1591

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// All theme assets for one musical note.
    /// </summary>
    /// <remarks>
    /// In this case it includes a name to be used
    /// on a button, and the audio clip that will play when activated.
    /// </remarks>
    [Serializable]
    public class MusicalNoteInformation
    {
        [SerializeField]
        private string name;
        public string Name => name;

        [SerializeField]
        private AudioClip musicalNote;
        public AudioClip MusicalNote => musicalNote;
    }

    /// <summary>
    /// A simple musical note theming profile in the form of a <see cref="ScriptableObject"/>, 
    /// that can be used as a data source.
    /// </summary>
    /// <remarks>
    /// By creating multiple profiles out of this <see cref="ScriptableObject"/>,, each profile can contain a different instrument, or a different octave.
    /// These can then be easily swapped via helper scripts such as the ThemeSelector.
    /// </remarks>
    [Serializable]
    [CreateAssetMenu(fileName = "Example_MusicInformation_Profile", menuName = "MRTK/Examples/Music Information Profile")]
    public class MusicInformationProfile : ScriptableObject
    {
        [SerializeField]
        [Tooltip("A list of musical notes providing name and audio clip for each.")]
        private MusicalNoteInformation[] notes;

        /// <summary>
        /// A list of musical notes providing name and audio clip for each.
        /// </summary>
        public MusicalNoteInformation[] Notes => notes;
    }
}
#pragma warning restore CS1591