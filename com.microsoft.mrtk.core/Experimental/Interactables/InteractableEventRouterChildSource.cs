// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental
{
    /// <summary>
    /// A Unity component that provides a mechanism to handle children being dynamically
    /// added to a container. A <see cref="InteractableEventRouter"/> can use this class
    /// to determine when children are dynamically added to a container.
    /// </summary>
    /// <remarks> 
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    [AddComponentMenu("MRTK/Core/Interactable Event Router Child Source")]
    public class InteractableEventRouterChildSource : MonoBehaviour
    {
        [SerializeField]
        [Experimental]
        [Tooltip("A Unity event fired when new child transforms are added to or removed from this game object.")]
        private UnityEvent childrenChanged = new UnityEvent();

        /// <summary>
        /// A Unity event fired when new child transforms are added to or removed from this game object.
        /// </summary>
        public UnityEvent ChildrenChanged => childrenChanged;

        private void OnTransformChildrenChanged()
        {
            childrenChanged?.Invoke();
        }
    }
}
