// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public abstract class BaseObjectCollection : MonoBehaviour
    {
        /// <summary>
        /// Action called when collection is updated
        /// </summary>
        public Action<BaseObjectCollection> OnCollectionUpdated { get; set; }

        /// <summary>
        /// List of objects with generated data on the object.
        /// </summary>
        protected List<ObjectCollectionNode> NodeList { get; } = new List<ObjectCollectionNode>();

        [Tooltip("Whether to include space for inactive transforms in the layout")]
        [SerializeField]
        private bool ignoreInactiveTransforms = true;

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
        private CollationOrder sortType = CollationOrder.None;

        /// <summary>
        /// Type of sorting to use.
        /// </summary>
        public CollationOrder SortType
        {
            get { return sortType; }
            set { sortType = value; }
        }

        /// <summary>
        /// Rebuilds / updates the collection layout.
        /// Update collection is called from the editor button on the inspector.
        /// </summary>
        public virtual void UpdateCollection()
        {
            // Check for empty nodes and remove them
            var emptyNodes = new List<ObjectCollectionNode>();

            for (int i = 0; i < NodeList.Count; i++)
            {
                if (NodeList[i].Transform == null || (IgnoreInactiveTransforms && !NodeList[i].Transform.gameObject.activeSelf) || NodeList[i].Transform.parent == null || !(NodeList[i].Transform.parent.gameObject == gameObject))
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
#if UNITY_EDITOR
                Undo.RecordObject(child, "ObjectCollection modify transform");
#endif
                if (!ContainsNode(child) && (child.gameObject.activeSelf || !IgnoreInactiveTransforms))
                {
                    NodeList.Add(new ObjectCollectionNode { Name = child.name, Transform = child });
                }
            }

            switch (SortType)
            {
                case CollationOrder.ChildOrder:
                    NodeList.Sort((c1, c2) => (c1.Transform.GetSiblingIndex().CompareTo(c2.Transform.GetSiblingIndex())));
                    break;

                case CollationOrder.Alphabetical:
                    NodeList.Sort((c1, c2) => (string.CompareOrdinal(c1.Name, c2.Name)));
                    break;

                case CollationOrder.AlphabeticalReversed:
                    NodeList.Sort((c1, c2) => (string.CompareOrdinal(c1.Name, c2.Name)));
                    NodeList.Reverse();
                    break;

                case CollationOrder.ChildOrderReversed:
                    NodeList.Sort((c1, c2) => (c1.Transform.GetSiblingIndex().CompareTo(c2.Transform.GetSiblingIndex())));
                    NodeList.Reverse();
                    break;
            }

            LayoutChildren();

            OnCollectionUpdated?.Invoke(this);
        }

        /// <summary>
        /// Check if a node exists in the NodeList.
        /// </summary>
        protected bool ContainsNode(Transform node)
        {
            for (int i = 0; i < NodeList.Count; i++)
            {
                if (NodeList[i].Transform == node)
                {
                    return true;
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