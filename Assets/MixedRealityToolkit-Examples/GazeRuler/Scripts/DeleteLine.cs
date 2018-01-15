// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.GazeRuler
{
    public class DeleteLine : MonoBehaviour
    {
        /// <summary>
        /// when tip text is tapped, destroy this tip and relative objects.
        /// </summary>
        public void OnSelect()
        {
            var parent = gameObject.transform.parent.gameObject;
            if (parent != null)
            {
                Destroy(parent);
            }
        }
    }
}