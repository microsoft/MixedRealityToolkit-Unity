using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Data
{
    public class DataCollectionEventsHandler : DataCollectionEventsGOBase
    {
        [SerializeField]
        public UnityEvent Attach = new UnityEvent();

        [SerializeField]
        public UnityEvent Detach = new UnityEvent();

        [SerializeField]
        public UnityEvent StartPlacement = new UnityEvent();

        [SerializeField]
        public UnityEvent EndPlacement = new UnityEvent();

        [SerializeField]
        public UnityEvent ItemPlaced = new UnityEvent();

        [SerializeField]
        public UnityEvent CollectionAtStart = new UnityEvent();

        [SerializeField]
        public UnityEvent CollectionAtEnd = new UnityEvent();

        [SerializeField]
        public UnityEvent CollectionInMiddle = new UnityEvent();

        [SerializeField]
        public UnityEvent CollectionCanGoBackward = new UnityEvent();

        [SerializeField]
        public UnityEvent CollectionCanGoForward= new UnityEvent();

        [SerializeField]
        public UnityEvent CollectionContextSwitch = new UnityEvent();

        [SerializeField]
        public UnityEvent CollectionChanged = new UnityEvent();

        [SerializeField]
        public UnityEvent CollectionEmpty = new UnityEvent();

        [SerializeField]
        public UnityEvent CollectionNotEmpty = new UnityEvent();

        [SerializeField]
        public UnityEvent CollectionScrolledForward = new UnityEvent();

        [SerializeField]
        public UnityEvent CollectionScrolledBackward = new UnityEvent();

        [SerializeField]
        public UnityEvent CollectionPagedForward = new UnityEvent();

        [SerializeField]
        public UnityEvent CollectionPagedBackward = new UnityEvent();


        /// <summmary>
        /// Item placer has just been attached after initialization or dormant state
        /// </summmary>
        public override void OnAttach()
        {
            Attach.Invoke();
        }


        /// <summmary>
        /// Item placer is being detached and put in dormant state or prior to destroy.
        /// </summmary>
        public override void OnDetach()
        {
            Detach.Invoke();
        }

        /// <summmary>
        /// A placement set is starting. Zero or more items
        /// will be placed.
        /// </summmary>
        public override void OnStartPlacement()
        {
            StartPlacement.Invoke();
        }
        /// <summary>
        /// A set of item placements has ended. 
        /// </summary>
        /// <remarks>
        /// Note that it is possible that no items were actually placed.
        /// </remarks>
        public override void OnEndPlacement()
        {
            EndPlacement.Invoke();
        }

        /// <summary>
        /// One item placement has just occured.
        /// </summary>
        public override void OnItemPlaced()
        {
            ItemPlaced.Invoke();
        }

        /// <summary>
        /// Collection scrolled/paged to start of list
        /// </summary>
        public override void OnCollectionAtStart()
        {
            CollectionAtStart.Invoke();
        }

        /// <summary>
        /// Collection scrolled/paged to end of list
        /// </summary>
        public override void OnCollectionAtEnd()
        {
            CollectionAtEnd.Invoke();
        }

        public override void OnCollectionInMiddle()
        {
            CollectionInMiddle.Invoke();
        }

        public override void OnCollectionCanGoBackward()
        {
            CollectionCanGoBackward.Invoke();
        }

        public override void OnCollectionCanGoForward()
        {
            CollectionCanGoForward.Invoke();
        }

        /// <summary>
        /// Collection contents completely replaced
        /// </summary>
        public override void OnCollectionContextSwitch()
        {
            CollectionContextSwitch.Invoke();
        }


        /// <summary>
        /// Collection contents has changed
        /// </summary>
        public override void OnCollectionChanged()
        {
            CollectionChanged.Invoke();
        }


        /// <summary>
        /// Collection contents transitioned to empty set
        /// </summary>
        public override void OnCollectionEmpty()
        {
            CollectionEmpty.Invoke();
        }


        /// <summary>
        /// Collection contents transitioned from empty to not empty
        /// </summary>
        public override void OnCollectionNotEmpty()
        {
            CollectionNotEmpty.Invoke();
        }


        /// <summary>
        /// Collection was scrolled forward towards end of list
        /// </summary>
        public override void OnCollectionScrolledForward()
        {
            CollectionScrolledForward.Invoke();
        }


        /// <summary>
        /// Collection was scrolled backwards towards start of list
        /// </summary>
        public override void OnCollectionScrolledBackward()
        {
            CollectionScrolledBackward.Invoke();
        }


        /// <summary>
        /// Collection was paged forward towards end of list
        /// </summary>
        public override void OnCollectionPagedForward()
        {
            CollectionPagedForward.Invoke();
        }


        /// <summary>
        /// Collection was paged backward towards start of list
        /// </summary>
        public override void OnCollectionPagedBackward()
        {
            CollectionPagedBackward.Invoke();
        }

    }
}
