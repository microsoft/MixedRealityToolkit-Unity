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
        #region public accessors

        [Tooltip("Type of surface to map the collection to")]
        [SerializeField]
        private ObjectOrientationSurfaceType surfaceType = ObjectOrientationSurfaceType.Plane;

        /// <summary>
        /// Type of surface to map the collection to.
        /// </summary>
        public ObjectOrientationSurfaceType SurfaceType
        {
            get { return surfaceType; }
            set { surfaceType = value; }
        }

        [Tooltip("Should the objects in the collection be rotated / how should they be rotated")]
        [SerializeField]
        private OrientationType orientType = OrientationType.FaceOrigin;

        /// <summary>
        /// Should the objects in the collection face the origin of the collection
        /// </summary>
        public OrientationType OrientType
        {
            get { return orientType; }
            set { orientType = value; }
        }

        [Tooltip("Whether to sort objects by row first or by column first")]
        [SerializeField]
        private LayoutOrderType layout = LayoutOrderType.ColumnThenRow;

        /// <summary>
        /// Whether to sort objects by row first or by column first
        /// </summary>
        public LayoutOrderType Layout
        {
            get { return layout; }
            set { layout = value; }
        }

        [Range(0.05f, 100.0f)]
        [Tooltip("Radius for the sphere or cylinder")]
        [SerializeField]
        private float radius = 2f;

        /// <summary>
        /// This is the radius of either the Cylinder or Sphere mapping and is ignored when using the plane mapping.
        /// </summary>
        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        [SerializeField]
        [Tooltip("Radial range for radial layout")]
        [Range(5f, 360f)]
        private float radialRange = 180f;

        /// <summary>
        /// This is the radial range for creating a radial fan layout.
        /// </summary>
        public float RadialRange
        {
            get { return radialRange; }
            set { radialRange = value; }
        }

        [Tooltip("Number of rows per column")]
        [SerializeField]
        private int rows = 3;

        /// <summary>
        /// Number of rows per column, column number is automatically determined
        /// </summary>
        public int Rows
        {
            get { return rows; }
            set { rows = value; }
        }

        [Tooltip("Width of cell per object")]
        [SerializeField]
        private float cellWidth = 0.5f;

        /// <summary>
        /// Width of the cell per object in the collection.
        /// </summary>
        public float CellWidth
        {
            get { return cellWidth; }
            set { cellWidth = value; }
        }

        [Tooltip("Height of cell per object")]
        [SerializeField]
        private float cellHeight = 0.5f;

        /// <summary>
        /// Height of the cell per object in the collection.
        /// </summary>
        public float CellHeight
        {
            get { return cellHeight; }
            set { cellHeight = value; }
        }

        /// <summary>
        /// Total Width of collection
        /// </summary>
        public float Width
        {
            get
            {
                return columns * CellWidth;
            }
        }

        /// <summary>
        /// Total Height of collection
        /// </summary>
        public float Height
        {
            get
            {
                return rows * CellHeight;
            }
        }


        private Mesh sphereMesh;

        /// <summary>
        /// Reference mesh to use for rendering the sphere layout
        /// </summary>
        public Mesh SphereMesh
        {
            get;
            set;
        }

        private Mesh cylinderMesh;

        /// <summary>
        /// Reference mesh to use for rendering the cylinder layout
        /// </summary>
        public Mesh CylinderMesh
        {
            get;
            set;
        }

        #endregion public accessors

        #region private fields

        protected int columns;
        
        protected Vector2 halfCell;

        #endregion private fields

        /// <summary>
        /// Overriding base function for laying out all the children when UpdateCollection is called.
        /// </summary>
        protected override void LayoutChildren()
        {
            float startOffsetX;
            float startOffsetY;
            Vector3[] nodeGrid = new Vector3[NodeList.Count];
            Vector3 newPos = Vector3.zero;

            // Now lets lay out the grid
            columns = Mathf.CeilToInt((float)NodeList.Count / rows);
            startOffsetX = (columns * 0.5f) * CellWidth;
            startOffsetY = (rows * 0.5f) * CellHeight;
            halfCell = new Vector2(CellWidth * 0.5f, CellHeight * 0.5f);

            // First start with a grid then project onto surface
            ResolveGridLayout(nodeGrid, startOffsetX, startOffsetY, layout);

            switch (SurfaceType)
            {
                case ObjectOrientationSurfaceType.Plane:
                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        ObjectCollectionNode node = NodeList[i];
                        newPos = nodeGrid[i];
                        //Debug.Log(newPos);
                        node.transform.localPosition = newPos;
                        UpdateNodeFacing(node);
                        NodeList[i] = node;
                    }
                    break;

                case ObjectOrientationSurfaceType.Cylinder:
                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        ObjectCollectionNode node = NodeList[i];
                        newPos = VectorExtensions.CylindricalMapping(nodeGrid[i], radius);
                        node.transform.localPosition = newPos;
                        UpdateNodeFacing(node);
                        NodeList[i] = node;
                    }
                    break;

                case ObjectOrientationSurfaceType.Sphere:

                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        ObjectCollectionNode node = NodeList[i];
                        newPos = VectorExtensions.SphericalMapping(nodeGrid[i], radius);
                        node.transform.localPosition = newPos;
                        UpdateNodeFacing(node);
                        NodeList[i] = node;
                    }
                    break;

                case ObjectOrientationSurfaceType.Radial:
                    int curColumn = 0;
                    int curRow = 1;

                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        ObjectCollectionNode node = NodeList[i];
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

                        node.transform.localPosition = newPos;
                        UpdateNodeFacing(node);
                        NodeList[i] = node;
                    }
                    break;
            }
        }

        protected void ResolveGridLayout(Vector3[] grid, float offsetX, float offsetY, LayoutOrderType order)
        {
            int cellCounter = 0;
            float iMax;
            float jMax;

            if (order == LayoutOrderType.RowThenColumn)
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
                        grid[cellCounter].Set
                            (
                            ((i * CellWidth) - offsetX + halfCell.x) + NodeList[cellCounter].Offset.x,
                            (-(j * CellHeight) + offsetY - halfCell.y) + NodeList[cellCounter].Offset.y,
                            0.0f
                            );
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
        protected void UpdateNodeFacing(ObjectCollectionNode node)
        {
            Vector3 centerAxis;
            Vector3 pointOnAxisNearestNode;
            switch (OrientType)
            {
                case OrientationType.FaceOrigin:
                    node.transform.rotation = Quaternion.LookRotation(node.transform.position - transform.position, transform.up);
                    break;

                case OrientationType.FaceOriginReversed:
                    node.transform.rotation = Quaternion.LookRotation(transform.position - node.transform.position, transform.up);
                    break;

                case OrientationType.FaceCenterAxis:
                    centerAxis = Vector3.Project(node.transform.position - transform.position, transform.up);
                    pointOnAxisNearestNode = transform.position + centerAxis;
                    node.transform.rotation = Quaternion.LookRotation(node.transform.position - pointOnAxisNearestNode, transform.up);
                    break;

                case OrientationType.FaceCenterAxisReversed:
                    centerAxis = Vector3.Project(node.transform.position - transform.position, transform.up);
                    pointOnAxisNearestNode = transform.position + centerAxis;
                    node.transform.rotation = Quaternion.LookRotation(pointOnAxisNearestNode - node.transform.position, transform.up);
                    break;

                case OrientationType.FaceParentFoward:
                    node.transform.forward = transform.rotation * Vector3.forward;
                    break;

                case OrientationType.FaceParentForwardReversed:
                    node.transform.forward = transform.rotation * Vector3.back;
                    break;

                case OrientationType.FaceParentUp:
                    node.transform.forward = transform.rotation * Vector3.up;
                    break;

                case OrientationType.FaceParentDown:
                    node.transform.forward = transform.rotation * Vector3.down;
                    break;

                case OrientationType.None:
                    break;

                default:
                    Debug.LogWarning("OrientationType out of range");
                    break;
            }
        }

        // Gizmos to draw when the Collection is selected.
        protected virtual void OnDrawGizmosSelected()
        {
            Vector3 scale = (2f * radius) * Vector3.one;
            switch (surfaceType)
            {
                case ObjectOrientationSurfaceType.Plane:
                    break;
                case ObjectOrientationSurfaceType.Cylinder:
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireMesh(cylinderMesh, transform.position, transform.rotation, scale);
                    break;
                case ObjectOrientationSurfaceType.Sphere:
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireMesh(sphereMesh, transform.position, transform.rotation, scale);
                    break;
            }
        }
    }
}
