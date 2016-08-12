// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// All possible plane types that a SurfacePlane can be.
    /// </summary>
    [Flags]
    public enum PlaneTypes
    {
        Wall = 0x1,
        Floor = 0x2,
        Ceiling = 0x4,
        Table = 0x8,
        Unknown = 0x10
    }

    /// <summary>
    /// The SurfacePlane class is used by SurfaceMeshesToPlanes to create different types of planes (walls, floors, tables, etc.) 
    /// based on the Spatial Mapping data returned by the SpatialMappingManager's source.
    /// This script should be a component on the SufacePlane prefab, which is used by SurfaceMeshesToPlanes.
    /// </summary>
    public class SurfacePlane : MonoBehaviour
    {
        [Tooltip("Thickness to make each plane.")]
        [Range(0.0f, 1.0f)]
        public float PlaneThickness = 0.01f;

        [Tooltip("Threshold for acceptable normals (the closer to 1, the stricter the standard). Used when determining plane type.")]
        [Range(0.0f, 1.0f)]
        public float UpNormalThreshold = 0.9f;

        [Tooltip("Buffer to use when determining if a horizontal plane near the floor should be considered part of the floor.")]
        [Range(0.0f, 1.0f)]
        public float FloorBuffer = 0.1f;

        [Tooltip("Buffer to use when determining if a horizontal plane near the ceiling should be considered part of the ceiling.")]
        [Range(0.0f, 1.0f)]
        public float CeilingBuffer = 0.1f;

        [Tooltip("Material to use when rendering Wall planes.")]
        public Material WallMaterial;

        [Tooltip("Material to use when rendering floor planes.")]
        public Material FloorMaterial;

        [Tooltip("Material to use when rendering ceiling planes.")]
        public Material CeilingMaterial;

        [Tooltip("Material to use when rendering table planes.")]
        public Material TableMaterial;

        [Tooltip("Material to use when rendering planes of the unknown type.")]
        public Material UnknownMaterial;

        [Tooltip("Type of plane that the object has been classified as.")]
        public PlaneTypes PlaneType = PlaneTypes.Unknown;

        /// <summary>
        /// The BoundedPlane associated with the SurfacePlane object.
        /// </summary>
        private BoundedPlane plane = new BoundedPlane();

        /// <summary>
        /// Gets or Sets the BoundedPlane, which determines the orientation/size/position of the gameObject.
        /// </summary>
        public BoundedPlane Plane
        {
            get
            {
                return plane;
            }
            set
            {
                plane = value;
                UpdateSurfacePlane();
            }
        }

        /// <summary>
        /// Gets the normal of the plane that was determined by the BoundedPlane object.
        /// </summary>
        public Vector3 SurfaceNormal { get; private set; }

        /// <summary>
        /// Gets or sets the visibility of the current gameObject.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return gameObject.GetComponent<Renderer>().enabled;
            }
            set
            {
                if (IsVisible != value)
                {
                    gameObject.GetComponent<Renderer>().enabled = value;
                }
            }
        }

        private void Awake()
        {
            plane = new BoundedPlane(transform);
        }

        private void Start()
        {
            UpdateSurfacePlane();
        }

        /// <summary>
        /// Updates the SurfacePlane object to have the same configuration of the BoundingPlane object.
        /// Determine what type of plane the SurfacePlane aligns to.
        /// Sets the material based on the plane type.
        /// </summary>
        private void UpdateSurfacePlane()
        {
            SetPlaneGeometry();
            SetPlaneType();
            SetPlaneMaterialByType();
        }

        /// <summary>
        /// Updates the plane geometry to match the bounded plane found by SurfaceMeshesToPlanes.
        /// </summary>
        private void SetPlaneGeometry()
        {
            // Set the SurfacePlane object to have the same extents as the BoundingPlane object.
            gameObject.transform.position = plane.Bounds.Center;
            gameObject.transform.rotation = plane.Bounds.Rotation;
            Vector3 extents = plane.Bounds.Extents * 2;
            gameObject.transform.localScale = new Vector3(extents.x, extents.y, PlaneThickness);
        }

        /// <summary>
        /// Classifies the surface as a floor, wall, ceiling, table, etc.
        /// </summary>
        private void SetPlaneType()
        {
            SurfaceNormal = plane.Plane.normal;
            float floorYPosition = SurfaceMeshesToPlanes.Instance.FloorYPosition;
            float ceilingYPosition = SurfaceMeshesToPlanes.Instance.CeilingYPosition;

            // Determine what type of plane this is.
            // Use the upNormalThreshold to help determine if we have a horizontal or vertical surface.
            if (SurfaceNormal.y >= UpNormalThreshold)
            {
                // If we have a horizontal surface with a normal pointing up, classify it as a floor.
                PlaneType = PlaneTypes.Floor;

                if (gameObject.transform.position.y > (floorYPosition + FloorBuffer))
                {
                    // If the plane is too high to be considered part of the floor, classify it as a table.
                    PlaneType = PlaneTypes.Table;
                }
            }
            else if (SurfaceNormal.y <= -(UpNormalThreshold))
            {
                // If we have a horizontal surface with a normal pointing down, classify it as a ceiling.
                PlaneType = PlaneTypes.Ceiling;

                if (gameObject.transform.position.y < (ceilingYPosition - CeilingBuffer))
                {
                    // If the plane is not high enough to be considered part of the ceiling, classify it as a table.
                    PlaneType = PlaneTypes.Table;
                }
            }
            else if (Mathf.Abs(SurfaceNormal.y) <= (1 - UpNormalThreshold))
            {
                // If the plane is vertical, then classify it as a wall.
                PlaneType = PlaneTypes.Wall;
            }
            else
            {
                // The plane has a strange angle, classify it as 'unknown'.
                PlaneType = PlaneTypes.Unknown;
            }
        }

        /// <summary>
        /// Sets the renderer material to match the object's plane type.
        /// </summary>
        private void SetPlaneMaterialByType()
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();

            switch (PlaneType)
            {
                case PlaneTypes.Floor:
                    if (FloorMaterial != null)
                    {
                        renderer.material = FloorMaterial;
                    }
                    break;
                case PlaneTypes.Table:
                    if (TableMaterial != null)
                    {
                        renderer.material = TableMaterial;
                    }
                    break;
                case PlaneTypes.Ceiling:
                    if (CeilingMaterial != null)
                    {
                        renderer.material = CeilingMaterial;
                    }
                    break;
                case PlaneTypes.Wall:
                    if (WallMaterial != null)
                    {
                        renderer.material = WallMaterial;
                    }
                    break;
                default:
                    if (UnknownMaterial != null)
                    {
                        renderer.material = UnknownMaterial;
                    }
                    break;
            }
        }
    }
}