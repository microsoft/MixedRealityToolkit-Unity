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
        /// <summary>
        /// Action called when collection is updated
        /// </summary>
        public Action<BaseObjectCollection> OnCollectionUpdated;

        /// <summary>
        /// List of objects with generated data on the object.
        /// </summary>
        [HideInInspector]
        public List<ObjectCollectionNode> NodeList = new List<ObjectCollectionNode>();
        #endregion

        #region private fields
        /// <summary>
        /// Whether to treat inactive transforms as 'invisible'
        /// </summary>
        [Tooltip("Whether to treat inactive transforms as 'invisible'")]
        [SerializeField]
        protected bool ignoreInactiveTransforms = true;

        /// <summary>
        /// Type of sorting to use.
        /// </summary>
        [Tooltip("Type of sorting to use")]
        [SerializeField]
        protected CollationOrderTypeEnum SortType = CollationOrderTypeEnum.None;
        #endregion

        #region public accessors
        public bool IgnoreInactiveTransforms
        {
            get { return ignoreInactiveTransforms; }
            set { ignoreInactiveTransforms = value; }
        }

        #endregion


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
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Transform child = this.transform.GetChild(i);

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
                    break;

                case CollationOrderTypeEnum.Transform:
                    NodeList.Sort(delegate (ObjectCollectionNode c1, ObjectCollectionNode c2) { return c1.transform.GetSiblingIndex().CompareTo(c2.transform.GetSiblingIndex()); });
                    break;

                case CollationOrderTypeEnum.Alphabetical:
                    NodeList.Sort(delegate (ObjectCollectionNode c1, ObjectCollectionNode c2) { return c1.Name.CompareTo(c2.Name); });
                    break;

                case CollationOrderTypeEnum.AlphabeticalReversed:
                    NodeList.Sort(delegate (ObjectCollectionNode c1, ObjectCollectionNode c2) { return c1.Name.CompareTo(c2.Name); });
                    NodeList.Reverse();
                    break;

                case CollationOrderTypeEnum.TransformReversed:
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
        /// Protected Internal function for laying out all children when UpdateCollection is called.
        /// </summary>
        protected abstract void LayoutChildren()
    }
}
