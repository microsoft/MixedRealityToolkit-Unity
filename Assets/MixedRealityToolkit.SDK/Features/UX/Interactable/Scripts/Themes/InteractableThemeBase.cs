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
        public bool Loaded;

        /// <summary>
        /// Indicates whether the current Theme engine implementation supports easing between state values
        /// </summary>
        public virtual bool IsEasingSupported => true;

        /// <summary>
        /// Indicates whether the current Theme engine implementation supports shader targeting on state properties
        /// </summary>
        public virtual bool AreShadersSupported => false;

        /// <summary>
        /// Instruct theme to set value for current property with given index state and at given lerp percentage
        /// </summary>
        /// <param name="property">property to update value</param>
        /// <param name="index">index of state to access array of values</param>
        /// <param name="percentage">percentage transition between values</param>
        public abstract void SetValue(ThemeStateProperty property, int index, float percentage);

        /// <summary>
        /// Get the current property value for the provided state property
        /// </summary>
        /// <param name="property">state property to access</param>
        /// <returns>Value currently for given state property</returns>
        public abstract ThemePropertyValue GetProperty(ThemeStateProperty property);

        /// <summary>
        /// Generates the default theme definition configuration for the current theme implementation
        /// </summary>
        /// <returns>Default ThemeDefinition to initialize with the current theme engine implemenetation</returns>
        public abstract ThemeDefinition GetDefaultThemeDefinition();

        private bool hasFirstState = false;
        private int lastState = -1;

        public static InteractableThemeBase CreateTheme(Type themeType)
        {
            if (!themeType.IsSubclassOf(typeof(InteractableThemeBase)))
            {
                Debug.LogError($"Trying to initialize theme of type {themeType} but type does not extend {typeof(InteractableThemeBase)}");
                return null;
            }

            return (InteractableThemeBase)Activator.CreateInstance(themeType);
        }

        public static InteractableThemeBase CreateAndInitTheme(ThemeDefinition definition, GameObject host = null)
        {
            var theme = CreateTheme(definition.ThemeType);
            theme.Init(host, definition);
            return theme;
        }

        public virtual void Init(GameObject host, ThemeDefinition definition)
        {
            Host = host;

            this.StateProperties = new List<ThemeStateProperty>(definition.StateProperties.Count);
            foreach (ThemeStateProperty stateProp in definition.StateProperties)
            {
                // This is a temporary workaround to support backward compatible themes
                // If the current state properties is one we know supports shaders, try to migrate data
                // See ThemeStateProperty class for more details
                if (ThemeStateProperty.IsShaderPropertyType(stateProp.Type))
                {
                    stateProp.MigrateShaderData();
                }

                this.StateProperties.Add(new ThemeStateProperty()
                {
                    Name = stateProp.Name,
                    Type = stateProp.Type,
                    Values = stateProp.Values,
                    Default = stateProp.Default,
                    TargetShader = stateProp.TargetShader,
                    ShaderPropertyName = stateProp.ShaderPropertyName,
                });
            }

            this.Properties = new List<ThemeProperty>(definition.CustomProperties.Count);
            foreach (ThemeProperty prop in definition.CustomProperties)
            {
                this.Properties.Add(new ThemeProperty()
                {
                    Name = prop.Name,
                    Type = prop.Type,
                    Value = prop.Value,
                });
            }

            Ease = definition.Easing.Copy();
            Ease.Stop();

            Loaded = true;
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

        protected float LerpFloat(float s, float e, float t)
        {
            return (e - s) * t + s;
        }

        protected int LerpInt(int s, int e, float t)
        {
            return Mathf.RoundToInt((e - s) * t) + s;
        }
    }
}
