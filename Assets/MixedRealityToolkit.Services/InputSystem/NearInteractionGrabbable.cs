// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Add a NearInteractionGrabbable component to any GameObject that has a collidable
    /// on it in order to make that collidable near grabbable.
    /// 
    /// Any IMixedRealityNearPointer will then dispatch pointer events
    /// to the closest near grabbable objects.
    ///
    /// Additionally, the near pointer will send focus enter and exit events when the 
    /// decorated object is the closest object to the near pointer
    /// </summary>
    public class NearInteractionGrabbable : MonoBehaviour
    {
        [Tooltip("Check to show a tether from the position where object was grabbed to the hand when manipulating. Useful for things like bounding boxes where resizing/rotating might be constrained.")]
        public bool ShowTetherWhenManipulating = false;
    }
}