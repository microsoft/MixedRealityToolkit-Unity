// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing
{
    /// <summary>
    /// Very simple consumer of <see cref="ISpatialCoordinate"/> to demonstrate usage.
    /// </summary>
    public class SpatialLocalizer : MonoBehaviour
    {
        [SerializeField]
        private GameObject targetRoot = null;

        [SerializeField]
        private bool autoToggleActive = false;

        public Vector3 CoordinateRelativePosition;
        public Quaternion CoordinateRelativeRotation;

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
