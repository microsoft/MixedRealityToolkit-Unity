// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class UIMaterialInstantiator
    {
        // this set ensures that we do not end up creating multiple copies of materials for every theme targetting the same instance
        private static HashSet<int> targetInstances = new HashSet<int>();

        public static void TryCreateMaterialCopy(ref Graphic targetGraphic)
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
