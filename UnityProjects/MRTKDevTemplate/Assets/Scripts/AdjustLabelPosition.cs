// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for samples. While nice to have, this XML documentation is not required for samples.
#pragma warning disable CS1591

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Script to adjust the position of this <see cref="GameObject"/> when another
    /// <see cref="GameObject"/> resizes.
    /// </summary>
    /// <remarks>
    /// Used to position see-it say-it labels when non-canvas button backplate resizes. 
    /// </remarks>
    public class AdjustLabelPosition : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The object whose dimensions controls the placement of this object.")]
        private Transform control;

        /// <summary>
        /// A Unity event function that is called when an enabled script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            // Calculate the new position of the label based on the current size of the backplate
            float childOffset = ((control.lossyScale.y) / 2f * -1);
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, childOffset, gameObject.transform.localPosition.z);
        }
    }
}
#pragma warning restore CS1591