using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.UI;

public class DisplayKeywords : MonoBehaviour
{
    public KeywordManager keywordManager;
    public Text textPanel;

    void Start()
    {
        if (keywordManager == null)
        {
            Debug.Log("Please specify a KeywordManager in the Inspector on DisplayKeywords.cs on" + name + ".");
        }
        if (textPanel == null)
        {
            Debug.Log("Please specify a Text field in the Inspector on DisplayKeywords.cs on " + name + ".");
        }
        if (textPanel == null || keywordManager == null)
        {
            return;
        }

        textPanel.text = "Try saying:\n";
        foreach (KeywordManager.KeywordAndResponse k in keywordManager.KeywordsAndResponses)
        {
            textPanel.text += k.Keyword + "\n";
        }
    }
}