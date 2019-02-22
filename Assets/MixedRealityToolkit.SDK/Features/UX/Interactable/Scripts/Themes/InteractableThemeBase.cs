// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.Themes
{
    /// <summary>
    /// Base class for themes
    /// </summary>
    
    public abstract class InteractableThemeBase
    {
        public Type[] Types;
        public string Name = "Base Theme";
        public List<InteractableThemeProperty> ThemeProperties = new List<InteractableThemeProperty>();
        public List<InteractableThemePropertyValue> CustomSettings = new List<InteractableThemePropertyValue>();
        public GameObject Host;
        public Easing Ease;
        public bool NoEasing;
        public bool Loaded;

        private bool hasFirstState = false;

        private int lastState = -1;

        //! find a way to set the default values of the properties, like scale should be Vector3.one
        // these should be custom, per theme

        public abstract void SetValue(InteractableThemeProperty property, int index, float percentage);

        public abstract InteractableThemePropertyValue GetProperty(InteractableThemeProperty property);

        public virtual void Init(GameObject host, InteractableThemePropertySettings settings)
        {
            Host = host;

            for (int i = 0; i < settings.Properties.Count; i++)
            {
                InteractableThemeProperty prop = ThemeProperties[i];
                prop.ShaderOptionNames = settings.Properties[i].ShaderOptionNames;
                prop.ShaderOptions = settings.Properties[i].ShaderOptions;
                prop.PropId = settings.Properties[i].PropId;
                prop.Values = settings.Properties[i].Values;
                
                
                ThemeProperties[i] = prop;
            }

            Ease = CopyEase(settings.Easing);
            Ease.Stop();

            Loaded = true;

        }

        protected float LerpFloat(float s, float e, float t)
        {
            return (e - s) * t + s;
        }

        protected int LerpInt(int s, int e, float t)
        {
            return Mathf.RoundToInt((e - s) * t) + s;
        }

        protected Easing CopyEase(Easing ease)
        {
            Easing newEase = new Easing();
            newEase.Curve = ease.Curve;
            newEase.Enabled = ease.Enabled;
            newEase.LerpTime = ease.LerpTime;

            return newEase;
        }

        public virtual void OnUpdate(int state, bool force = false)
        {
            if(state != lastState || force)
            {
                int themePropCount = ThemeProperties.Count;
                for (int i = 0; i < themePropCount; i++)
                {
                    InteractableThemeProperty current = ThemeProperties[i];
                    current.StartValue = GetProperty(current);
                    if (hasFirstState || force)
                    {
                        Ease.Start();
                        SetValue(current, state, Ease.GetCurved());
                        hasFirstState = true;
                    }
                    else
                    {
                        SetValue(current, state, 1);
                        if(i >= themePropCount - 1)
                        {
                            hasFirstState = true;
                        }
                    }
                    ThemeProperties[i] = current;
                }

                lastState = state;
            }
            else if(Ease.Enabled && Ease.IsPlaying())
            {
                Ease.OnUpdate();
                int themePropCount = ThemeProperties.Count;
                for (int i = 0; i < themePropCount; i++)
                {
                    InteractableThemeProperty current = ThemeProperties[i];
                    SetValue(current, state, Ease.GetCurved());
                }
            }

            lastState = state;
        }
    }
}
