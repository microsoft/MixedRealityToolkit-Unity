using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class UpdateSliderTrackLine : MonoBehaviour
{
    [SerializeField]
    private GameObject activeLine = null;
    private float roundedValue;

    public void OnSliderUpdated(SliderEventData eventData)
    {
            roundedValue = Mathf.Round(eventData.NewValue * 100f) / 100f;
            activeLine.transform.localScale = new Vector3 (transform.localScale.x, roundedValue, transform.localScale.z);
    }
}
