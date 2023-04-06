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
            //var interactor = FindObjectOfType<SpeechInteractor>();
            //var subsystem = XRSubsystemHelpers.KeywordRecognitionSystem;
            //if (interactor != null && subsystem != null)
            //{
            GameObject label = Instantiate(SeeItSayItPrefab, transform, false);

            if (label.GetComponent<RectTransform>())
            {
                label.GetComponent<RectTransform>().anchoredPosition = new Vector2(PositionControl.gameObject.GetComponent<RectTransform>().rect.width, 0.5f * PositionControl.gameObject.GetComponent<RectTransform>().rect.height);
            }
            else
            {
                label.transform.localPosition = new Vector3(PositionControl.localPosition.x, (PositionControl.lossyScale.y / 2f * -1) -.01f, PositionControl.localPosition.z);
            }

            foreach (Transform child in label.transform)
            {
                child.gameObject.SetActive(false);
            }
            //}
        }
    }
}
