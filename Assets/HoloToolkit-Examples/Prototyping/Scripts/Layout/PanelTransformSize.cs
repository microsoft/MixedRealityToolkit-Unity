// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.Prototyping
{
    [ExecuteInEditMode]
    public class PanelTransformSize : MonoBehaviour
    {

        public float BasePixelScale = 2048;
        public Vector3 ItemSize = new Vector3(594, 246, 15);

        private void UpdateSize()
        {
            Vector3 newScale = new Vector3(ItemSize.x / BasePixelScale, ItemSize.y / BasePixelScale, ItemSize.z / BasePixelScale);
            transform.localScale = newScale;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateSize();
        }
    }
}
