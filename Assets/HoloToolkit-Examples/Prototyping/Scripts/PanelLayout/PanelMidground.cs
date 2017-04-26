using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PanelMidground : MonoBehaviour
{
    public float BasePixelScale = 2048;
    public GameObject Background;
    public float Depth = 20;
    public float TopBuffer = 10;
    public float RightBuffer = 10;
    public float LeftBuffer = 10;
    public float BottomBuffer = 10;
    public float ZOffset = -5;

    private void UpdatePosition()
    {
        float xOffset = (RightBuffer - LeftBuffer) / 2;
        float yOffset = (TopBuffer - BottomBuffer) / 2;

        Vector3 newPosition = Background.transform.localPosition + Vector3.right * (xOffset / BasePixelScale) + Vector3.down * (yOffset / BasePixelScale) + Vector3.forward * (ZOffset / BasePixelScale);
        transform.localPosition = newPosition;
    }

    private void UpdateSize()
    {
        Vector3 newScale = new Vector3((Background.transform.localScale.x * BasePixelScale - (RightBuffer + LeftBuffer)) / BasePixelScale, (Background.transform.localScale.y * BasePixelScale - (TopBuffer + BottomBuffer)) / BasePixelScale, Depth / BasePixelScale);
        transform.localScale = newScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Background != null)
        {
            if (Background != null)
            {
                UpdateSize();
                UpdatePosition();
            }
            
        }
    }
}
