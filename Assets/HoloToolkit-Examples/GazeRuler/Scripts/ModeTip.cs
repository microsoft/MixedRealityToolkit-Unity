using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

/// <summary>
/// provide a tip text of current measure mode
/// </summary>
public class ModeTip : Singleton<ModeTip>
{
    TextMesh text;

    int fadeTime = 100;

    
    void Start()
    {
        text = GetComponent<TextMesh>();

        switch (MeasureManager.Instance.mode)
        {

            case GeometryMode.Line:
                text.text = "Line测距模式";
                break;
            default:
                text.text = "Ploygon图形测面积模式";
                break;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy)
        {

            Debug.Log("pos: " + gameObject.transform.position);

            switch (MeasureManager.Instance.mode)
            {

                case GeometryMode.Line:
                    if (!text.text.Contains("Line测距模式"))
                        text.text = "Line测距模式";
                    break;
                default:
                    if (!text.text.Contains("Ploygon图形测面积模式"))
                        text.text = "Ploygon图形测面积模式";
                    break;

            }


            var render = GetComponent<MeshRenderer>().material;
            fadeTime = 100;

            // fade tip text
            if (fadeTime == 0)
            {
                var color = render.color;
                fadeTime = 100;
                color.a = 1f;
                render.color = color;

                gameObject.SetActive(false);
            }
            else
            {
                var color = render.color;

                color.a -= 0.01f;
                render.color = color;
                fadeTime--;
            }
        }
    }
}
