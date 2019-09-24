// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// This is required since UnityUI Graphic elements do not support MaterialPropertyBlocks, and any shader operations can end up modifying the material permanenetly across all shared instances.
    /// To prevent that we create a runtime copy of the material.
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIMaterialInstancing : MonoBehaviour
    {
        private IList<Material> materialCopies;

        private void Awake()
        {
            materialCopies = new List<Material>();
            var graphics =  gameObject.GetComponentsInChildren<Graphic>(true);
            foreach (var graphic in graphics)
            {
                graphic.material = Instantiate(graphic.material);
                materialCopies.Add(graphic.material);
            }

            enabled = false;
        }

        private void OnDestroy()
        {
            foreach (var mat in materialCopies)
            {
                Destroy(mat);
            }
        }
    }
}
