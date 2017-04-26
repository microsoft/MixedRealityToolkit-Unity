using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PanelGlow : MonoBehaviour
{
    public float BasePixelScale = 2048;

    public enum BorderTypes { Top, Right, Bottom, Left };
    public BorderTypes Type;
    public float Thickness = 3;
    public float Depth = 30.72f;
    public float Offset = 1.5f;
    public float ZOffset = 0;

    public GameObject Background;

    private void UpdatePosition()
    {
        Vector3 newPosition;

        switch (Type)
        {
            case BorderTypes.Top:
                newPosition = new Vector3(0, Background.transform.localScale.y * 0.5f + Offset / BasePixelScale, ZOffset / BasePixelScale);
                break;
            case BorderTypes.Right:
                newPosition = new Vector3(Background.transform.localScale.x * 0.5f + Offset / BasePixelScale, 0, ZOffset / BasePixelScale);
                break;
            case BorderTypes.Bottom:
                newPosition = new Vector3(0, (-Background.transform.localScale.y * 0.5f) - Offset / BasePixelScale, ZOffset / BasePixelScale);
                break;
            case BorderTypes.Left:
                newPosition = new Vector3((-Background.transform.localScale.x * 0.5f) - Offset / BasePixelScale, 0, ZOffset / BasePixelScale);
                break;
            default:
                newPosition = Vector3.zero;
                break;
        }
        transform.localPosition = newPosition;
    }

    private void UpdateSize()
    {
        Vector3 newScale;

        switch (Type)
        {
            case BorderTypes.Top:
                newScale = new Vector3(Background.transform.localScale.x + Thickness / BasePixelScale, Depth / BasePixelScale, Depth / BasePixelScale);
                break;
            case BorderTypes.Right:
                newScale = new Vector3(Background.transform.localScale.y + (Thickness * 2) / BasePixelScale, Depth / BasePixelScale, Depth / BasePixelScale);
                break;
            case BorderTypes.Bottom:
                newScale = new Vector3(Background.transform.localScale.x + Thickness / BasePixelScale, Depth / BasePixelScale, Depth / BasePixelScale);
                break;
            case BorderTypes.Left:
                newScale = new Vector3(Background.transform.localScale.y + (Thickness * 2) / BasePixelScale, Depth / BasePixelScale, Depth / BasePixelScale);
                break;
            default:
                newScale = Vector3.zero;
                break;
        }

        transform.localScale = newScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Background != null)
        {
            UpdateSize();
            UpdatePosition();
        }
    }
}
