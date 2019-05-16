// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractablePressScaleTheme : InteractableThemeBase
    {
        protected InteractableThemePropertyValue startScaleValue = new InteractableThemePropertyValue();
        
        protected float timer = 0;
        protected InteractablePressData pressData;
        protected bool hasPress;

        public InteractablePressScaleTheme()
        {
            Types = new Type[] { typeof(Transform) };
            Name = "Press Scale Theme";
            ThemeProperties = new List<InteractableThemeProperty>()
            {
                new InteractableThemeProperty()
                {
                    Name = "Scale",
                    Type = InteractableThemePropertyValueTypes.Vector3,
                    Values = new List<InteractableThemePropertyValue>(),
                    Default = new InteractableThemePropertyValue(){ Vector3 = Vector3.one}
                }
            };

            // adding a custom value and not showing Theme Properties for this theme
            CustomSettings = new List<InteractableCustomSetting>()
            {
                new InteractableCustomSetting()
                {
                    Name = "MaxPressDistance",
                    Type = InteractableThemePropertyValueTypes.Float,
                    Value = new InteractableThemePropertyValue() { Float = 0.35f }
                },
                new InteractableCustomSetting()
                {
                    Name = "ScaleOffset",
                    Type = InteractableThemePropertyValueTypes.Vector3,
                    Value = new InteractableThemePropertyValue() { Vector3 = Vector3.one }
                }
            };

            pressData = new InteractablePressData();
        }

        public override void Init(GameObject host, InteractableThemePropertySettings settings)
        {
            base.Init(host, settings);
            if (host != null)
            {
                startScaleValue = new InteractableThemePropertyValue();
                startScaleValue.Vector3 = host.transform.localScale;
            }

            timer = Ease.LerpTime;
        }

        public override InteractableThemePropertyValue GetProperty(InteractableThemeProperty property)
        {
            if (Host == null)
            {
                return startScaleValue;
            }

            InteractableThemePropertyValue prop = new InteractableThemePropertyValue();
            prop.Vector3 = Host.transform.localScale;

            return prop;
        }

        public override void OnUpdate(int state, Interactable source, bool force = false)
        {
            base.OnUpdate(state, source, force);

            if (Host == null)
            {
                return;
            }
            
            pressData.MaxDistance = CustomSettings[0].Value.Float;
            pressData.ProjectedDirection = source.transform.forward * pressData.MaxDistance;
            Vector3 maxPressScale = Vector3.Scale(startScaleValue.Vector3, CustomSettings[1].Value.Vector3);

            if (source.HasPhysicalTouch)
            {
                pressData = InteractablePointerDataManager.GetPressData(source, pressData);
                Host.transform.localScale = Vector3.Lerp(startScaleValue.Vector3, maxPressScale, pressData.Percentage);
                pressData.PressedValue = Host.transform.localScale;

                pressData.HasPress = true;
                hasPress = true;
            }
            else
            {
                if (pressData.HasPress)
                {
                    // ending
                    pressData.Direction = Vector3.zero;
                    pressData.HasPress = false;
                    if (!Ease.Enabled)
                    {
                        Host.transform.localScale = startScaleValue.Vector3;
                        pressData.Percentage = 0;
                    }
                    else
                    {
                        timer = Ease.LerpTime - Ease.LerpTime * pressData.Percentage;
                    }
                }
                else if (Ease.Enabled && timer < Ease.LerpTime && pressData.Percentage > 0)
                {
                    timer += Time.deltaTime;
                    float percent = 1 - Mathf.Clamp01(timer / Ease.LerpTime);
                    Host.transform.localScale = Vector3.Lerp(startScaleValue.Vector3, pressData.PressedValue, Ease.Curve.Evaluate(percent));
                }

                // is there a transition from physical press?
                hasPress = timer < Ease.LerpTime;
            }
        }

        public override void SetValue(InteractableThemeProperty property, int index, float percentage)
        {
            if (!hasPress && Host != null)
            {
                Host.transform.localScale = Vector3.Lerp(property.StartValue.Vector3, Vector3.Scale(startScaleValue.Vector3, property.Values[index].Vector3), percentage);
            }
            else
            {
                // there is physical press so make sure Ease is not running
                Ease.Stop();
            }
        }
    }
}
