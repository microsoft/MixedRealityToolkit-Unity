// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// virtual base GameObject class that makes it possible to create subclasses that are
    /// "drag and droppable" via the Unity Editor inspector and can be added as a component to other
    /// game objects.
    /// </summary>
    public class DataCollectionEventsGOBase : MonoBehaviour, IDataCollectionEvents
    {
        /// <summmary>
        /// Item placer has just been attached after initialization or dormant state
        /// </summmary>
        public virtual void OnAttach() { }

        /// <summmary>
        /// Item placer is being detached and put in dormant state or prior to destroy.
        /// </summmary>
        public virtual void OnDetach() { }

        /// <summmary>
        /// A placement set is starting. Zero or more items
        /// will be placed.
        /// </summmary>
        public virtual void OnStartPlacement() {}
        /// <summary>
        /// A set of item placements has ended.
        /// </summary>
        /// <remarks>
        /// Note that it is possible that no items were actually placed.
        /// </remarks>
        public virtual void OnEndPlacement() {}

        /// <summary>
        /// One item placement has just occured.
        /// </summary>
        public virtual void OnItemPlaced() {}

        /// <summary>
        /// Collection scrolled/paged to start of list
        /// </summary>
        public virtual void OnCollectionAtStart() {}

        /// <summary>
        /// Collection scrolled/paged to end of list
        /// </summary>
        public virtual void OnCollectionAtEnd() {}


        public virtual void OnCollectionInMiddle() {}

        public virtual void OnCollectionCanGoBackward() {}

        public virtual void OnCollectionCanGoForward() {}

        /// <summary>
        /// Collection contents completely replaced
        /// </summary>
        public virtual void OnCollectionContextSwitch() {}

        /// <summary>
        /// Collection contents has changed
        /// </summary>
        public virtual void OnCollectionChanged() {}

        /// <summary>
        /// Collection contents transitioned to empty set
        /// </summary>
        public virtual void OnCollectionEmpty() {}

        /// <summary>
        /// Collection contents transitioned from empty to not empty
        /// </summary>
        public virtual void OnCollectionNotEmpty() {}

        /// <summary>
        /// Collection was scrolled forward towards end of list
        /// </summary>
        public virtual void OnCollectionScrolledForward() {}

        /// <summary>
        /// Collection was scrolled backwards towards start of list
        /// </summary>
        public virtual void OnCollectionScrolledBackward() {}

        /// <summary>
        /// Collection was paged forward towards end of list
        /// </summary>
        public virtual void OnCollectionPagedForward() {}

        /// <summary>
        /// Collection was paged backward towards start of list
        /// </summary>
        public virtual void OnCollectionPagedBackward() {}
    }
}
