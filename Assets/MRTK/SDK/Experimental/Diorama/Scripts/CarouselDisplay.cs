using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarouselDisplay : MonoBehaviour
{
    public Color activeColor;
    public Color inactiveColor;
    public Transform highlightFrame;
    public List<RectTransform> elements;
    public int index;
    public float lerpTime;

    private Vector3 highlightFrameScale;
    private float highlightFrameVerticalOffset;
    private List<TextMeshPro> textObjects = new List<TextMeshPro>();
    // Start is called before the first frame update
    void OnValidate()
    {
        highlightFrameScale = highlightFrame.localScale;
        highlightFrameVerticalOffset = highlightFrame.localPosition.y - elements[0].localPosition.y;
        textObjects.Clear();
        foreach(var e in elements)
        {
            textObjects.Add(e.GetComponent<TextMeshPro>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        highlightFrame.localPosition = Vector3.Lerp(highlightFrame.localPosition, elements[index].localPosition + Vector3.up * highlightFrameVerticalOffset, lerpTime);
        highlightFrame.localScale = new Vector3(Mathf.Lerp(highlightFrame.localScale.x, elements[index].rect.width, lerpTime), highlightFrameScale.y, highlightFrameScale.z);

        for(int i = 0; i < textObjects.Count; i++)
        {
            textObjects[i].color = Color.Lerp(textObjects[i].color, i == index ? activeColor : inactiveColor, lerpTime);
        }
    }
}
