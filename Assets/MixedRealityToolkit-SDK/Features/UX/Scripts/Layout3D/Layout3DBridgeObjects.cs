// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Layout3D
{
    /// <summary>
    /// rotates and scales an Unity Primitive to span two objects.
    /// </summary>
    [ExecuteInEditMode]
    public class Layout3DBridgeObjects : MonoBehaviour
    {
        /// <summary>
        /// The start transform
        /// </summary>
        [Tooltip("Starting point")]
        public Transform From;

        /// <summary>
        /// the target transform
        /// </summary>
        [Tooltip("Ending point")]
        public Transform To;
        
        // Update is called once per frame
        void Update()
        {

            if (From != null && To != null)
            {
                transform.position = From.position;
                Vector3 direction = To.position - From.position;
                transform.localScale = new Vector3(1, direction.magnitude, 1);
                transform.rotation = Quaternion.LookRotation(direction);
            }

        }
    }
}
