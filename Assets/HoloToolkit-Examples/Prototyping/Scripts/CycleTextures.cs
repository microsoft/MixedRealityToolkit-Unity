// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace HoloToolkit.Examples.Prototyping
{
    public class CycleTextures : CycleArray<Texture>
    {
        public int DefaultTextureIndex = 0;
        public int TextureIndex { get; private set; }

        private Renderer mRenderer;

        override protected void Awake()
        {
            base.Awake();

            mRenderer = TargetObject.GetComponent<Renderer>();
            if (mRenderer == null)
            {
                Debug.LogError("A Rrenderer does not exist on the assigned TargetObject!");
                Destroy(this);
            }
        }

        public override void SetIndex(int index)
        {
            base.SetIndex(index);

            if (index > -1 && index < Array.Length)
            {
                if (mRenderer != null)
                {
                    mRenderer.material.SetTexture("_MainTex", Array[index]);
                }

                TextureIndex = index;
            }
        }

        public void SetNewArray(Texture[] arr)
        {
            Array = arr;
            SetIndex(0);
        }

        private void OnDestroy()
        {
            if (mRenderer != null)
            {
                Destroy(mRenderer.material);
            }
        }
    }
}
