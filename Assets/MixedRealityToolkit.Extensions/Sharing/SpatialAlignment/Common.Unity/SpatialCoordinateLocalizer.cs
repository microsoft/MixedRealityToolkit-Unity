// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.Common
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
        public Vector3 CoordinateRelativePosition = Vector3.zero;

        [Tooltip("The relative orientation to the coordinate with which to orient the targetRoot.")]
        public Quaternion CoordinateRelativeRotation = Quaternion.identity;

        /// <summary>
        /// Check to enable debug logging.
        /// </summary>
        [Tooltip("Check to enable debug logging.")]
        public bool debugLogging = false;

        /// <summary>
        /// Check to show debug visuals.
        /// </summary>
        [Tooltip("Check to show debug visuals.")]
        public bool showDebugVisuals = false;

        /// <summary>
        /// Game Object to render at spatial coordinate location when showing debug visuals.
        /// </summary>
        [Tooltip("Game Object to render at spatial coordinate locations when showing debug visuals.")]
        public GameObject debugVisual = null;

        /// <summary>
        /// Debug visual scale.
        /// </summary>
        [Tooltip("Debug visual scale.")]
        public float debugVisualScale = 1.0f;

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

                    if (targetRoot == null)
                    {
                        DebugLog("TargetRoot was null when setting the spatial coordinate, this is unexpected behavior.");
                        return;
                    }

                    var position = spatialCoordinate.CoordinateToWorldSpace(CoordinateRelativePosition);
                    var rotation = spatialCoordinate.CoordinateToWorldSpace(CoordinateRelativeRotation);
                    targetRoot.transform.position = position;
                    targetRoot.transform.rotation = rotation;
                    DebugLog($"SpatialCoordindate updated for SpatialCoordinateLocalizer {targetRoot.name}, Coordinate id: {spatialCoordinate.Id}, Coordinate state: {spatialCoordinate.State.ToString()}, Coordinate Space Position: {position.ToString("G4")}, Coordinate Space Rotation {rotation.ToString("G4")}");

                    if (showDebugVisuals)
                    {
                        if (debugVisual != null)
                        {
                            if (debugGameObject == null)
                            {
                                debugGameObject = Instantiate(debugVisual);
                            }

                            debugGameObject.transform.parent = targetRoot.transform;
                            debugGameObject.transform.position = position;
                            debugGameObject.transform.rotation = rotation;
                            debugGameObject.transform.localScale = debugVisualScale * Vector3.one;
                            debugGameObject.name = $"{targetRoot.name} - SpatialCoordinateLocalizer DebugVisual";
                            DebugLog($"Created {debugGameObject.name}, World Space Position: {position.ToString("G4")}, World Space Rotation {rotation.ToString("G4")}");
                        }
                        else
                        {
                            DebugLog($"SpatialCoordinateLocalizer for {targetRoot.name}, Coordinate id: {spatialCoordinate.Id} had showDebugVisuals enabled but no debugVisual specified.");
                        }
                    }
                }
            }
        }
        private ISpatialCoordinate spatialCoordinate = null;
        private GameObject debugGameObject = null;

        private void Awake()
        {
            if (targetRoot is null)
            {
                targetRoot = gameObject;
            }
        }

        private void Update()
        {
            bool isEnabled = (Coordinate?.State ?? LocatedState.Tracking) == LocatedState.Tracking;

            if (isEnabled)
            {
                targetRoot.transform.position = Coordinate?.CoordinateToWorldSpace(CoordinateRelativePosition) ?? targetRoot.transform.position;
                targetRoot.transform.rotation = Coordinate?.CoordinateToWorldSpace(CoordinateRelativeRotation) ?? targetRoot.transform.rotation;
            }

            if (autoToggleActive)
            {
                targetRoot.SetActive(isEnabled);
            }
        }

        private void DebugLog(string message)
        {
            if (debugLogging)
            {
                Debug.Log($"SpatialCoordinateLocalizer: {message}");
            }
        }
    }
}
