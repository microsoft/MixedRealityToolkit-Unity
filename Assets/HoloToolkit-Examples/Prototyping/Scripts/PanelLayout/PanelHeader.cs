using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PanelHeader : MonoBehaviour
{
    public float BasePixelScale = 2048;
    public PanelBorder TopBorder;
    public float Height = 100;
    public float Depth = 20;
    public float TopBuffer = 10;
    public float RightBuffer = 10;
    public float LeftBuffer = 10;
    public float ZOffset = -5;

    private void UpdatePosition()
    {
        float xOffset = RightBuffer - LeftBuffer;
        float yOffset = Height * 0.5f + TopBuffer;

        Vector3 newPosition = TopBorder.transform.localPosition + Vector3.right * (xOffset / BasePixelScale) + Vector3.down * (yOffset / BasePixelScale) + Vector3.forward * (ZOffset / BasePixelScale);
        transform.localPosition = newPosition;
    }

    private void UpdateSize()
    {
        Vector3 newScale = new Vector3((TopBorder.transform.localScale.x * TopBorder.BasePixelScale - (RightBuffer + LeftBuffer)) / BasePixelScale, Height / BasePixelScale, Depth / BasePixelScale);
        transform.localScale = newScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (TopBorder != null)
        {
            UpdateSize();
            UpdatePosition();
        }
    }
}
