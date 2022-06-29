// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// All themable assets for one musical note. In this case it includes a name to be used
    /// on a button, and the audio clip that will play when activated.
    /// </summary>
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
    /// A simple musical note theming profile in the form of a ScriptableObject, that can be used as a DataSource via DataSourceReflection.
    /// </summary>
    /// <remarks>
    /// By creating multiple profiles out of this ScriptableObject, each profile can contain a different instrument, or a different octave.
    /// These can then be easily swapped via helper scripts such as the ThemeSelector.
    /// </remarks>
    [CreateAssetMenu(fileName = "Example_MusicInformation_Profile", menuName = "MRTK/Examples/Music Information Profile")]
    [Serializable]
    public class MusicInformationProfile : ScriptableObject
    {
        [Tooltip("A list of musical notes providing name and audioclip for each.")]
        [SerializeField]
        private MusicalNoteInformation[] notes;
        public MusicalNoteInformation[] Notes => notes;
    }
}
