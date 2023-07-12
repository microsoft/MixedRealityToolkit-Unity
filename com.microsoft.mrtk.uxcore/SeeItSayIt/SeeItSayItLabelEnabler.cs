// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro;
using UnityEngine;
#if MRTK_INPUT_PRESENT && MRTK_SPEECH_PRESENT
using Microsoft.MixedReality.Toolkit.Input;
#endif

namespace Microsoft.MixedReality.Toolkit.UX
{
    [RequireComponent(typeof(PressableButton))]
    [AddComponentMenu("MRTK/UX/See It Say It Label")]
    public class SeeItSayItLabelEnabler : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The GameObject for the see-it say-it label to be enabled.")]
        private GameObject seeItSayItLabel;

        /// <summary>
        /// The GameObject for the see-it say-it label to be enabled.
        /// </summary>
        public GameObject SeeItSayItLabel
        {
            get => seeItSayItLabel;
            set => seeItSayItLabel = value;
        }

        [SerializeField]
        [Tooltip("The Transform that the label will be dynamically positioned off of. Empty by default. If positioning a Canvas label, this must be a RectTransform.")]
        private Transform positionControl;

        /// <summary>
        /// The Transform that the label will be dynamically positioned off of. Empty by default. If positioning a Canvas label, this must be a RectTransform.
        /// </summary>
        public Transform PositionControl
        {
            get => positionControl;
            set => positionControl = value;
        }

        private float canvasOffset = -10f;
        private float nonCanvasOffset = -.004f;

        private void Start()
        {
            // Check if voice commands are enabled for this button
            PressableButton pressablebutton = gameObject.GetComponent<PressableButton>();
            if (pressablebutton != null && pressablebutton.AllowSelectByVoice)
            {
                // Check if input and speech packages are present
#if MRTK_INPUT_PRESENT && MRTK_SPEECH_PRESENT
                // If we can't find any active speech interactors, then do not enable the labels.
                if (!ComponentCache<SpeechInteractor>.FindFirstActiveInstance())
                {
                    return;
                }

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

                // If a Transform is specified, use it to reposition the object dynamically
                if (positionControl != null)
                {
                    // The control RectTransform used to position the label's height
                    RectTransform controlRectTransform = PositionControl.gameObject.GetComponent<RectTransform>();

                    // If PositionControl is a RectTransform, reposition label relative to Canvas button
                    if (controlRectTransform != null &&  SeeItSayItLabel.transform.childCount > 0)
                    {
                        // The parent RectTransform used to center the label
                        RectTransform canvasTransform = SeeItSayItLabel.GetComponent<RectTransform>();

                        // The child RectTransform used to set the final position of the label 
                        RectTransform labelTransform = SeeItSayItLabel.transform.GetChild(0).gameObject.GetComponent<RectTransform>();

                        if (labelTransform != null && canvasTransform != null)
                        {
                            labelTransform.anchoredPosition3D = new Vector3(canvasTransform.rect.width / 2f, canvasTransform.rect.height / 2f + (controlRectTransform.rect.height /  2f * -1) + canvasOffset, canvasOffset);
                        }
                    }
                    else
                    {
                        SeeItSayItLabel.transform.localPosition = new Vector3(PositionControl.localPosition.x, (PositionControl.lossyScale.y / 2f * -1) + nonCanvasOffset, PositionControl.localPosition.z + nonCanvasOffset);
                    }
                }
#endif
            }
        }
    }
}
