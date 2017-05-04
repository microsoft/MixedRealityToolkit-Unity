// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace HoloToolkit.Examples.Prototyping
{
    /// <summary>
    /// sets the texture of a material based on the selected item in the array
    /// </summary>
    public class CycleTextures : CycleArray<Texture>
    {
        private Material mMaterial;

        /// <summary>
        /// get the material to assign textures to
        /// </summary>
        override protected void Awake()
        {
            base.Awake();

            Renderer renderer = TargetObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                mMaterial = renderer.material;
            }
            else
            {
                Debug.LogError("CycleTexture requires a renderer and material on the assigned GameObject!");
                Destroy(this);
            }
        }

        /// <summary>
        /// set the texture
        /// </summary>
        /// <param name="index"></param>
        public override void SetIndex(int index)
        {
            base.SetIndex(index);

            if (index > -1 && index < Array.Length)
            {
                if (mMaterial != null)
                {
                    mMaterial.SetTexture("_MainTex", Array[index]);
                }
            }
        }

        /// <summary>
        /// Update the current set of textures
        /// </summary>
        /// <param name="arr"></param>
        public void SetNewArray(Texture[] arr)
        {
            Array = arr;
            SetIndex(0);
        }

        /// <summary>
        /// clean up if material was created dynamically
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
