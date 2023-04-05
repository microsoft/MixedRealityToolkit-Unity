// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Script to adjust the position of this GameObject when another GameObject resizes.
    /// Used to position see-it say-it labels when non-canvas button backplate resizes. 
    /// </summary>
    public class AdjustLabelPosition : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The object whose dimensions controls the placement of this object.")]
        private Transform control;

        private void Awake()
        {
            // Calculate the new position of the label based on the current size of the backplate
            float childOffset = ((control.lossyScale.y) / 2f * -1);
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, childOffset, gameObject.transform.localPosition.z);
        }
    }
}
