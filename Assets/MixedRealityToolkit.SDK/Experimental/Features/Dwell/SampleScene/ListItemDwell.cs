// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    public class ListItemDwell : BaseDwellSample, IMixedRealityFocusHandler
    {
        [SerializeField]
        private TextMeshProUGUI itemName = null;

        [SerializeField]
        private TextMeshProUGUI displayLabel = null;

        public override void DwellCompleted(IMixedRealityPointer pointer)
        {
            base.DwellCompleted(pointer);
            dwellVisualImage.fillAmount = 0;
        }

        protected override void Awake()
        {
            base.Awake();
            dwellHandler = this.GetComponentInChildren<DwellHandler>();
        }

        public void Update()
        {
            if (isDwelling)
            {
                float value = dwellHandler.CalculateDwellProgress(dwellVisualImage.fillAmount);
                dwellVisualImage.fillAmount = value;
            }
        }

        public override void ButtonExecute()
        {
            displayLabel.text = "Selected Item: " + itemName.text;
        }

        public void OnFocusEnter(FocusEventData eventData)
        {
            
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            
        }
    }
}