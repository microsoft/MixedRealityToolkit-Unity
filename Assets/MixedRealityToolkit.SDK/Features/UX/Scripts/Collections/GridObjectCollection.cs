// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// A Grid Object Collection is simply a set of child objects organized with some
    /// layout parameters.  The collection can be used to quickly create 
    /// control panels or sets of prefab/objects.
    /// </summary>
    public class GridObjectCollection : BaseObjectCollection
    {
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
        private LayoutOrder layout = LayoutOrder.ColumnThenRow;

        /// <summary>
        /// Whether to sort objects by row first or by column first
        /// </summary>
        public LayoutOrder Layout
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
        public float Width => Columns * CellWidth;

        /// <summary>
        /// Total Height of collection
        /// </summary>
        public float Height => rows * CellHeight;

        /// <summary>
        /// Reference mesh to use for rendering the sphere layout
        /// </summary>
        public Mesh SphereMesh { get; set; }

        /// <summary>
        /// Reference mesh to use for rendering the cylinder layout
        /// </summary>
        public Mesh CylinderMesh { get; set; }

        protected int Columns;

        protected Vector2 HalfCell;

        /// <summary>
        /// Overriding base function for laying out all the children when UpdateCollection is called.
        /// </summary>
        protected override void LayoutChildren()
        {
            var nodeGrid = new Vector3[NodeList.Count];
            Vector3 newPos;

            // Now lets lay out the grid
            Columns = Mathf.CeilToInt((float)NodeList.Count / rows);
            HalfCell = new Vector2(CellWidth * 0.5f, CellHeight * 0.5f);

            // First start with a grid then project onto surface
            ResolveGridLayout(nodeGrid, layout);

            switch (SurfaceType)
            {
                case ObjectOrientationSurfaceType.Plane:
                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        ObjectCollectionNode node = NodeList[i];
                        newPos = nodeGrid[i];
                        node.Transform.localPosition = newPos;
                        UpdateNodeFacing(node);
                        NodeList[i] = node;
                    }
                    break;

                case ObjectOrientationSurfaceType.Cylinder:
                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        ObjectCollectionNode node = NodeList[i];
                        newPos = VectorExtensions.CylindricalMapping(nodeGrid[i], radius);
                        node.Transform.localPosition = newPos;
                        UpdateNodeFacing(node);
                        NodeList[i] = node;
                    }
                    break;

                case ObjectOrientationSurfaceType.Sphere:

                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        ObjectCollectionNode node = NodeList[i];
                        newPos = VectorExtensions.SphericalMapping(nodeGrid[i], radius);
                        node.Transform.localPosition = newPos;
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
                        newPos = VectorExtensions.RadialMapping(nodeGrid[i], radialRange, radius, curRow, rows, curColumn, Columns);

                        if (curColumn == (Columns - 1))
                        {
                            curColumn = 0;
                            ++curRow;
                        }
                        else
                        {
                            ++curColumn;
                        }

                        node.Transform.localPosition = newPos;
                        UpdateNodeFacing(node);
                        NodeList[i] = node;
                    }
                    break;
            }
        }

        protected void ResolveGridLayout(Vector3[] grid, LayoutOrder order)
        {
            int cellCounter = 0;
            int iMax, jMax;

            switch (order)
            {
                case LayoutOrder.RowThenColumn:
                    iMax = Rows;
                    jMax = Columns;
                    break;
                case LayoutOrder.ColumnThenRow:
                    iMax = Columns;
                    jMax = Rows;
                    break;
                case LayoutOrder.Vertical:
                    iMax = 1;
                    jMax = NodeList.Count;
                    break;
                case LayoutOrder.Horizontal:
                    iMax = NodeList.Count;
                    jMax = 1;
                    break;
                default:
                    iMax = Mathf.CeilToInt((float)NodeList.Count / rows);
                    jMax = rows;
                    break;
            }

            float startOffsetX = (iMax * 0.5f) * CellWidth;
            float startOffsetY = (jMax * 0.5f) * CellHeight;

            for (int i = 0; i < iMax; i++)
            {
                for (int j = 0; j < jMax; j++)
                {
                    if (cellCounter < NodeList.Count)
                    {
                        grid[cellCounter].Set((-startOffsetX + (i * CellWidth) + HalfCell.x) + NodeList[cellCounter].Offset.x,
                                             (startOffsetY - (j * CellHeight) - HalfCell.y) + NodeList[cellCounter].Offset.y,
                                             0.0f);
                    }
                    cellCounter++;
                }
            }
        }

        /// <summary>
        /// Update the facing of a node given the nodes new position for facing origin with node and orientation type
        /// </summary>
        /// <param name="node"></param>
        protected void UpdateNodeFacing(ObjectCollectionNode node)
        {
            Vector3 centerAxis;
            Vector3 pointOnAxisNearestNode;
            switch (OrientType)
            {
                case OrientationType.FaceOrigin:
                    node.Transform.rotation = Quaternion.LookRotation(node.Transform.position - transform.position, transform.up);
                    break;

                case OrientationType.FaceOriginReversed:
                    node.Transform.rotation = Quaternion.LookRotation(transform.position - node.Transform.position, transform.up);
                    break;

                case OrientationType.FaceCenterAxis:
                    centerAxis = Vector3.Project(node.Transform.position - transform.position, transform.up);
                    pointOnAxisNearestNode = transform.position + centerAxis;
                    node.Transform.rotation = Quaternion.LookRotation(node.Transform.position - pointOnAxisNearestNode, transform.up);
                    break;

                case OrientationType.FaceCenterAxisReversed:
                    centerAxis = Vector3.Project(node.Transform.position - transform.position, transform.up);
                    pointOnAxisNearestNode = transform.position + centerAxis;
                    node.Transform.rotation = Quaternion.LookRotation(pointOnAxisNearestNode - node.Transform.position, transform.up);
                    break;

                case OrientationType.FaceParentFoward:
                    node.Transform.forward = transform.rotation * Vector3.forward;
                    break;

                case OrientationType.FaceParentForwardReversed:
                    node.Transform.forward = transform.rotation * Vector3.back;
                    break;

                case OrientationType.FaceParentUp:
                    node.Transform.forward = transform.rotation * Vector3.up;
                    break;

                case OrientationType.FaceParentDown:
                    node.Transform.forward = transform.rotation * Vector3.down;
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