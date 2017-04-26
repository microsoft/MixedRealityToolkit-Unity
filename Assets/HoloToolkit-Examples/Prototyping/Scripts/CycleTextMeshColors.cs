// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.Prototyping
{
    public class CycleTextMeshColors : CycleArray<Color>
    {

        public TextMesh TextMeshObject;

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
