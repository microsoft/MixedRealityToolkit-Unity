// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    public class TextureWidget : InteractiveWidget
    {
        public GameObject Target;

        public TextureButtonTheme TextureTheme;
        
        protected Material mMaterial;

        void Awake()
        {
            if (Target == null)
            {
                Target = this.gameObject;
            }

            Renderer renderer = Target.GetComponent<Renderer>();

            if (renderer != null)
            {
                mMaterial = renderer.material;
                mMaterial.SetTexture("_MainTex", TextureTheme.GetThemeValue(Interactive.ButtonStateEnum.Default));
            }
            else
            {
                Debug.LogError("A Renderer does not exist on the Target!");
                Destroy(this);
            }
        }

        public override void SetState(Interactive.ButtonStateEnum state)
        {
            base.SetState(state);
            
            mMaterial.SetTexture("_MainTex", TextureTheme.GetThemeValue(state));
        }

        private void OnDestroy() 
        {
            if (mMaterial != null)
            {
                Destroy(mMaterial);
            }
        }
    }
}
