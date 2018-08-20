// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Blend;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;						
using UnityEngine;

namespace Interact.Widgets
{

    [System.Serializable]
    public struct WidgetThemeData
    {
        public bool UseBlend;
        public BlendInstance Blend;
        public BlendInstanceProperties BlendProperties;
        public bool Inited;
    }

    /// <summary>
    /// A version of InteractiveWidget that uses an InteractiveTheme to define each state
    /// </summary>
    public abstract class InteractiveThemeWidget : InteractiveWidget
    {
        [HideInInspector]
        [Tooltip("A tag for finding the theme in the scene")]
        public string ThemeTag = "defaultTheme";

        [HideInInspector]
        public WidgetThemeData BlendSettings;

        // checks if the theme has changed since the last SetState was called.
        protected bool mThemeUpdated;

        protected string mCheckThemeTag = "";

        /// <summary>
        /// Sets the themes based on the Theme Tags
        /// </summary>
        public abstract void SetTheme();

        /// <summary>
        /// check to see if the widget found a theme yet
        /// </summary>
        /// <returns></returns>
        protected abstract bool HasTheme();

        /// <summary>
        /// For handling BlendInstances within a widget - apply the lerpped theme value to the object
        /// </summary>
        /// <param name="percent"></param>
        protected abstract void ApplyBlendValues(float percent);

        /// <summary>
        /// BlendInstance handling, set the start value, the target value and play
        /// </summary>
        /// <param name="state"></param>
        protected abstract void GetBlendValues(Interactive.ButtonStateEnum state);

        /// <summary>
        /// If the themes have changed since the last SetState was called, update the widget
        /// </summary>
        public void RefreshIfNeeded()
        {
            if (mThemeUpdated)
            {
                SetState(State);
            }
        }

        /// <summary>
        /// Sets the state of the widget
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            mThemeUpdated = !HasTheme();

            if (BlendSettings.Blend != null && BlendSettings.UseBlend)
            {
                GetBlendValues(state);
            }
        }

        /// <summary>
        /// get a new theme if themeTag has changed.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (BlendSettings.UseBlend && BlendSettings.Inited)
            {
                if (BlendSettings.Blend == null)
                {
                    BlendSettings.Blend = new BlendInstance();
                    BlendSettings.Blend.SetupProperties(BlendSettings.BlendProperties);
                }
                else
                {
                    if (BlendSettings.Blend.IsPlaying)
                    {
                        BlendSettings.Blend.Update(Time.deltaTime, BlendSettings.BlendProperties);
                        ApplyBlendValues(BlendSettings.Blend.GetLerpValue());
                    }
                }
            }

            if (!mCheckThemeTag.Equals(ThemeTag) && InteractiveHost != null)
            {
                SetTheme();
                RefreshIfNeeded();
                mCheckThemeTag = ThemeTag;
            }
        }

        public virtual List<string> GetTags()
        {
            List<string> tags = new List<string>();
            Type type = this.GetType();
            FieldInfo[] info = (FieldInfo[])type.GetFields();

            for (int i = 0; i < info.Length; i++)
            {
                if (info[i].FieldType == typeof(string))
                {
                    string tag = (string)info[i].GetValue(this);
                    if (tag != "")
                    {
                        tags.Add(tag);
                    }
                }
            }

            return tags;
        }			 
    }
}
