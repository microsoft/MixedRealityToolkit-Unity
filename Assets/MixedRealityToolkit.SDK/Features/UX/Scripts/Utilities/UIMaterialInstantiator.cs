// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// This is required since UnityUI Graphic elements do not support MaterialPropertyBlocks, and any shader operations can end up modifying the material permanently across all shared instances.
    /// To prevent that we create a runtime copy of the material.
    /// </summary>
    public class UIMaterialInstantiator
    {
        // this set ensures that we do not end up creating multiple copies of materials for every theme targeting the same instance
        private static HashSet<int> targetInstances = new HashSet<int>();

        /// <summary>
        /// Invoke this method to create a copy of the material and use that copy at runtime for Graphic objects to prevent modifying materials in editor or impact shared materials.
        /// </summary>
        /// <param name="targetGraphic">Graphic element that needs to clone its material </param>
        public static void TryCreateMaterialCopy(Graphic targetGraphic)
        {
            int targetId = targetGraphic.GetInstanceID();
            if (!targetInstances.Contains(targetId))
            {
                targetInstances.Add(targetId);
                targetGraphic.material = new Material(targetGraphic.material);
            }
        }
    }
}
