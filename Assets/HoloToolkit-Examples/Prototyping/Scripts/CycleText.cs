// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.Prototyping
{

    public class CycleText : CycleArray<string>
    {

        public TextMesh Label;
        protected override void Awake()
        {
            base.Awake();

            if (Label == null)
            {
                Label = GetComponent<TextMesh>();
            }

            if (Label == null)
            {
                Debug.LogError("Textmesh:Label is not set in CycleText!");
                Destroy(this);
            }
        }

        public override void SetIndex(int index)
        {
            base.SetIndex(index);

            Label.text = Array[index];
        }
    }
}
