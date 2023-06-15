// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A subclass of DataCollectionEventsGOBase that exposes every
    /// supported event and can be used to expose these in the Unity Editor inspector
    /// on any Monobehaviour.
    /// </summary>
    [AddComponentMenu("MRTK/Data Binding/Data Collection Events Handler")]
    public class DataCollectionEventsHandler : DataCollectionEventsGOBase
    {
        [SerializeField, Experimental, FormerlySerializedAs("Attach")]
        private UnityEvent attach = new UnityEvent();

        [SerializeField, FormerlySerializedAs("Detach")]
        private UnityEvent detach = new UnityEvent();

        [SerializeField, FormerlySerializedAs("StartPlacement")]
        private UnityEvent startPlacement = new UnityEvent();

        [SerializeField, FormerlySerializedAs("EndPlacement")]
        private UnityEvent endPlacement = new UnityEvent();

        [SerializeField, FormerlySerializedAs("ItemPlaced")]
        private UnityEvent itemPlaced = new UnityEvent();

        [SerializeField, FormerlySerializedAs("CollectionAtStart")]
        private UnityEvent collectionAtStart = new UnityEvent();

        [SerializeField, FormerlySerializedAs("CollectionAtEnd")]
        private UnityEvent collectionAtEnd = new UnityEvent();

        [SerializeField, FormerlySerializedAs("CollectionInMiddle")]
        private UnityEvent collectionInMiddle = new UnityEvent();

        [SerializeField, FormerlySerializedAs("CollectionCanGoBackward")]
        private UnityEvent collectionCanGoBackward = new UnityEvent();

        [SerializeField, FormerlySerializedAs("CollectionCanGoForward")]
        private UnityEvent collectionCanGoForward = new UnityEvent();

        [SerializeField, FormerlySerializedAs("CollectionContextSwitch")]
        private UnityEvent collectionContextSwitch = new UnityEvent();

        [SerializeField, FormerlySerializedAs("CollectionChanged")]
        private UnityEvent startCollectionChanged = new UnityEvent();

        private UnityEvent endCollectionChanged = new UnityEvent();

        [SerializeField, FormerlySerializedAs("CollectionEmpty")]
        private UnityEvent collectionEmpty = new UnityEvent();

        [SerializeField, FormerlySerializedAs("CollectionNotEmpty")]
        private UnityEvent collectionNotEmpty = new UnityEvent();

        [SerializeField, FormerlySerializedAs("CollectionScrolledForward")]
        private UnityEvent collectionScrolledForward = new UnityEvent();

        [SerializeField, FormerlySerializedAs("CollectionScrolledBackward")]
        private UnityEvent collectionScrolledBackward = new UnityEvent();

        [SerializeField, FormerlySerializedAs("CollectionPagedForward")]
        private UnityEvent collectionPagedForward = new UnityEvent();

        [SerializeField, FormerlySerializedAs("CollectionPagedBackward")]
        private UnityEvent collectionPagedBackward = new UnityEvent();

        /// <summmary>
        /// Item placer has just been attached after initialization or dormant state
        /// </summmary>
        public override void OnAttach()
        {
            attach?.Invoke();
        }

        /// <summmary>
        /// Item placer is being detached and put in dormant state or prior to destroy.
        /// </summmary>
        public override void OnDetach()
        {
            detach?.Invoke();
        }

        /// <summmary>
        /// A placement set is starting. Zero or more items
        /// will be placed.
        /// </summmary>
        public override void OnStartPlacement()
        {
            startPlacement?.Invoke();
        }

        /// <summary>
        /// A set of item placements has ended.
        /// </summary>
        /// <remarks>
        /// Note that it is possible that no items were actually placed.
        /// </remarks>
        public override void OnEndPlacement()
        {
            endPlacement?.Invoke();
        }

        /// <summary>
        /// One item placement has just occurred.
        /// </summary>
        public override void OnItemPlaced()
        {
            itemPlaced?.Invoke();
        }

        /// <summary>
        /// Collection scrolled/paged to start of list
        /// </summary>
        public override void OnCollectionAtStart()
        {
            collectionAtStart?.Invoke();
        }

        /// <summary>
        /// Collection scrolled/paged to end of list
        /// </summary>
        public override void OnCollectionAtEnd()
        {
            collectionAtEnd?.Invoke();
        }

        /// <inheritdoc/>
        public override void OnCollectionInMiddle()
        {
            collectionInMiddle?.Invoke();
        }

        /// <inheritdoc/>
        public override void OnCollectionCanGoBackward()
        {
            collectionCanGoBackward?.Invoke();
        }

        /// <inheritdoc/>
        public override void OnCollectionCanGoForward()
        {
            collectionCanGoForward?.Invoke();
        }

        /// <summary>
        /// Collection contents completely replaced
        /// </summary>
        public override void OnCollectionContextSwitch()
        {
            collectionContextSwitch?.Invoke();
        }

        /// <summary>
        /// Collection contents are about to change
        /// </summary>
        public override void OnStartCollectionChange()
        {
            startCollectionChanged?.Invoke();
        }

        /// <summary>
        /// Collection contents have finished changing
        /// </summary>
        public override void OnEndCollectionChange()
        {
            endCollectionChanged?.Invoke();
        }

        /// <summary>
        /// Collection contents transitioned to empty set
        /// </summary>
        public override void OnCollectionEmpty()
        {
            collectionEmpty?.Invoke();
        }

        /// <summary>
        /// Collection contents transitioned from empty to not empty
        /// </summary>
        public override void OnCollectionNotEmpty()
        {
            collectionNotEmpty?.Invoke();
        }

        /// <summary>
        /// Collection was scrolled forward towards end of list
        /// </summary>
        public override void OnCollectionScrolledForward()
        {
            collectionScrolledForward?.Invoke();
        }

        /// <summary>
        /// Collection was scrolled backwards towards start of list
        /// </summary>
        public override void OnCollectionScrolledBackward()
        {
            collectionScrolledBackward?.Invoke();
        }

        /// <summary>
        /// Collection was paged forward towards end of list
        /// </summary>
        public override void OnCollectionPagedForward()
        {
            collectionPagedForward?.Invoke();
        }

        /// <summary>
        /// Collection was paged backward towards start of list
        /// </summary>
        public override void OnCollectionPagedBackward()
        {
            collectionPagedBackward?.Invoke();
        }
    }
}
