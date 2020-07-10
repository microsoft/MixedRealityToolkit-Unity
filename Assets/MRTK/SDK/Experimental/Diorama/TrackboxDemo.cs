using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrackboxDemo : MonoBehaviour
{
    public float minValue;
    public float maxValue;
    public Trackbox trackbox;
    public TextMeshProUGUI valueText;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<PinchSlider>().OnValueUpdated.AddListener(Test);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Test(SliderEventData sed)
    {
        float value = Mathf.Lerp(minValue, maxValue, sed.NewValue);
        trackbox.ManipulationScale = value;
        valueText.text = $"Sensitivity: {value:F1}x";
    }
}
