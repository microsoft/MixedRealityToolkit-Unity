using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;


public class UpdateTrackScale : MonoBehaviour
{
    [SerializeField]
    private GameObject trackScaleNode = null;
    private float roundedValue;

    
    public void OnSliderUpdated(SliderEventData eventData)
    {
            roundedValue = Mathf.Round(eventData.NewValue * 100f) / 100f;
            trackScaleNode.transform.localScale = new Vector3 (roundedValue, transform.localScale.y, transform.localScale.z);
    }
}