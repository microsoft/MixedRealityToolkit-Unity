//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections.Generic;
using System;

namespace HoloToolkit.Unity.Collections
{
    /// <summary>
    /// An Object Collection is simply a set of child objects organized with some
    /// layout parameters.  The object collection can be used to quickly create 
    /// control panels or sets of prefab/objects.
    /// </summary>
    public class ObjectCollection : MonoBehaviour
    {
        #region public members
        /// <summary>
        /// Action called when collection is updated
        /// </summary>
        public Action<ObjectCollection> OnCollectionUpdated;

        /// <summary>
        /// List of objects with generated data on the object.
        /// </summary>
        [SerializeField]
        public List<CollectionNode> NodeList = new List<CollectionNode>();

        /// <summary>
        /// Type of surface to map the collection to.
        /// </summary>
        [Tooltip("Type of surface to map the collection to")]
        public SurfaceTypeEnum SurfaceType = SurfaceTypeEnum.Plane;

        /// <summary>
        /// Type of sorting to use.
        /// </summary>
        [Tooltip("Type of sorting to use")]
        public SortTypeEnum SortType = SortTypeEnum.None;

        /// <summary>
        /// Should the objects in the collection face the origin of the collection
        /// </summary>
        [Tooltip("Should the objects in the collection be rotated / how should they be rotated")]
        public OrientTypeEnum OrientType = OrientTypeEnum.FaceOrigin;

        /// <summary>
        /// Whether to sort objects by row first or by column first
        /// </summary>
        [Tooltip("Whether to sort objects by row first or by column first")]
        public LayoutTypeEnum LayoutType = LayoutTypeEnum.ColumnThenRow;

        /// <summary>
        /// Whether to treat inactive transforms as 'invisible'
        /// </summary>
        public bool IgnoreInactiveTransforms = true;

        /// <summary>
        /// This is the radius of either the Cylinder or Sphere mapping and is ignored when using the plane mapping.
        /// </summary>
        [Range(0.05f, 5.0f)]
        [Tooltip("Radius for the sphere or cylinder")]
        public float Radius = 2f;

        /// <summary>
        /// Number of rows per column, column number is automatically determined
        /// </summary>
        [Tooltip("Number of rows per column")]
        public int Rows = 3;

        /// <summary>
        /// Width of the cell per object in the collection.
        /// </summary>
        [Tooltip("Width of cell per object")]
        public float CellWidth = 0.5f;

        /// <summary>
        /// Height of the cell per object in the collection.
        /// </summary>
        [Tooltip("Height of cell per object")]
        public float CellHeight = 0.5f;

        /// <summary>
        /// Reference mesh to use for rendering the sphere layout
        /// </summary>
        [HideInInspector]
        public Mesh SphereMesh;

        /// <summary>
        /// Reference mesh to use for rendering the cylinder layout
        /// </summary>
        [HideInInspector]
        public Mesh CylinderMesh;
        #endregion


        #region private variables
        private int _columns;
        private float _width;
        private float _height;
        private float _circumference;
        private Vector2 _halfCell;
        #endregion

        public float Width
        {
            get { return _width; }
        }

        public float Height
        {
            get { return _height; }
        }

        /// <summary>
        /// Update collection is called from the editor button on the inspector.
        /// This function rebuilds / updates the layout.
        /// </summary>
        public void UpdateCollection()
        {
            // Check for empty nodes and remove them
            List<CollectionNode> emptyNodes = new List<CollectionNode>();

            for (int i = 0; i < NodeList.Count; i++)
            {
                if (NodeList[i].transform == null || (IgnoreInactiveTransforms && !NodeList[i].transform.gameObject.activeSelf))
                {
                    emptyNodes.Add(NodeList[i]);
                }
            }

            // Now delete the empty nodes
            for (int i = 0; i < emptyNodes.Count; i++)
            {
                NodeList.Remove(emptyNodes[i]);
            }

            emptyNodes.Clear();

            // Check when children change and adjust
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Transform child = this.transform.GetChild(i);

                if (!ContainsNode(child) && (child.gameObject.activeSelf || !IgnoreInactiveTransforms))
                {
                    CollectionNode node = new CollectionNode();

                    node.Name = child.name;
                    node.transform = child;
                    NodeList.Add(node);
                }
            }

            switch (SortType)
            {
                case SortTypeEnum.None:
                    break;

                case SortTypeEnum.Transform:
                    NodeList.Sort(delegate (CollectionNode c1, CollectionNode c2) { return c1.transform.GetSiblingIndex().CompareTo(c2.transform.GetSiblingIndex()); });
                    break;

                case SortTypeEnum.Alphabetical:
                    NodeList.Sort(delegate (CollectionNode c1, CollectionNode c2) { return c1.Name.CompareTo(c2.Name); });
                    break;

                case SortTypeEnum.AlphabeticalReversed:
                    NodeList.Sort(delegate (CollectionNode c1, CollectionNode c2) { return c1.Name.CompareTo(c2.Name); });
                    NodeList.Reverse();
                    break;

                case SortTypeEnum.TransformReversed:
                    NodeList.Sort(delegate (CollectionNode c1, CollectionNode c2) { return c1.transform.GetSiblingIndex().CompareTo(c2.transform.GetSiblingIndex()); });
                    NodeList.Reverse();
                    break;
            }

            _columns = Mathf.CeilToInt(NodeList.Count / Rows);
            _width = _columns * CellWidth;
            _height = Rows * CellHeight;
            _halfCell = new Vector2(CellWidth / 2f, CellHeight / 2f);
            _circumference = 2f * Mathf.PI * Radius;

            LayoutChildren();

            if (OnCollectionUpdated != null)
            {
                OnCollectionUpdated.Invoke(this);
            }
        }

        /// <summary>
        /// Internal function for laying out all the children when UpdateCollection is called.
        /// </summary>
        private void LayoutChildren() {
        
            int cellCounter = 0;
            float startOffsetX;
            float startOffsetY;

            Vector3[] nodeGrid = new Vector3[NodeList.Count];
            Vector3 newPos = Vector3.zero;
            Vector3 newRot = Vector3.zero;

            // Now lets lay out the grid
            startOffsetX = (_columns * 0.5f) * CellWidth;
            startOffsetY = (Rows * 0.5f) * CellHeight;

            cellCounter = 0;
            
            // First start with a grid then project onto surface
            switch (LayoutType)
            {
                case LayoutTypeEnum.ColumnThenRow:
                default:
                    for (int c = 0; c < _columns; c++)
                    {
                        for (int r = 0; r < Rows; r++)
                        {
                            if (cellCounter < NodeList.Count)
                            {
                                nodeGrid[cellCounter] = new Vector3((c * CellWidth) - startOffsetX + _halfCell.x, -(r * CellHeight) + startOffsetY - _halfCell.y, 0f) + (Vector3)((NodeList[cellCounter])).Offset;
                            }
                            cellCounter++;
                        }
                    }
                    break;

                case LayoutTypeEnum.RowThenColumn:
                    for (int r = 0; r < Rows; r++)
                    {
                        for (int c = 0; c < _columns; c++)
                        {
                            if (cellCounter < NodeList.Count)
                            {
                                nodeGrid[cellCounter] = new Vector3((c * CellWidth) - startOffsetX + _halfCell.x, -(r * CellHeight) + startOffsetY - _halfCell.y, 0f) + (Vector3)((NodeList[cellCounter])).Offset;
                            }
                            cellCounter++;
                        }
                    }
                    break;

            }

            switch (SurfaceType) {
                case SurfaceTypeEnum.Plane:
                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        newPos = nodeGrid[i];
                        NodeList[i].transform.localPosition = newPos;
                        switch (OrientType)
                        {
                            case OrientTypeEnum.FaceOrigin:
                            case OrientTypeEnum.FaceFoward:
                                NodeList[i].transform.forward = transform.forward;
                                break;

                            case OrientTypeEnum.FaceOriginReversed:
                            case OrientTypeEnum.FaceForwardReversed:
                                newRot = Vector3.zero;
                                NodeList[i].transform.forward = transform.forward;
                                NodeList[i].transform.Rotate(0f, 180f, 0f);
                                break;

                            case OrientTypeEnum.None:
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    break;
                case SurfaceTypeEnum.Cylinder:
                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        newPos = CylindricalMapping(nodeGrid[i], Radius);
                        switch (OrientType)
                        {
                            case OrientTypeEnum.FaceOrigin:
                                newRot = new Vector3(newPos.x, 0.0f, newPos.z);
                                NodeList[i].transform.rotation = Quaternion.LookRotation(newRot);
                                break;

                            case OrientTypeEnum.FaceOriginReversed:
                                newRot = new Vector3(newPos.x, 0f, newPos.z);
                                NodeList[i].transform.rotation = Quaternion.LookRotation(newRot);
                                NodeList[i].transform.Rotate(0f, 180f, 0f);
                                break;

                            case OrientTypeEnum.FaceFoward:
                                NodeList[i].transform.forward = transform.forward;
                                break;

                            case OrientTypeEnum.FaceForwardReversed:
                                NodeList[i].transform.forward = transform.forward;
                                NodeList[i].transform.Rotate(0f, 180f, 0f);
                                break;

                            case OrientTypeEnum.None:
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        NodeList[i].transform.localPosition = newPos;
                    }
                    break;
                case SurfaceTypeEnum.Sphere:

                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        newPos = SphericalMapping(nodeGrid[i], Radius);
                        switch (OrientType)
                        {
                            case OrientTypeEnum.FaceOrigin:
                                newRot = newPos;
                                NodeList[i].transform.rotation = Quaternion.LookRotation(newRot);
                                break;

                            case OrientTypeEnum.FaceOriginReversed:
                                newRot = newPos;
                                NodeList[i].transform.rotation = Quaternion.LookRotation(newRot);
                                NodeList[i].transform.Rotate(0f, 180f, 0f);
                                break;

                            case OrientTypeEnum.FaceFoward:
                                NodeList[i].transform.forward = transform.forward;
                                break;

                            case OrientTypeEnum.FaceForwardReversed:
                                NodeList[i].transform.forward = transform.forward;
                                NodeList[i].transform.Rotate(0f, 180f, 0f);
                                break;

                            case OrientTypeEnum.None:
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        NodeList[i].transform.localPosition = newPos;
                    }
                    break;

                case SurfaceTypeEnum.Scatter:
                    // Get randomized planar mapping
                    // Calculate radius of each node while we're here
                    // Then use the packer function to shift them into place
                    for (int i = 0; i < NodeList.Count; i++)
                    {
                        newPos = ScatterMapping (nodeGrid[i], Radius);
                        Collider nodeCollider = NodeList[i].transform.GetComponentInChildren<Collider>();
                        if (nodeCollider != null)
                        {
                            // Make the radius the largest of the object's dimensions to avoid overlap
                            Bounds bounds = nodeCollider.bounds;
                            NodeList[i].Radius = Mathf.Max (Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z) / 2;
                        }
                        else
                        {
                            // Make the radius a default value
                            // TODO move this into a public field ?
                            NodeList[i].Radius = 1f;
                        }
                        NodeList[i].transform.localPosition = newPos;
                        switch (OrientType)
                        {
                            case OrientTypeEnum.FaceOrigin:
                            case OrientTypeEnum.FaceFoward:
                                NodeList[i].transform.rotation = Quaternion.LookRotation(newRot);
                                break;

                            case OrientTypeEnum.FaceOriginReversed:
                            case OrientTypeEnum.FaceForwardReversed:
                                NodeList[i].transform.rotation = Quaternion.LookRotation(newRot);
                                NodeList[i].transform.Rotate(0f, 180f, 0f);
                                break;

                            case OrientTypeEnum.None:
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    // Iterate [x] times
                    // TODO move center, iterations and padding into a public field
                    for (int i = 0; i < 100; i++)
                    {
                        IterateScatterPacking (NodeList, Radius);
                    }
                    break;
            }
        }

        /// <summary>
        /// Internal function for getting the relative mapping based on a source Vec3 and a radius for spherical mapping.
        /// </summary>
        /// <param name="source">The source <see cref="Vector3"/> to be mapped to sphere</param>
        /// <param name="radius">This is a <see cref="float"/> for the radius of the sphere</param>
        /// <returns></returns>
        private Vector3 SphericalMapping(Vector3 source, float radius)
        {
            Radius = radius >= 0 ? Radius : radius;
            Vector3 newPos = new Vector3(0f, 0f, Radius);

            float xAngle = (source.x / _circumference) * 360f;
            float yAngle = -(source.y / _circumference) * 360f;

            Quaternion rot = Quaternion.Euler(yAngle, xAngle, 0.0f);
            newPos = rot * newPos;

            return newPos;
        }

        /// <summary>
        /// Internal function for getting the relative mapping based on a source Vec3 and a radius for cylinder mapping.
        /// </summary>
        /// <param name="source">The source <see cref="Vector3"/> to be mapped to cylinder</param>
        /// <param name="radius">This is a <see cref="float"/> for the radius of the cylinder</param>
        /// <returns></returns>
        private Vector3 CylindricalMapping(Vector3 source, float radius)
        {
            Radius = radius >= 0 ? Radius : radius;
            Vector3 newPos = new Vector3(0f, source.y, Radius);

            float xAngle = (source.x / _circumference) * 360f;

            Quaternion rot = Quaternion.Euler(0.0f, xAngle, 0.0f);
            newPos = rot * newPos;

            return newPos;
        }

        /// <summary>
        /// Internal function to check if a node exists in the NodeList.
        /// </summary>
        /// <param name="node">A <see cref="Transform"/> of the node to see if it's in the NodeList</param>
        /// <returns></returns>
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
        /// Internal function for randomized mapping based on a source Vec3 and a radius for randomization distance.
        /// </summary>
        /// <param name="source">The source <see cref="Vector3"/> to be mapped to cylinder</param>
        /// <param name="radius">This is a <see cref="float"/> for the radius of the cylinder</param>
        /// <returns></returns>
        private Vector3 ScatterMapping(Vector3 source, float radius)
        {
            source.x = UnityEngine.Random.Range(-radius, radius);
            source.y = UnityEngine.Random.Range(-radius, radius);
            return source;
        }

    
        /// <summary>
        /// Internal function to pack randomly spaced nodes so they don't overlap
        /// Usually requires about 25 iterations for decent packing
        /// </summary>
        /// <returns></returns>
        private void IterateScatterPacking(List<CollectionNode> nodes, float radiusPadding)
        {
            // Sort by closest to center (don't worry about z axis)
            // Use the position of the collection as the packing center
            nodes.Sort(delegate (CollectionNode circle1, CollectionNode circle2) {
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

        /// <summary>
        /// Gizmos to draw when the Collection is selected.
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            Vector3 scale = (2f * Radius) * Vector3.one;
            switch (SurfaceType)
            {
                case SurfaceTypeEnum.Plane:
                    break;
                case SurfaceTypeEnum.Cylinder:
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireMesh(CylinderMesh, transform.position, transform.rotation, scale);
                    break;
                case SurfaceTypeEnum.Sphere:
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireMesh(SphereMesh, transform.position, transform.rotation, scale);
                    break;
            }
        }
    }
}
