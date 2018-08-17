// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Collections
{
    /// <summary>
    /// An Object Collection is simply a set of child objects organized with some
    /// layout parameters.  The object collection can be used to quickly create 
    /// control panels or sets of prefab/objects.
    /// </summary>
    public class GridObjectCollection : BaseObjectCollection
    {
        #region private variables
        /// <summary>
        /// Type of surface to map the collection to.
        /// </summary>
        [Tooltip("Type of surface to map the collection to")]
        private ObjectOrientationSurfaceType surfaceType = ObjectOrientationSurfaceType.Plane;

        /// <summary>
        /// Should the objects in the collection face the origin of the collection
        /// </summary>
        [Tooltip("Should the objects in the collection be rotated / how should they be rotated")]
        private OrientationType orientType = OrientationType.FaceOrigin;

        /// <summary>
        /// Whether to sort objects by row first or by column first
        /// </summary>
        [Tooltip("Whether to sort objects by row first or by column first")]
        private LayoutType layout = LayoutType.ColumnThenRow;

        /// <summary>
        /// This is the radius of either the Cylinder or Sphere mapping and is ignored when using the plane mapping.
        /// </summary>
        [Range(0.05f, 5.0f)]
        [Tooltip("Radius for the sphere or cylinder")]
        [SerializeField]
        private float radius = 2f;

        /// <summary>
        /// Number of rows per column, column number is automatically determined
        /// </summary>
        [Tooltip("Number of rows per column")]
        [SerializeField]
        private int rows = 3;

        /// <summary>
        /// Width of the cell per object in the collection.
        /// </summary>
        [Tooltip("Width of cell per object")]
        [SerializeField]
        private float cellWidth = 0.5f;

        /// <summary>
        /// Height of the cell per object in the collection.
        /// </summary>
        [Tooltip("Height of cell per object")]
        [SerializeField]
        private float cellHeight = 0.5f;

        private Mesh sphereMesh;
        private Mesh cylinderMesh;
        private int columns;
        private float width;
        private float height;
        private float circumferenc;
        private Vector2 halfcell;
        #endregion

        #region public accessors
        public ObjectOrientationSurfaceType SurfaceType
        {
            get { return surfaceType; }
            set { surfaceType = value; }

        }
        public OrientationType OrientType
        {
            get { return orientType; }
            set { orientType = value; }

        }
        public LayoutType Layout
        {
            get { return layout; }
            set { layout = value; }

        }
        public float Radius
        {
            get { return radius; }
            set { radius = value; }

        }
        public int Rows
        {
            get { return rows; }
            set { rows = value; }

        }
        public float CellWidth
        {
            get { return cellWidth; }
            set { cellWidth = value; }

        }
        public float CellHeight
        {
            get { return cellHeight; }
            set { cellHeight = value; }

        }

        /// <summary>
        /// Reference mesh to use for rendering the sphere layout
        /// </summary>
        public Mesh SphereMesh
        {
            get { return sphereMesh; }
            set { sphereMesh = value; }

        }

        /// <summary>
        /// Reference mesh to use for rendering the cylinder layout
        /// </summary>
        public Mesh CylinderMesh
        {
            get { return cylinderMesh; }
            set { cylinderMesh = value; }
        }

        public float Width
        {
            get { return width; }
        }

        public float Height
        {
            get { return height; }
        }
        #endregion


        /// <summary>
        /// Overriding base function function for laying out all the children when UpdateCollection is called.
        /// </summary>
        protected override void LayoutChildren()
        {

            columns = Mathf.CeilToInt((float)NodeList.Count / Rows);
            width = columns * CellWidth;
            height = Rows * CellHeight;
            halfcell = new Vector2(CellWidth / 2f, CellHeight / 2f);
            circumferenc = 2f * Mathf.PI * Radius;


            int cellCounter = 0;
            float startOffsetX;
            float startOffsetY;

            Vector3[] nodeGrid = new Vector3[NodeList.Count];
            Vector3 newPos = Vector3.zero;
            Vector3 newRot = Vector3.zero;

            // Now lets lay out the grid
            startOffsetX = (columns * 0.5f) * CellWidth;
            startOffsetY = (Rows * 0.5f) * CellHeight;

            cellCounter = 0;

            // First start with a grid then project onto surface
            switch (layout)
            {
                case LayoutType.ColumnThenRow:
                default:
                    for (int c = 0; c < columns; c++)
                    {
                        for (int r = 0; r < Rows; r++)
                        {
                            if (cellCounter < NodeList.Count)
                            {
                                nodeGrid[cellCounter] = new Vector3((c * CellWidth) - startOffsetX + halfcell.x, -(r * CellHeight) + startOffsetY - halfcell.y, 0f) + (Vector3)((NodeList[cellCounter])).Offset;
                            }
                            cellCounter++;
                        }
                    }
                    break;

                case LayoutType.RowThenColumn:
                    for (int r = 0; r < Rows; r++)
                    {
                        for (int c = 0; c < columns; c++)
                        {
                            if (cellCounter < NodeList.Count)
                            {
                                nodeGrid[cellCounter] = new Vector3((c * CellWidth) - startOffsetX + halfcell.x, -(r * CellHeight) + startOffsetY - halfcell.y, 0f) + (Vector3)((NodeList[cellCounter])).Offset;
                            }
                            cellCounter++;
                        }
                    }
                    break;

            }

            switch (SurfaceType)
            {
                case ObjectOrientationSurfaceType.Plane:
                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        newPos = nodeGrid[i];
                        NodeList[i].transform.localPosition = newPos;
                        switch (OrientType)
                        {
                            case OrientationType.FaceOrigin:
                            case OrientationType.FaceFoward:
                                NodeList[i].transform.forward = transform.forward;
                                break;

                            case OrientationType.FaceOriginReversed:
                            case OrientationType.FaceForwardReversed:
                                newRot = Vector3.zero;
                                NodeList[i].transform.forward = transform.forward;
                                NodeList[i].transform.Rotate(0f, 180f, 0f);
                                break;

                            case OrientationType.None:
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    break;
                case ObjectOrientationSurfaceType.Cylinder:
                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        newPos = CylindricalMapping(nodeGrid[i], Radius);
                        switch (OrientType)
                        {
                            case OrientationType.FaceOrigin:
                                newRot = new Vector3(newPos.x, 0.0f, newPos.z);
                                NodeList[i].transform.rotation = Quaternion.LookRotation(newRot);
                                break;

                            case OrientationType.FaceOriginReversed:
                                newRot = new Vector3(newPos.x, 0f, newPos.z);
                                NodeList[i].transform.rotation = Quaternion.LookRotation(newRot);
                                NodeList[i].transform.Rotate(0f, 180f, 0f);
                                break;

                            case OrientationType.FaceFoward:
                                NodeList[i].transform.forward = transform.forward;
                                break;

                            case OrientationType.FaceForwardReversed:
                                NodeList[i].transform.forward = transform.forward;
                                NodeList[i].transform.Rotate(0f, 180f, 0f);
                                break;

                            case OrientationType.None:
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        NodeList[i].transform.localPosition = newPos;
                    }
                    break;
                case ObjectOrientationSurfaceType.Sphere:

                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        newPos = SphericalMapping(nodeGrid[i], Radius);
                        switch (OrientType)
                        {
                            case OrientationType.FaceOrigin:
                                newRot = newPos;
                                NodeList[i].transform.rotation = Quaternion.LookRotation(newRot);
                                break;

                            case OrientationType.FaceOriginReversed:
                                newRot = newPos;
                                NodeList[i].transform.rotation = Quaternion.LookRotation(newRot);
                                NodeList[i].transform.Rotate(0f, 180f, 0f);
                                break;

                            case OrientationType.FaceFoward:
                                NodeList[i].transform.forward = transform.forward;
                                break;

                            case OrientationType.FaceForwardReversed:
                                NodeList[i].transform.forward = transform.forward;
                                NodeList[i].transform.Rotate(0f, 180f, 0f);
                                break;

                            case OrientationType.None:
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        NodeList[i].transform.localPosition = newPos;
                    }
                    break;

                case ObjectOrientationSurfaceType.Scatter:
                    // Get randomized planar mapping
                    // Calculate radius of each node while we're here
                    // Then use the packer function to shift them into place
                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        newPos = ScatterMapping(nodeGrid[i], Radius);
                        Collider nodeCollider = NodeList[i].transform.GetComponentInChildren<Collider>();
                        if (nodeCollider != null)
                        {
                            // Make the radius the largest of the object's dimensions to avoid overlap
                            Bounds bounds = nodeCollider.bounds;
                            NodeList[i].Radius = Mathf.Max(Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z) / 2;
                        }
                        else
                        {
                            NodeList[i].Radius = Radius;
                        }
                        NodeList[i].transform.localPosition = newPos;
                        switch (OrientType)
                        {
                            case OrientationType.FaceOrigin:
                            case OrientationType.FaceFoward:
                                NodeList[i].transform.rotation = Quaternion.LookRotation(newRot);
                                break;

                            case OrientationType.FaceOriginReversed:
                            case OrientationType.FaceForwardReversed:
                                NodeList[i].transform.rotation = Quaternion.LookRotation(newRot);
                                NodeList[i].transform.Rotate(0f, 180f, 0f);
                                break;

                            case OrientationType.None:
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    // Iterate [x] times
                    for (int i = 0; i < 100; i++)
                    {
                        IterateScatterPacking(NodeList, Radius);
                    }
                    break;
            }
        }

        // Internal function for getting the relative mapping based on a source Vec3 and a radius for spherical mapping.
        // The source to be mapped to sphere, radius the radius of the sphere</param>
        private Vector3 SphericalMapping(Vector3 source, float radius)
        {
            Radius = radius >= 0 ? Radius : radius;
            Vector3 newPos = new Vector3(0f, 0f, Radius);

            float xAngle = (source.x / circumferenc) * 360f;
            float yAngle = -(source.y / circumferenc) * 360f;

            Quaternion rot = Quaternion.Euler(yAngle, xAngle, 0.0f);
            newPos = rot * newPos;

            return newPos;
        }

        // Internal function for getting the relative mapping based on a source Vec3 and a radius for cylinder mapping.
        // source to be mapped to cylinder, Radius for the radius of the cylinder
        private Vector3 CylindricalMapping(Vector3 source, float radius)
        {
            Radius = radius >= 0 ? Radius : radius;
            Vector3 newPos = new Vector3(0f, source.y, Radius);

            float xAngle = (source.x / circumferenc) * 360f;

            Quaternion rot = Quaternion.Euler(0.0f, xAngle, 0.0f);
            newPos = rot * newPos;

            return newPos;
        }

        // Internal function to check if a node exists in the NodeList.
        private bool ContainsNode(Transform node)
        {
            for (int i = 0; i < NodeList.Count; i++)
            {
                if (NodeList[i] != null)
                {
                    if (NodeList[i].transform == node)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Internal function for randomized mapping based on a source Vec3 and a radius for randomization distance.
        private Vector3 ScatterMapping(Vector3 source, float radius)
        {
            source.x = UnityEngine.Random.Range(-radius, radius);
            source.y = UnityEngine.Random.Range(-radius, radius);
            return source;
        }


        // Internal function to pack randomly spaced nodes so they don't overlap
        // Usually requires about 25 iterations for decent packing
        private void IterateScatterPacking(List<ObjectCollectionNode> nodes, float radiusPadding)
        {
            // Sort by closest to center (don't worry about z axis)
            // Use the position of the collection as the packing center
            nodes.Sort(delegate (ObjectCollectionNode circle1, ObjectCollectionNode circle2) {
                float distance1 = (circle1.transform.localPosition).sqrMagnitude;
                float distance2 = (circle2.transform.localPosition).sqrMagnitude;
                return distance1.CompareTo(distance2);
            });

            Vector3 difference;
            Vector2 difference2D;

            // Move them closer together
            float radiusPaddingSquared = Mathf.Pow(radiusPadding, 2f);

            for (int i = 0; i < nodes.Count - 1; i++)
            {
                for (int j = i + 1; j < nodes.Count; j++)
                {
                    if (i != j)
                    {
                        difference = nodes[j].transform.localPosition - nodes[i].transform.localPosition;
                        // Ignore Z axis
                        difference2D.x = difference.x;
                        difference2D.y = difference.y;
                        float combinedRadius = nodes[i].Radius + nodes[j].Radius;
                        float distance = difference2D.SqrMagnitude() - radiusPaddingSquared;
                        float minSeparation = Mathf.Min(distance, radiusPaddingSquared);
                        distance -= minSeparation;

                        if (distance < (Mathf.Pow(combinedRadius, 2)))
                        {
                            difference2D.Normalize();
                            difference *= ((combinedRadius - Mathf.Sqrt(distance)) * 0.5f);
                            nodes[j].transform.localPosition += difference;
                            nodes[i].transform.localPosition -= difference;
                        }
                    }
                }
            }
        }

        // Gizmos to draw when the Collection is selected.
        protected virtual void OnDrawGizmosSelected()
        {
            Vector3 scale = (2f * Radius) * Vector3.one;
            switch (surfaceType)
            {
                case ObjectOrientationSurfaceType.Plane:
                    break;
                case ObjectOrientationSurfaceType.Cylinder:
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireMesh(CylinderMesh, transform.position, transform.rotation, scale);
                    break;
                case ObjectOrientationSurfaceType.Sphere:
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireMesh(SphereMesh, transform.position, transform.rotation, scale);
                    break;
            }
        }
    }
}
