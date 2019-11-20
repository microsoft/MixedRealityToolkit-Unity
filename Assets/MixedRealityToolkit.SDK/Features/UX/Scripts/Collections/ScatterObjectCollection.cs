// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// A Scatter Object Collection is simply a set of child objects randomly laid out within a radius.
    /// Pressing "update collection" will run the randomization, feel free to run as many times until you get the desired result.
    /// </summary>
    public class ScatterObjectCollection : GridObjectCollection
    {
        /// <summary>
        /// Overriding base function for laying out all the children when UpdateCollection is called.
        /// </summary>
        protected override void LayoutChildren()
        {
            Vector3[] nodeGrid = new Vector3[NodeList.Count];
            Vector3 newPos;

            // Now lets lay out the grid
            Columns = Mathf.CeilToInt((float)NodeList.Count / Rows);
            HalfCell = new Vector2(CellWidth * 0.5f, CellHeight * 0.5f);

            // First start with a grid then project onto surface
            ResolveGridLayout(nodeGrid, Layout);

            // Get randomized planar mapping
            // Calculate radius of each node while we're here
            // Then use the packer function to shift them into place
            for (int i = 0; i < NodeList.Count; i++)
            {
                ObjectCollectionNode node = NodeList[i];

                newPos = VectorExtensions.ScatterMapping(nodeGrid[i], Radius);
                Collider nodeCollider = NodeList[i].Transform.GetComponentInChildren<Collider>();
                if (nodeCollider != null)
                {
                    // Make the radius the largest of the object's dimensions to avoid overlap
                    Bounds bounds = nodeCollider.bounds;
                    node.Radius = Mathf.Max(Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z) * 0.5f;
                }
                else
                {
                    // Make the radius a default value
                    node.Radius = 1f;
                }
                node.Transform.localPosition = newPos;
                UpdateNodeFacing(node);
                NodeList[i] = node;
            }

            // Iterate [x] times
            for (int i = 0; i < 100; i++)
            {
                IterateScatterPacking(NodeList, Radius);
            }
        }

        /// <summary>
        /// Pack randomly spaced nodes so they don't overlap
        /// Usually requires about 25 iterations for decent packing
        /// </summary>
        private static void IterateScatterPacking(List<ObjectCollectionNode> nodes, float radiusPadding)
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
                        difference = nodes[j].Transform.localPosition - nodes[i].Transform.localPosition;
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
                            nodes[j].Transform.localPosition += difference;
                            nodes[i].Transform.localPosition -= difference;
                        }
                    }
                }
            }
        }

        private static int ScatterSort(ObjectCollectionNode circle1, ObjectCollectionNode circle2)
        {
            float distance1 = (circle1.Transform.localPosition).sqrMagnitude;
            float distance2 = (circle2.Transform.localPosition).sqrMagnitude;
            return distance1.CompareTo(distance2);
        }
    }
}
