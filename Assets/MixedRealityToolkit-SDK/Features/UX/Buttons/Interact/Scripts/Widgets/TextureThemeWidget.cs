// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Interact.Themes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Widgets
{
    /// <summary>
    /// An Interactive Theme Widget for swapping textures based on interactive state
    /// </summary>
    public class TextureThemeWidget : InteractiveThemeWidget
    {
        [Tooltip("The target object with the material to swap textures on : optional, leave blank for self")]
        public GameObject Target;

        // The theme with the texture states
        private TextureInteractiveTheme mTextureTheme;

        // material to swap the texture on
        private Material mMaterial;

        void Awake()
        {
            // set the target
            if (Target == null)
            {
                Target = this.gameObject;
            }

            // set the renderer
            Renderer renderer = Target.GetComponent<Renderer>();
            
            if (renderer != null)
            {
                mMaterial = renderer.material;
                if (mTextureTheme != null)
                {
                    SetTexture(State);
                }
            }
            else
            {
                Debug.LogError("A Renderer does not exist on the Target!");
                Destroy(this);
            }
        }

        public override void SetTheme()
        {
            mTextureTheme = InteractiveThemeManager.GetTextureTheme(ThemeTag);
        }

        protected override bool HasTheme()
        {
            return mTextureTheme != null;
        }

        /// <summary>
        /// From InteractiveWidget
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            
            SetTexture(state);
        }

        /// <summary>
        /// swap the texture based on the theme
        /// </summary>
        /// <param name="state"></param>
        private void SetTexture(Interactive.ButtonStateEnum state)
        {
            if (mTextureTheme != null)
            {
                mMaterial.SetTexture("_MainTex", mTextureTheme.GetThemeValue(state));
            }
        }

        /// <summary>
        /// Clean up the material if one was created dynamically
        /// </summary>
        private void OnDestroy() 
        {
            if (mMaterial != null)
            {
                Destroy(mMaterial);
            }
        }
		protected override void ApplyBlendValues(float percent)
        {
            //throw new NotImplementedException();
        }

        protected override void GetBlendValues(Interactive.ButtonStateEnum state)
        {
            //throw new NotImplementedException();
        }
    }
}
