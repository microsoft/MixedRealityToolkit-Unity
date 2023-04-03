// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{

    /// <summary>
    /// Script to adjust the position of a another GameObject when this GameObject resizes.
    /// EX Used to position see-it say-it labels when non-canvas button backplate resizes. 
    /// </summary>
    public class AdjustLabelPosition : MonoBehaviour
    {

        [SerializeField]
        private Transform label;

        private Vector2 _lastSize;

        public void Update()
        {
            // Check if the size of the backplate has changed
            if (gameObject.transform.lossyScale != (Vector3)_lastSize)
            {
                // Calculate the new position of the label based on the current size of the backplate
                float childOffset = ((gameObject.transform.lossyScale.y) / 2f * -1);

                label.localPosition = new Vector3(label.localPosition.x, childOffset, label.localPosition.z);


                // Store the current size of the backplate for the next frame
                _lastSize = gameObject.transform.lossyScale;
            }
        }
    }
}
