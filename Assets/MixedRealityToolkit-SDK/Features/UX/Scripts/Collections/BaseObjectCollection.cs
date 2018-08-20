// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Collections
{
    public abstract class BaseObjectCollection : MonoBehaviour
    {
        #region public members
        protected Action<BaseObjectCollection> onCollectionUpdated;
        
        /// <summary>
        /// Action called when collection is updated
        /// </summary>
        public Action<BaseObjectCollection> OnCollectionUpdated
        {
            get { return onCollectionUpdated; }
            set { onCollectionUpdated = value; }
        }

        protected List<ObjectCollectionNode> nodeList = new List<ObjectCollectionNode>();

        /// <summary>
        /// List of objects with generated data on the object.
        /// </summary>
        public List<ObjectCollectionNode> NodeList
        {
            get { return nodeList; }
            set { nodeList = value; }
        }

        [Tooltip("Whether to include space for inactive transforms in the layout")]
        [SerializeField]
        protected bool ignoreInactiveTransforms = true;

        /// <summary>
        /// Whether to include space for inactive transforms in the layout
        /// </summary>
        public bool IgnoreInactiveTransforms
        {
            get { return ignoreInactiveTransforms; }
            set { ignoreInactiveTransforms = value; }
        }

        [Tooltip("Type of sorting to use")]
        [SerializeField]
        protected CollationOrderTypeEnum sortType = CollationOrderTypeEnum.None;

        /// <summary>
        /// Type of sorting to use.
        /// </summary>
        public CollationOrderTypeEnum SortType
        {
            get { return sortType; }
            set { sortType = value; }
        }
        #endregion public members


        /// <summary>
        /// Rebuilds / updates the collection layout.
        /// Update collection is called from the editor button on the inspector.
        /// </summary>
        public virtual void UpdateCollection()
        {
            // Check for empty nodes and remove them
            List<ObjectCollectionNode> emptyNodes = new List<ObjectCollectionNode>();

            for (int i = 0; i < NodeList.Count; i++)
            {
                if (NodeList[i].transform == null || (IgnoreInactiveTransforms && !NodeList[i].transform.gameObject.activeSelf) || NodeList[i].transform.parent == null || !(NodeList[i].transform.parent.gameObject == gameObject))
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
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                if (!ContainsNode(child) && (child.gameObject.activeSelf || !IgnoreInactiveTransforms))
                {
                    ObjectCollectionNode node = new ObjectCollectionNode();

                    node.Name = child.name;
                    node.transform = child;
                    NodeList.Add(node);
                }
            }

            switch (SortType)
            {
                case CollationOrderTypeEnum.None:
                default:
                    break;

                case CollationOrderTypeEnum.ChildOrder:
                    NodeList.Sort(delegate (ObjectCollectionNode c1, ObjectCollectionNode c2) { return c1.transform.GetSiblingIndex().CompareTo(c2.transform.GetSiblingIndex()); });
                    break;

                case CollationOrderTypeEnum.Alphabetical:
                    NodeList.Sort(delegate (ObjectCollectionNode c1, ObjectCollectionNode c2) { return c1.Name.CompareTo(c2.Name); });
                    break;

                case CollationOrderTypeEnum.AlphabeticalReversed:
                    NodeList.Sort(delegate (ObjectCollectionNode c1, ObjectCollectionNode c2) { return c1.Name.CompareTo(c2.Name); });
                    NodeList.Reverse();
                    break;

                case CollationOrderTypeEnum.ChildOrderReversed:
                    NodeList.Sort(delegate (ObjectCollectionNode c1, ObjectCollectionNode c2) { return c1.transform.GetSiblingIndex().CompareTo(c2.transform.GetSiblingIndex()); });
                    NodeList.Reverse();
                    break;
            }

            LayoutChildren();

            if (OnCollectionUpdated != null)
            {
                OnCollectionUpdated.Invoke(this);
            }
        }

        // Check if a node exists in the NodeList.
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
        /// Implement for laying out all children when UpdateCollection is called.
        /// </summary>
        protected abstract void LayoutChildren();
    }
}
