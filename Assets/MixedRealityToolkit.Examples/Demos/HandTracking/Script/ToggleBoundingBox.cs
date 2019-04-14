// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class ToggleBoundingBox : MonoBehaviour
    {
        public BoundingBox boundingBox;

        public void ToggleBoundingBoxActiveState()
        {
            // Do something on specified distance for fire event
            if (boundingBox != null)
            {
                boundingBox.Active = !boundingBox.Active;
            }

        }
    }
}