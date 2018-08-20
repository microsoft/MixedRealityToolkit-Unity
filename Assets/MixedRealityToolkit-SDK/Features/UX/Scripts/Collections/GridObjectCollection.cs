// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Collections
{
    /// <summary>
    /// A Grid Object Collection is simply a set of child objects organized with some
    /// layout parameters.  The collection can be used to quickly create 
    /// control panels or sets of prefab/objects.
    /// </summary>
    public class GridObjectCollection : BaseObjectCollection
    {
        #region private fields
        [Tooltip("Type of surface to map the collection to")]
        private ObjectOrientationSurfaceTypeEnum surfaceType = ObjectOrientationSurfaceTypeEnum.Plane;

        [Tooltip("Should the objects in the collection be rotated / how should they be rotated")]
        private OrientationTypeEnum orientType = OrientationTypeEnum.FaceOrigin;

        [Tooltip("Whether to sort objects by row first or by column first")]
        private LayoutOrderTypeEnum layout = LayoutOrderTypeEnum.ColumnThenRow;

        [Range(0.05f, 5.0f)]
        [Tooltip("Radius for the sphere or cylinder")]
        [SerializeField]
        private float radius = 2f;

        [SerializeField]
        [Tooltip("Radial range for radial layout")]
        [Range(5f, 360f)]
        private float radialRange = 180f;

        [Tooltip("Number of rows per column")]
        [SerializeField]
        private int rows = 3;

        [Tooltip("Width of cell per object")]
        [SerializeField]
        private float cellWidth = 0.5f;

        [Tooltip("Height of cell per object")]
        [SerializeField]
        private float cellHeight = 0.5f;

        private Mesh sphereMesh;
        private Mesh cylinderMesh;
        private int columns;
        private float width;
        private float height;
        private float circumference;
        private Vector2 halfCell;
        #endregion private fields

        #region public accessors
        /// <summary>
        /// Type of surface to map the collection to.
        /// </summary>
        public ObjectOrientationSurfaceTypeEnum SurfaceType
        {
            get { return surfaceType; }
            set { surfaceType = value; }

        }

        /// <summary>
        /// Should the objects in the collection face the origin of the collection
        /// </summary>
        public OrientationTypeEnum OrientType
        {
            get { return orientType; }
            set { orientType = value; }

        }

        /// <summary>
        /// Whether to sort objects by row first or by column first
        /// </summary>
        public LayoutOrderTypeEnum Layout
        {
            get { return layout; }
            set { layout = value; }

        }

        /// <summary>
        /// This is the radius of either the Cylinder or Sphere mapping and is ignored when using the plane mapping.
        /// </summary>
        public float Radius
        {
            get { return radius; }
            set { radius = value; }

        }

        /// <summary>
        /// This is the radial range for creating a radial fan layout.
        /// </summary>
        public float RadialRange
        {
            get { return radialRange; }
            set { radialRange = value; }
        }

        /// <summary>
        /// Number of rows per column, column number is automatically determined
        /// </summary>
        public int Rows
        {
            get { return rows; }
            set { rows = value; }

        }

        /// <summary>
        /// Width of the cell per object in the collection.
        /// </summary>
        public float CellWidth
        {
            get { return cellWidth; }
            set { cellWidth = value; }

        }

        /// <summary>
        /// Height of the cell per object in the collection.
        /// </summary>
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
        #endregion public accessors


        /// <summary>
        /// Overriding base function function for laying out all the children when UpdateCollection is called.
        /// </summary>
        protected override void LayoutChildren()
        {
            float startOffsetX;
            float startOffsetY;

            Vector3[] nodeGrid = new Vector3[NodeList.Count];
            Vector3 newPos = Vector3.zero;

            // Now lets lay out the grid
            startOffsetX = (columns * 0.5f) * CellWidth;
            startOffsetY = (rows * 0.5f) * CellHeight;
            halfCell = new Vector2(CellWidth * 0.5f, CellHeight * 0.5f);

            // First start with a grid then project onto surface
            ResolveGridLayout(nodeGrid, startOffsetX, startOffsetY, layout);

            switch (SurfaceType)
            {
                case ObjectOrientationSurfaceTypeEnum.Plane:
                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        newPos = nodeGrid[i];
                        NodeList[i].transform.localPosition = newPos;
                        UpdateNodeFacing(NodeList[i], orientType, newPos);
                    }
                    break;
                case ObjectOrientationSurfaceTypeEnum.Cylinder:
                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        newPos = VectorExtensions.CylindricalMapping(nodeGrid[i], radius);
                        NodeList[i].transform.localPosition = newPos;
                        UpdateNodeFacing(NodeList[i], orientType, newPos);
                    }
                    break;
                case ObjectOrientationSurfaceTypeEnum.Sphere:

                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        newPos = VectorExtensions.SphericalMapping(nodeGrid[i], radius);
                        NodeList[i].transform.localPosition = newPos;
                        UpdateNodeFacing(NodeList[i], orientType, newPos);
                    }
                    break;
                case ObjectOrientationSurfaceTypeEnum.Radial:
                    int curColumn = 0;
                    int curRow = 1;

                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        newPos = VectorExtensions.RadialMapping(nodeGrid[i], radialRange, radius, curRow, rows, curColumn, columns);
                        if (curColumn == (columns - 1))
                        {
                            curColumn = 0;
                            ++curRow;
                        }
                        else
                        {
                            ++curColumn;
                        }

                        NodeList[i].transform.localPosition = newPos;
                        UpdateNodeFacing(NodeList[i], orientType, newPos);
                    }
                    break;
                case ObjectOrientationSurfaceTypeEnum.Scatter:
                    // Get randomized planar mapping
                    // Calculate radius of each node while we're here
                    // Then use the packer function to shift them into place
                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        newPos = VectorExtensions.ScatterMapping(nodeGrid[i], Radius);
                        Collider nodeCollider = NodeList[i].transform.GetComponentInChildren<Collider>();
                        if (nodeCollider != null)
                        {
                            // Make the radius the largest of the object's dimensions to avoid overlap
                            Bounds bounds = nodeCollider.bounds;
                            NodeList[i].Radius = Mathf.Max(Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z) * 0.5f;
                        }
                        else
                        {
                            // Make the radius a default value
                            // TODO move this into a public field ?
                            NodeList[i].Radius = 1f;
                        }
                        NodeList[i].transform.localPosition = newPos;
                        UpdateNodeFacing(NodeList[i], orientType, newPos);
                    }

                    // Iterate [x] times
                    // TODO move center, iterations and padding into a public field
                    for (int i = 0; i < 100; i++)
                    {
                        IterateScatterPacking(NodeList, Radius);
                    }
                    break;
            }
        }

        private void ResolveGridLayout(Vector3[] grid, float offsetX, float offsetY, LayoutOrderTypeEnum order)
        {
            int cellCounter = 0;
            float iMax;
            float jMax;

            if (order == LayoutOrderTypeEnum.RowThenColumn)
            {
                iMax = Rows;
                jMax = columns;
            }
            else
            {
                iMax = columns;
                jMax = Rows;
            }

            for (int i = 0; i < iMax; i++)
            {
                for (int j = 0; j < jMax; j++)
                {
                    if (cellCounter < NodeList.Count)
                    {
                        grid[cellCounter].Set(
                            ((i * CellWidth) - offsetX + halfCell.x) + NodeList[cellCounter].Offset.x, (-(j * CellHeight) + offsetY - halfCell.y) + NodeList[cellCounter].Offset.y, 0.0f);
                    }
                    cellCounter++;
                }
            }
        }

        /// <summary>
        /// Update the facing of a node given the nodes new position for facing orign with node and orientation type
        /// </summary>
        /// <param name="node"></param>
        /// <param name="orientType"></param>
        /// <param name="newPos"></param>
        private void UpdateNodeFacing(ObjectCollectionNode node, OrientationTypeEnum orientType, Vector3 newPos = default(Vector3))
        {
            Vector3 centerAxis;
            Vector3 pointOnAxisNearestNode;
            switch (OrientType)
            {
                case OrientationTypeEnum.FaceOrigin:
                    node.transform.rotation = Quaternion.LookRotation(node.transform.position - this.transform.position, this.transform.up);
                    break;

                case OrientationTypeEnum.FaceOriginReversed:
                    node.transform.rotation = Quaternion.LookRotation(this.transform.position - node.transform.position, this.transform.up);
                    break;

                case OrientationTypeEnum.FaceCenterAxis:
                    centerAxis = Vector3.Project(node.transform.position - this.transform.position, this.transform.up);
                    pointOnAxisNearestNode = this.transform.position + centerAxis;
                    node.transform.rotation = Quaternion.LookRotation(node.transform.position - pointOnAxisNearestNode, this.transform.up);
                    break;

                case OrientationTypeEnum.FaceCenterAxisReversed:
                    centerAxis = Vector3.Project(node.transform.position - this.transform.position, this.transform.up);
                    pointOnAxisNearestNode = this.transform.position + centerAxis;
                    node.transform.rotation = Quaternion.LookRotation(pointOnAxisNearestNode - node.transform.position, this.transform.up);
                    break;

                case OrientationTypeEnum.FaceFoward:
                    node.transform.forward = transform.rotation * Vector3.forward;
                    break;

                case OrientationTypeEnum.FaceForwardReversed:
                    node.transform.forward = transform.rotation * Vector3.back;
                    break;

                case OrientationTypeEnum.FaceParentUp:
                    node.transform.forward = transform.rotation * Vector3.up;
                    break;

                case OrientationTypeEnum.FaceParentDown:
                    node.transform.forward = transform.rotation * Vector3.down;
                    break;

                case OrientationTypeEnum.None:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Check if a node exists in the NodeList.
        /// </summary>
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
        /// <summary>
        /// Pack randomly spaced nodes so they don't overlap
        /// Usually requires about 25 iterations for decent packing
        /// </summary>
        private void IterateScatterPacking(List<ObjectCollectionNode> nodes, float radiusPadding)
        {
            // Sort by closest to center (don't worry about z axis)
            // Use the position of the collection as the packing center
            nodes.Sort(ScatterSort);

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

        private int ScatterSort(ObjectCollectionNode circle1, ObjectCollectionNode circle2)
        {
            float distance1 = (circle1.transform.localPosition).sqrMagnitude;
            float distance2 = (circle2.transform.localPosition).sqrMagnitude;
            return distance1.CompareTo(distance2);
        }

        // Gizmos to draw when the Collection is selected.
        protected virtual void OnDrawGizmosSelected()
        {
            Vector3 scale = (2f * Radius) * Vector3.one;
            switch (surfaceType)
            {
                case ObjectOrientationSurfaceTypeEnum.Plane:
                    break;
                case ObjectOrientationSurfaceTypeEnum.Cylinder:
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireMesh(CylinderMesh, transform.position, transform.rotation, scale);
                    break;
                case ObjectOrientationSurfaceTypeEnum.Sphere:
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireMesh(SphereMesh, transform.position, transform.rotation, scale);
                    break;
            }
        }
    }
}
