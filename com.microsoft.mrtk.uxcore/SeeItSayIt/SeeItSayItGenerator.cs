using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    [AddComponentMenu("MRTK/UX/See It Say It Label")]
    public class SeeItSayItGenerator : MonoBehaviour
    {
        [SerializeField]
        private GameObject SeeItSayItPrefab;

        [SerializeField]
        private Transform PositionControl;

        private void Awake()
        {
            PressableButton pressablebutton = gameObject.GetComponent<PressableButton>();
            if (pressablebutton != null && pressablebutton.AllowSelectByVoice)
            {
#if MRTK_INPUT_PRESENT && MRTK_SPEECH_PRESENT
                GameObject label = Instantiate(SeeItSayItPrefab, transform, false);
                RectTransform canvasTransform = label.GetComponent<RectTransform>();

                if (canvasTransform != null)
                {
                    RectTransform labelTransform = label.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
                    if (labelTransform != null)
                    {
                        labelTransform.anchoredPosition = new Vector2(canvasTransform.rect.width / 2f, canvasTransform.rect.height / 2f + (PositionControl.gameObject.GetComponent<RectTransform>().rect.height /  2f * -1) - 10f);
                    }
                }
                else
                {
                    label.transform.localPosition = new Vector3(PositionControl.localPosition.x, (PositionControl.lossyScale.y / 2f * -1) -.007f, PositionControl.localPosition.z);
                }

                foreach (Transform child in label.transform)
                {
                    child.gameObject.SetActive(false);
                }
#endif
            }
        }
    }
}
