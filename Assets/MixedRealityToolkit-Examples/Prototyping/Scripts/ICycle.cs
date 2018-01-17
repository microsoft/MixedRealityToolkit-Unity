// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace MixedRealityToolkit.Examples.Prototyping
{
    /// <summary>
    /// An interface for components that cycle through an array of properties.
    /// A component can be built to cycle through colors, positions, textures and apply these values to the gameObject they are assigned to.
    /// </summary>
    public interface ICycle
    {
        int Index { get; set; }

        /// <summary>
        /// Assign a specific element from the array.
        /// Place your custom logic to assign an element to TargetObject here...
        /// </summary>
        /// <param name="index">the desired item index</param>
        void SetIndex(int index);

        /// <summary>
        /// Move to the next item in the array
        /// </summary>
        void MoveNext();

        /// <summary>
        /// Move to the previous item in the array
        /// </summary>
        void MovePrevious();

        /// <summary>
        /// Retrieve the last index of the array.
        /// </summary>
        /// <returns></returns>
        int GetLastIndex();
    }
}
