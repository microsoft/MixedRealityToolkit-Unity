using Microsoft.MixedReality.Toolkit.Experimental.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackboxHydration : MonoBehaviour
{
    private static ElasticExtentProperties<float> elasticExtent = new ElasticExtentProperties<float>
    {
        SnapToEnds = true,
        SnapPoints = new float[] { },
        MaxStretch = 1.0f,
        MinStretch = -1.0f
    };

    private static ElasticProperties elasticProperties = new ElasticProperties
    {
        Mass = 0.02f,
        HandK = 3.0f,
        EndK = 2.0f,
        SnapRadius = 1.0f,
        Drag = 0.25f
    };

    LinearElasticSystem hydrationElastic = new LinearElasticSystem(0.001f, 0.0f, elasticExtent, elasticProperties);
    float scaleGoal;
    Light trackboxLight;
    float initialLightIntensity;

    // Start is called before the first frame update
    void Start()
    {
        trackboxLight = GetComponentInChildren<Light>();
        if(trackboxLight != null)
        {
            initialLightIntensity = trackboxLight.intensity;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one * Mathf.Clamp(hydrationElastic.ComputeIteration(scaleGoal, Time.deltaTime), 0.001f, 10.0f);
        if (trackboxLight != null)
        {
            trackboxLight.intensity = hydrationElastic.GetCurrentValue() * initialLightIntensity;
        }
    }

    public void Hydrate()
    {
        scaleGoal = 1.0f;
    }

    public void Dehydrate()
    {
        scaleGoal = -1.0f;
    }
}
