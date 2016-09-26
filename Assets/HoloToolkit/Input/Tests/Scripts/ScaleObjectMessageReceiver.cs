using UnityEngine;
using System.Collections;

public class ScaleObjectMessageReceiver : MonoBehaviour
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