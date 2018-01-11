// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MixedRealityToolkit.Utilities
{
    /// <summary>
    /// Min-heap priority queue. In other words, lower priorities will be removed from the queue first.
    /// See http://en.wikipedia.org/wiki/Binary_heap for more info.
    /// </summary>
    /// <typeparam name="TPriority">Type for the priority used for ordering.</typeparam>
    /// <typeparam name="TValue">Type of values in the queue.</typeparam>
    class PriorityQueue<TPriority, TValue> : IEnumerable<KeyValuePair<TPriority, TValue>>
    {
        public class ValueCollection : IEnumerable<TValue>
        {
            private readonly PriorityQueue<TPriority, TValue> parentCollection;

            public ValueCollection(PriorityQueue<TPriority, TValue> parentCollection)
            {
                this.parentCollection = parentCollection;
            }

            #region IEnumerable

            public IEnumerator<TValue> GetEnumerator()
            {
                foreach (KeyValuePair<TPriority, TValue> pair in parentCollection)
                {
                    yield return pair.Value;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        private readonly IComparer<TPriority> priorityComparer;

        public PriorityQueue() : this(Comparer<TPriority>.Default) { }

        public PriorityQueue(IComparer<TPriority> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException();
            }

            priorityComparer = comparer;
        }

        private readonly List<KeyValuePair<TPriority, TValue>> queue = new List<KeyValuePair<TPriority, TValue>>();
        private ValueCollection valueCollection;

        public ValueCollection Values
        {
            get
            {
                if (valueCollection == null)
                {
                    valueCollection = new ValueCollection(this);
                }

                return valueCollection;
            }
        }

        #region IEnumerable

        public IEnumerator<KeyValuePair<TPriority, TValue>> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Clears the priority queue
        /// </summary>
        public void Clear()
        {
            queue.Clear();
        }

        /// <summary>
        /// Add an element to the priority queue.
        /// </summary>
        /// <param name="priority">Priority of the element</param>
        /// <param name="value"></param>
        public void Push(TPriority priority, TValue value)
        {
            queue.Add(new KeyValuePair<TPriority, TValue>(priority, value));
            BubbleUp();
        }

        /// <summary>
        /// Number of elements in priority queue
        /// </summary>
        public int Count
        {
            get
            {
                return queue.Count;
            }
        }

        /// <summary>
        /// Get the element with the minimum priority in the queue. The Key in the return value is the priority.
        /// </summary>
        public KeyValuePair<TPriority, TValue> Top
        {
            get
            {
                return queue[0];
            }
        }

        /// <summary>
        /// Pop the minimal element of the queue. Will fail at runtime if queue is empty.
        /// </summary>
        /// <returns>The minimal element</returns>
        public KeyValuePair<TPriority, TValue> Pop()
        {
            KeyValuePair<TPriority, TValue> ret = queue[0];
            queue[0] = queue[queue.Count - 1];
            queue.RemoveAt(queue.Count - 1);
            BubbleDown();
            return ret;
        }

        /// <summary>
        /// Returns whether or not the value is contained in the queue
        /// </summary>
        public bool Contains(TValue value)
        {
            return queue.Any(itm => EqualityComparer<TValue>.Default.Equals(itm.Value, value));
        }

        /// <summary>
        /// Removes the first element that equals the value from the queue
        /// </summary>
        public bool Remove(TValue value)
        {
            int idx = queue.FindIndex(itm => EqualityComparer<TValue>.Default.Equals(itm.Value, value));
            if (idx == -1)
            {
                return false;
            }

            queue[idx] = queue[queue.Count - 1];
            queue.RemoveAt(queue.Count - 1);
            BubbleDown();

            return true;
        }

        /// <summary>
        /// Removes all elements with this priority from the queue.
        /// </summary>
        /// <returns>True if elements were removed</returns>
        public bool RemoveAtPriority(TPriority priority, Predicate<TValue> shouldRemove)
        {
            bool removed = false;

            for (int i = queue.Count - 1; i >= 0; --i)
            {
                // TODO: early out if key < priority
                if (queue[i].Key.Equals(priority) && (shouldRemove == null || shouldRemove(queue[i].Value)))
                {
                    queue[i] = queue[queue.Count - 1];
                    queue.RemoveAt(queue.Count - 1);
                    BubbleDown();

                    removed = true;
                }
            }

            return removed;
        }

        /// <summary>
        /// Bubble up the last element in the queue until it's in the correct spot.
        /// </summary>
        private void BubbleUp()
        {
            int node = queue.Count - 1;
            while (node > 0)
            {
                int parent = (node - 1) >> 1;
                if (priorityComparer.Compare(queue[parent].Key, queue[node].Key) < 0)
                {
                    break; // we're in the right order, so we're done
                }
                KeyValuePair<TPriority, TValue> tmp = queue[parent];
                queue[parent] = queue[node];
                queue[node] = tmp;
                node = parent;
            }
        }

        /// <summary>
        /// Bubble down the first element until it's in the correct spot.
        /// </summary>
        private void BubbleDown()
        {
            int node = 0;
            while (true)
            {
                // Find smallest child
                int child0 = (node << 1) + 1;
                int child1 = (node << 1) + 2;
                int smallest;
                if (child0 < queue.Count && child1 < queue.Count)
                {
                    smallest = priorityComparer.Compare(queue[child0].Key, queue[child1].Key) < 0 ? child0 : child1;
                }
                else if (child0 < queue.Count)
                {
                    smallest = child0;
                }
                else if (child1 < queue.Count)
                {
                    smallest = child1;
                }
                else
                {
                    break; // 'node' is a leaf, since both children are outside the array
                }

                if (priorityComparer.Compare(queue[node].Key, queue[smallest].Key) < 0)
                {
                    break; // we're in the right order, so we're done.
                }

                KeyValuePair<TPriority, TValue> tmp = queue[node];
                queue[node] = queue[smallest];
                queue[smallest] = tmp;
                node = smallest;
            }
        }
    }
}
