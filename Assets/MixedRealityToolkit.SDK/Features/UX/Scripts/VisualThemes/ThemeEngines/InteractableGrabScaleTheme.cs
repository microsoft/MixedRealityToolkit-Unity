// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// ThemeEngine to control initialized GameObject's scale based on associated Interactable grab state and related state changes
    /// </summary>
    public class InteractableGrabScaleTheme : InteractableThemeBase
    {
        protected ThemePropertyValue startScaleValue = new ThemePropertyValue();

        protected float timer = 0;
        protected bool hasGrab;
        protected float grabPercentage;
        protected bool grabTransition;

        protected Interactable targetInteractable;

        public InteractableGrabScaleTheme()
        {
            Types = new Type[] { typeof(Transform) };
            Name = "Grab Scale Theme";
        }

        /// <inheritdoc />
        public override ThemeDefinition GetDefaultThemeDefinition()
        {
            return new ThemeDefinition()
            {
                ThemeType = GetType(),
                StateProperties = new List<ThemeStateProperty>()
                {
                    new ThemeStateProperty()
                    {
                        Name = "Scale",
                        Type = ThemePropertyTypes.Vector3,
                        Values = new List<ThemePropertyValue>(),
                        Default = new ThemePropertyValue(){ Vector3 = Vector3.one}
                    },
                },
                CustomProperties = new List<ThemeProperty>()
                {
                    new ThemeProperty()
                    {
                        Name = "ScaleMagnifier",
                        Type = ThemePropertyTypes.Vector3,
                        Value = new ThemePropertyValue() { Vector3 = Vector3.one }
                    },
                    new ThemeProperty()
                    {
                        Name = "GrabTimer",
                        Type = ThemePropertyTypes.Float,
                        Value = new ThemePropertyValue() { Float = 0.3f }
                    },
                },
            };
        }


        /// <inheritdoc />
        public override void Init(GameObject host, ThemeDefinition settings)
        {
            base.Init(host, settings);

            if (host != null)
            {
                startScaleValue = new ThemePropertyValue();
                startScaleValue.Vector3 = host.transform.localScale;
            }

            timer = Ease.LerpTime;
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            if (Host == null)
            {
                return startScaleValue;
            }

            ThemePropertyValue prop = new ThemePropertyValue();
            prop.Vector3 = Host.transform.localScale;

            return prop;
        }

        /// <inheritdoc />
        public override void OnUpdate(int state, bool force = false)
        {
            base.OnUpdate(state, force);

            if (Host == null)
            {
                return;
            }
            
            Vector3 maxGrabScale = Properties[0].Value.Vector3;
            float grabTime = Properties[1].Value.Float;
            Vector3 grabScale = Vector3.Scale(startScaleValue.Vector3, maxGrabScale);

            var targetInteractable = Host.FindAncestorComponent<Interactable>(true);

            if (targetInteractable.HasGrab)
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

        /// <inheritdoc />
        public override void SetValue(ThemeStateProperty property, int index, float percentage)
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
