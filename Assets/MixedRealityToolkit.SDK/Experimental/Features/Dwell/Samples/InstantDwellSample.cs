// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    public class InstantDwellSample : BaseDwellSample
    {
        [SerializeField]
        private Transform listItems = null;

        public void Update()
        {
            float value = dwellHandler.CalculateDwellProgress();
            dwellVisualImage.transform.localScale = new Vector3(value, 1, 0);
        }

        public override void DwellStarted(IMixedRealityPointer pointer)
        {
            base.DwellStarted(pointer);
        }

        public override void DwellCanceled(IMixedRealityPointer pointer)
        {
            isDwelling = false;
        }

        public override void DwellCompleted(IMixedRealityPointer pointer)
        {
            isDwelling = false;
            dwellVisualImage.transform.localScale = Vector3.zero;
            base.DwellCompleted(pointer);
        }

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