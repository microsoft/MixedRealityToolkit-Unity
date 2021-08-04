
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Abstract base GameObject class that makes it possible to create subclasses that are
    /// "drag and droppable" via the Unity Editor inspector and can be added as a component to other
    /// game objects.
    /// </summary>
    public abstract class DataCollectionEventsGOBase : MonoBehaviour, IDataCollectionEvents
    {

        /// <summary>
        /// Collection scrolled/paged to start of list
        /// </summary>
        public abstract void OnCollectionAtStart();

        /// <summary>
        /// Collection scrolled/paged to end of list
        /// </summary>
        public abstract void OnCollectionAtEnd();


        public abstract void OnCollectionInMiddle();

        /// <summary>
        /// Collection contents completely replaced
        /// </summary>
        public abstract void OnCollectionContextSwitch();

        /// <summary>
        /// Collection contents has changed
        /// </summary>
        public abstract void OnCollectionChanged();

        /// <summary>
        /// Collection contents transitioned to empty set
        /// </summary>
        public abstract void OnCollectionEmpty();

        /// <summary>
        /// Collection contents transitioned from empty to not empty
        /// </summary>
        public abstract void OnCollectionNotEmpty();

        /// <summary>
        /// Collection was scrolled forward towards end of list
        /// </summary>
        public abstract void OnCollectionScrolledForward();

        /// <summary>
        /// Collection was scrolled backwards towards start of list
        /// </summary>
        public abstract void OnCollectionScrolledBackward();

        /// <summary>
        /// Collection was paged forward towards end of list
        /// </summary>
        public abstract void OnCollectionPagedForward();

        /// <summary>
        /// Collection was paged backward towards start of list
        /// </summary>
        public abstract void OnCollectionPagedBackward();
    }
}
