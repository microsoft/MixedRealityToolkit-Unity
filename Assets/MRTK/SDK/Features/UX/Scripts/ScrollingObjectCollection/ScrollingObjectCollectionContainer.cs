using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Container for all renderers and colliders used in the ScrollingObjectCollection.
    /// It's main purpose is to find and cache those Components.
    /// Thanks to that we don't have to run GetComponentsInChildren every frame. 
    /// </summary>
    public class ScrollingObjectCollectionContainer : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] children;

        /// <summary>
        /// Dictionary containing ScrollingObjectCollection's items child renderers.
        /// </summary>
        public Dictionary<GameObject, Renderer[]> ItemRenderersMap { get; } = new Dictionary<GameObject, Renderer[]>();

        /// <summary>
        /// Dictionary containing ScrollingObjectCollection's items child colliders.
        /// </summary>
        public Dictionary<GameObject, Collider[]> ItemCollidersMap { get; } = new Dictionary<GameObject, Collider[]>();

        /// <summary>
        /// Position of the Container.
        /// </summary>
        public Vector3 LocalPosition
        {
            get => transform.localPosition;
            set => transform.localPosition = value;
        }

        private GameObject[] Children
        {
            get
            {
                var childCount = transform.childCount;
                if (children == null || children.Length != childCount)
                {
                    children = new GameObject[childCount];
                    for (var i = 0; i < children.Length; ++i)
                        children[i] = transform.GetChild(i).gameObject;
                }

                return children;
            }
        }

        private void OnTransformChildrenChanged()
        {
            foreach (var child in Children)
            {
                AddItemWithRenderers(child);
                AddItemWithColliders(child);
            }
        }

        private void AddItemWithRenderers(GameObject child)
        {
            if (ItemRenderersMap.ContainsKey(child) && !child)
            {
                ItemRenderersMap.Remove(child);
                return;
            }
            
            if (ItemRenderersMap.ContainsKey(child))
            {
                ItemRenderersMap[child] = child.GetComponentsInChildren<Renderer>(true);
                return;
            }

            ItemRenderersMap.Add(child, child.GetComponentsInChildren<Renderer>(true));
        }

        private void AddItemWithColliders(GameObject child)
        {
            if (ItemCollidersMap.ContainsKey(child) && !child)
            {
                ItemCollidersMap.Remove(child);
                return;
            }
            
            if (ItemCollidersMap.ContainsKey(child))
            {
                ItemCollidersMap[child] = child.GetComponentsInChildren<Collider>(true);
                return;
            }

            ItemCollidersMap.Add(child, child.GetComponentsInChildren<Collider>(true));
        }
    }
}