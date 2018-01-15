// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.Prototyping
{
    /// <summary>
    /// Sets the color of a TextMesh object based on the selected value in the array
    /// </summary>
    public class CycleTextMeshColors : CycleArray<Color>
    {
        [Tooltip("TextMesh to assign the selected text")]
        public TextMesh TextMeshObject;

        protected override void Awake()
        {
            if (TextMeshObject == null)
            {
                TextMeshObject = GetComponent<TextMesh>();
            }

            base.Awake();
        }

        /// <summary>
        /// apply the selected text
        /// </summary>
        /// <param name="index"></param>
        public override void SetIndex(int index)
        {
            base.SetIndex(index);

            if (TextMeshObject == null)
            {
                TextMeshObject = GetComponent<TextMesh>();
            }

            TextMeshObject.color = Array[Index];
        }
    }
}
