using Microsoft.MixedReality.Toolkit.Experimental.Physics;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackboxHydration : MonoBehaviour
{
    #region Elastic Properties
    private static ElasticExtentProperties<float> elasticExtent = new ElasticExtentProperties<float>
    {
        SnapToEnds = false,
        SnapPoints = new float[] { },
        MaxStretch = 1.0f,
        MinStretch = -1.0f
    };

    private static ElasticProperties boxElasticProperties = new ElasticProperties
    {
        Mass = 0.02f,
        HandK = 4.0f,
        EndK = 2.0f,
        SnapRadius = 1.0f,
        Drag = 0.25f
    };
    
    private static ElasticProperties optionsElasticProperties = new ElasticProperties
    {
        Mass = 0.015f,
        HandK = 4.0f,
        EndK = 2.0f,
        SnapRadius = 1.0f,
        Drag = 0.15f
    };
    #endregion

    [SerializeField]
    private ObjectManipulator boxManipulator;
    public ObjectManipulator objectManipulator
    {
        get => boxManipulator;
        set => boxManipulator = value;
    }

    [SerializeField]
    private List<Transform> optionsPanels;
    public List<Transform> OptionsPanels
    {
        get => optionsPanels;
        set => optionsPanels = value;
    }

    private List<LinearElasticSystem> optionsElastic = new List<LinearElasticSystem>();
    private List<float> optionsElasticGoals = new List<float>();

    LinearElasticSystem boxHydrationElastic = new LinearElasticSystem(0.001f, 0.0f, elasticExtent, boxElasticProperties);
    float scaleGoal;
    Vector3 initialScale;
    Light trackboxLight;
    float initialLightIntensity;
    Collider[] colliders = new Collider[] { };

    bool useElastic = true;

    // Start is called before the first frame update
    void Start()
    {
        trackboxLight = GetComponentInChildren<Light>();
        if(trackboxLight != null)
        {
            initialLightIntensity = trackboxLight.intensity;
        }
        initialScale = transform.localScale;
        colliders = GetComponentsInChildren<Collider>();

        boxManipulator.OnManipulationStarted.AddListener(BoxManipulationStarted);
        boxManipulator.OnManipulationEnded.AddListener(BoxManipulationEnded);

        foreach(var panel in optionsPanels)
        {
            optionsElastic.Add(new LinearElasticSystem(0.001f, 0.0f, elasticExtent, optionsElasticProperties));
            optionsElasticGoals.Add(-1.0f);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (useElastic)
        {
            transform.localScale = initialScale * Mathf.Clamp(boxHydrationElastic.ComputeIteration(scaleGoal, Time.deltaTime), 0.001f, 10.0f);
        }

        for(var i = 0; i < optionsPanels.Count; i++)
        {
            optionsPanels[i].localRotation = Quaternion.Euler(new Vector3(-45.0f * optionsElastic[i].ComputeIteration(optionsElasticGoals[i], Time.deltaTime), 0, 0));
        }

        if (trackboxLight != null)
        {
            trackboxLight.intensity = boxHydrationElastic.GetCurrentValue() * initialLightIntensity;
        }
    }

    void BoxManipulationStarted(ManipulationEventData data)
    {
        useElastic = false;
    }

    void BoxManipulationEnded(ManipulationEventData data)
    {
        useElastic = true;
        initialScale = transform.localScale;
    }

    public void Hydrate()
    {
        foreach (var c in colliders)
        {
            c.enabled = true;
        }
        scaleGoal = 1.0f;
        StartCoroutine(HydrateOptionsPanel());
    }

    public void Dehydrate()
    {
        foreach(var c in colliders)
        {
            c.enabled = false;
        }
        scaleGoal = -1.0f;
        for (var i = 0; i < optionsElasticGoals.Count; i++)
        {
            optionsElasticGoals[i] = -1.0f;
        }
    }

    public void SlowDehydrate()
    {
        foreach (var c in colliders)
        {
            c.enabled = false;
        }
        StartCoroutine(SlowDehydrateCoroutine());
    }

    private IEnumerator HydrateOptionsPanel()
    {
        yield return new WaitForSeconds(0.2f);

        for(var i = 0; i < optionsElasticGoals.Count; i++)
        {
            optionsElasticGoals[i] = 1.0f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator SlowDehydrateCoroutine()
    {
        for (var i = optionsElasticGoals.Count - 1; i >= 0; i--)
        {
            optionsElasticGoals[i] = -1.0f;
            yield return new WaitForSeconds(0.1f);
        }

        scaleGoal = -1.0f;
    }
}
