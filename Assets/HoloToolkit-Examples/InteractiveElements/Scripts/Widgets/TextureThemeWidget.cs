// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    /// <summary>
    /// An Interactive Theme Widget for swapping textures based on interactive state
    /// </summary>
    public class TextureThemeWidget : InteractiveThemeWidget
    {
        [Tooltip("A tag for finding the theme in the scene")]
        public string ThemeTag = "defaultTexture";

        [Tooltip("The target object with the material to swap textures on : optional, leave blank for self")]
        public GameObject Target;

        /// <summary>
        /// The theme with the texture states
        /// </summary>
        protected TextureInteractiveTheme mTextureTheme;

        /// <summary>
        /// material to swap the texture on
        /// </summary>
        protected Material mMaterial;

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
                    SetTexture(Interactive.ButtonStateEnum.Default);
                }
            }
            else
            {
                Debug.LogError("A Renderer does not exist on the Target!");
                Destroy(this);
            }
        }

        /// <summary>
        /// Find the theme is none was manually set
        /// </summary>
        private void Start()
        {
            if (mTextureTheme == null)
            {
                mTextureTheme = GetTextureTheme(ThemeTag);
                SetTexture(State);
            }
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
    }
}
