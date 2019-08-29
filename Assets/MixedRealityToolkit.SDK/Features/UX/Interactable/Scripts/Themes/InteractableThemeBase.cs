// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Base class for themes
    /// </summary>

    public abstract class InteractableThemeBase
    {
        public Type[] Types;
        public string Name = "Base Theme";
        public List<ThemeStateProperty> StateProperties = new List<ThemeStateProperty>();
        public List<ThemeProperty> Properties = new List<ThemeProperty>();
        public GameObject Host;
        public Easing Ease;
        public bool NoEasing;
        public bool Loaded;
        public string AssemblyQualifiedName;

        private bool hasFirstState = false;

        private int lastState = -1;

        //! find a way to set the default values of the properties, like scale should be Vector3.one
        // these should be custom, per theme

        public abstract void SetValue(ThemeStateProperty property, int index, float percentage);

        public abstract ThemePropertyValue GetProperty(ThemeStateProperty property);

        // TODO: Troy - Add comment here
        public abstract ThemeDefinition GetDefaultThemeDefinition();

        public virtual void Init(GameObject host, ThemeDefinition settings)
        {
            Host = host;

            for (int i = 0; i < settings.StateProperties.Count; i++)
            {
                ThemeStateProperty prop = StateProperties[i];
                prop.ShaderOptionNames = settings.StateProperties[i].ShaderOptionNames;
                prop.ShaderOptions = settings.StateProperties[i].ShaderOptions;
                prop.PropId = settings.StateProperties[i].PropId;
                prop.Values = settings.StateProperties[i].Values;
                
                StateProperties[i] = prop;
            }

            for (int i = 0; i < settings.CustomProperties.Count; i++)
            {
                ThemeProperty setting = Properties[i];
                setting.Name = settings.CustomProperties[i].Name;
                setting.Type = settings.CustomProperties[i].Type;
                setting.Value = settings.CustomProperties[i].Value;
                Properties[i] = setting;
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

        public virtual void OnUpdate(int state, Interactable source, bool force = false)
        {

            if (state != lastState || force)
            {
                int themePropCount = StateProperties.Count;
                for (int i = 0; i < themePropCount; i++)
                {
                    ThemeStateProperty current = StateProperties[i];
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
                        if (i >= themePropCount - 1)
                        {
                            hasFirstState = true;
                        }
                    }
                    StateProperties[i] = current;
                }

                lastState = state;
            }
            else if (Ease.Enabled && Ease.IsPlaying())
            {
                Ease.OnUpdate();
                int themePropCount = StateProperties.Count;
                for (int i = 0; i < themePropCount; i++)
                {
                    ThemeStateProperty current = StateProperties[i];
                    SetValue(current, state, Ease.GetCurved());
                }
            }

            lastState = state;
        }
    }
}
