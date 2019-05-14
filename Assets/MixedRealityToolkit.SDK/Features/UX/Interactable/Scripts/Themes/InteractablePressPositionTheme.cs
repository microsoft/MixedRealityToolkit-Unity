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
    public class InteractablePressPositionTheme : InteractableThemeBase
    {
        protected InteractableThemePropertyValue startValue = new InteractableThemePropertyValue();
        protected float timer = 0;
        protected InteractablePressData pressData;
        protected bool hasPress;

        public InteractablePressPositionTheme()
        {
            Types = new Type[] { typeof(Transform) };
            Name = "Press Theme";
            ThemeProperties = new List<InteractableThemeProperty>(){
                new InteractableThemeProperty()
                {
                    Name = "Position",
                    Type = InteractableThemePropertyValueTypes.Vector3,
                    Values = new List<InteractableThemePropertyValue>(),
                    Default = new InteractableThemePropertyValue(){ Vector3 = Vector3.zero}
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
                }
            };

            pressData = new InteractablePressData();
        }

        public override void Init(GameObject host, InteractableThemePropertySettings settings)
        {
            base.Init(host, settings);
            if (host != null)
            {
                startValue = new InteractableThemePropertyValue();
                startValue.Vector3 = host.transform.localPosition;
            }

            timer = Ease.LerpTime;
        }

        public override InteractableThemePropertyValue GetProperty(InteractableThemeProperty property)
        {
            if (Host == null)
            {
                return startValue;
            }

            InteractableThemePropertyValue prop = new InteractableThemePropertyValue();
            prop.Vector3 = Host.transform.localPosition;

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
            
            if (source.HasPhysicalTouch)
            {
                pressData = InteractablePointerDataManager.GetPressData(source, pressData);
                
                Host.transform.localPosition = startValue.Vector3 + Host.transform.InverseTransformDirection(pressData.ProjectedDirection) * pressData.Percentage;
                pressData.PressedValue = Host.transform.localPosition;

                pressData.HasTouch = true;
                hasPress = true;
            }
            else
            {
                if (pressData.HasTouch)
                {
                    // ending
                    pressData.Direction = Vector3.zero;
                    pressData.HasTouch = false;
                    if (!Ease.Enabled)
                    {
                        Host.transform.localPosition = startValue.Vector3;
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
                    Host.transform.localPosition = Vector3.Lerp(startValue.Vector3, pressData.PressedValue, Ease.Curve.Evaluate(percent));
                }

                // is there a transition from physical press?
                hasPress = timer < Ease.LerpTime;
            }
        }

        public override void SetValue(InteractableThemeProperty property, int index, float percentage)
        {
            if (!hasPress && Host != null)
            {
                Host.transform.localPosition = Vector3.Lerp(property.StartValue.Vector3, startValue.Vector3 + property.Values[index].Vector3, percentage);
            }
            else
            {
                // there is physical press so make sure Ease is not running
                Ease.Stop();
            }
        }
    }
}
