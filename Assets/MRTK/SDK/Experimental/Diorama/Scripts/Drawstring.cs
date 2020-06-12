using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Drawstring : MonoBehaviour
{
    [Tooltip("Drawstring target.")]
    [SerializeField]
    private Transform target;

    /// <summary>
    /// Drawstring target.
    /// </summary>
    public Transform Target
    {
        get => target;
        set => target = value;
    }

    [SerializeField]
    [Tooltip("Nonlinear mapping for drawstring -> object scale.")]
    private AnimationCurve scaleMapping = null;

    /// <summary>
    /// Animation curve for growing/shrinking the value label.
    /// </summary>
    public AnimationCurve ScaleMapping
    {
        get => scaleMapping;
        set => scaleMapping = value;
    }

    [SerializeField]
    [Tooltip("Nonlinear mapping for magnetizing the diorama to the drawstring.")]
    private AnimationCurve magnetMapping = null;

    /// <summary>
    /// Nonlinear mapping for magnetizing the diorama to the drawstring.
    /// </summary>
    public AnimationCurve MagnetMapping
    {
        get => magnetMapping;
        set => magnetMapping = value;
    }


    [Tooltip("Manipulator which generates the value for the drawstring.")]
    [SerializeField]
    private TwoHandleManipulator manipulator;

    /// <summary>
    /// Manipulator which generates the value for the drawstring.
    /// </summary>
    public TwoHandleManipulator Manipulator
    {
        get
        {
            return manipulator ?? (manipulator = GetComponent<TwoHandleManipulator>());
        }
        set => manipulator = value;
    }

    [Tooltip("Text label that displays the current drawstring value.")]
    [SerializeField]
    private TextMeshPro valueText;

    /// <summary>
    /// Manipulator which generates the value for the drawstring.
    /// </summary>
    public TextMeshPro ValueText
    {
        get => valueText;
        set => valueText = value;
    }

    [SerializeField]
    [Tooltip("Transform that contains the value label.")]
    private Transform valueTransform = null;

    /// <summary>
    /// Transform that contains the value label.
    /// </summary>
    public Transform ValueTransform
    {
        get => valueTransform;
        set => valueTransform = value;
    }

    [SerializeField]
    [Tooltip("Animation curve for growing/shrinking the value label")]
    private AnimationCurve valueAnimationCurve = null;

    /// <summary>
    /// Animation curve for growing/shrinking the value label.
    /// </summary>
    public AnimationCurve ValueAnimationCurve
    {
        get => valueAnimationCurve;
        set => valueAnimationCurve = value;
    }

    [SerializeField]
    [Tooltip("Duration of value label animation.")]
    private float valueAnimationDuration = 0.25f;

    /// <summary>
    /// Duration of value label animation.
    /// </summary>
    public float ValueAnimationDuration => valueAnimationDuration;

    [SerializeField]
    [Tooltip("Drawstring levels, should be the same length as the snap points on the manipulator.")]
    private List<float> discreteLevels;

    /// <summary>
    /// Drawstring levels, should be the same length as the snap points on the manipulator,
    /// and the same number of levels as carousel entries.
    /// These are described in the nonlinear units.
    /// </summary>
    public List<float> DiscreteLevels
    {
        get => discreteLevels;
        set => discreteLevels = value;
    }

    [SerializeField]
    [Tooltip("Carousel display")]
    private CarouselDisplay carousel;

    /// <summary>
    /// Carousel display
    /// </summary>
    public CarouselDisplay Carousel
    {
        get => carousel;
        set => carousel = value;
    }

    [SerializeField]
    [Tooltip("Attachment point")]
    private Transform attachmentPoint;

    /// <summary>
    /// Attachment point
    /// </summary>
    public Transform AttachmentPoint
    {
        get => attachmentPoint;
        set => attachmentPoint = value;
    }

    private Vector3 valueLabelInitialScale;
    private float valueLabelAnimationTime;
    private Vector3 targetInitialScale;
    private Vector3 targetInitialPosition;

    // Start is called before the first frame update
    void Start()
    {
        valueLabelInitialScale = valueTransform.localScale;
        targetInitialScale = target.localScale;
        targetInitialPosition = target.position;
    }

    // Update is called once per frame
    void Update()
    {
        var animationDirection = Manipulator.IsGrabbed ? 1.0f : -1.0f;

        float scaling = scaleMapping.Evaluate(Manipulator.HandleDistance);

        valueLabelAnimationTime = Mathf.Clamp01(valueLabelAnimationTime + animationDirection * Time.deltaTime / valueAnimationDuration);
        valueTransform.localScale = valueLabelInitialScale * valueAnimationCurve.Evaluate(valueLabelAnimationTime);
        valueText.text = scaling.ToString("F2") + "x";

        int closestIndex = carousel.index;
        float closestValue = Mathf.Abs(Manipulator.HandleDistance - DiscreteLevels[closestIndex]);

        for (int i = 0; i < DiscreteLevels.Count; i++)
        {
            var variance = Mathf.Abs(Manipulator.HandleDistance - DiscreteLevels[i]);
            if (variance < closestValue && variance < 0.05f)
            {
                closestIndex = i;
                closestValue = Mathf.Abs(Manipulator.HandleDistance - DiscreteLevels[i]);
            }
        }

        carousel.index = closestIndex;

        target.localScale = Vector3.one * scaleMapping.Evaluate(Manipulator.HandleDistance);

        target.position = Vector3.Lerp(attachmentPoint.position, Vector3.zero, magnetMapping.Evaluate(scaling));
    }
}
