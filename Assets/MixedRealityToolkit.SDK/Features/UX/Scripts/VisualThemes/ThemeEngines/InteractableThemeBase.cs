// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Base abstract class for all Theme Engines. Extend to create custom Theme logic
    /// </summary>
    public abstract class InteractableThemeBase
    {
        /// <summary>
        /// Types of component this Theme Engine will target on the initialized GameObject or related GameObjects
        /// </summary>
        public Type[] Types { get; protected set; } = Array.Empty<Type>();

        /// <summary>
        /// Name of Theme Engine
        /// </summary>
        public string Name { get; protected set; } = "Base Theme";

        /// <summary>
        /// List of Properties with values per state
        /// </summary>
        public List<ThemeStateProperty> StateProperties { get; set; } = new List<ThemeStateProperty>();

        /// <summary>
        /// List of global Theme Engine properties
        /// </summary>
        public List<ThemeProperty> Properties { get; set; } = new List<ThemeProperty>();

        /// <summary>
        /// GameObject initialized with this ThemeEngine and being targeted based on state changes
        /// </summary>
        public GameObject Host { get; set; }

        /// <summary>
        /// Defines how to ease between values during state changes
        /// </summary>
        public Easing Ease { get; set; } = new Easing();

        /// <summary>
        /// True if Theme Engine has been initialized, false otherwise
        /// </summary>
        public bool Loaded { get; protected set; } = false;

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
        /// <returns>Default ThemeDefinition to initialize with the current theme engine implementation</returns>
        public abstract ThemeDefinition GetDefaultThemeDefinition();

        private bool hasFirstState = false;
        private int lastState = -1;

        /// <summary>
        /// Helper method to instantiate a Theme Engine of provided type. Type must extend InteractableThemeBase
        /// </summary>
        /// <param name="themeType">Type of ThemeEngine to create</param>
        /// <returns>Instance of ThemeEngine of given type</returns>
        public static InteractableThemeBase CreateTheme(Type themeType)
        {
            if (!themeType.IsSubclassOf(typeof(InteractableThemeBase)))
            {
                Debug.LogError($"Trying to initialize theme of type {themeType} but type does not extend {typeof(InteractableThemeBase)}");
                return null;
            }

            return (InteractableThemeBase)Activator.CreateInstance(themeType);
        }

        /// <summary>
        /// Helper method to create and initialize a Theme Engine for given configuration and targeted GameObject
        /// </summary>
        /// <param name="definition">Theme configuration with type information and properties to initialize ThemeEngine with</param>
        /// <param name="host">GameObject for Theme Engine to target</param>
        /// <returns>Instance of Theme Engine initialized</returns>
        public static InteractableThemeBase CreateAndInitTheme(ThemeDefinition definition, GameObject host = null)
        {
            var theme = CreateTheme(definition.ThemeType);
            theme.Init(host, definition);
            return theme;
        }

        /// <summary>
        /// Initialize current Theme Engine with given configuration and target the provided GameObject
        /// </summary>
        /// <param name="host">GameObject to target changes against</param>
        /// <param name="definition">Configuration information to initialize Theme Engine</param>
        public virtual void Init(GameObject host, ThemeDefinition definition)
        {
            Host = host;

            this.StateProperties = new List<ThemeStateProperty>();
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

            this.Properties = new List<ThemeProperty>();
            foreach (ThemeProperty prop in definition.CustomProperties)
            {
                this.Properties.Add(new ThemeProperty()
                {
                    Name = prop.Name,
                    Type = prop.Type,
                    Value = prop.Value,
                });
            }

            if (definition.Easing != null)
            {
                Ease = definition.Easing.Copy();
                Ease.Stop();
            }

            Loaded = true;
        }

        /// <summary>
        /// Update ThemeEngine for given state based on Theme logic. Check, sets, and possibly eases values based on given state
        /// </summary>
        /// <param name="state">current state to target</param>
        /// <param name="force">force update call even if state is not new</param>
        public virtual void OnUpdate(int state, bool force = false)
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
