// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Dwell
{
    /// <summary>
    /// Dwell sample with no DwellIntended delay
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/InstantDwellSample")]
    public class InstantDwellSample : BaseDwellSample
    {
        [SerializeField]
        private Transform listItems = null;

        private void Update()
        {
            float value = DwellHandler.DwellProgress;
            dwellVisualImage.transform.localScale = new Vector3(value, 1, 0);
        }

        /// <inheritdoc/>
        public override void DwellCompleted(IMixedRealityPointer pointer)
        {
            dwellVisualImage.transform.localScale = Vector3.zero;
            base.DwellCompleted(pointer);
        }

        /// <inheritdoc/>
        public override void ButtonExecute()
        {
            var textMeshObjects = listItems.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (var textObject in textMeshObjects)
            {
                textObject.text = int.Parse(textObject.text) + 5 + "";
            }
        }
    }
}