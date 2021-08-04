using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// An interface for receiving a variety of state change
    /// events that can be used to update a user experience
    /// </summary>
 
    public interface IDataCollectionEvents
    {
        /// <summary>
        /// Collection scrolled/paged to start of list
        /// </summary>
        void OnCollectionAtStart();

        /// <summary>
        /// Collection scrolled/paged to end of list
        /// </summary>
        void OnCollectionAtEnd();

        /// <summary>
        /// Collection contents completely replaced
        /// </summary>
        /// 

        /// <summary>
        /// Collection in middle (ie. not at start or at end)
        /// </summary>
        void OnCollectionContextSwitch();

        /// <summary>
        /// Collection contents has changed
        /// </summary>
        void OnCollectionChanged();

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
