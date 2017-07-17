using UnityEngine;

namespace HoloToolkit.Examples
{
    [RequireComponent(typeof(TextMesh))]
    public class DebugText : MonoBehaviour
    {
        private TextMesh mText;

        private void Awake()
        {
            mText = GetComponent<TextMesh>();
        }

        public void SetText(string text)
        {
            mText.text = text;
        }
    }
}
