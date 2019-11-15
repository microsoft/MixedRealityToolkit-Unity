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
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_ObjectCollection.html")]
    public partial class GridObjectCollection : BaseObjectCollection
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
        private OrientationType orientType = OrientationType.None;

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

        [SerializeField, Tooltip("Where the grid is anchored relative to local origin")]
        private LayoutAnchor anchor = LayoutAnchor.MiddleCenter;
        public LayoutAnchor Anchor 
        {
            get { return anchor; }
            set { anchor = value; }
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

        [SerializeField]
        [Tooltip("Distance for plane layout")]
        [Range(0f, 100f)]
        private float distance = 0f;

        /// <summary>
        /// This is the Distance for an offset for the Plane mapping and is ignored for the other mappings.
        /// </summary>
        public float Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        private static readonly int defaultValueRowsCols = 3;

        [Tooltip("Number of rows per column")]
        [SerializeField]
        private int rows = defaultValueRowsCols;

        /// <summary>
        /// Number of rows per column. Can only be assigned when layout type is
        /// RowsThenColumns
        /// </summary>
        public int Rows
        {
            get { return rows; }
            set 
            {
                if (Layout == LayoutOrder.ColumnThenRow) 
                {
                    Debug.LogError("When using ColumnThenRow layout, assign Columns instead of Rows.");
                    return;                    
                }
                rows = value; 
            }
        }

        [Tooltip("Number of columns per row")]
        [SerializeField]
        private int columns = defaultValueRowsCols;

        /// <summary>
        /// Number of columns per row. Can only be assigned when layout type is 
        /// ColumnsThenRows
        /// </summary>
        public int Columns
        {
            get { return columns; }
            set 
            { 
                if (Layout == LayoutOrder.RowThenColumn)
                {
                    Debug.LogError("When using RowThenColumn layout, assign Rows instead of Columns.");
                    return;
                }
                columns = value;
            }
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

        protected Vector2 HalfCell;

        /// <summary>
        /// Overriding base function for laying out all the children when UpdateCollection is called.
        /// </summary>
        protected override void LayoutChildren()
        {
            var nodeGrid = new Vector3[NodeList.Count];
            Vector3 newPos;

            // Now lets lay out the grid
            if (Layout == LayoutOrder.RowThenColumn)
            {
                columns = Mathf.CeilToInt((float)NodeList.Count / rows);
            }
            else if (Layout == LayoutOrder.ColumnThenRow)
            {
                rows = Mathf.CeilToInt((float)NodeList.Count / columns);
            }
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
                        newPos.z = distance;
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
                    iMax = Columns;
                    jMax = Rows;
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
            if (anchor == LayoutAnchor.BottomLeft || anchor == LayoutAnchor.UpperLeft || anchor == LayoutAnchor.MiddleLeft)
            {
                startOffsetX = 0;
            }
            else if (anchor == LayoutAnchor.BottomRight || anchor == LayoutAnchor.UpperRight || anchor == LayoutAnchor.MiddleRight)
            {
                startOffsetX = iMax * CellWidth;
            }

            float startOffsetY = (jMax * 0.5f) * CellHeight;
            if (anchor == LayoutAnchor.UpperLeft || anchor == LayoutAnchor.UpperCenter || anchor == LayoutAnchor.UpperRight)
            {
                startOffsetY = 0;
            }
            else if (anchor == LayoutAnchor.BottomLeft || anchor == LayoutAnchor.BottomCenter || anchor == LayoutAnchor.BottomRight)
            {
                startOffsetY = jMax * CellHeight;
            }

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
    

        public void OnValidate()
        {
            if (Application.isPlaying)
            {   // Don't validate during play mode
                return;
            }

            // Check upgrade from MRTK 2.X to 2.2
            // We used to always specify rows even when layout was column then row
            // 
            if (Layout == LayoutOrder.ColumnThenRow)
            {
                // We count number of children this way to avoid re-laying out children without button press
                int nodeListCount = 0;
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    if (!ContainsNode(child) && (child.gameObject.activeSelf || !IgnoreInactiveTransforms))
                    {
                        nodeListCount++;
                    }
                }

                // If we have an old asset, then rows could be != default value, columns would be default value
                bool upgradeScenarioA = rows != defaultValueRowsCols && columns == defaultValueRowsCols 
                    // We actually want default # of columns
                    && (nodeListCount <= rows * (columns - 1) || nodeListCount > rows * columns);
                // Edge case: user specified defaultValue rows in old code. Rows would be defaultValue, cols would be defaultValue.
                // This will be okay unless the number of children exceeds rows * cols
                bool upgradeScenarioB = rows == defaultValueRowsCols && columns == defaultValueRowsCols && nodeListCount > rows * columns;
                if (upgradeScenarioA || upgradeScenarioB)
                {
                    // Try to guess what the desired columns would be
                    int columnsGuess = Mathf.CeilToInt((float)nodeListCount / rows);
                    string objectName = gameObject.name;
                     if (gameObject.transform.parent != null) 
                     {
                         objectName += " (parent " + transform.parent.gameObject.name + ")";
                     }
                    Debug.Log("GridObjectCollection on " + objectName + " has layout ColumnsThenRows but columns are not specified. Most likely from asset upgrade to MRTK 2.2. Settings Columns property to " + nodeListCount + "/ " + rows + " = " + columnsGuess +". Check your asset to make sure GridObjectCollection has the correct values.");
                    Columns = columnsGuess;
                }
            }
        }

    }
}
