// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Dwell
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

        /// <inheritdoc/>
        protected override void Awake()
        {
            DwellHandler = this.GetComponentInChildren<DwellHandler>();
        }

        private void Update()
        {
            if (IsDwelling || dwellVisualImage.fillAmount > 0)
            {
                float value = DwellHandler.DwellProgress;
                dwellVisualImage.fillAmount = value;
            }
        }

        /// <inheritdoc/>
        public override void DwellCompleted(IMixedRealityPointer pointer)
        {
            base.DwellCompleted(pointer);
            dwellVisualImage.fillAmount = 0;
            ButtonExecute();
        }

        /// <inheritdoc/>
        public override void ButtonExecute()
        {
            displayLabel.text = "Selected Item: " + itemName.text;
        }
    }
}
