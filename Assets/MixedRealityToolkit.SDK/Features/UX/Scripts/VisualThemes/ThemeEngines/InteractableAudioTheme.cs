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
            audioSource = host.GetComponentInChildren<AudioSource>();
            base.Init(host, settings);
        }

        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            ThemePropertyValue start = new ThemePropertyValue();
            if (audioSource == null)
            {
                audioSource = Host.GetComponentInChildren<AudioSource>();
            }

            if (audioSource != null)
            {
                start.AudioClip = audioSource.clip;
            }

            return start;
        }

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
        {
            SetValue(property, property.Values[index]);
        }

        /// <inheritdoc />
        protected override void SetValue(ThemeStateProperty property, ThemePropertyValue value)
        {
            if (audioSource == null)
            {
                audioSource = Host.AddComponent<AudioSource>();
            }

            audioSource.clip = value.AudioClip;
            audioSource.Play();
        }
    }
}
