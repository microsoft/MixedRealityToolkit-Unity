using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DebugInfo : MonoBehaviour
{
    private static DebugInfo m_instance;

    private static DebugInfo Instance
    {
        get
        {
            if (m_instance == null)
            {
                var debugRoot = new GameObject();
                debugRoot.name = "DebugInfo";
                m_instance = debugRoot.AddComponent<DebugInfo>();
            }
            return m_instance;
        }
    }

    public static void Hide()
    {
        Instance.gameObject.SetActive(false);
    }

    public static void Show()
    {
        Instance.gameObject.SetActive(true);
    }

    public static void Status(string logString)
    {
        if (Instance == null || !Instance.gameObject.activeInHierarchy)
        {
            return;
        }
        Instance.m_stringBuilder.Append(string.Concat(logString, "\n"));
    }

    public static void PutText(string text, Vector3 location)
    {
        if (Instance == null || !Instance.gameObject.activeInHierarchy)
        {
            return;
        }
        Instance._PutText(text, location);
    }

    public static void ClearTextInWorld()
    {
        Instance.ClearAllTextObjectsInWorld();
    }
    [Tooltip("screen space x, y, and z is distance from camera. In pixel units from top left corner.")]
    public Vector3 m_textPositionScreenSpace = new Vector3(20, 20, 2f);

    [Tooltip("Show the output of Debug.Log below status messages")]
    public bool m_showConsole = true;

    private List<GameObject> m_textObjectsToPutInWorld = new List<GameObject>();
    private List<GameObject> m_textObjectsInWorld = new List<GameObject>();

    private TextMesh m_debugText;
    private StringBuilder m_stringBuilder = new StringBuilder();

    Queue<string> m_logMessages = new Queue<string>();
    private int m_logMessageCount = 10;

    public void Awake()
    {
        m_instance = this;
        // Build the object up
        BuildDebugInfo();
        Application.logMessageReceived += HandleLog;

        if (m_debugText == null)
        {
            m_debugText = GetComponent<TextMesh>();
        }
    }

    /// <summary>
    /// Builds the debugInfo UI components like text mesh,
    /// etc used to display debug info
    /// </summary>
    private void BuildDebugInfo()
    {
        m_debugText = CreateText("debugText", Vector3.zero, transform, TextAnchor.UpperLeft, Color.red, "debugText");
    }

    private void HandleLog(string condition, string stacktrace, LogType type)
    {
        m_logMessages.Enqueue(condition);
        if (m_logMessages.Count > m_logMessageCount)
        {
            m_logMessages.Dequeue();
        }
    }

    /// <summary>
    /// LateUpdate gets called after all other GameObjects have called Update
    /// </summary>
    void LateUpdate()
    {
        if (m_stringBuilder != null)
        {
            if (m_showConsole)
            {
                m_stringBuilder.Append("\n");
                m_stringBuilder.Append("LOG: \n");
                foreach (var item in m_logMessages.Reverse())
                {
                    m_stringBuilder.AppendLine(item);
                }
            }

            m_debugText.text = m_stringBuilder.ToString();
            
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(m_textPositionScreenSpace.x, Camera.main.pixelHeight - m_textPositionScreenSpace.y, m_textPositionScreenSpace.z));
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

            // After drawing to debugText, clear out the current log string to prep it for the next frame
            m_stringBuilder.Length = 0;
            Status(string.Format("FPS: {0:F2}\n", 1.0f / Time.deltaTime));
        }
        PutTextObjectsInWorld();
    }



    private void _PutText(string text, Vector3 location)
    {
        var tp = CreateText(
            $"DebugInfo.Text {m_textObjectsInWorld.Count + m_textObjectsInWorld.Count}",
            location,
            null,
            TextAnchor.UpperLeft,
            Color.white,
            text
            );
        m_textObjectsToPutInWorld.Add(tp.gameObject);
    }
    private void PutTextObjectsInWorld()
    {
        foreach (var o in m_textObjectsToPutInWorld)
        {
            m_textObjectsInWorld.Add(o);

            // rotate to face camera
            var gazeDirection = Camera.main.transform.forward;
            // Does this need to be negative? we will see
            o.transform.rotation = Quaternion.LookRotation(gazeDirection);
        }
        m_textObjectsToPutInWorld.Clear();
    }
    private void ClearAllTextObjectsInWorld()
    {
        foreach (var o in m_textObjectsInWorld)
        {
            Destroy(o);
        }
        m_textObjectsInWorld.Clear();
    }


    private static TextMesh CreateText(string name, Vector3 position, Transform parent, TextAnchor anchor, Color color, string text)
    {
        GameObject obj = new GameObject(name);
        obj.transform.localScale = Vector3.one * 0.0128f;
        obj.transform.parent = parent;
        obj.transform.localPosition = position;
        TextMesh textMesh = obj.AddComponent<TextMesh>();
        textMesh.fontSize = 48;
        textMesh.anchor = anchor;
        textMesh.color = color;
        textMesh.text = text;
        textMesh.richText = false;

        Renderer renderer = obj.GetComponent<Renderer>();
        OptimizeRenderer(renderer);

        return textMesh;
    }

    private static void OptimizeRenderer(Renderer renderer)
    {
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
        renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        renderer.allowOcclusionWhenDynamic = false;
    }

}
