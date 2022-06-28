// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Adds and removes objects from a given Object Bar to demonstrate the back plate auto adjust behavior. 
    /// </summary>
    [AddComponentMenu("MRTK/UX/Object Bar Add\\Remove Items")]
    public class ObjectBarAddRemoveItems : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The target Object Bar.")]
        private ObjectBar objectBar;

        /// <summary>
        /// The target Object Bar.
        /// </summary>
        public ObjectBar ObjectBar
        {
            get => objectBar;
            set => objectBar = value;
        }

        [SerializeField]
        [Tooltip("The prefab to instantiate and add to the ObjectBar.ObjectBarObjects list.")]
        private GameObject prefabToInstantiate;

        /// <summary>
        /// The prefab to instantiate and add to the ObjectBar.ObjectBarObjects list.
        /// </summary>
        public GameObject PrefabToInstantiate
        {
            get => prefabToInstantiate;
            set => prefabToInstantiate = value;
        }

        [SerializeField]
        [Tooltip("Target parent transform of the instantiated prefab.")]
        private Transform targetParent;

        /// <summary>
        /// Target parent transform of the instantiated prefab.
        /// </summary>
        public Transform TargetParent
        {
            get => targetParent;
            set => targetParent = value;
        }

        /// <summary>
        /// Instantiate a specified prefab and add it to the ObjectBarObjects list with the Target parent as the parent transform.
        /// </summary>
        public void AddToObjectBarList()
        {
            if (ObjectBar != null)
            {
                GameObject instance = Instantiate(PrefabToInstantiate, TargetParent);
                ObjectBar.ObjectBarObjects.Add(instance);
            }
        }

        /// <summary>
        /// Remove the last item from the ObjectBarObjects list.
        /// </summary>
        public void RemoveLastFromObjectBarList()
        {
            if (ObjectBar.ObjectBarObjects.Count > 2 && ObjectBar != null)
            {
                int lastIndex = ObjectBar.ObjectBarObjects.Count - 1;
                GameObject objectToRemove = ObjectBar.ObjectBarObjects[lastIndex];
                objectBar.ObjectBarObjects.RemoveAt(lastIndex);
                Destroy(objectToRemove);
            }
        }
    }
}
