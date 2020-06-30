using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpringDemo : MonoBehaviour
{
    public PinchSlider handK;
    public PinchSlider snapK;
    public PinchSlider damping;
    public PinchSlider mass;

    public TextMeshProUGUI handKLabel;
    public TextMeshProUGUI snapKLabel;
    public TextMeshProUGUI dampingLabel;
    public TextMeshProUGUI massLabel;

    private float newHandK => Mathf.Lerp(0.1f, 10.0f, handK.SliderValue);
    private float newSnapK => Mathf.Lerp(0.1f, 10.0f, snapK.SliderValue);
    private float newDamping => Mathf.Lerp(0.0f, 0.15f, damping.SliderValue);

    private float newMass => Mathf.Lerp(0.0001f, 0.01f, mass.SliderValue);

    // Start is called before the first frame update
    void Start()
    {
        handK.SliderValue = Mathf.InverseLerp(0.1f, 10.0f, 4.0f);
        snapK.SliderValue = Mathf.InverseLerp(0.1f, 10.0f, 7.0f);
        damping.SliderValue = Mathf.InverseLerp(0.0f, 0.15f, 0.08f);
        mass.SliderValue = Mathf.InverseLerp(0.0001f, 0.01f, 0.005f);

        handK.OnInteractionEnded.AddListener(UpdateSprings);
        snapK.OnInteractionEnded.AddListener(UpdateSprings);
        damping.OnInteractionEnded.AddListener(UpdateSprings);
        mass.OnInteractionEnded.AddListener(UpdateSprings);

        handK.OnValueUpdated.AddListener(UpdateLabels);
        snapK.OnValueUpdated.AddListener(UpdateLabels);
        damping.OnValueUpdated.AddListener(UpdateLabels);
        mass.OnValueUpdated.AddListener(UpdateLabels);

        UpdateLabels(null);
        UpdateSprings(null);
    }

    public void UpdateLabels(SliderEventData s)
    {
        handKLabel.text = newHandK.ToString("F2");
        snapKLabel.text = newSnapK.ToString("F2");
        dampingLabel.text = newDamping.ToString("F3");
        massLabel.text = newMass.ToString("F4");
    }

    public void UpdateSprings(SliderEventData s)
    {
        foreach (var bc in FindObjectsOfType<BoundsControl>())
        {
            bc.elasticProperties.HandK = newHandK;
            bc.elasticProperties.SnapK = newSnapK;
            bc.elasticProperties.Drag = newDamping;
            bc.elasticProperties.Mass = newMass;
        }
    }
}
