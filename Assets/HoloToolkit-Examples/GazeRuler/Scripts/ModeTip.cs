using UnityEngine;
using HoloToolkit.Unity;

/// <summary>
/// provide a tip text of current measure mode
/// </summary>
public class ModeTip : Singleton<ModeTip>
{
    private const string LineMode = "Line Mode";
    private const string PloygonMode = "Geometry Mode";
    private TextMesh text;
    private int fadeTime = 100;

    void Start()
    {
        text = GetComponent<TextMesh>();
        switch (MeasureManager.Instance.Mode)
        {
            case GeometryMode.Line:
                text.text = LineMode;
                break;
            default:
                text.text = PloygonMode;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            // if you want log the position of mode tip text, just uncomment it.
            // Debug.Log("pos: " + gameObject.transform.position);
            switch (MeasureManager.Instance.Mode)
            {
                case GeometryMode.Line:
                    if (!text.text.Contains(LineMode))
                        text.text = LineMode;
                    break;
                default:
                    if (!text.text.Contains(PloygonMode))
                        text.text = PloygonMode;
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
