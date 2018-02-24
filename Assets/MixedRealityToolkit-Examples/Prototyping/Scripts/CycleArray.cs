// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.Prototyping
{
    /// <summary>
    /// CycleArray is a class for incrementing through component properties sequentially or assigning specific elements in the array.
    /// A good use-case is updating visuals on a GameObject easily through the UnityEditor.
    /// Use with Interactive or InteractiveEffects for consistent visual feedback patterns
    /// </summary>
    public class CycleArray<Type> : MonoBehaviour, ICycle
    {
        /// <summary>
        /// Placeholder for the array of objects to cycle through
        /// </summary>
        [SerializeField]
        protected Type[] Array;

        /// <summary>
        /// GameObject to manipulate, uses the host object if no object is specified.
        /// </summary>
        public GameObject TargetObject = null;

        /// <summary>
        /// The element of the array to use on awake
        /// </summary>
        public int DefaultIndex = 0;
        
        /// <summary>
        /// the current index of the applied item in the array
        /// </summary>
        public int Index { get; set;}

        /// <summary>
        /// the current entry in the array
        /// </summary>
        public Type Current
        {
            get
            {
                return Array[Index % Array.Length];
            }
        }

        private bool mHasInit = false;
        
        /// <summary>
        /// set the default TargetObject
        /// </summary>
        protected virtual void Awake()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }
        }

        /// <summary>
        /// set the default values
        /// </summary>
        protected virtual void Start()
        {
            if (!mHasInit)
            {
                SetIndex(DefaultIndex);
            }
        }

        /// <summary>
        /// Assign a specific element from the array.
        /// Place your custom logic to assign an element to TargetObject here...
        /// </summary>
        /// <param name="index">the desired item index</param>
        public virtual void SetIndex(int index)
        {
            if (index > -1 && index <= GetLastIndex())
            {
                Index = index;
            }
            else
            {
                Debug.LogError("index out of bounds!");
            }

            mHasInit = true;

            // do something with the updated index and apply it to the object
            // Sample: TargetObject.transform.localScale = ScaleArray[CurrentIndex];
        }

        /// <summary>
        /// Move to the next item in the array
        /// </summary>
        public virtual void MoveNext()
        {
            if (Index < GetLastIndex())
            {
                SetIndex(Index + 1);
            }
            else
            {
                SetIndex(0);
            }
        }

        /// <summary>
        /// Move to the previous item in the array
        /// </summary>
        public virtual void MovePrevious()
        {
            if (Index > 0)
            {
                SetIndex(Index - 1);
            }
            else
            {
                SetIndex(GetLastIndex());
            }
        }

        /// <summary>
        /// Returns the last index in the array
        /// </summary>
        /// <returns></returns>
        public virtual int GetLastIndex()
        {
            // return the last index of the new array added above.
            return Array.Length - 1;
        }
    }
}
