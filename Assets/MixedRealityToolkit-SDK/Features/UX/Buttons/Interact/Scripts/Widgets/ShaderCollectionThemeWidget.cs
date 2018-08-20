// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Blend;
using Interact.Themes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Widgets
{
    [System.Serializable]
    public struct BlendShaderWidgetData
    {
        public string Tag;
        public ColorInteractiveTheme ColorTheme;
        public FloatInteractiveTheme FloatTheme;
        public bool Blend;
        public BlendShaderData BlendData;
    }

    public class ShaderCollectionThemeWidget : InteractiveThemeWidget
    {
        [HideInInspector]
        public List<BlendShaderWidgetData> BlendData = new List<BlendShaderWidgetData>();

        private BlendShaderCollection collection;

        private void Awake()
        {
            collection = new BlendShaderCollection(this);
            // set the themes

        }

        private void UpateBlendData()
        {
            List<BlendShaderData> shaderData = new List<BlendShaderData>();

            for (int i = 0; i < BlendData.Count; i++)
            {
                BlendShaderWidgetData data = BlendData[i];

                if (data.BlendData.ShaderProperty.Type == ShaderPropertyType.Color)
                {
                    // get ColorTheme value
                    if (data.ColorTheme != null)
                    {
                        data.BlendData.ColorValues.TargetValue = data.ColorTheme.GetThemeValue(State);
                    }
                }
                else
                {
                    // get FloatTheme values
                    if (data.FloatTheme != null)
                    {
                        data.BlendData.FloatValues.TargetValue = data.FloatTheme.GetThemeValue(State);
                    }
                }

                if (!data.Blend)
                {
                    data.BlendData.InstanceProperties.LerpTime = 0;
                }

                BlendData[i] = data;

                shaderData.Add(data.BlendData);
            }

            collection.UpdateData(shaderData);
        }

        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);

            UpateBlendData();
            collection.Play();
        }

        private void OnDestroy()
        {
            collection.Destroy();
        }

        public override void SetTheme()
        {
            for (int i = 0; i < BlendData.Count; i++)
            {
                BlendShaderWidgetData widgetData = BlendData[i];
                if (widgetData.BlendData.ShaderProperty.Type == ShaderPropertyType.Color)
                {
                    if (widgetData.ColorTheme == null || widgetData.ColorTheme.Tag != widgetData.Tag)
                    {
                        widgetData.ColorTheme = InteractiveThemeManager.GetColorTheme(widgetData.Tag);
                    }
                }
                else
                {
                    if (widgetData.FloatTheme == null || widgetData.FloatTheme.Tag != widgetData.Tag)
                    {
                        widgetData.FloatTheme = InteractiveThemeManager.GetFloatTheme(widgetData.Tag);
                    }
                }

                BlendData[i] = widgetData;
            }
        }

        protected override bool HasTheme()
        {
            int themeCount = 0;

            for (int i = 0; i < BlendData.Count; i++)
            {
                if (BlendData[i].BlendData.ShaderProperty.Type == ShaderPropertyType.Color && BlendData[i].ColorTheme != null)
                {
                    if (BlendData[i].ColorTheme != null)
                    {
                        themeCount++;
                    }
                }
                else
                {
                    if (BlendData[i].FloatTheme != null)
                    {
                        themeCount++;
                    }
                }
            }

            return themeCount >= BlendData.Count;
        }

        protected override void ApplyBlendValues(float percent)
        {
            //throw new System.NotImplementedException();
        }

        protected override void GetBlendValues(Interactive.ButtonStateEnum state)
        {
            //throw new System.NotImplementedException();
        }
    }
}
