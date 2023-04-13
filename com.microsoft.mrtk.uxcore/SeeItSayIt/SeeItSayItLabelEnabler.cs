// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    [RequireComponent(typeof(PressableButton))]
    [AddComponentMenu("MRTK/UX/See It Say It Label")]
    public class SeeItSayItLabelEnabler : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The prefab for the see-it say-it label to be generated.")]
        private GameObject seeItSayItLabel;

        /// <summary>
        /// The prefab for the see-it say-it label to be generated.
        /// </summary>
        public GameObject SeeItSayItLabel
        {
            get => seeItSayItLabel;
            set => seeItSayItLabel = value;
        }

        private void Start()
        {
            // Check if voice commands are enabled for this button
            PressableButton pressablebutton = gameObject.GetComponent<PressableButton>();
            if (pressablebutton != null && pressablebutton.AllowSelectByVoice)
            {
                // Check if input and speech packages are present
#if MRTK_INPUT_PRESENT && MRTK_SPEECH_PRESENT
                SeeItSayItLabel.SetActive(true);

                // Children must be disabled so that they are not initially visible 
                foreach (Transform child in SeeItSayItLabel.transform)
                {
                    child.gameObject.SetActive(false);
                }

                // Set the label text to reflect the speech recognition keyword
                string keyword = pressablebutton.SpeechRecognitionKeyword;
                if (keyword != null)
                {
                    TMP_Text labelText = SeeItSayItLabel.GetComponentInChildren<TMP_Text>(true);
                    if (labelText != null)
                    {
                        labelText.text = $"Say '{keyword}'";
                    }
                }
#endif
            }
        }
    }
}
