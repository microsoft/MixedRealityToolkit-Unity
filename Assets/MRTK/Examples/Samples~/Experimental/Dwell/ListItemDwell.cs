// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    /// <summary>
    /// Dwell sample to work on a list of dwell targets
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/ListItemDwell")]
    public class ListItemDwell : BaseDwellSample
    {
        [SerializeField]
        private TextMeshProUGUI itemName = null;

        [SerializeField]
        private TextMeshProUGUI displayLabel = null;

        protected override void Awake()
        {
            base.Awake();
            dwellHandler = this.GetComponentInChildren<DwellHandler>();
        }

        public void Update()
        {
            if (isDwelling)
            {
                float value = dwellHandler.DwellProgress;
                dwellVisualImage.fillAmount = value;
            }
        }

        public override void DwellCompleted(IMixedRealityPointer pointer)
        {
            base.DwellCompleted(pointer);
            dwellVisualImage.fillAmount = 0;
            ButtonExecute();
        }

        public override void ButtonExecute()
        {
            displayLabel.text = "Selected Item: " + itemName.text;
        }
    }
}
