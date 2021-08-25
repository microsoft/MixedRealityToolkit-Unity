using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Data
{
    public class DataCollectionEventsHandler : DataCollectionEventsGOBase
    {
        [SerializeField]
        protected UnityEvent CollectionAtStart = new UnityEvent();

        [SerializeField]
        protected UnityEvent CollectionAtEnd = new UnityEvent();

        [SerializeField]
        protected UnityEvent CollectionInMiddle = new UnityEvent();

        [SerializeField]
        protected UnityEvent CollectionCanGoBackward = new UnityEvent();

        [SerializeField]
        protected UnityEvent CollectionCanGoForward= new UnityEvent();

        [SerializeField]
        protected UnityEvent CollectionContextSwitch = new UnityEvent();

        [SerializeField]
        protected UnityEvent CollectionChanged = new UnityEvent();

        [SerializeField]
        protected UnityEvent CollectionEmpty = new UnityEvent();

        [SerializeField]
        protected UnityEvent CollectionNotEmpty = new UnityEvent();

        [SerializeField]
        protected UnityEvent CollectionScrolledForward = new UnityEvent();

        [SerializeField]
        protected UnityEvent CollectionScrolledBackward = new UnityEvent();

        [SerializeField]
        protected UnityEvent CollectionPagedForward = new UnityEvent();

        [SerializeField]
        protected UnityEvent CollectionPagedBackward = new UnityEvent();



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
