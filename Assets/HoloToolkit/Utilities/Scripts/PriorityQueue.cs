// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Min-heap priority queue. In other words, lower priorities will be removed from the queue first.
    /// See http://en.wikipedia.org/wiki/Binary_heap for more info
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
                foreach (KeyValuePair<TPriority, TValue> pair in this.parentCollection)
                {
                    yield return pair.Value;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion
        }

        private readonly IComparer<TPriority> priorityComparer;

        public PriorityQueue()
            : this(Comparer<TPriority>.Default)
        {
        }

        public PriorityQueue(IComparer<TPriority> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException();

            priorityComparer = comparer;
        }

        private readonly List<KeyValuePair<TPriority, TValue>> queue = new List<KeyValuePair<TPriority, TValue>>();
        private ValueCollection valueCollection = null;

        public ValueCollection Values
        {
            get
            {
                if (this.valueCollection == null)
                {
                    this.valueCollection = new ValueCollection(this);
                }

                return this.valueCollection;
            }
        }

        #region IEnumerable

        public IEnumerator<KeyValuePair<TPriority, TValue>> GetEnumerator()
        {
            return this.queue.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Clears the priority queue
        /// </summary>
        public void Clear()
        {
            this.queue.Clear();
        }

        /// <summary>
        /// Add an element to the priority queue.
        /// </summary>
        /// <param name="priority">Priority of the element</param>
        /// <param name="value"></param>
        public void Push(TPriority priority, TValue value)
        {
            this.queue.Add(new KeyValuePair<TPriority, TValue>(priority, value));
            BubbleUp();
        }

        /// <summary>
        /// Number of elements in priority queue
        /// </summary>
        public int Count
        {
            get
            {
                return this.queue.Count;
            }
        }

        /// <summary>
        /// Get the element with the minimum priority in the queue. The Key in the return value is the priority.
        /// </summary>
        public KeyValuePair<TPriority, TValue> Top
        {
            get
            {
                return this.queue[0];
            }
        }

        /// <summary>
        /// Pop the minimal element of the queue. Will fail at runtime if queue is empty.
        /// </summary>
        /// <returns>The minmal element</returns>
        public KeyValuePair<TPriority, TValue> Pop()
        {
            KeyValuePair<TPriority, TValue> ret = this.queue[0];
            this.queue[0] = this.queue[queue.Count - 1];
            this.queue.RemoveAt(this.queue.Count - 1);
            BubbleDown();
            return ret;
        }

        /// <summary>
        /// Returns whether or not the value is contained in the queue
        /// </summary>
        public bool Contains(TValue value)
        {
            return this.queue.Any(itm => EqualityComparer<TValue>.Default.Equals(itm.Value, value));
        }

        /// <summary>
        /// Removes the first element that equals the value from the queue
        /// </summary>
        public bool Remove(TValue value)
        {
            var idx = this.queue.FindIndex(itm => EqualityComparer<TValue>.Default.Equals(itm.Value, value));
            if (idx == -1)
            {
                return false;
            }

            this.queue[idx] = this.queue[this.queue.Count - 1];
            this.queue.RemoveAt(this.queue.Count - 1);
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

            for (int i = this.queue.Count - 1; i >= 0; --i)
            {
                // TODO: early out if key < priority
                if (this.queue[i].Key.Equals(priority) && (shouldRemove == null || shouldRemove(this.queue[i].Value)))
                {
                    this.queue[i] = this.queue[this.queue.Count - 1];
                    this.queue.RemoveAt(this.queue.Count - 1);
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
            int node = this.queue.Count - 1;
            while (node > 0)
            {
                int parent = (node - 1) >> 1;
                if (priorityComparer.Compare(this.queue[parent].Key, this.queue[node].Key) < 0)
                {
                    break; // we're in the right order, so we're done
                }
                var tmp = this.queue[parent];
                this.queue[parent] = this.queue[node];
                this.queue[node] = tmp;
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
                if (child0 < this.queue.Count && child1 < this.queue.Count)
                {
                    smallest = priorityComparer.Compare(this.queue[child0].Key, this.queue[child1].Key) < 0 ? child0 : child1;
                }
                else if (child0 < this.queue.Count)
                {
                    smallest = child0;
                }
                else if (child1 < this.queue.Count)
                {
                    smallest = child1;
                }
                else
                {
                    break; // 'node' is a leaf, since both children are outside the array
                }

                if (priorityComparer.Compare(this.queue[node].Key, this.queue[smallest].Key) < 0)
                {
                    break; // we're in the right order, so we're done.
                }

                var tmp = this.queue[node];
                this.queue[node] = this.queue[smallest];
                this.queue[smallest] = tmp;
                node = smallest;
            }
        }
    }
}
