// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing
{
    /// <summary>
    /// Very simple consumer of <see cref="ISpatialCoordinate"/> to demonstrate usage.
    /// </summary>
    public class SpatialCoordinateLocalizer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Target gameObject to position, enable/disable.")]
        private GameObject targetRoot = null;

        [SerializeField]
        [Tooltip("Set to true to enable/disable the gameObject according to Coordinate.IsLocated value. Ensure targetRoot is not current gameObject.")]
        private bool autoToggleActive = false;

        [Tooltip("The relative location to the coordinate at which to position the targetRoot.")]
        public Vector3 CoordinateRelativePosition;

        [Tooltip("The relative orientation to the coordinate with which to orient the targetRoot.")]
        public Quaternion CoordinateRelativeRotation;

        /// <summary>
        /// The coordinate to use for position the targetRoot.
        /// </summary>
        public ISpatialCoordinate Coordinate { get; set; }

        private void Update()
        {
            bool isEnabled = Coordinate?.IsLocated ?? false;

            if (isEnabled)
            {
                targetRoot.transform.position = Coordinate.CoordinateToWorldSpace(CoordinateRelativePosition);
                targetRoot.transform.rotation = Coordinate.CoordinateToWorldSpace(CoordinateRelativeRotation);
            }

            if (autoToggleActive)
            {
                targetRoot.SetActive(isEnabled);
            }
        }
    }
}
