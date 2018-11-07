// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Layout3D
{
    [ExecuteInEditMode]
    public class Layout3DSizeOutput : MonoBehaviour
    {
        public float BasePixelSize = 2048;
        public Transform Anchor;

        private void Update()
        {
            TextMesh mesh = GetComponent<TextMesh>();
            if(mesh != null && Anchor != null)
            {
                mesh.text = "Size: " + (Anchor.localScale * BasePixelSize);
            }
            
        }
    }
}
