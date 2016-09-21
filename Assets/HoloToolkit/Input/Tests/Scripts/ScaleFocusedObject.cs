using UnityEngine;
using HoloToolkit.Unity;

public class ScaleFocusedObject : Interactable
{
    private const float DefaultSizeFactor = 2.0f;

    [Tooltip("Size multiplier to use when scaling the object up and down.")]
    public float SizeFactor = DefaultSizeFactor;

    private void Start()
    {
        if (SizeFactor <= 0.0f)
        {
            SizeFactor = DefaultSizeFactor;
        }
    }

    protected override void KeywordRecognized(string keyword)
    {
        if (keyword.Equals("make bigger"))
        {
            OnMakeBigger();
        }
        if (keyword.Equals("make smaller"))
        {
            OnMakeSmaller();
        }
    }

    public void OnMakeBigger()
    {
        Vector3 scale = transform.localScale;
        scale *= SizeFactor;
        transform.localScale = scale;
    }

    public void OnMakeSmaller()
    {
        Vector3 scale = transform.localScale;
        scale /= SizeFactor;
        transform.localScale = scale;
    }
}