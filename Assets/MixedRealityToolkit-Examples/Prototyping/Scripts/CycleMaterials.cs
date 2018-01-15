// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.Prototyping
{
    /// <summary>
    /// sets the material of an object to a material selected in the array
    /// </summary>
    public class CycleMaterials : CycleArray<Material>
    {
        public override void SetIndex(int index)
        {
            base.SetIndex(index);

            Renderer renderer = TargetObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = Current;
            }
        }
    }
}
