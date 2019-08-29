// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractableAudioTheme : InteractableThemeBase
    {
        private AudioSource audioSource;

        public InteractableAudioTheme()
        {
            Types = new Type[] { typeof(Transform) };
            Name = "Audio Theme";
            NoEasing = true;
        }

        /// <inheritdoc />
        public override ThemeDefinition GetDefaultThemeDefinition()
        {
            Type t = GetType();
            return new ThemeDefinition()
            {
                ClassName = t.Name,
                AssemblyQualifiedName = t.AssemblyQualifiedName,
                Type = t,
                NoEasing = this.NoEasing,
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
