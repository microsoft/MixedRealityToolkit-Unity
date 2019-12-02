// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    [AddComponentMenu("Scripts/MRTK/Examples/ToggleGameObject")]
    public class ToggleGameObject : MonoBehaviour
    {
        [SerializeField]
        private GameObject objToShowHide = null;

        public void ShowIt()
        {
            ShowIt(true);
        }

        public void HideIt()
        {
            ShowIt(false);
        }

        private void ShowIt(bool showIt)
        {
            if (objToShowHide != null)
            {
                objToShowHide.SetActive(showIt);
            }
        }
    }
}
