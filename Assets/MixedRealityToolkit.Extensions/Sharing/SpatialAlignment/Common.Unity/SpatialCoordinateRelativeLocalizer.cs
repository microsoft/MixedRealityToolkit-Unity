// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.Common
{
    /// <summary>
    /// Very simple consumer of <see cref="ISpatialCoordinate"/> to demonstrate usage.
    /// </summary>
    public class SpatialCoordinateRelativeLocalizer : MonoBehaviour
    {
        private ISpatialCoordinate spatialCoordinate = null;

        /// <summary>
        /// The coordinate to use for position the targetRoot.
        /// </summary>
        public ISpatialCoordinate Coordinate
        {
            get
            {
                return spatialCoordinate;
            }
            set
            {
                if (spatialCoordinate == null ||
                    spatialCoordinate != value)
                {
                    spatialCoordinate = value;

                    if (spatialCoordinate == null)
                    {
                        return;
                    }
                    
                    var position = spatialCoordinate.CoordinateToWorldSpace(Vector3.zero);
                    var rotation = spatialCoordinate.CoordinateToWorldSpace(Quaternion.identity);
                    this.transform.localPosition = position;
                    this.transform.localRotation = rotation;
                }
            }
        }

        private void Update()
        {
            bool isEnabled = (Coordinate?.State ?? LocatedState.Tracking) == LocatedState.Tracking;

            if (isEnabled)
            {
                this.transform.localPosition = Coordinate?.CoordinateToWorldSpace(Vector3.zero) ?? this.transform.localPosition;
                this.transform.localRotation = Coordinate?.CoordinateToWorldSpace(Quaternion.identity) ?? this.transform.localRotation;
            }
        }
    }
}
