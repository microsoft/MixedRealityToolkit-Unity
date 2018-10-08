// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    /// <summary>
    /// Base class for themes
    /// </summary>
    
    public abstract class ThemeBase
    {
        public Type[] Types;
        public string Name = "Base Theme";
        public List<ThemeProperty> ThemeProperties = new List<ThemeProperty>();
        public List<ThemePropertyValue> CustomSettings = new List<ThemePropertyValue>();
        public GameObject Host;
        public ThemeEaseSettings Ease;
        public bool Loaded;

        private bool hasFirstState = false;

        private int lastState = -1;

        //! find a way to set the default values of the properties, like scale should be Vector3.one
        // these should be custom, per theme

        public abstract void SetValue(ThemeProperty property, int index, float percentage);

        public abstract ThemePropertyValue GetProperty(ThemeProperty property);

        public virtual void Init(GameObject host, ThemePropertySettings settings)
        {
            Host = host;

            for (int i = 0; i < settings.Properties.Count; i++)
            {
                ThemeProperty prop = ThemeProperties[i];
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

        protected ThemeEaseSettings CopyEase(ThemeEaseSettings ease)
        {
            ThemeEaseSettings newEase = new ThemeEaseSettings();
            newEase.Curve = ease.Curve;
            newEase.EaseValues = ease.EaseValues;
            newEase.LerpTime = ease.LerpTime;

            return newEase;
        }

        public virtual void OnUpdate(int state, bool force = false)
        {
            if(state != lastState || force)
            {
                for (int i = 0; i < ThemeProperties.Count; i++)
                {
                    ThemeProperty current = ThemeProperties[i];
                    current.StartValue = GetProperty(current);
                    if (hasFirstState)
                    {
                        Ease.Start();
                        SetValue(current, state, Ease.GetCurved());
                    }
                    else
                    {
                        SetValue(current, state, 1);
                        if(i >= ThemeProperties.Count - 1)
                        {
                            hasFirstState = true;
                        }
                    }
                    ThemeProperties[i] = current;
                }

                lastState = state;
            }
            else if(Ease.EaseValues && Ease.IsPlaying())
            {
                Ease.OnUpdate();
                for (int i = 0; i < ThemeProperties.Count; i++)
                {
                    ThemeProperty current = ThemeProperties[i];
                    SetValue(current, state, Ease.GetCurved());
                }
            }

            lastState = state;
        }
    }
}
