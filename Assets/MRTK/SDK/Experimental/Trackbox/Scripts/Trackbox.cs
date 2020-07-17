using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Trackbox : ObjectManipulator
{
    [SerializeField]
    private float manipulationScale;
    public float ManipulationScale
    {
        get => manipulationScale;
        set => manipulationScale = value;
    }

    [SerializeField]
    private AnimationCurve manipulationCurve;
    public AnimationCurve ManipulationCurve
    {
        get => manipulationCurve;
        set => manipulationCurve = value;
    }

    [SerializeField]
    private TextMeshPro scaleLabel;
    public TextMeshPro ScaleLabel
    {
        get => scaleLabel;
        set => scaleLabel = value;
    }

    [SerializeField]
    private Transform crosshairTransform;
    public Transform CrosshairTransform
    {
        get => crosshairTransform;
        set => crosshairTransform = value;
    }

    [SerializeField]
    private Transform gridTransform;
    public Transform GridTransform
{
        get => gridTransform;
        set => gridTransform = value;
    }

    [SerializeField]
    private Transform lightTransform;
    public Transform LightTransform
    {
        get => lightTransform;
        set => lightTransform = value;
    }

    [SerializeField]
    private bool rotationEnabled;
    public bool RotationEnabled
    {
        get => rotationEnabled;
        set => rotationEnabled = value;
    }

    [SerializeField]
    private bool translationEnabled;
    public bool TranslationEnabled
    {
        get => translationEnabled;
        set => translationEnabled = value;
    }

    [SerializeField]
    private bool scaleEnabled;
    public bool ScaleEnabled
    {
        get => scaleEnabled;
        set => scaleEnabled = value;
    }


    [SerializeField]
    private Interactable rotationToggle;
    public Interactable RotationToggle
    {
        get => rotationToggle;
        set => rotationToggle = value;
    }

    [SerializeField]
    private Interactable translationToggle;
    public Interactable TranslationToggle
    {
        get => translationToggle;
        set => translationToggle = value;
    }

    [SerializeField]
    private Interactable scaleToggle;
    public Interactable ScaleToggle
    {
        get => scaleToggle;
        set => scaleToggle = value;
    }

    private TrackpadMoveLogic trackpadLogic;

    private Vector3 originalLightPosition;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        crosshairTransform.gameObject.SetActive(false);
        trackpadLogic = new TrackpadMoveLogic();
        OnManipulationEnded.AddListener(ResetLight);

        originalLightPosition = lightTransform.localPosition;

        translationToggle.IsToggled = true;

        // Set the initial -enabled properties to the toggle states.
        UpdateToggles();

        rotationToggle.OnClick.AddListener(UpdateToggles);
        translationToggle.OnClick.AddListener(UpdateToggles);
        scaleToggle.OnClick.AddListener(UpdateToggles);
    }

    // Update is called once per frame
    void Update()
    {
        ManipulationScale = ManipulationCurve.Evaluate(transform.lossyScale.x);
        scaleLabel.text = ManipulationScale.ToString("F1") + "x";
    }

    public void UpdateToggles()
    {
        rotationEnabled = rotationToggle.IsToggled;
        translationEnabled = translationToggle.IsToggled;
        scaleEnabled = scaleToggle.IsToggled;

        // When using a trackbox, the base TransformFlags.Move should always be zero.
        oneHandedManipulationType = (rotationEnabled ? TransformFlags.Rotate : 0);
        twoHandedManipulationType = (rotationEnabled ? TransformFlags.Rotate : 0) | (scaleEnabled ? TransformFlags.Scale : 0);
    }

    public void ResetLight(ManipulationEventData data)
    {
        lightTransform.localPosition = originalLightPosition;
    }

    #region Private Event Handlers
    protected override void HandleTwoHandManipulationStarted()
    {
        UpdateToggles();
        base.HandleTwoHandManipulationStarted();

        MixedRealityPose hostPose = new MixedRealityPose(HostTransform.position, HostTransform.rotation);
        trackpadLogic.Setup(GetPointersGrabPoint(), hostPose);

        crosshairTransform.gameObject.SetActive(true);
    }

    protected override void HandleOneHandMoveStarted()
    {
        UpdateToggles();
        base.HandleOneHandMoveStarted();

        MixedRealityPose hostPose = new MixedRealityPose(HostTransform.position, HostTransform.rotation);
        trackpadLogic.Setup(GetPointersGrabPoint(), hostPose);

        crosshairTransform.gameObject.SetActive(true);
    }

    protected override void UpdateOneHandedLogic(TransformFlags transformType, ref MixedRealityTransform targetTransform)
    {
        base.UpdateOneHandedLogic(transformType, ref targetTransform);

        if(translationEnabled)
        {
            targetTransform.Position = trackpadLogic.Update(GetPointersGrabPoint(), ManipulationScale);
            crosshairTransform.position = GetPointersGrabPoint();
            lightTransform.position = GetPointersGrabPoint();
        }
    }

    protected override void UpdateTwoHandedLogic(TransformFlags transformType, ref MixedRealityTransform targetTransform)
    {
        base.UpdateTwoHandedLogic(transformType, ref targetTransform);

        if (translationEnabled)
        {
            targetTransform.Position = trackpadLogic.Update(GetPointersGrabPoint(), ManipulationScale);
            crosshairTransform.position = GetPointersGrabPoint();
            lightTransform.position = GetPointersGrabPoint();
        }
    }
    #endregion
}