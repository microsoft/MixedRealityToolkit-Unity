// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Collections
{
    public abstract class BaseCollection : MonoBehaviour
    {
        #region public members
        /// <summary>
        /// Action called when collection is updated
        /// </summary>
        public Action<BaseCollection> OnCollectionUpdated;

        /// <summary>
        /// List of objects with generated data on the object.
        /// </summary>
        [HideInInspector]
        public List<CollectionNode> NodeList = new List<CollectionNode>();

        /// <summary>
        /// Whether to treat inactive transforms as 'invisible'
        /// </summary>
        public bool IgnoreInactiveTransforms = true;

        /// <summary>
        /// Type of sorting to use.
        /// </summary>
        [Tooltip("Type of sorting to use")]
        public SortTypeEnum SortType = SortTypeEnum.None;
        #endregion


        public virtual void UpdateCollection()
        {

            // Check for empty nodes and remove them
            List<CollectionNode> emptyNodes = new List<CollectionNode>();

            for (int i = 0; i < NodeList.Count; i++)
            {
                if (NodeList[i].transform == null || (IgnoreInactiveTransforms && !NodeList[i].transform.gameObject.activeSelf) || NodeList[i].transform.parent == null || !(NodeList[i].transform.parent.gameObject == this.gameObject))
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

            LayoutChildren();

            if (OnCollectionUpdated != null)
            {
                OnCollectionUpdated.Invoke(this);
            }
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
        /// Protected Internal function for laying out all children when UpdateCollection is called.
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        protected virtual void LayoutChildren() { }

    }
}
