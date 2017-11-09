// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// A class with general purpose extensions methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns the absolute duration of the curve from first to last key frame
        /// </summary>
        /// <param name="curve">The animation curve to check duration of.</param>
        /// <returns>Returns 0 if the curve is null or has less than 1 frame, otherwise returns time difference between first and last frame.</returns>
        public static float Duration(this AnimationCurve curve)
        {
            if (curve == null || curve.length <= 1)
            {
                return 0.0f;
            }

            return Mathf.Abs(curve[curve.length - 1].time - curve[0].time);
        }

        /// <summary>
        /// Determines whether or not a ray is valid.
        /// </summary>
        /// <param name="ray">The ray being tested.</param>
        /// <returns>True if the ray is valid, false otherwise.</returns>
        public static bool IsValid(this Ray ray)
        {
            return (ray.direction != Vector3.zero);
        }

        #region Collections

        /// <summary>
        /// Creates a read-only wrapper around an existing collection.
        /// </summary>
        /// <typeparam name="TElement">The type of element in the collection.</typeparam>
        /// <param name="elements">The collection to be wrapped.</param>
        /// <returns>The new, read-only wrapper around <paramref name="elements"/>.</returns>
        public static ReadOnlyCollection<TElement> AsReadOnly<TElement>(this IList<TElement> elements)
        {
            return new ReadOnlyCollection<TElement>(elements);
        }

        /// <summary>
        /// Creates a read-only copy of an existing collection.
        /// </summary>
        /// <typeparam name="TElement">The type of element in the collection.</typeparam>
        /// <param name="elements">The collection to be copied.</param>
        /// <returns>The new, read-only copy of <paramref name="elements"/>.</returns>
        public static ReadOnlyCollection<TElement> ToReadOnlyCollection<TElement>(this IEnumerable<TElement> elements)
        {
            return elements.ToArray().AsReadOnly();
        }

        /// <summary>
        /// Inserts an item in its sorted position into an already sorted collection. This is useful if you need to consume the
        /// collection in between insertions and need it to stay correctly sorted the whole time. If you just need to insert a
        /// bunch of items and then consume the sorted collection at the end, it's faster to add all the elements and then use
        /// <see cref="List{T}.Sort"/> at the end.
        /// </summary>
        /// <typeparam name="TElement">The type of element in the collection.</typeparam>
        /// <param name="elements">The collection of sorted elements to be inserted into.</param>
        /// <param name="toInsert">The element to insert.</param>
        /// <param name="comparer">The comparer to use when sorting or <see cref="null"/> to use <see cref="Comparer{T}.Default"/>.</param>
        /// <returns></returns>
        public static int SortedInsert<TElement>(this List<TElement> elements, TElement toInsert, IComparer<TElement> comparer = null)
        {
            var effectiveComparer = (comparer ?? Comparer<TElement>.Default);

#if DEBUG || UNITY_EDITOR
            for (int iElement = 0; iElement < (elements.Count - 1); iElement++)
            {
                var element = elements[iElement];
                var nextElement = elements[iElement + 1];

                if (effectiveComparer.Compare(element, nextElement) > 0)
                {
                    Debug.Assert(false, "elements must already be sorted to call this method.");
                    break;
                }
            }
#endif

            int searchResult = elements.BinarySearch(toInsert, effectiveComparer);

            int insertionIndex = (searchResult >= 0)
                ? searchResult
                : (~searchResult);

            elements.Insert(insertionIndex, toInsert);

            return insertionIndex;
        }

        /// <summary>
        /// Disposes of all non-null elements in a collection.
        /// </summary>
        /// <typeparam name="TElement">The type of element in the collection.</typeparam>
        /// <param name="elements">The collection of elements to be disposed.</param>
        public static void DisposeElements<TElement>(this IEnumerable<TElement> elements)
            where TElement : IDisposable
        {
            foreach (var element in elements)
            {
                if (element != null)
                {
                    element.Dispose();
                }
            }
        }

        /// <summary>
        /// Disposes of all non-null elements in a collection.
        /// </summary>
        /// <typeparam name="TElement">The type of element in the collection.</typeparam>
        /// <param name="elements">The collection of elements to be disposed.</param>
        public static void DisposeElements<TElement>(this IList<TElement> elements)
            where TElement : IDisposable
        {
            for (int iElement = 0; iElement < elements.Count; iElement++)
            {
                var element = elements[iElement];

                if (element != null)
                {
                    element.Dispose();
                }
            }
        }

        #endregion

        #region Numerics

        /// <summary>
        /// Checks if two numbers are approximately equal. Similar to <see cref="Mathf.Approximately(float, float)"/>, but the tolerance
        /// can be specified.
        /// </summary>
        /// <param name="number">One of the numbers to compare.</param>
        /// <param name="other">The other number to compare.</param>
        /// <param name="tolerance">The amount of tolerance to allow while still considering the numbers approximately equal.</param>
        /// <returns>True if the difference between the numbers is less than or equal to the tolerance, false otherwise.</returns>
        public static bool Approximately(this float number, float other, float tolerance)
        {
            return (Mathf.Abs(number - other) <= tolerance);
        }

        /// <summary>
        /// Checks if two numbers are approximately equal. Similar to <see cref="Mathf.Approximately(float, float)"/>, but the tolerance
        /// can be specified.
        /// </summary>
        /// <param name="number">One of the numbers to compare.</param>
        /// <param name="other">The other number to compare.</param>
        /// <param name="tolerance">The amount of tolerance to allow while still considering the numbers approximately equal.</param>
        /// <returns>True if the difference between the numbers is less than or equal to the tolerance, false otherwise.</returns>
        public static bool Approximately(this double number, double other, double tolerance)
        {
            return (Math.Abs(number - other) <= tolerance);
        }

        #endregion

        #region UnityEngine.Object

        public static void DontDestroyOnLoad(this Object target)
        {
#if UNITY_EDITOR // Skip Don't Destroy On Load when editor isn't playing so test runner passes.
            if (UnityEditor.EditorApplication.isPlaying)
#endif
                Object.DontDestroyOnLoad(target);
        }

        #endregion

        #region GameObject

        /// <summary>
        /// Determines whether or not a game object's layer is included in the specified layer mask.
        /// </summary>
        /// <param name="gameObject">The game object whose layer to test.</param>
        /// <param name="layerMask">The layer mask to test against.</param>
        /// <returns>True if <paramref name="gameObject"/>'s layer is included in <paramref name="layerMask"/>, false otherwise.</returns>
        public static bool IsInLayerMask(this GameObject gameObject, LayerMask layerMask)
        {
            LayerMask gameObjectMask = (1 << gameObject.layer);
            return ((gameObjectMask & layerMask) == gameObjectMask);
        }

        #endregion

        #region Comparer

        /// <summary>
        /// Gets a comparer that sorts elements in the opposite order of the original comparer.
        /// </summary>
        /// <typeparam name="TElement">The type of element the comparer compares.</typeparam>
        /// <param name="originalComparer">The comparer whose order should be reversed.</param>
        /// <returns>A comparer that sorts elements in the opposite order of <paramref name="originalComparer"/>.</returns>
        public static IComparer<TElement> GetReversed<TElement>(this IComparer<TElement> originalComparer)
        {
            return new ReverseComparer<TElement>(originalComparer);
        }

        private class ReverseComparer<TElement> :
            IComparer<TElement>
        {
            private readonly IComparer<TElement> originalComparer;

            public ReverseComparer(IComparer<TElement> originalComparer)
            {
                Debug.Assert(originalComparer != null, "originalComparer cannot be null.");

                this.originalComparer = originalComparer;
            }

            public int Compare(TElement left, TElement right)
            {
                return originalComparer.Compare(right, left);
            }
        }

        #endregion
    }
}
