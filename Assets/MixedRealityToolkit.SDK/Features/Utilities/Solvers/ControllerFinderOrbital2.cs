// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    public class ControllerFinderOrbital2 : ControllerFinder
    {
        public Transform GetControllerTransform(Handedness handedness)
        {
            Handedness = handedness;
            if (handedness == Handedness.None)
            {
                return null;
            }
            return ControllerTransform;
        }
    }
}
