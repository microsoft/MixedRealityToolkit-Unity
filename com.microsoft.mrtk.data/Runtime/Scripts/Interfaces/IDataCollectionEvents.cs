// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// An interface for receiving a variety of state change
    /// events that can be used to update a user experience
    /// </summary>

    public interface IDataCollectionEvents
    {
        /// <summmary>
        /// A placement set is starting. Zero or more items
        /// will be placed.
        /// </summmary>
        void OnStartPlacement();

        /// <summary>
        /// A set of item placements has ended.
        /// </summary>
        /// <remarks>
        /// Note that it is possible that no items were actually placed.
        /// </remarks>
        void OnEndPlacement();

        /// <summary>
        /// One item placement has just occurred.
        /// </summary>
        void OnItemPlaced();

        /// <summary>
        /// Collection scrolled/paged to start of list
        /// </summary>
        void OnCollectionAtStart();

        /// <summary>
        /// Collection scrolled/paged to end of list
        /// </summary>
        void OnCollectionAtEnd();

        /// <summary>
        /// Collection entered state of being in the middle.
        /// </summary>
        /// <remarks>
        /// This means not at the start or the end.
        ///
        /// Not that on a small list that is same or smaller
        /// than the available space, this will never get
        /// triggered.
        /// </remarks>
        void OnCollectionInMiddle();

        /// <summary>
        /// Collection transitioned to a state where you can scroll or page backward
        /// </summary>
        /// <remarks>
        /// This is triggered when the list is not at the start and
        /// the list is large enough to even require scrolling.
        /// </remarks>
        void OnCollectionCanGoBackward();

        /// <summary>
        /// Collection transitioned to a state where you can scroll or page forward
        /// </summary>
        /// <remarks>
        /// This is triggered when the list is not at the end and
        /// the list is large enough to even require scrolling.
        /// </remarks>
        void OnCollectionCanGoForward();

        /// <summary>
        /// Collection contents completely replaced
        /// </summary>
        ///
        void OnCollectionContextSwitch();

        /// <summary>
        /// Collection contents has changed
        /// </summary>
        void OnEndCollectionChange();

        /// <summary>
        /// Collection contents transitioned to empty set
        /// </summary>
        void OnCollectionEmpty();

        /// <summary>
        /// Collection contents transitioned from empty to not empty
        /// </summary>
        void OnCollectionNotEmpty();

        /// <summary>
        /// Collection was scrolled forward towards end of list
        /// </summary>
        void OnCollectionScrolledForward();

        /// <summary>
        /// Collection was scrolled backwards towards start of list
        /// </summary>
        void OnCollectionScrolledBackward();

        /// <summary>
        /// Collection was paged forward towards end of list
        /// </summary>
        void OnCollectionPagedForward();

        /// <summary>
        /// Collection was paged backward towards start of list
        /// </summary>
        void OnCollectionPagedBackward();
    }
}
