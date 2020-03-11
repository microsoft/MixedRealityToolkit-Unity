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
        protected Vector3 originalLocalScale = Vector3.zero;

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
            if (host != null)
            {
                originalLocalScale = host.transform.localScale;

#pragma warning disable 0618
                // Keep initializing property to support consumers who have not migrated.
                startScaleValue = new ThemePropertyValue();
                startScaleValue.Vector3 = host.transform.localScale;
#pragma warning restore 0618

            }

            timer = Ease.LerpTime;

            base.Init(host, settings);
        }

        /// <inheritdoc />
        public override ThemePropertyValue GetProperty(ThemeStateProperty property)
        {
            if (Host == null)
            {
                return new ThemePropertyValue()
                {
                    Vector3 = originalLocalScale
                };
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
            Vector3 grabScale = Vector3.Scale(originalLocalScale, maxGrabScale);

            var targetInteractable = Host.FindAncestorComponent<Interactable>(true);

            if (targetInteractable.HasGrab)
            {
                if (!hasGrab)
                {
                    timer = 0;
                }

                timer += Time.deltaTime;
                grabPercentage = Mathf.Clamp01(timer / grabTime);
                Host.transform.localScale = Vector3.Lerp(originalLocalScale, grabScale, grabPercentage);
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
                        Host.transform.localScale = originalLocalScale;
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
                    Host.transform.localScale = Vector3.Lerp(originalLocalScale, grabScale, Ease.Curve.Evaluate(percent));
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
                Host.transform.localScale = Vector3.Lerp(property.StartValue.Vector3, Vector3.Scale(originalLocalScale, property.Values[index].Vector3), percentage);
            }
            else
            {
                // there is near interaction grab so make sure Ease is not running
                Ease.Stop();
            }
        }

        /// <inheritdoc />
        protected override void SetValue(ThemeStateProperty property, ThemePropertyValue value)
        {
            if (Host != null)
            {
                Host.transform.localScale = value.Vector3;
            }
        }

        #region Obsolete
        
        [System.Obsolete("startScaleValue is no longer supported. Use originalLocalScale instead.")]
        protected ThemePropertyValue startScaleValue = new ThemePropertyValue();

        #endregion
    }
}
