// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Experimental.HandMenu
{
    [AddComponentMenu("Scripts/MRTK/Examples/UpdateSliderTrackLine")]
    public class UpdateSliderTrackLine : MonoBehaviour
    {
        [SerializeField]
        private GameObject activeLine = null;

        public void OnSliderUpdated(SliderEventData eventData)
        {
            activeLine.transform.localScale = new Vector3(transform.localScale.x, eventData.NewValue, transform.localScale.z);
        }
    }
}