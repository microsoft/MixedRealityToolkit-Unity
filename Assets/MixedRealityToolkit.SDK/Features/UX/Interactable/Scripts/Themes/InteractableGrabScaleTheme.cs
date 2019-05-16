// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractableGrabScaleTheme : InteractableThemeBase
    {
        protected InteractableThemePropertyValue startScaleValue = new InteractableThemePropertyValue();

        protected float timer = 0;
        protected bool hasGrab;
        protected float grabPercentage;
        protected bool grabTransition;

        public InteractableGrabScaleTheme()
        {
            Types = new Type[] { typeof(Transform) };
            Name = "Grab Scale Theme";
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
                    Name = "ScaleMagnifier",
                    Type = InteractableThemePropertyValueTypes.Vector3,
                    Value = new InteractableThemePropertyValue() { Vector3 = Vector3.one }
                },
                new InteractableCustomSetting()
                {
                    Name = "GrabTimer",
                    Type = InteractableThemePropertyValueTypes.Float,
                    Value = new InteractableThemePropertyValue() { Float = 0.3f }
                }
            };
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
            
            Vector3 maxGrabScale = CustomSettings[0].Value.Vector3;
            float grabTime = CustomSettings[1].Value.Float;
            Vector3 grabScale = Vector3.Scale(startScaleValue.Vector3, maxGrabScale);
            
            if (source.HasGrab)
            {
                if (!hasGrab)
                {
                    timer = 0;
                }

                timer += Time.deltaTime;
                grabPercentage = Mathf.Clamp01(timer / grabTime);
                Host.transform.localScale = Vector3.Lerp(startScaleValue.Vector3, grabScale, grabPercentage);
                hasGrab = true;
                grabTransition = true;
            }
            else
            {
                if (grabTransition)
                {
                    // ending
                    grabTransition = false;
                    if (!Ease.Enabled)
                    {
                        Host.transform.localScale = startScaleValue.Vector3;
                        grabPercentage = 0;
                    }
                    else
                    {
                        timer = Ease.LerpTime - Ease.LerpTime * grabPercentage;
                    }
                }
                else if (Ease.Enabled && timer < Ease.LerpTime && grabPercentage > 0)
                {
                    timer += Time.deltaTime;
                    float percent = 1 - Mathf.Clamp01(timer / Ease.LerpTime);
                    Host.transform.localScale = Vector3.Lerp(startScaleValue.Vector3, grabScale, Ease.Curve.Evaluate(percent));
                }

                // is there a transition from physical press?
                hasGrab = timer < Ease.LerpTime;
            }
        }

        public override void SetValue(InteractableThemeProperty property, int index, float percentage)
        {
            if (!hasGrab && Host != null)
            {
                Host.transform.localScale = Vector3.Lerp(property.StartValue.Vector3, Vector3.Scale(startScaleValue.Vector3, property.Values[index].Vector3), percentage);
            }
            else
            {
                // there is near interaction grab so make sure Ease is not running
                Ease.Stop();
            }
        }
    }
}
