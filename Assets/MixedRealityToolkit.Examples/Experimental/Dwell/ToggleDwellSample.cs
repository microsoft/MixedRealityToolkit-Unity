// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    /// <summary>
    /// Example script to demonstrate a toggle button using dwell
    /// </summary>
    public class ToggleDwellSample : BaseDwellSample
    {
        [SerializeField]
        private TextMeshProUGUI dwellStatus = null;

        [SerializeField]
        private Image buttonBackground = null;

        private bool isDwellEnabled = true;

        [SerializeField]
        private Color dwellOnColor = Color.white;

        [SerializeField]
        private Color dwellOffColor = Color.cyan;

        [SerializeField]
        private Color dwellIntendedColor = Color.cyan;

        [SerializeField]
        private float dwellVisualCancelDurationInFrames = 60;

        private float cancelStartScale = 0;

        public void Update()
        {
            if (isDwelling)
            {
                float value = dwellHandler.DwellProgress;
                dwellVisualImage.transform.localScale = new Vector3(value, value, value);
            }
            else if (!isDwelling && dwellVisualImage.transform.localScale.x > 0)
            {
                float value = Mathf.Clamp(dwellVisualImage.transform.localScale.x - (cancelStartScale / dwellVisualCancelDurationInFrames), 0f, 1f);
                dwellVisualImage.transform.localScale = new Vector3(value, value, value);
            }
        }

        public override void DwellIntended(IMixedRealityPointer pointer)
        {
            buttonBackground.color = dwellIntendedColor;
            dwellVisualImage.transform.localScale = Vector3.zero;
        }

        public override void DwellCanceled(IMixedRealityPointer pointer)
        {
            base.DwellCanceled(pointer);
            buttonBackground.color = isDwellEnabled ? this.dwellOnColor : this.dwellOffColor;
            cancelStartScale = dwellVisualImage.transform.localScale.x;
        }

        public override void DwellCompleted(IMixedRealityPointer pointer)
        {
            base.DwellCompleted(pointer);
            dwellVisualImage.transform.localScale = Vector3.zero;
        }

        public override void ButtonExecute()
        {
            isDwellEnabled = !isDwellEnabled;
            dwellStatus.text = isDwellEnabled ? "On" : "Off";
            
            // swap the button background and dwell visuals overla color
            buttonBackground.color = isDwellEnabled ? this.dwellOnColor : this.dwellOffColor;
            dwellVisualImage.color = isDwellEnabled ? this.dwellOffColor : this.dwellOnColor;

            var foundObjects = FindObjectsOfType<DwellHandler>();

            foreach (var obj in foundObjects)
            {
                obj.enabled = isDwellEnabled;
            }

            dwellHandler.enabled = true;
        }
    }
}
