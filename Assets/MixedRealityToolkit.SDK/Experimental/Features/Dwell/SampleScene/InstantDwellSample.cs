// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using TMPro;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    public class InstantDwellSample : BaseDwellSample
    {
        [SerializeField]
        private Transform listItems = null;

        bool isComplete = false;

        public override void DwellStarted(IMixedRealityPointer pointer)
        {
            base.DwellStarted(pointer);
            isComplete = false;
        }

        public override void DwellCanceled(IMixedRealityPointer pointer)
        {
            isDwelling = false;
           // wasDwelling = true;
        }

        public override void DwellCompleted(IMixedRealityPointer pointer)
        {
            isDwelling = false;
            dwellVisualImage.transform.localScale = Vector3.zero;
            base.DwellCompleted(pointer);
            isComplete = true;
        }

        public void Update()
        {
            if (!isComplete)
            {
                float value = dwellHandler.CalculateDwellProgress(dwellVisualImage.transform.localScale.x);
                dwellVisualImage.transform.localScale = new Vector3(value, 1, 0);
            }
            //else if (wasDwelling)
            //{

            //}
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