// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class ScaleFadeAndDestroy : MonoBehaviour
    {
        [Tooltip("The value to scale to")]
        public Vector3 ScaleTo;

        [Tooltip("The curve to effect scale")]
        public AnimationCurve ScaleCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 1) });

        [Tooltip("The value to fade to")]
        public float FadeTo;

        [Tooltip("The curve to effect fade")]
        public AnimationCurve FadeCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 1) });

        [Tooltip("The life span of this object - will scale and fade over lifespan")]
        public float Lifespan;
        
        private string colorProperty = "_Color";
        private Vector3 startScale;
        private Color startColor;
        private Color endColor;
        private float timer;

        private MaterialPropertyBlock propertyBlock;

        private void Start()
        {
            startScale = transform.localScale;
            propertyBlock = InteractableThemeShaderUtils.GetMaterialPropertyBlock(gameObject, new ShaderProperties[] { new ShaderProperties() { Name = colorProperty, Range = new Vector2(0, 1), Type = ShaderPropertyType.Color } });
            startColor = propertyBlock.GetColor(colorProperty);
            endColor = new Color(startColor.r, startColor.g, startColor.b, FadeTo);
        }

        private void Update()
        {
            timer += Time.deltaTime;

            float percent = Mathf.Clamp01(timer / Lifespan);

            transform.localScale = Vector3.Lerp(startScale, ScaleTo, ScaleCurve.Evaluate(percent));

            Renderer renderer = GetComponent<Renderer>();
            if(renderer != null && propertyBlock != null)
            {
                renderer.GetPropertyBlock(propertyBlock);
                Color color = Color.Lerp(startColor, endColor, FadeCurve.Evaluate(percent));

                propertyBlock.SetColor(colorProperty, color);
                renderer.SetPropertyBlock(propertyBlock);
            }
            
            if (timer >= Lifespan)
            {
                Destroy(gameObject);
            }
        }
    }
}
