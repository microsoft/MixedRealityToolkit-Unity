// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
