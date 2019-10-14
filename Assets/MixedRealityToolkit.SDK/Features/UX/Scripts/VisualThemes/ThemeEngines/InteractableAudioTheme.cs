// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Theme Engine to play particular audio files based on state changes.
    /// Add AudioSource component if none is found on initialized GameObject or in children
    /// </summary>
    public class InteractableAudioTheme : InteractableThemeBase
    {
        /// <inheritdoc />
        public override bool IsEasingSupported => false;

        private AudioSource audioSource;

        public InteractableAudioTheme()
        {
            Types = new Type[] { typeof(Transform) };
            Name = "Audio Theme";
        }

        /// <inheritdoc />
        public override ThemeDefinition GetDefaultThemeDefinition()
        {
            return new ThemeDefinition()
            {
                ThemeType = GetType(),
                StateProperties = new List<ThemeStateProperty>()
                {
                    new ThemeStateProperty()
                    {
                        Name = "Audio",
                        Type = ThemePropertyTypes.AudioClip,
                        Values = new List<ThemePropertyValue>(),
                        Default = new ThemePropertyValue() { AudioClip = null }
                    },
                },
                CustomProperties = new List<ThemeProperty>(),
            };
        }

        public override void Init(GameObject host, ThemeDefinition settings)
        {
            base.Init(host, settings);
            audioSource = Host.GetComponentInChildren<AudioSource>();
        }

        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();
            AudioSource audioSource = Host.GetComponentInChildren<AudioSource>();
            if (audioSource != null)
            {
                start.AudioClip = audioSource.clip;
            }
            return start;
        }

        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            if (audioSource == null)
            {
                audioSource = Host.AddComponent<AudioSource>();
            }

            audioSource.clip = property.Values[index].AudioClip;
            audioSource.Play();
        }
    }
}
