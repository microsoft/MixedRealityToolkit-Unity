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
    public struct BlendTransformWidgetData
    {
        public string Tag;
        public Vector3InteractiveTheme VectorTheme;
        public FloatInteractiveTheme FloatTheme;
        public QuaternionInteractiveTheme QuaternionTheme;
        public bool Blend;
        public BlendTransformData BlendData;
    }

    public class TransformCollectionThemeWidget : InteractiveThemeWidget
    {
        [HideInInspector]
        public List<BlendTransformWidgetData> BlendData = new List<BlendTransformWidgetData>();

        private BlendTransformCollection collection;

        private void Awake()
        {
            collection = new BlendTransformCollection(this);
            // set the themes

        }

        private void UpateBlendData()
        {
            List<BlendTransformData> transformData = new List<BlendTransformData>();

            for (int i = 0; i < BlendData.Count; i++)
            {
                BlendTransformWidgetData data = BlendData[i];

                if (data.BlendData.TransformProperties.Type == TransformTypes.Quaternion)
                {
                    if (data.QuaternionTheme != null)
                    {
                        data.BlendData.QuaternionValues.TargetValue = data.QuaternionTheme.GetThemeValue(State);
                    }
                }
                else
                {
                    if (data.VectorTheme != null)
                    {
                        data.BlendData.VectorValues.TargetValue = data.VectorTheme.GetThemeValue(State);
                    }
                }

                if (!data.Blend)
                {
                    data.BlendData.InstanceProperties.LerpTime = 0;
                }

                BlendData[i] = data;

                transformData.Add(data.BlendData);
            }

            collection.UpdateData(transformData);
        }

        /// <summary>
        /// standard set state function responds to button state
        /// </summary>
        /// <param name="state"></param>
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

        /// <summary>
        /// Set up the themes
        /// </summary>
        public override void SetTheme()
        {
            for (int i = 0; i < BlendData.Count; i++)
            {
                BlendTransformWidgetData widgetData = BlendData[i];

                if (widgetData.BlendData.TransformProperties.Type == TransformTypes.Quaternion)
                {
                    if (widgetData.QuaternionTheme == null || widgetData.QuaternionTheme.Tag != widgetData.Tag)
                    {
                        widgetData.QuaternionTheme = InteractiveThemeManager.GetQuaternionTheme(widgetData.Tag);
                    }
                }
                else
                {
                    if (widgetData.VectorTheme == null || widgetData.VectorTheme.Tag != widgetData.Tag)
                    {
                        widgetData.VectorTheme = InteractiveThemeManager.GetVector3Theme(widgetData.Tag);
                    }
                }
                
                BlendData[i] = widgetData;
            }
        }

        /// <summary>
        /// do we have themes?
        /// </summary>
        /// <returns></returns>
        protected override bool HasTheme()
        {
            int themeCount = 0;

            for (int i = 0; i < BlendData.Count; i++)
            {
                if (BlendData[i].BlendData.TransformProperties.Type == TransformTypes.Quaternion)
                {
                    if (BlendData[i].QuaternionTheme != null)
                    {
                        themeCount++;
                    }
                }
                else
                {
                    if (BlendData[i].VectorTheme != null)
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
